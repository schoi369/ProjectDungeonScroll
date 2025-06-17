using UnityEngine;

public class UpgradeFirstAid : MonoBehaviour
{
	public FirstAidUpgradeSO SourceSO { get; set; }
	private PlayerController m_player;

	private void Awake()
	{
		m_player = GetComponent<PlayerController>();
	}

	private void OnEnable()
	{
		// StageManager의 '적 처치' 이벤트를 구독합니다.
		StageManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
	}

	private void OnDisable()
	{
		// 컴포넌트 비활성화 시 이벤트 구독을 해제합니다.
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
		}
	}

	private void OnEnemyDefeated(EnemyBase a_defeatedEnemy)
	{
		if (m_player != null && SourceSO != null)
		{
			Debug.Log($"'응급 치료' 효과 발동! HP {SourceSO.m_healAmount} 회복.");
			m_player.Heal(SourceSO.m_healAmount);
		}
	}
}