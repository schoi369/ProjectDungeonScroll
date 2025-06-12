using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
	protected Vector3Int m_TilemapPos;
	public Vector3Int TilemapPos
	{
		get { return m_TilemapPos; } set { m_TilemapPos = value; }
	}

	public bool m_canBeAttacked;

	public virtual void Init(Vector3Int a_tilemapPos)
	{
		m_TilemapPos = a_tilemapPos;
	}

	public virtual void GetAttacked(int a_damage)
	{

	}

	public virtual void PlayerEntered()
	{

	}

	public virtual bool PlayerWantsToEnter()
	{
		return true;
	}

	public virtual void GetDestroyedFromBoard()
	{
		Destroy(gameObject);
	}
}
