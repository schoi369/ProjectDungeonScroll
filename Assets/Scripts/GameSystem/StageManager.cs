using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class StageManager : MonoBehaviour
{
	public static StageManager Instance { get; private set; }

	public enum GameState
	{
		GameStart,
		PlayerTurn,
		WorldTurn,
		UpgradeSelection,
		GameOver,
	}
	public GameState CurrentState { get; private set; }
	public event Action<GameState> OnGameStateChanged;

	public UpgradeDatabase m_upgradeDatabase;

	public PlayerController m_player;
	public BoardManager m_boardManager;
	public int m_collapseTurnInterval = 5;
	int m_turnCounterForCollapse = 0;
	int m_nextCollapseColumnIndex = 0;


	int FloorCount { get; set; } = 0;
	private List<EnemyBase> m_enemies = new List<EnemyBase>();

	public event Action OnPlayerTurnEnded;
	public event Action<EnemyBase> OnEnemyDefeated;

	void Awake()
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

	public void Start()
	{
		StartStage();
	}

	public void StartStage()
	{
		m_turnCounterForCollapse = 0;
		m_nextCollapseColumnIndex = 0;

		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.GameStarted);

		OverlayCanvas.Instance.ShowHideGameOverPanel(false);

		m_boardManager.Init();

		m_player.Init();
		m_player.Spawn(m_boardManager);

		UpdateGameState(GameState.PlayerTurn);
	}

	public bool IsPlayerAt(Vector3Int a_tilemapPos)
	{
		return m_player.TilemapPos == a_tilemapPos;
	}

	public void UpdateGameState(GameState a_newState)
	{
		CurrentState = a_newState;

		switch (CurrentState)
		{
			case GameState.GameStart:
				// 향후 게임 시작 로직 (e.g. 인트로)
				break;
			case GameState.PlayerTurn:
				// 플레이어 턴 시작 로직
				break;
			case GameState.WorldTurn:
				StartCoroutine(ProcessWorldTurn());
				break;
			case GameState.UpgradeSelection:
				Debug.Log("업그레이드 선택 상태로 진입.");
				break;
			case GameState.GameOver:
				// 게임 오버 로직
				break;
		}

		OnGameStateChanged?.Invoke(CurrentState);
	}

	public void EndPlayerTurn()
	{
		if (CurrentState == GameState.PlayerTurn)
		{
			OnPlayerTurnEnded?.Invoke();

			m_turnCounterForCollapse++;

			if (m_turnCounterForCollapse == m_collapseTurnInterval - 1)
			{
				m_boardManager.WarnColumn(m_nextCollapseColumnIndex);
			}
			else if (m_turnCounterForCollapse >= m_collapseTurnInterval)
			{
				m_turnCounterForCollapse = 0;
				m_boardManager.DestroyColumn(m_nextCollapseColumnIndex);
				m_nextCollapseColumnIndex++;
			}

			// 플레이어 턴이 끝날 때 레벨업 처리.
			if (m_player.PendingLevelUps > 0)
			{
				m_player.ConsumePendingLevelUp();
				StartUpgradeSelection();
			}
			else
			{
				UpdateGameState(GameState.WorldTurn);
			}
		}
	}

	public void RegisterEnemy(EnemyBase enemy)
	{
		if (!m_enemies.Contains(enemy))
		{
			m_enemies.Add(enemy);
		}
	}

	public void UnregisterEnemy(EnemyBase enemy)
	{
		if (m_enemies.Contains(enemy))
		{
			m_enemies.Remove(enemy);
		}
	}

	private IEnumerator ProcessWorldTurn()
	{
		// 월드 턴 동안 플레이어 조작 방지 (필요 시 UI 피드백 추가)

		var enemiesToProcess = new List<EnemyBase>(m_enemies);
		foreach (var enemy in enemiesToProcess)
		{
			if (enemy != null && !enemy.IsDead)
			{
				enemy.ExecuteTurn();
				yield return new WaitForSeconds(0.1f); // 적들의 행동을 순차적으로 보여주기 위한 딜레이
			}
		}

		// 모든 적의 행동이 끝나면 플레이어 턴으로 전환
		if (CurrentState != GameState.GameOver) // 플레이어가 적의 턴에 죽었을 수 있으므로 체크
		{
			UpdateGameState(GameState.PlayerTurn);
		}
	}

	public void StartUpgradeSelection()
	{
		UpdateGameState(GameState.UpgradeSelection);

		var playerUpgrades = GameManager.Instance.CurrentPlayerData.m_acquiredUpgrades;
		var availableUpgrades = m_upgradeDatabase.m_allUpgrades.
			Where(upgrade => !playerUpgrades.Contains(upgrade)).ToList();

		// 최대 3개를 선택 (가능한 업그레이드가 3개 미만이면 그만큼만 선택)
		var options = GetLevelUpUpgradeOptions();

		if (options.Count > 0)
		{
			OverlayCanvas.Instance.ShowUpgradeSelection(options);
		}
		else
		{
			Debug.Log("선택할 수 있는 업그레이드가 더 이상 없습니다!");
			EndUpgradeSelection();
		}
	}

	public void EndUpgradeSelection()
	{
		OverlayCanvas.Instance.HideUpgradeSelection();

		if (m_player.PendingLevelUps > 0)
		{
			m_player.ConsumePendingLevelUp();
			StartUpgradeSelection();
		}
		else
		{
			UpdateGameState(GameState.WorldTurn);
		}
	}

	/// <summary>
	/// 레벨업 시 제공할 업그레이드 선택지를 생성합니다. (단순화된 규칙 적용)
	/// </summary>
	/// <returns>플레이어에게 보여줄 업그레이드 SO 리스트 (최대 3개)</returns>
	public List<UpgradeSO> GetLevelUpUpgradeOptions()
	{
		var acquiredUpgrades = GameManager.Instance.CurrentPlayerData.m_acquiredUpgrades;
		List<UpgradeSO> availableUpgrades = m_upgradeDatabase.m_allUpgrades.Except(acquiredUpgrades).ToList();

		// --- 최종 선택지를 담을 리스트와, 뽑을 멤버 목록 정의 ---
		// TODO: 지금은 직접 멤버를 지정하지만, 나중엔 누구누구 있는지 참조해서.
		List<UpgradeSO> finalOptions = new List<UpgradeSO>();
		var membersToPickFrom = new List<EIdolMember> { EIdolMember.Sakura, EIdolMember.Kazuha, EIdolMember.Yunjin };

		// --- 각 멤버 순서대로 추첨 ---
		foreach (var member in membersToPickFrom)
		{
			// 뽑을 업그레이드가 더이상 없으면 즉시 종료
			if (availableUpgrades.Count == 0)
			{
				break;
			}

			// 해당 멤버의 전용 업그레이드 중, 아직 선택 가능한 것들만 추림
			List<UpgradeSO> specificMemberPool = availableUpgrades.Where(u => u.m_idolMember == member).ToList();

			UpgradeSO pickedUpgrade = null;

			// 규칙 1: 전용 풀에 뽑을 것이 있으면 거기서 뽑기
			if (specificMemberPool.Count > 0)
			{
				int randomIndex = UnityEngine.Random.Range(0, specificMemberPool.Count);
				pickedUpgrade = specificMemberPool[randomIndex];
			}
			// 규칙 2: 전용 풀이 비었다면, 남은 전체 풀에서 대신 뽑기
			else
			{
				int randomIndex = UnityEngine.Random.Range(0, availableUpgrades.Count);
				pickedUpgrade = availableUpgrades[randomIndex];
			}

			// 뽑은 업그레이드를 최종 목록에 추가하고, 전체 가능 목록에서는 제거 (중복 방지)
			finalOptions.Add(pickedUpgrade);
			availableUpgrades.Remove(pickedUpgrade);
		}

		// 규칙 3: 위의 과정을 거쳐 만들어진 최종 목록을 반환 (0~3개의 아이템을 가질 수 있음)
		return finalOptions;
	}

	public void KickEnemyDefeatedEvent(EnemyBase a_enemy)
	{
		OnEnemyDefeated?.Invoke(a_enemy);
	}
}