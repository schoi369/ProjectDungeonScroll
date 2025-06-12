using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
	public class CellData
	{
		public TileProperty ContainedTileProperty { get; private set; }
		public CellObject ContainedObject { get; set; }

		public bool Passable => ContainedTileProperty.IsWalkable;

		public CellData(TileProperty a_tileProperty)
		{
			ContainedTileProperty = a_tileProperty;
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

	public float m_cellSize = 1f;

	public Tilemap m_groundTilemap;

	public void Init()
	{
		CreateLogicalMap();
	}

	/// <summary>
	/// 보드 형태를 읽어내서, m_boardData에 논리적 맵을 형성함.
	/// </summary>
	void CreateLogicalMap()
	{
		BoundsInt bounds = m_groundTilemap.cellBounds;
		m_mapOrigin = bounds.min;

		m_cellDataMap = new CellData[bounds.size.x, bounds.size.y];

		Debug.Log($"Bounds: {bounds.size.x} , {bounds.size.y}");
		
		foreach (var pos in bounds.allPositionsWithin)
		{
			LogicalTile tile = m_groundTilemap.GetTile<LogicalTile>(pos);

			// 월드 좌표 -> 배열 좌표
			int x = pos.x - m_mapOrigin.x;
			int y = pos.y - m_mapOrigin.y;

			if (tile != null)
			{
				//Debug.Log($"Pos ({x}, {y}) Type {tile.m_tileType}");

				TileProperty property = CreateTilePropertyFromType(tile.m_tileType);
				m_cellDataMap[x, y] = new CellData(property);
			}
			else
			{
				m_cellDataMap[x, y] = null;
			}
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

	public Vector3 GridToWorld(Vector3Int a_gridPos)
	{
		return m_groundTilemap.GetCellCenterWorld(a_gridPos);
	}

	public CellData GetCellData(Vector3Int a_gridPos)
	{
		// 그리드 좌표를 배열 인덱스로 변환
		int x = a_gridPos.x - m_mapOrigin.x;
		int y = a_gridPos.y - m_mapOrigin.y;

		// 배열 범위를 체크하여 안전하게 데이터 반환
		if (x >= 0 && x < m_cellDataMap.GetLength(0) && y >= 0 && y < m_cellDataMap.GetLength(1))
		{
			return m_cellDataMap[x, y];
		}

		return null; // 맵 범위를 벗어난 경우
	}

	/// <summary>
	/// Vector2Int를 지원하는 임시 메소드.
	/// </summary>
	/// <param name="a_gridPos2"></param>
	/// <returns></returns>
	public CellData GetCellData(Vector2Int a_gridPos2)
	{
		// 그리드 좌표를 배열 인덱스로 변환
		int x = a_gridPos2.x - m_mapOrigin.x;
		int y = a_gridPos2.y - m_mapOrigin.y;

		// 배열 범위를 체크하여 안전하게 데이터 반환
		if (x >= 0 && x < m_cellDataMap.GetLength(0) && y >= 0 && y < m_cellDataMap.GetLength(1))
		{
			return m_cellDataMap[x, y];
		}

		return null; // 맵 범위를 벗어난 경우
	}

	/// <summary>
	/// 해당 셀이 다른 오브젝트가 없고, 지나갈 수 있는 타일인지 확인합니다.
	/// </summary>
	public bool IsCellWalkable(Vector3Int a_cellPos)
	{
		var data = GetCellData(a_cellPos);
		if (data == null) // 보드 바깥
		{
			return false;
		}

		if (data.ContainedObject != null) // 다른 오브젝트가 있음
		{
			return false;
		}

		if (GameManager.Instance.IsPlayerAt(a_cellPos)) // 플레이어가 있음
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

	public Vector3 CellPosToWorldPos(Vector3Int a_cellPos)
	{
		return m_cellSize * new Vector3(a_cellPos.x, a_cellPos.y, 0f);
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

	void AddObject(CellObject a_obj, Vector3Int a_cellPos)
	{
		CellData data = m_cellDataMap[a_cellPos.x, a_cellPos.y];
		a_obj.transform.position = GridToWorld(a_cellPos);
		data.ContainedObject = a_obj;
		a_obj.Init(a_cellPos);
	}

	

	/// <summary>
	/// 특정 CellObject를 새로운 위치로 이동시킵니다. (보드 데이터 및 실제 위치 포함)
	/// </summary>
	public void MoveObjectOnBoard(CellObject a_obj, Vector3Int a_toPos)
	{
		Vector3Int fromPos = a_obj.CellPos;

		// 이전 위치의 데이터를 정리
		GetCellData(fromPos).ContainedObject = null;

		// 새로운 위치에 오브젝트 정보 설정
		GetCellData(a_toPos).ContainedObject = a_obj;

		// 오브젝트 내부의 위치 정보 갱신
		a_obj.CellPos = a_toPos;

		// 실제 게임 오브젝트의 위치를 이동
		a_obj.transform.position = CellPosToWorldPos(a_toPos);
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
