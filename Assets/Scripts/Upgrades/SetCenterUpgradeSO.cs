using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "SetCenterUpgrade", menuName = "Upgrades/Set Center Upgrade")]
public class SetCenterUpgradeSO : UpgradeSO
{
	[Header("Center Settings")]
	[Tooltip("이 업그레이드가 '센터'로 지정할 아이돌 멤버입니다.")]
	public EIdolMember m_memberToSetAsCenter;

	[Tooltip("이 센터로 지정되었을 때 설정될 기본 공격력입니다.")]
	public int m_centerAttackPower = 1;

	private readonly System.Type m_effectComponentType = typeof(UpgradeSetCenter);

	public override void Apply(GameObject playerObject)
	{
		base.Apply(playerObject);

		if (!playerObject.TryGetComponent<PlayerController>(out var player)) return;
		var playerData = player.CurrentPlayerData;

		// 기존 센터 업그레이드를 찾아 제거를 요청.
		UpgradeSO oldCenterUpgrade = playerData.m_acquiredUpgrades
			.FirstOrDefault(upgrade => upgrade is SetCenterUpgradeSO && upgrade != this);
		if (oldCenterUpgrade != null)
		{
			playerData.RemoveUpgrade(oldCenterUpgrade);
		}

		// 현재 센터를 이 업그레이드가 지정하는 멤버로 변경.
		playerData.m_currentCenter = m_memberToSetAsCenter;
		Debug.Log($"새로운 센터를 '{m_memberToSetAsCenter}'(으)로 설정합니다.");

		var newEffect = playerObject.AddComponent<UpgradeSetCenter>();
		newEffect.SourceSO = this;

		player.UpdateAttackTelegraph();
	}

	public override void Remove(GameObject playerObject)
	{
		base.Remove(playerObject);

		if (playerObject == null) return;
		if (!playerObject.TryGetComponent<PlayerController>(out var player)) return;

		// 그냥 GetComponent로 아무거나 찾는 대신, 모든 관련 컴포넌트를 가져와서
		// 자신의 SO를 참조하는 '정확한' 컴포넌트만 찾아서 파괴합니다.
		UpgradeSetCenter[] effects = playerObject.GetComponents<UpgradeSetCenter>();
		foreach (var effect in effects)
		{
			if (effect.SourceSO == this)
			{
				Destroy(effect);
				break; // 찾아서 파괴했으면 루프 종료
			}
		}

		// 센터를 기본 상태로 되돌리는 로직
		var playerData = player.CurrentPlayerData;
		if (playerData.m_currentCenter == m_memberToSetAsCenter)
		{
			playerData.m_currentCenter = EIdolMember.Common;
			player.UpdateAttackTelegraph();
		}
	}
}