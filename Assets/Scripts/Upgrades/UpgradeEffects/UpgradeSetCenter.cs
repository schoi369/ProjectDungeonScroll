using UnityEngine;
using static Unity.VisualScripting.Member;

// 모든 효과 컴포넌트의 부모인 UpgradeEffect를 상속받습니다.
public class UpgradeSetCenter : UpgradeEffectBase<SetCenterUpgradeSO>
{
	private int m_attackPowerBeforeApply; // 이 효과가 적용되기 직전의 공격력을 기억할 변수
	private bool m_isApplied = false;     // 효과가 적용되었는지 여부를 확인하는 플래그

	private void Start()
	{
		if (m_player != null && SourceSO != null)
		{
			// 현재 공격력을 '기억'합니다.
			m_attackPowerBeforeApply = m_player.CurrentPlayerData.m_attackPower;

			// SO에 설정된 새로운 공격력으로 '변경'합니다.
			m_player.CurrentPlayerData.m_attackPower = SourceSO.m_centerAttackPower;
			m_isApplied = true;
			Debug.Log($"센터 '{SourceSO.m_memberToSetAsCenter}' 효과 적용. 공격력이 {m_attackPowerBeforeApply}에서 {SourceSO.m_centerAttackPower}(으)로 변경됩니다.");
		}
	}

	private void OnDestroy()
	{
		if (m_isApplied && m_player != null)
		{
			// 현재 플레이어의 센터가 '나'일 경우에만 공격력을 되돌립니다.
			// 다른 센터 업그레이드에 의해 내가 제거된 경우에는, 이 조건이 false가 되어
			// 새로운 센터의 공격력을 덮어쓰지 않습니다.
			if (m_player.CurrentPlayerData.m_currentCenter == SourceSO.m_memberToSetAsCenter)
			{
				m_player.CurrentPlayerData.m_attackPower = m_attackPowerBeforeApply;
				Debug.Log($"센터 '{SourceSO.m_memberToSetAsCenter}' 효과 제거. 공격력이 {m_attackPowerBeforeApply}(으)로 복구됩니다.");
			}
		}
	}
}