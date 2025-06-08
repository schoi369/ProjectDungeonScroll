using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public AttackAreaSO m_attackAreaSetting;

	BoardManager m_board;
	Vector2Int m_cellPos;
	public Vector2Int CellPos => m_cellPos;

	public float m_moveSpeed = 5f;

	public int m_maxHP = 5;
	int CurrentHP { get; set; }


	bool IsMoving { get; set; }
	Vector3 MoveTarget { get; set; }

	public int m_maxFoodAmount = 100;
	int CurrentFoodAmount { get; set; }
	
	public bool IsGameOver { get; set; } = false;

	bool RequestMovement { get; set; } = false;
	BoardManager.Direction RequestedDirection { get; set; } = BoardManager.Direction.NONE; 

	/// <summary>
	/// Initialize
	/// </summary>
	public void Init()
	{
		IsMoving = false;
		IsGameOver = false;

		CurrentHP = m_maxHP;
        CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerMaxHPChanged, m_maxHP);
        CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentHP);
	}

	/// <summary>
	/// Board(Stage) related
	/// </summary>
	/// <param name="a_boardManager"></param>
	/// <param name="a_cellPos"></param>
	public void Spawn(BoardManager a_boardManager, Vector2Int a_cellPos)
	{
		m_board = a_boardManager;
		MoveTo(a_cellPos, instant: true);
	}

	public void MoveTo(Vector2Int a_cellPos, BoardManager.Direction a_direction = BoardManager.Direction.NONE, bool instant = false)
	{
		var cellPosBeforeMove = m_cellPos;
		m_cellPos = a_cellPos;

		if (instant)
		{
			IsMoving = false;
			transform.position = m_board.CellPosToWorldPos(m_cellPos);
		}
		else
		{
			IsMoving = true;
			MoveTarget = m_board.CellPosToWorldPos(m_cellPos);
		}
	}

	public void TakeDamage(int a_damage)
	{
		if (IsGameOver) // TODO: CanTakeDamage 등의 변수로 전체적인 체크 하도록 업데이트.
		{
			return;
		}

		Debug.Log("Player taking damage");

		CurrentHP -= a_damage;
        CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentHP);

        if (CurrentHP <= 0)
		{
			GameOver();
		}
	}


	public void GameOver()
	{
		IsGameOver = true;
		OverlayCanvas.Instance.ShowHideGameOverPanel(true);

		m_board.StopBoardDestroying();
	}

	bool CanMove()
	{
		bool result = true;

		result &= !IsGameOver;

		return result;
	}

	void Update()
	{
		if (!CanMove())
		{
			return;
		}

		if (IsMoving && MoveTarget != null)
		{
			transform.position = Vector3.MoveTowards(transform.position, MoveTarget, m_moveSpeed * Time.deltaTime);
			if (transform.position == MoveTarget)
			{
				IsMoving = false;
				var cellData = m_board.GetCellData(m_cellPos);
				if (cellData.m_containedObject != null)
				{
					cellData.m_containedObject.PlayerEntered();
				}
			}

			return;
		}

		Vector2Int newCellTargetPos = m_cellPos; // First, set the current cell pos as target.
		if (RequestMovement)
		{
			// Check the direction if there are anything that the player would attack.
			bool attackedSomething = false;
			var cellPosList = m_board.GetAttackAreaCellPositions(m_attackAreaSetting, m_cellPos, RequestedDirection);
			foreach (var targetCellPos in cellPosList)
			{
				Vector3 cellWorldPos = m_board.CellPosToWorldPos(targetCellPos);
				AttackCellVisualPool.Instance.SpawnVisual(cellWorldPos);
				var data = m_board.GetCellData(targetCellPos);
				if (data.m_containedObject && data.m_containedObject.m_canBeAttacked)
				{
					attackedSomething = true;
					data.m_containedObject.GetAttacked(1);
				}
			}

			if (attackedSomething)
			{

			}
			else
			{
				switch (RequestedDirection)
				{
					case BoardManager.Direction.UP:
						newCellTargetPos.y += 1;
						break;
					case BoardManager.Direction.DOWN:
						newCellTargetPos.y -= 1;
						break;
					case BoardManager.Direction.RIGHT:
						newCellTargetPos.x += 1;
						break;
					case BoardManager.Direction.LEFT:
						newCellTargetPos.x -= 1;
						break;
					default:
						break;
				}

				BoardManager.CellData cellData = m_board.GetCellData(newCellTargetPos);
				if (cellData != null && cellData.Passable)
				{
					if (cellData.m_containedObject == null)
					{
						MoveTo(newCellTargetPos, RequestedDirection);
					}
					else if (cellData.m_containedObject.PlayerWantsToEnter())
					{
						MoveTo(newCellTargetPos, RequestedDirection);
					}
				}
			}


			GameManager.Instance.TurnManager.Tick();

			// Set Request values to default.
			RequestMovement = false;
			RequestedDirection = BoardManager.Direction.NONE;
		}
	}

	public void OnInputMoveUp(InputAction.CallbackContext a_context)
	{
		if (!CanAcceptMoveInput())
		{
			return;
		}

		if (a_context.performed)
		{
			RequestMovement = true;
			RequestedDirection = BoardManager.Direction.UP;
		}
	}

	public void OnInputMoveDown(InputAction.CallbackContext a_context)
	{
		if (!CanAcceptMoveInput())
		{
			return;
		}

		if (a_context.performed)
		{
			RequestMovement = true;
			RequestedDirection = BoardManager.Direction.DOWN;
		}
	}

	public void OnInputMoveRight(InputAction.CallbackContext a_context)
	{
		if (!CanAcceptMoveInput())
		{
			return;
		}

		if (a_context.performed)
		{
			RequestMovement = true;
			RequestedDirection = BoardManager.Direction.RIGHT;
		}
	}

	public void OnInputMoveLeft(InputAction.CallbackContext a_context)
	{
		if (!CanAcceptMoveInput())
		{
			return;
		}

		if (a_context.performed)
		{
			RequestMovement = true;
			RequestedDirection = BoardManager.Direction.LEFT;
		}
	}

	bool CanAcceptMoveInput()
	{
		bool result = true;

		result &= !IsMoving;

		return result;
	}

	public void OnInputRestart(InputAction.CallbackContext a_context)
	{
		if (!IsGameOver)
		{
			return;
		}

		if (a_context.performed)
		{
			GameManager.Instance.StartNewGame();
		}
	}
}
