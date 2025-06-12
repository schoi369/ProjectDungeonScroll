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

public abstract class TileProperty
{
	public virtual bool IsWalkable => true;
	public virtual void OnStepOn(CellObject a_stepper) { }
}

public class FloorTileProperty : TileProperty
{

}

public class WallTileProperty : TileProperty
{
	public override bool IsWalkable => false;
}

/// <summary>
/// 프로젝트 창에서 우클릭으로 쉽게 'LogicalTile' 애셋을 생성할 수 있도록 메뉴를 추가합니다.
/// </summary>
[CreateAssetMenu(fileName = "New LogicalTile", menuName = "Tiles/Logical Tile")]
public class LogicalTile : Tile
{
	public TileType m_tileType;
}