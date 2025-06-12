using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
	public class CellData
	{
		public Vector3Int ArrayPos { get; private set; }
		public Vector3Int TilemapPos => GameManager.Instance.m_boardManager.ArrayPosToTilemapPos(ArrayPos);

		public TileProperty ContainedTileProperty { get; private set; }
		public CellObject ContainedObject { get; set; }

		public bool Passable => ContainedTileProperty.IsWalkable;

		public CellData(TileProperty a_tileProperty, Vector3Int a_arrayPos)
		{
			ContainedTileProperty = a_tileProperty;
			ArrayPos = a_arrayPos;
		}

		public void CleanUp()
		{
			if (ContainedObject)
			{
				ContainedObject.GetDestroyedFromBoard();
			}
			ContainedObject = null;
		}
	}

	public enum Direction
	{
		NONE,
		UP,
		RIGHT,
		DOWN,
		LEFT,
	}

	CellData[,] m_cellDataMap;
	Vector3Int m_mapOrigin; // 오프셋 계산을 위한 맵의 원점

	public Tilemap m_groundTilemap;
	public Tilemap m_cellObjectsTilemap;

	[Header("Tiles")]
	public LogicalTile[] m_floorTiles;
	public LogicalTile[] m_wallTiles;

	public void Init()
	{
		CreateLogicalMap();
		InitializeCellObjectsInMap();
	}

	/// <summary>
	/// 보드 형태를 읽어내서, m_cellDataMap에 논리적 맵을 형성함.
	/// </summary>
	void CreateLogicalMap()
	{
		BoundsInt bounds = m_groundTilemap.cellBounds;
		m_mapOrigin = bounds.min;

		m_cellDataMap = new CellData[bounds.size.x, bounds.size.y];

		foreach (var tilemapPos in bounds.allPositionsWithin)
		{
			LogicalTile tile = m_groundTilemap.GetTile<LogicalTile>(tilemapPos);

			Vector3Int arrayPos = TilemapPosToArrayPos(tilemapPos);

			if (tile != null)
			{
				TileProperty property = CreateTilePropertyFromType(tile.m_tileType);
				property.Init(tilemapPos, tile);

				m_cellDataMap[arrayPos.x, arrayPos.y] = new CellData(property, arrayPos);
			}
			else
			{
				m_cellDataMap[arrayPos.x, arrayPos.y] = null;
			}
		}
	}

	void InitializeCellObjectsInMap()
	{
		CellObject[] cellObjects = m_cellObjectsTilemap.transform.GetComponentsInChildren<CellObject>();
		foreach (var cellObject in cellObjects)
		{
			Vector3Int tilemapPos = m_cellObjectsTilemap.WorldToCell(cellObject.transform.position);
			cellObject.Init(tilemapPos);

			CellData thatCell = GetCellData(tilemapPos);
			thatCell.ContainedObject = cellObject;
		}
	}

	/// <summary>
	/// TileType enum 값에 따라 해당하는 TileProperty 인스턴스를 생성하여 반환합니다.
	/// </summary>
	private TileProperty CreateTilePropertyFromType(TileType a_type)
	{
		switch (a_type)
		{
			case TileType.Floor:
				return new FloorTileProperty();
			case TileType.Wall:
				return new WallTileProperty();
			// 새로운 타일 타입을 추가할 때마다 이곳에 case를 추가하면 됩니다.
			default:
				Debug.LogWarning($"정의되지 않은 TileType: {a_type}. 기본 Walkable로 처리합니다.");
				return null;
		}
	}

	public Vector3Int WorldToGrid(Vector3 a_worldPos)
	{
		return m_groundTilemap.WorldToCell(a_worldPos);
	}

	public Vector3 TilemapPosToWorldPos(Vector3Int a_tilemapPos)
	{
		return m_groundTilemap.GetCellCenterWorld(a_tilemapPos);
	}

	public Vector3Int TilemapPosToArrayPos(Vector3Int a_tilemapPos)
	{
		int x = a_tilemapPos.x - m_mapOrigin.x;
		int y = a_tilemapPos.y - m_mapOrigin.y;

		return new Vector3Int(x, y);
	}

	public Vector3Int ArrayPosToTilemapPos(Vector3Int a_arrayPos)
	{
		int x = a_arrayPos.x + m_mapOrigin.x;
		int y = a_arrayPos.y + m_mapOrigin.y;

		return new Vector3Int(x, y);
	}

	public CellData GetCellData(Vector3Int a_tilemapPos)
	{
		Vector3Int arrayPos = TilemapPosToArrayPos(a_tilemapPos);

		// 배열 범위를 체크하여 안전하게 데이터 반환
		if (arrayPos.x >= 0 && arrayPos.x < m_cellDataMap.GetLength(0) && arrayPos.y >= 0 && arrayPos.y < m_cellDataMap.GetLength(1))
		{
			return m_cellDataMap[arrayPos.x, arrayPos.y];
		}

		return null; // 맵 범위를 벗어난 경우
	}

	LogicalTile GetAppropriateLogicalTile(LogicalTile[] a_tiles, TilePhysicalState a_state)
	{
		return a_tiles[(int)a_state];
	}

	public void SetTileByPhysicalState(Vector3Int a_tilemapPos, TileType a_tileType, TilePhysicalState a_state)
	{
		switch (a_tileType)
		{
			case TileType.Floor:
				m_groundTilemap.SetTile(a_tilemapPos, GetAppropriateLogicalTile(m_floorTiles, a_state));
				break;
			case TileType.Wall:
				m_groundTilemap.SetTile(a_tilemapPos, GetAppropriateLogicalTile(m_wallTiles, a_state));
				break;
		}
	}

	/// <summary>
	/// 해당 셀이 다른 오브젝트가 없고, 지나갈 수 있는 타일인지 확인합니다.
	/// </summary>
	public bool IsCellWalkable(Vector3Int a_tilemapPos)
	{
		var data = GetCellData(a_tilemapPos);
		if (data == null) // 보드 바깥
		{
			return false;
		}

		if (data.ContainedObject != null) // 다른 오브젝트가 있음
		{
			return false;
		}

		if (GameManager.Instance.IsPlayerAt(a_tilemapPos)) // 플레이어가 있음
		{
			return false;
		}

		return data.Passable; // 타일의 Passable 속성 반환
	}

	// ============================================================================

	public void Clean()
	{
		if (m_cellDataMap == null)
		{
			return;
		}
	}

	public List<Vector3Int> GetAttackAreaCellPositions(AttackAreaSO a_area, Vector3Int a_center, Direction a_direction)
	{
		List<Vector3Int> result = new();

		for (int y = 0; y < AttackAreaSO.GridSize; y++)
		{
			for (int x = 0; x < AttackAreaSO.GridSize; x++)
			{
				if (a_area.GetCell(x, y))
				{
					int offsetX = 0;
					int offsetY = 0;

					switch (a_direction)
					{
						case Direction.UP:
							offsetX = y - AttackAreaSO.Center;
							offsetY = x - AttackAreaSO.Center;
							break;
						case Direction.RIGHT:
							offsetX = x - AttackAreaSO.Center;
							offsetY = y - AttackAreaSO.Center;
							break;
						case Direction.DOWN:
							offsetX = AttackAreaSO.Center - y;
							offsetY = AttackAreaSO.Center - x;
							break;
						case Direction.LEFT:
							offsetX = AttackAreaSO.Center - x;
							offsetY = AttackAreaSO.Center - y;
							break;
					}

					Vector3Int target = new(a_center.x + offsetX, a_center.y + offsetY);
					if (GetCellData(target) != null)
					{
						result.Add(target);
					}
				}
			}
		}

		return result;
	}

	void AddObject(CellObject a_obj, Vector3Int a_tilemapPos)
	{
		CellData data = m_cellDataMap[a_tilemapPos.x, a_tilemapPos.y];
		a_obj.transform.position = TilemapPosToWorldPos(a_tilemapPos);
		data.ContainedObject = a_obj;
		a_obj.Init(a_tilemapPos);
	}

	/// <summary>
	/// 특정 CellObject를 새로운 위치로 이동시킵니다. (보드 데이터 및 실제 위치 포함)
	/// </summary>
	public void MoveObjectOnBoard(CellObject a_obj, Vector3Int a_toPos)
	{
		Vector3Int fromPos = a_obj.TilemapPos;

		GetCellData(fromPos).ContainedObject = null;
		GetCellData(a_toPos).ContainedObject = a_obj;

		// 오브젝트 내부의 위치 정보 갱신
		a_obj.TilemapPos = a_toPos;

		// 실제 게임 오브젝트의 위치를 이동
		a_obj.transform.position = TilemapPosToWorldPos(a_toPos);
	}

	/// <summary>
	/// 두 Vector3Int가 인접해 있는가 판단하는 Util 메소드.
	/// </summary>
	/// <param name="a_vector1"></param>
	/// <param name="a_vector2"></param>
	/// <returns></returns>
	public bool ArePositionsAdjacent(Vector3Int a_vector1, Vector3Int a_vector2)
	{
		if (a_vector1 == a_vector2)
		{
			return false;
		}

		// 각 축의 거리 차이(절대값)를 모두 더합니다.
		int manhattanDistance = Mathf.Abs(a_vector1.x - a_vector2.x) +
								Mathf.Abs(a_vector1.y - a_vector2.y) +
								Mathf.Abs(a_vector1.z - a_vector2.z);
		return manhattanDistance == 1;
	}

	public void WarnColumn(int a_columnIndex)
	{
		//if (a_columnIndex < m_width)
		//{
		//	for (int y = 0; y < m_height; y++)
		//	{
		//		var data = GetCellData(new Vector2Int(a_columnIndex, y));
		//		//if (data.m_groundTile == null) continue;
		//		//data.m_groundTile.SetPhysicalState(GroundTile.PhysicalState.Warned);
		//	}
		//}
	}

	// 특정 열을 파괴하는 public 메서드
	public void DestroyColumn(int a_columnIndex)
	{
		//	if (a_columnIndex < m_width)
		//	{
		//		for (int y = 0; y < m_height; y++)
		//		{
		//			var data = GetCellData(new Vector2Int(a_columnIndex, y));
		//			//if (data.m_groundTile == null) continue;
		//			//data.m_groundTile.SetPhysicalState(GroundTile.PhysicalState.Destroyed);

		//			if (data.m_containedObject)
		//			{
		//				data.m_containedObject.GetDestroyedFromBoard();
		//				data.m_containedObject = null;
		//			}
		//		}

		//		var player = GameManager.Instance.m_player;
		//		if (player.CellPos.x == a_columnIndex)
		//		{
		//			player.GameOver();
		//		}
		//	}
	}
}
