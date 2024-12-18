using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public BoardManager m_boardManager;
	public PlayerController m_player;

	public TurnManager TurnManager { get; private set; }

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
		m_boardManager.Clean();
		m_boardManager.Init();

		m_player.Spawn(m_boardManager, new Vector2Int(1, 1)); // Player Start Pos. Need Refactoring, directly set in multiple places.
	}

	void OnTurnHappen()
	{
		Debug.Log("GameManager: OnTurnHappen");
	}
}
