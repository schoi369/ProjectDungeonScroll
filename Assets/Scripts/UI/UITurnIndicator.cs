using UnityEngine;

public class UITurnIndicator : MonoBehaviour
{
	[Header("UI Object References")]
	public GameObject m_playerTurnDisplay; // 플레이어 턴에 켤 오브젝트
	public GameObject m_worldTurnDisplay;  // 월드 턴에 켤 오브젝트

	void OnEnable()
	{
		// GameManager의 상태 변경 이벤트를 구독
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnGameStateChanged += HandleGameStateChanged;
		}
	}

	void OnDisable()
	{
		// 오브젝트가 비활성화되거나 파괴될 때 구독을 해제 (메모리 누수 방지)
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
		}
	}

	/// <summary>
	/// 게임 상태 변경 이벤트를 받았을 때 호출될 메서드
	/// </summary>
	private void HandleGameStateChanged(StageManager.GameState a_newState)
	{
		if (a_newState == StageManager.GameState.PlayerTurn)
		{
			m_playerTurnDisplay.SetActive(true);
			m_worldTurnDisplay.SetActive(false);
		}
		else if (a_newState == StageManager.GameState.WorldTurn)
		{
			m_playerTurnDisplay.SetActive(false);
			m_worldTurnDisplay.SetActive(true);
		}
		else // UpgradeSelection, GameOver 등 다른 상태일 경우
		{
			// 둘 다 끈다.
			m_playerTurnDisplay.SetActive(false);
			m_worldTurnDisplay.SetActive(false);
		}
	}
}