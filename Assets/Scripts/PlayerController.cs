using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	BoardManager m_board;
	Vector2Int m_cellPos;

	public void Spawn(BoardManager a_boardManager, Vector2Int a_cellPos)
	{
		m_board = a_boardManager;
		MoveTo(a_cellPos);
	}

	public void MoveTo(Vector2Int a_cellPos)
	{
		m_cellPos = a_cellPos;
		transform.position = m_board.CellPosToWorldPos(m_cellPos);
	}

	// Update is called once per frame
	void Update()
	{
		Vector2Int newCellTargetPos = m_cellPos; // put current first
		bool hasMoved = false;

		if (Input.GetKeyDown(KeyCode.W))
		{
			newCellTargetPos.y += 1;
			hasMoved = true;
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			newCellTargetPos.y -= 1;
			hasMoved = true;
		}
		else if (Input.GetKeyDown(KeyCode.A))
		{
			newCellTargetPos.x -= 1;
			hasMoved = true;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			newCellTargetPos.x += 1;
			hasMoved = true;
		}

		if (hasMoved)
		{
			BoardManager.CellData cellData = m_board.GetCellData(newCellTargetPos);

			if (cellData != null && cellData.Passable)
			{
				GameManager.Instance.TurnManager.Tick();
				if (cellData.m_containedObject == null)
				{
					MoveTo(newCellTargetPos);
				}
				else if (cellData.m_containedObject.PlayerWantsToEnter())
				{
					MoveTo(newCellTargetPos);
				}
			}
		}
	}
}
