using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public AttackAreaSO m_attackAreaSetting;

	BoardManager m_board;
	Vector3Int m_gridPos;
	public Vector3Int CellPos => m_gridPos;

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

	int m_pendingLevelUps = 0;
	public int PendingLevelUps => m_pendingLevelUps;

	// Upgrades
	private List<UpgradeSO> m_activeUpgrades = new();
	public List<UpgradeSO> ActiveUpgrades => m_activeUpgrades;
	public event Action<CellObject, BoardManager.Direction> OnAttackLanded;

	public event Action<UpgradeSO> OnUpgradeAdded;

	public List<UpgradeSO> m_testUpgrades = new();
	public int PeacefulTurns { get; set; } = 0;

	[Header("Visuals")]
	public float m_hitScaleMultiplier = 1.2f;
	public float m_hitEffectDuration = 0.1f;
	Vector3 m_originalScale;
	Coroutine m_hitScaleEffectCoroutine;

	/// <summary>
	/// Initialize
	/// </summary>
	public void Init()
	{
		// 업그레이드 초기화
		// 모든 업그레이드의 구독 해제
		for (int i = m_activeUpgrades.Count - 1; i >= 0; i--)
		{
			m_activeUpgrades[i].Remove(this.gameObject);
		}
		m_activeUpgrades.Clear(); // 업그레이드 리스트 비우기

		//
		IsMoving = false;
		IsGameOver = false;

		CurrentHP = m_maxHP;

		// 비주얼
		m_originalScale = transform.localScale;

		// 레벨, 경험치 관련 초기화
		Level = 1;
		CurrentExp = 0;
		m_pendingLevelUps = 0;

		// 업그레이드 관련 변수 초기화
		PeacefulTurns = 0;

		// 테스트용 업그레이드 적용
		foreach (var upgrade in m_testUpgrades)
		{
			AddUpgrade(upgrade);
		}

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerMaxHPChanged, m_maxHP);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentHP);

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerLevelChanged, Level);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerExpChanged, (CurrentExp, m_expToLevelUp));
	}

	/// <summary>
	/// Board(Stage) related
	/// </summary>
	public void Spawn(BoardManager a_boardManager, Vector3Int a_cellPos)
	{
		m_board = a_boardManager;
		MoveTo(a_cellPos, instant: true);
	}

	public void MoveTo(Vector3Int a_newGridPos, bool instant = false)
	{
		m_gridPos = a_newGridPos;

		if (instant)
		{
			IsMoving = false;
			transform.position = m_board.GridToWorld(m_gridPos);
		}
		else
		{
			IsMoving = true;
			MoveTarget = m_board.GridToWorld(m_gridPos);
		}
	}

	public void TakeDamage(int a_damage)
	{
		if (IsGameOver)
		{
			return;
		}

		if (m_hitScaleEffectCoroutine != null)
		{
			StopCoroutine(m_hitScaleEffectCoroutine);
		}
		transform.localScale = m_originalScale;
		m_hitScaleEffectCoroutine = StartCoroutine(HitEffectCoroutine());

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
	}

	public void GainExp(int a_amount)
	{
		CurrentExp += a_amount;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerExpChanged, (CurrentExp, m_expToLevelUp));

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

		m_pendingLevelUps++; // 레벨업 처리 예약 +1

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerLevelChanged, Level);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerExpChanged, (CurrentExp, m_expToLevelUp));
	}

	public void ConsumePendingLevelUp()
	{
		if (m_pendingLevelUps > 0)
		{
			m_pendingLevelUps--;
		}
	}

	public void GameOver()
	{
		IsGameOver = true;
		GameManager.Instance.UpdateGameState(GameManager.GameState.GameOver);
		OverlayCanvas.Instance.ShowHideGameOverPanel(true);
	}

	void Update()
	{
		if (IsMoving)
		{
			transform.position = Vector3.MoveTowards(transform.position, MoveTarget, m_moveSpeed * Time.deltaTime);
			if (transform.position == MoveTarget)
			{
				IsMoving = false;
				var cellData = m_board.GetCellData(m_gridPos);
				if (cellData.ContainedObject != null)
				{
					cellData.ContainedObject.PlayerEntered();
				}
			}
		}
	}

	private void ProcessPlayerAction(BoardManager.Direction a_direction)
	{
		// Check the direction if there are anything that the player would attack.
		bool attackedSomething = false;
		var cellPosList = m_board.GetAttackAreaCellPositions(m_attackAreaSetting, m_gridPos, a_direction);
		foreach (var targetCellPos in cellPosList)
		{
			var data = m_board.GetCellData(targetCellPos);
			if (data.ContainedObject && data.ContainedObject.m_canBeAttacked)
			{
				attackedSomething = true;
				PeacefulTurns = 0;

				Vector3 cellWorldPos = m_board.CellPosToWorldPos(targetCellPos);
				VFXManager.Instance.PlaySlashEffect(cellWorldPos, Color.cyan);

				data.ContainedObject.GetAttacked(1);
				OnAttackLanded?.Invoke(data.ContainedObject, a_direction);
			}
		}

		if (!attackedSomething)
		{
			Vector3Int newCellTargetPos = m_gridPos;
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
				if (cellData.ContainedObject == null)
				{
					MoveTo(newCellTargetPos);
				}
				else if (cellData.ContainedObject.PlayerWantsToEnter())
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

		OnUpgradeAdded?.Invoke(a_upgrade);
	}

	IEnumerator HitEffectCoroutine()
	{
		Vector3 hitScale = m_originalScale * m_hitScaleMultiplier;
		float halfDuration = m_hitEffectDuration / 2f;
		float timer = 0f;

		while (timer < halfDuration)
		{
			transform.localScale = Vector3.Lerp(m_originalScale, hitScale, timer / halfDuration);
			timer += Time.deltaTime;
			yield return null;
		}

		timer = 0f;

		while (timer < halfDuration)
		{
			transform.localScale = Vector3.Lerp(hitScale, m_originalScale, timer / halfDuration);
			timer += Time.deltaTime;
			yield return null;
		}

		transform.localScale = m_originalScale;
		m_hitScaleEffectCoroutine = null;
	}

	//----------------------------------------------------------------
	// Inputs
	#region Inputs
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
	#endregion
}