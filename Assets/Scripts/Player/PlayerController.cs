using System;
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

	// Level Related
	public int Level { get; set; }
	public int CurrentExp { get; private set; }
	public int m_expToLevelUp = 30; // 차후 ScriptableObject 등으로 전체적 레벨업 세팅 가능하게 변화할지도.

	private List<UpgradeSO> m_activeUpgrades = new();
	public event Action<CellObject, BoardManager.Direction> OnAttackLanded;

	public List<UpgradeSO> m_testUpgrades = new();
	public int PeacefulTurns { get; set; } = 0;

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

		// 레벨, 경험치 관련 초기화
		Level = 1;
		CurrentExp = 0;

		// 업그레이드 관련 변수 초기화
		PeacefulTurns = 0;

		// 테스트용 업그레이드 적용
		foreach (var upgrade in m_testUpgrades)
		{
			AddUpgrade(upgrade);
		}
	}

	/// <summary>
	/// Board(Stage) related
	/// </summary>
	public void Spawn(BoardManager a_boardManager, Vector2Int a_cellPos)
	{
		m_board = a_boardManager;
		MoveTo(a_cellPos, instant: true);
	}

	public void MoveTo(Vector2Int a_cellPos, bool instant = false)
	{
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
		if (IsGameOver)
		{
			return;
		}

		Debug.Log("Player taking damage");
		PeacefulTurns = 0;

		CurrentHP -= a_damage;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentHP);

		if (CurrentHP <= 0)
		{
			GameOver();
		}
	}

	public void Heal(int a_heal)
	{
		CurrentHP = Mathf.Min(CurrentHP + a_heal, m_maxHP);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentHP);
		Debug.Log($"{a_heal} HP 회복.");
	}

	public void GainExp(int a_amount)
	{
		CurrentExp += a_amount;
		Debug.Log($"경험치 {a_amount} 획득! 현재: {CurrentExp}/{m_expToLevelUp}");

		while (CurrentExp >= m_expToLevelUp)
		{
			LevelUp();
		}
	}

	void LevelUp()
	{
		Level++;
		CurrentExp -= m_expToLevelUp; // 넘긴 경험치 남기기.
		
		//m_expToLevelUp = (int) (m_expToLevelUp * 1.5f); // 필요 경험치 증가 예시.
		
		Debug.Log($"레벨업! 레벨 {Level} 달성");
	}

	public void GameOver()
	{
		IsGameOver = true;
		GameManager.Instance.UpdateGameState(GameManager.GameState.GameOver);
		OverlayCanvas.Instance.ShowHideGameOverPanel(true);

		m_board.StopBoardDestroying();
	}

	void Update()
	{
		if (IsMoving)
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
		}
	}

	private void ProcessPlayerAction(BoardManager.Direction a_direction)
	{
		// Check the direction if there are anything that the player would attack.
		bool attackedSomething = false;
		var cellPosList = m_board.GetAttackAreaCellPositions(m_attackAreaSetting, m_cellPos, a_direction);
		foreach (var targetCellPos in cellPosList)
		{
			Vector3 cellWorldPos = m_board.CellPosToWorldPos(targetCellPos);
			AttackCellVisualPool.Instance.SpawnVisual(cellWorldPos);
			var data = m_board.GetCellData(targetCellPos);
			if (data.m_containedObject && data.m_containedObject.m_canBeAttacked)
			{
				attackedSomething = true;
				PeacefulTurns = 0;
				data.m_containedObject.GetAttacked(1);
				OnAttackLanded?.Invoke(data.m_containedObject, a_direction);
			}
		}

		if (!attackedSomething)
		{
			Vector2Int newCellTargetPos = m_cellPos;
			switch (a_direction)
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
			}

			BoardManager.CellData cellData = m_board.GetCellData(newCellTargetPos);
			if (cellData != null && cellData.Passable)
			{
				if (cellData.m_containedObject == null)
				{
					MoveTo(newCellTargetPos);
				}
				else if (cellData.m_containedObject.PlayerWantsToEnter())
				{
					MoveTo(newCellTargetPos);
				}
			}

			PeacefulTurns++;
		}

		// 행동이 끝나면(이동, 공격, 혹은 아무것도 못함) 턴을 종료
		GameManager.Instance.EndPlayerTurn();
	}

	/// <summary>
	/// 새로운 업그레이드를 획득하고 적용합니다.
	/// </summary>
	public void AddUpgrade(UpgradeSO a_upgrade)
	{
		if (m_activeUpgrades.Contains(a_upgrade))
		{
			return; // 중복 획득 방지
		}

		m_activeUpgrades.Add(a_upgrade);
		a_upgrade.Apply(this.gameObject);

		Debug.Log($"업그레이드 획득: {a_upgrade.upgradeName}");
	}

	//----------------------------------------------------------------
	// Inputs

	private bool CanAcceptMoveInput()
	{
		return !IsMoving && !IsGameOver && GameManager.Instance.CurrentState == GameManager.GameState.PlayerTurn;
	}

	public void OnInputMoveUp(InputAction.CallbackContext a_context)
	{
		if (a_context.performed && CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.UP);
		}
	}

	public void OnInputMoveDown(InputAction.CallbackContext a_context)
	{
		if (a_context.performed && CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.DOWN);
		}
	}

	public void OnInputMoveRight(InputAction.CallbackContext a_context)
	{
		if (a_context.performed && CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.RIGHT);
		}
	}

	public void OnInputMoveLeft(InputAction.CallbackContext a_context)
	{
		if (a_context.performed && CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.LEFT);
		}
	}

	public void OnInputRestart(InputAction.CallbackContext a_context)
	{
		if (a_context.performed && IsGameOver)
		{
			GameManager.Instance.StartNewGame();
		}
	}
}