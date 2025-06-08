using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public BoardManager m_boardManager;
	public PlayerController m_player;

	public TurnManager TurnManager { get; private set; }

	int FloorCount { get; set; } = 0;

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
		TurnManager = new TurnManager();
		TurnManager.OnTick += OnTurnHappen;

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
	}

	public void NewLevel()
	{
		FloorCount++;
		SetFloorCount(FloorCount);

		m_boardManager.Clean();
		m_boardManager.Init();
		m_player.Spawn(m_boardManager, new Vector2Int(1, 1));
	}

	public bool IsPlayerAt(Vector2Int a_cellPos)
	{
		if (m_player == null)
		{
			return false;
		}
		return m_player.CellPos == a_cellPos;
	}

	void OnTurnHappen()
	{
		Debug.Log("GameManager: OnTurnHappen");
	}

	void SetFloorCount(int a_newCount)
	{
		FloorCount = a_newCount;
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.FloorChanged, FloorCount);
	}
}
