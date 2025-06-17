using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance; // In-Scene Singleton
	public PlayerDataSO CurrentPlayerData { get; private set; }

	// Board, Position
	BoardManager m_board;
	Vector3Int m_tilemapPos;
	public Vector3Int TilemapPos => m_tilemapPos;
	BoardManager.Direction m_facingDirection = BoardManager.Direction.RIGHT; // 기본 방향을 오른쪽으로 설정

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
	public event Action OnDamagedByEnemy; // 플레이어가 적에게 공격받았을 때 발생하는 이벤트. (업그레이드 효과 적용을 위해 사용)

	//
	private readonly HashSet<CellObject> m_hitTargetsThisAction = new();

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
		PlayerTelegraphManager.Instance.HideAllVisuals();

		m_tilemapPos = a_newTilemapPos;

		if (instant)
		{
			IsMoving = false;
			transform.position = m_board.TilemapPosToWorldPos(m_tilemapPos);
			UpdateAttackTelegraph();
		}
		else
		{
			IsMoving = true;
			MoveTarget = m_board.TilemapPosToWorldPos(m_tilemapPos);
		}
	}

	public void TakeDamage(int a_damage, bool a_fromEnemy)
	{
		if (IsGameOver)
		{
			return;
		}

		if (a_fromEnemy)
		{
			OnDamagedByEnemy?.Invoke();
		}

		if (m_hitScaleEffectCoroutine != null)
		{
			StopCoroutine(m_hitScaleEffectCoroutine);
		}
		transform.localScale = m_originalScale;
		m_hitScaleEffectCoroutine = StartCoroutine(HitEffectCoroutine());

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
				UpdateAttackTelegraph();

				var cellData = m_board.GetCellData(m_tilemapPos);
				if (cellData.ContainedObject != null)
				{
					cellData.ContainedObject.PlayerEntered();
				}
			}
		}
	}

	/// <summary>
	/// 현재 위치에서 모든 방향에 대한 공격 범위를 계산하고 텔레그래핑을 업데이트합니다.
	/// </summary>
	private void UpdateAttackTelegraph()
	{
		if (m_board == null || CurrentPlayerData == null) return;

		HashSet<Vector3Int> uniqueAttackTiles = new HashSet<Vector3Int>();

		// 모든 방향에 대해
		foreach (var attackAreaSO in CurrentPlayerData.m_memberAttackAreas)
		{
			var tiles = m_board.GetAttackAreaCellPositions(attackAreaSO, m_tilemapPos, m_facingDirection);
			uniqueAttackTiles.UnionWith(tiles);
		}

		// 플레이어 자신의 위치는 텔레그래핑에서 제외
		uniqueAttackTiles.Remove(m_tilemapPos);

		// 매니저에게 최종 타일 목록을 전달하여 표시 요청
		PlayerTelegraphManager.Instance.ShowVisuals(uniqueAttackTiles.ToList());
	}


	private void ProcessPlayerAction(BoardManager.Direction a_direction)
	{
		m_hitTargetsThisAction.Clear();

		m_facingDirection = a_direction;

		// 통합된 공격 범위와 실제 공격 성공 여부를 추적할 변수
		HashSet<Vector3Int> uniqueAttackTiles = new HashSet<Vector3Int>();
		bool attackedSomething = false;

		// 모든 멤버의 공격 범위를 순회하며 통합
		foreach (var memberAttackArea in CurrentPlayerData.m_memberAttackAreas)
		{
			if (memberAttackArea != null)
			{
				var tilemapPosList = m_board.GetAttackAreaCellPositions(memberAttackArea, m_tilemapPos, a_direction);
				uniqueAttackTiles.UnionWith(tilemapPosList);
			}
		}

		// 통합된 공격 범위에 있는 적들을 공격
		foreach (var targetTilemapPos in uniqueAttackTiles)
		{
			var data = m_board.GetCellData(targetTilemapPos);
			if (data.ContainedObject && data.ContainedObject.m_canBeAttacked)
			{
				if (!m_hitTargetsThisAction.Contains(data.ContainedObject))
				{
					// 공격이 성공하는 순간, 이동 로직을 막기 위해 플래그를 true로 설정
					attackedSomething = true;

					Vector3 cellWorldPos = m_board.TilemapPosToWorldPos(targetTilemapPos);
					VFXManager.Instance.PlaySlashEffect(cellWorldPos, Color.cyan);

					data.ContainedObject.GetAttacked(CurrentPlayerData.m_attackPower);

					m_hitTargetsThisAction.Add(data.ContainedObject);

					OnAttackLanded?.Invoke(data.ContainedObject, a_direction);

					UpdateAttackTelegraph();
				}
			}
		}

		// 만약 어떤 적도 공격하지 않았다면, 해당 방향으로 이동 시도
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
				if (cellData.ContainedObject == null || cellData.ContainedObject.PlayerWantsToEnter())
				{
					MoveTo(newCellTargetPos);
				}
			}
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

	/// <summary>
	/// 특정 업그레이드의 현재 카운터 값을 UI에 표시하기 위해 문자열로 반환합니다.
	/// </summary>
	public string GetCounterValueForUpgrade(UpgradeSO a_upgrade)
	{
		if (a_upgrade.m_counterType == UpgradeSO.CounterType.None)
		{
			return "";
		}

		switch (a_upgrade.m_counterType)
		{
			case UpgradeSO.CounterType.PeacefulTurns:
				var serenityEffect = GetComponent<UpgradeSerenity>();
				if (serenityEffect != null)
				{
					return serenityEffect.CurrentTurnCount.ToString();
				}
				return "?";
			case UpgradeSO.CounterType.RiskyDashTurns:
				var riskyDashEffect = GetComponent<UpgradeRiskyDash>();
				if (riskyDashEffect != null)
				{
					return riskyDashEffect.CurrentTurnCount.ToString();
				}
				return "?"; // 효과 컴포넌트를 못찾은 경우
			default:
				return "";
		}
	}


	// Debug
	[InspectorButton]
	void DebugDie()
	{
		TakeDamage(100, true);
	}

	[InspectorButton]
	void DebugLevelUp()
	{
		int expLeft = CurrentPlayerData.m_expToNextLevel - CurrentPlayerData.m_currentExp;

		GainExp(expLeft);
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