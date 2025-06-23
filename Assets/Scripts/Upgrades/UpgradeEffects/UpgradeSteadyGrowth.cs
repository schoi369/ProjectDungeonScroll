using UnityEngine;
using static Unity.VisualScripting.Member;

public class UpgradeSteadyGrowth : UpgradeEffectBase<SteadyGrowthUpgradeSO>
{
	private int m_peacefulMoveCounter = 0;
	public int PeacefulMoveCounter => m_peacefulMoveCounter;
	// 이 업그레이드를 통해 이번 스테이지에서 얻은 총 추가 공격력을 기록합니다.
	private int m_totalBonusFromThisStage = 0;

	private void OnEnable()
	{
		// 필요한 이벤트들을 구독합니다.
		if (m_player != null)
		{
			m_player.OnPeacefulMove += OnPeacefulMove;
			m_player.OnAttackLanded += OnActionInterrupted;
			m_player.OnDamagedByEnemy += OnDamagedByEnemy;
		}
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.FloorChanged, OnFloorChanged);
	}

	private void OnDisable()
	{
		// 구독을 해제합니다.
		if (m_player != null)
		{
			m_player.OnPeacefulMove -= OnPeacefulMove;
			m_player.OnAttackLanded -= OnActionInterrupted;
			m_player.OnDamagedByEnemy -= OnDamagedByEnemy;
		}
		if (CustomEventManager.Instance != null)
		{
			CustomEventManager.Instance.Unsubscribe(CustomEventManager.CustomGameEvent.FloorChanged, OnFloorChanged);
		}
	}

	private void OnDestroy()
	{
		// 컴포넌트가 완전히 파괴될 때(런 종료 등), 쌓아둔 스탯을 되돌립니다.
		RevertStats();
	}

	// '평화로운 이동' 시 호출될 메서드
	private void OnPeacefulMove()
	{
		m_peacefulMoveCounter++;

		if (m_peacefulMoveCounter >= SourceSO.m_movesRequired)
		{
			int bonus = SourceSO.m_attackPowerBonus;
			m_player.CurrentPlayerData.m_extraAttackPower += bonus;
			m_totalBonusFromThisStage += bonus; // 이번 스테이지에서 얻은 보너스 기록

			m_peacefulMoveCounter = 0; // 카운터 초기화 후 다시 시작

			Debug.Log($"'꾸준한 성장' 효과 발동! 공격력 +{bonus}.");
			CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.UpgradeEffectTriggered, SourceSO);
		}
	}

	// 공격/피격으로 '평화로운' 상태가 깨졌을 때 호출될 메서드
	private void OnActionInterrupted(CellObject target, BoardManager.Direction direction) // 다양한 이벤트 시그니처를 받기 위함
	{
		m_peacefulMoveCounter = 0;
	}

	private void OnDamagedByEnemy()
	{
		// 적으로부터 피해를 입으면 평화로운 턴 카운터를 리셋합니다.
		m_peacefulMoveCounter = 0;
	}


	// 새 스테이지 시작 시 호출될 메서드
	private void OnFloorChanged(object _ = null)
	{
		Debug.Log($"새 스테이지 시작. '꾸준한 성장' 스택 초기화. 이전 스테이지 보너스({m_totalBonusFromThisStage}) 제거.");
		// 1. 이전 스테이지에서 쌓았던 추가 공격력을 되돌립니다.
		RevertStats();
		// 2. 평화로운 이동 카운터도 초기화합니다.
		m_peacefulMoveCounter = 0;
	}

	private void RevertStats()
	{
		if (m_player != null)
		{
			m_player.CurrentPlayerData.m_extraAttackPower -= m_totalBonusFromThisStage;
		}
		m_totalBonusFromThisStage = 0;
	}
}