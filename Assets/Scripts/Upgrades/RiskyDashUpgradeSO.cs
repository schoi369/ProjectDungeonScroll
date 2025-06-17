using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeRiskyDash", menuName = "Upgrades/RiskyDash")]
public class RiskyDashUpgradeSO : UpgradeSO
{
	// 이 업그레이드의 효과를 처리할 컴포넌트의 타입을 지정합니다.
	// Apply/Remove에서 이 정보를 사용합니다.
	private readonly System.Type m_effectComponentType = typeof(UpgradeRiskyDash);

	public override void Apply(GameObject playerObject)
	{
		// 플레이어에게 이미 동일한 효과 컴포넌트가 있는지 확인하고, 없다면 추가합니다.
		if (playerObject.GetComponent(m_effectComponentType) == null)
		{
			playerObject.AddComponent(m_effectComponentType);
		}
	}

	public override void Remove(GameObject playerObject)
	{
		// 플레이어에게 부착된 효과 컴포넌트를 찾아서 제거합니다.
		Component componentToRemove = playerObject.GetComponent(m_effectComponentType);
		if (componentToRemove != null)
		{
			Destroy(componentToRemove);
		}
	}
}