using UnityEngine;

public class UpgradeRiskyDash : MonoBehaviour
{
	public RiskyDashUpgradeSO SourceSO { get; set; }

	// --- 내부 상태 변수 ---
	private PlayerController m_player;
	private int m_turnCounter = 0;

	// UI 표시를 위한 프로퍼티
	public int CurrentTurnCount => m_turnCounter;

	private void Awake()
	{
		m_player = GetComponent<PlayerController>();
	}

	private void OnEnable()
	{
		StageManager.Instance.OnPlayerTurnEnded += OnPlayerTurnEnd;
	}

	private void OnDisable()
	{
		// 컴포넌트가 파괴될 때(업그레이드 제거 시) 이벤트 구독을 해제하고, 얻었던 공격력을 원상복구
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnPlayerTurnEnded -= OnPlayerTurnEnd;
		}

		if (m_player != null)
		{
			m_player.CurrentPlayerData.m_attackPower -= SourceSO.m_attackPowerGain;
			Debug.Log($"'위험한 질주' 효과 제거. 공격력 {SourceSO.m_attackPowerGain} 감소.");
		}
	}

	private void Start()
	{
		m_player.CurrentPlayerData.m_attackPower += SourceSO.m_attackPowerGain;
		Debug.Log($"'위험한 질주' 획득! 공격력 {SourceSO.m_attackPowerGain} 증가.");
	}

	private void OnDestroy()
	{
		if (m_player != null)
		{
			m_player.CurrentPlayerData.m_attackPower -= SourceSO.m_attackPowerGain;
			Debug.Log($"'위험한 질주' 효과 제거. 공격력 {SourceSO.m_attackPowerGain} 감소.");
		}
	}

	private void OnPlayerTurnEnd()
	{
		m_turnCounter++;

		if (m_turnCounter >= SourceSO.m_turnsRequired)
		{
			Debug.Log("'위험한 질주' HP 감소 효과 발동!");
			m_player.TakeDamage(SourceSO.m_hpCost, false);
			m_turnCounter = 0;
		}
	}
}