using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

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

		// 제공 가능한 업그레이드들을 무작위로 섞음
		var shuffledUpgrades = availableUpgrades.OrderBy(x => UnityEngine.Random.value).ToList();

		// 최대 3개를 선택 (가능한 업그레이드가 3개 미만이면 그만큼만 선택)
		var options = shuffledUpgrades.Take(3).ToList();

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
}