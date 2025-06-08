using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public enum GameState
	{
		GameStart,
		PlayerTurn,
		WorldTurn,
		GameOver,
	}
	public GameState CurrentState { get; private set; }

	public BoardManager m_boardManager;
	public PlayerController m_player;

	int FloorCount { get; set; } = 0;
	private List<EnemyBase> m_enemies = new List<EnemyBase>();

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	void Start()
	{
		StartNewGame();
	}

	public void StartNewGame()
	{
		OverlayCanvas.Instance.ShowHideGameOverPanel(false);

		SetFloorCount(1);

		m_boardManager.Clean();
		m_boardManager.Init();

		m_player.Init(); // Only at StartNewGame, not at NewLevel().

		m_player.Spawn(m_boardManager, new Vector2Int(1, 1)); // Player Start Pos. Need Refactoring, directly set in multiple places.

		UpdateGameState(GameState.PlayerTurn);
	}

	public void NewLevel()
	{
		FloorCount++;
		SetFloorCount(FloorCount);

		m_boardManager.Clean();
		m_boardManager.Init();
		m_player.Spawn(m_boardManager, new Vector2Int(1, 1));

		UpdateGameState(GameState.PlayerTurn);
	}

	public bool IsPlayerAt(Vector2Int a_cellPos)
	{
		if (m_player == null)
		{
			return false;
		}
		return m_player.CellPos == a_cellPos;
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
			case GameState.GameOver:
				// 게임 오버 로직
				break;
		}
	}

	public void EndPlayerTurn()
	{
		if (CurrentState == GameState.PlayerTurn)
		{
			UpdateGameState(GameState.WorldTurn);
		}
	}

	void SetFloorCount(int a_newCount)
	{
		FloorCount = a_newCount;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.FloorChanged, FloorCount);
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
}