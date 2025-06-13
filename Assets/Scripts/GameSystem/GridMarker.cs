using UnityEngine;

public class GridMarker : MonoBehaviour
{
	protected Vector3Int m_TilemapPos;
	public Vector3Int TilemapPos
	{
		get { return m_TilemapPos; }
		set { m_TilemapPos = value; }
	}

	public virtual void Init(Vector3Int a_tilemapPos)
	{
		TilemapPos = a_tilemapPos;
	}
}