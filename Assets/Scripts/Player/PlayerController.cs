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
		SetMaxFoodAmount(m_maxFoodAmount);
		SetCurrentFoodAmount(m_maxFoodAmount);
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

			// Attack based on cellPosBeforeMove
			var cellPosList = m_board.GetAttackAreaCellPositions(m_attackAreaSetting, cellPosBeforeMove, a_direction);
			foreach (var targetCellPos in cellPosList)
			{
				Vector3 cellWorldPos = m_board.CellPosToWorldPos(targetCellPos);
				AttackCellVisualPool.Instance.SpawnVisual(cellWorldPos);
				// TODO: Inflict Damage
				
			}
		}
	}

	void ChangeCurrentFoodAmount(int a_delta)
	{
		SetCurrentFoodAmount(CurrentFoodAmount + a_delta);
	}

	void SetCurrentFoodAmount(int a_amount)
	{
		CurrentFoodAmount = a_amount;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentFoodAmountChanged, CurrentFoodAmount);

		if (CurrentFoodAmount <= 0)
		{
			GameOver();
		}
	}

	void SetMaxFoodAmount(int a_max)
	{
		m_maxFoodAmount = a_max;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerMaxFoodAmountChanged, m_maxFoodAmount);
	}

	void GameOver()
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
		bool needToMove = false;
		BoardManager.Direction direction = BoardManager.Direction.NONE;

		if (RequestMovement)
		{
			needToMove = true;
			direction = RequestedDirection;

			switch (direction)
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

			// Set Request values to default.
			RequestMovement = false;
			RequestedDirection = BoardManager.Direction.NONE;
		}

		if (needToMove)
		{
			BoardManager.CellData cellData = m_board.GetCellData(newCellTargetPos);

			if (cellData != null && cellData.Passable)
			{
				ChangeCurrentFoodAmount(-1);

				if (cellData.m_containedObject == null)
				{
					MoveTo(newCellTargetPos, direction);
				}
				else if (cellData.m_containedObject.PlayerWantsToEnter())
				{
					MoveTo(newCellTargetPos, direction);
				}

				GameManager.Instance.TurnManager.Tick();
			}
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
