using UnityEngine;
using System.Linq; // LINQ를 사용하여 리스트를 쉽게 검색하기 위해 추가합니다.

[CreateAssetMenu(fileName = "SetCenterUpgrade", menuName = "Upgrades/Set Center Upgrade")]
public class SetCenterUpgradeSO : UpgradeSO
{
	[Header("Center Settings")]
	[Tooltip("이 업그레이드가 '센터'로 지정할 아이돌 멤버입니다.")]
	public EIdolMember m_memberToSetAsCenter;

	// 업그레이드를 적용하는 로직
	public override void Apply(GameObject playerObject)
	{
		if (!playerObject.TryGetComponent<PlayerController>(out var player)) return;

		var playerData = player.CurrentPlayerData;

		// --- 기존에 적용되어 있던 다른 '센터 지정' 업그레이드를 찾아서 제거합니다. ---
		// 현재 획득한 업그레이드 목록에서, SetCenterUpgradeSO 타입이면서 지금 적용하려는 것과 다른 것을 찾습니다.
		UpgradeSO oldCenterUpgrade = playerData.m_acquiredUpgrades
									 .FirstOrDefault(upgrade => upgrade is SetCenterUpgradeSO && upgrade != this);

		if (oldCenterUpgrade != null)
		{
			Debug.Log($"기존 센터 업그레이드 '{oldCenterUpgrade.name}'을(를) 제거합니다.");
			playerData.RemoveUpgrade(oldCenterUpgrade);
		}

		// --- 현재 센터를 이 업그레이드가 지정하는 멤버로 변경합니다. ---
		playerData.m_currentCenter = m_memberToSetAsCenter;
		Debug.Log($"새로운 센터를 '{m_memberToSetAsCenter}'(으)로 설정합니다.");

		// --- 플레이어의 텔레그래핑을 즉시 업데이트하여 변경된 공격 범위를 보여줍니다. ---
		player.UpdateAttackTelegraph();

		base.Apply(playerObject);
	}

	// 업그레이드를 제거하는 로직
	public override void Remove(GameObject playerObject)
	{
		var player = playerObject.GetComponent<PlayerController>();
		if (player == null) return;

		var playerData = player.CurrentPlayerData;

		// --- 이 업그레이드가 현재 활성화된 센터일 경우에만, 센터를 기본 상태로 되돌립니다. ---
		// (다른 센터 업그레이드를 획득하여 이 업그레이드가 '제거'될 때, 센터를 Common으로 되돌리는 것을 방지하기 위함)
		if (playerData.m_currentCenter == m_memberToSetAsCenter)
		{
			playerData.m_currentCenter = EIdolMember.Common;
			Debug.Log($"센터를 기본 상태로 되돌립니다.");

			// 텔레그래핑 업데이트
			player.UpdateAttackTelegraph();
		}

		base.Remove(playerObject);
	}
}