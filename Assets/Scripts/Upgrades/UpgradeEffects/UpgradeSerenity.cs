using UnityEngine;

#if OLD_UPGRADE
public class UpgradeSerenity : UpgradeEffectBase<SerenityUpgradeSO>
{
	private int m_peacefulTurnCounter = 0;
	public int CurrentTurnCount => m_peacefulTurnCounter;

	private void OnEnable()
	{
		// 턴 종료 시 카운터 증가, 공격 시 카운터 리셋
		StageManager.Instance.OnPlayerTurnEnded += OnPlayerTurnEnd;

		if (m_player != null)
		{
			m_player.OnAttackLanded += OnPlayerAttack;
			m_player.OnDamagedByEnemy += OnPlayerDamagedByEnemy;
		}
	}

	private void OnDisable()
	{
		// 컴포넌트 파괴 시 모든 이벤트 구독 해제
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnPlayerTurnEnded -= OnPlayerTurnEnd;
		}

		if (m_player != null)
		{
			m_player.OnAttackLanded -= OnPlayerAttack;
			m_player.OnDamagedByEnemy -= OnPlayerDamagedByEnemy;
		}
	}

	private void OnPlayerTurnEnd()
	{
		m_peacefulTurnCounter++;

		if (m_peacefulTurnCounter >= SourceSO.m_turnsRequired)
		{
			m_player.Heal(SourceSO.m_healAmount);
			m_peacefulTurnCounter = 0;

			CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.UpgradeEffectTriggered, SourceSO);
		}
	}

	// 플레이어가 공격에 성공하면 평화로운 턴 카운터를 리셋
	private void OnPlayerAttack(CellObject a_target, BoardManager.Direction a_direction)
	{
		m_peacefulTurnCounter = 0;
	}

	private void OnPlayerDamagedByEnemy()
	{
		m_peacefulTurnCounter = 0;
	}
}
#endif