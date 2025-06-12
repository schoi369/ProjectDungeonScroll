using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : MonoBehaviour
{
	protected Vector3Int m_cellPos;
	public Vector3Int CellPos
	{
		get { return m_cellPos; } set { m_cellPos = value; }
	}

	public bool m_canBeAttacked;

	public virtual void Init(Vector3Int a_cellPos)
	{
		m_cellPos = a_cellPos;
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
