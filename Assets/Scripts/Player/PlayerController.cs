using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance; // In-Scene Singleton

	PlayerDataSO CurrentPlayerData { get; set; }

	// Board, Position
	BoardManager m_board;
	Vector3Int m_tilemapPos;
	public Vector3Int TilemapPos => m_tilemapPos;

	// Stage stat
	public bool IsGameOver { get; set; } = false;
	public float m_moveSpeed = 5f;
	bool IsMoving { get; set; }
	Vector3 MoveTarget { get; set; }

	// Level Related
	int m_pendingLevelUps = 0;
	public int PendingLevelUps => m_pendingLevelUps;

	// Upgrades
	public event Action<CellObject, BoardManager.Direction> OnAttackLanded; // 일부 Upgrade 적용을 위해 사용하는 멤버 변수.
	public int PeacefulTurns { get; set; } = 0;

	[Header("Visuals")]
	public float m_hitScaleMultiplier = 1.2f;
	public float m_hitEffectDuration = 0.1f;
	Vector3 m_originalScale;
	Coroutine m_hitScaleEffectCoroutine;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnEnable()
	{
		GameInputManager.onMoveUp += OnInputMoveUp;
		GameInputManager.onMoveDown += OnInputMoveDown;
		GameInputManager.onMoveRight += OnInputMoveRight;
		GameInputManager.onMoveLeft += OnInputMoveLeft;
		GameInputManager.onGameOverRestart += OnInputRestart;
	}

	private void OnDisable()
	{
		GameInputManager.onMoveUp -= OnInputMoveUp;
		GameInputManager.onMoveDown -= OnInputMoveDown;
		GameInputManager.onMoveRight -= OnInputMoveRight;
		GameInputManager.onMoveLeft -= OnInputMoveLeft;
		GameInputManager.onGameOverRestart -= OnInputRestart;
	}

	/// <summary>
	/// Stage 시작 때의 Init
	/// </summary>
	public void Init()
	{
		CurrentPlayerData = GameManager.Instance.CurrentPlayerData;

		//
		IsMoving = false;
		IsGameOver = false;

		// 비주얼
		m_originalScale = transform.localScale;

		// 레벨, 경험치 관련 초기화
		m_pendingLevelUps = 0;

		// 업그레이드 관련 변수 초기화
		PeacefulTurns = 0;

		// 사실상 HUD의 Refresh
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerMaxHPChanged, CurrentPlayerData.m_maxHP);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentPlayerData.m_currentHP);

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerLevelChanged, CurrentPlayerData.m_level);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerExpChanged, (CurrentPlayerData.m_currentExp, CurrentPlayerData.m_expToNextLevel));

		// 존재하는 업그레이드 적용
		foreach (var upgrade in CurrentPlayerData.m_acquiredUpgrades)
		{
			upgrade.Apply(gameObject);
			OverlayCanvas.Instance.AddUpgradeIcon(upgrade);
		}
	}

	/// <summary>
	/// 플레이어를 보드의 시작 위치로 이동시키는 메소드.
	/// </summary>
	public void Spawn(BoardManager a_boardManager)
	{
		m_board = a_boardManager;
		MoveTo(m_board.PlayerSpawnPosition, instant: true);
	}

	public void MoveTo(Vector3Int a_newTilemapPos, bool instant = false)
	{
		m_tilemapPos = a_newTilemapPos;

		if (instant)
		{
			IsMoving = false;
			transform.position = m_board.TilemapPosToWorldPos(m_tilemapPos);
		}
		else
		{
			IsMoving = true;
			MoveTarget = m_board.TilemapPosToWorldPos(m_tilemapPos);
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

		CurrentPlayerData.m_currentHP = Mathf.Max(CurrentPlayerData.m_currentHP - a_damage, 0); // HP가 0 미만으로 내려가지 않도록 보장

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentPlayerData.m_currentHP);

		if (CurrentPlayerData.m_currentHP <= 0)
		{
			GameOver();
		}
	}

	public void Heal(int a_heal)
	{
		CurrentPlayerData.m_currentHP = Mathf.Min(CurrentPlayerData.m_currentHP + a_heal, CurrentPlayerData.m_maxHP);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, CurrentPlayerData.m_currentHP);
	}

	public void GainExp(int a_amount)
	{
		CurrentPlayerData.m_currentExp += a_amount;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerExpChanged, (CurrentPlayerData.m_currentExp, CurrentPlayerData.m_expToNextLevel));

		while (CurrentPlayerData.m_currentExp >= CurrentPlayerData.m_expToNextLevel)
		{
			LevelUp();
		}
	}

	void LevelUp()
	{
		CurrentPlayerData.m_level++;
		CurrentPlayerData.m_currentExp -= CurrentPlayerData.m_expToNextLevel; // 넘긴 경험치 남기기.

		//m_expToLevelUp = (int) (m_expToLevelUp * 1.5f); // 필요 경험치 증가 예시.

		m_pendingLevelUps++; // 레벨업 처리 예약 +1

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerLevelChanged, CurrentPlayerData.m_level);
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerExpChanged, (CurrentPlayerData.m_currentExp, CurrentPlayerData.m_expToNextLevel));
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
		StageManager.Instance.UpdateGameState(StageManager.GameState.GameOver);
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
				var cellData = m_board.GetCellData(m_tilemapPos);
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
		var tilemapPosList = m_board.GetAttackAreaCellPositions(CurrentPlayerData.m_attackAreaSetting, m_tilemapPos, a_direction);
		foreach (var targetTilemapPos in tilemapPosList)
		{
			var data = m_board.GetCellData(targetTilemapPos);
			if (data.ContainedObject && data.ContainedObject.m_canBeAttacked)
			{
				attackedSomething = true;
				PeacefulTurns = 0;

				Vector3 cellWorldPos = m_board.TilemapPosToWorldPos(targetTilemapPos);
				VFXManager.Instance.PlaySlashEffect(cellWorldPos, Color.cyan);

				data.ContainedObject.GetAttacked(1);
				OnAttackLanded?.Invoke(data.ContainedObject, a_direction);
			}
		}

		if (!attackedSomething)
		{
			// 공격하지 않았을 경우 이동.
			Vector3Int newCellTargetPos = m_tilemapPos; // 우선 현재의 위치로.
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
		StageManager.Instance.EndPlayerTurn();
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

	[InspectorButton]
	void DebugDie()
	{
		TakeDamage(100);
	}

	//----------------------------------------------------------------
	// Inputs
	#region Inputs
	private bool CanAcceptMoveInput()
	{
		return !IsMoving && !IsGameOver && StageManager.Instance.CurrentState == StageManager.GameState.PlayerTurn;
	}

	void OnInputMoveUp()
	{
		if (CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.UP);
		}
	}

	void OnInputMoveDown()
	{
		if (CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.DOWN);
		}
	}

	void OnInputMoveRight()
	{
		if (CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.RIGHT);
		}
	}

	void OnInputMoveLeft()
	{
		if (CanAcceptMoveInput())
		{
			ProcessPlayerAction(BoardManager.Direction.LEFT);
		}
	}

	void OnInputRestart()
	{
		if (IsGameOver)
		{
			GameManager.Instance.RestartRun();
		}
	}
	#endregion
}