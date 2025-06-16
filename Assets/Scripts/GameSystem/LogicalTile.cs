using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 타일의 종류를 쉽게 구분하기 위한 Enum입니다.
/// </summary>
public enum TileType
{
	Floor,
	Wall,
}

public enum TilePhysicalState
{
	Default = 0,
	Warned = 1,
	Destroyed = 2,
}

public class TileProperty
{
	public Vector3Int TilemapPos { get; private set; }
	public LogicalTile TileInfo { get; private set; }

	public virtual bool IsWalkable { get; private set; } = true;

	TilePhysicalState m_physicalState;

	public virtual void OnStepOn(CellObject a_stepper) { }

	public void Init(Vector3Int a_tilemapPos, LogicalTile a_logicalTile)
	{
		TilemapPos = a_tilemapPos;
		TileInfo = a_logicalTile;

		SetPhysicalState(TilePhysicalState.Default);
	}

	public void SetPhysicalState(TilePhysicalState a_newState)
	{
		m_physicalState = a_newState;

		StageManager.Instance.m_boardManager.SetTileByPhysicalState(TilemapPos, TileInfo.m_tileType, m_physicalState);
		if (m_physicalState == TilePhysicalState.Destroyed)
		{
			IsWalkable = false; // 파괴된 타일은 더 이상 지나갈 수 없음
		}
		else
		{
			IsWalkable = true; // 기본 상태나 경고 상태는 지나갈 수 있음
		}
	}
}

public class FloorTileProperty : TileProperty
{

}

public class WallTileProperty : TileProperty
{
	public override bool IsWalkable => false;
}

/// <summary>
/// 타일의 정적인 정보를 보관함.
/// </summary>
[CreateAssetMenu(fileName = "New LogicalTile", menuName = "Tiles/Logical Tile")]
public class LogicalTile : Tile
{
	public TileType m_tileType;
}