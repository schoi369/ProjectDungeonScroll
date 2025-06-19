using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeRiskyDash", menuName = "Upgrades/RiskyDash")]
public class RiskyDashUpgradeSO : UpgradeSO
{
	[Header("Effect Settings")]
	public int m_turnsRequired = 3;
	public int m_hpCost = 3;
	public int m_attackPowerGain = 1;

	// 이 업그레이드의 효과를 처리할 컴포넌트의 타입을 지정합니다.
	// Apply/Remove에서 이 정보를 사용합니다.
	private readonly System.Type m_effectComponentType = typeof(UpgradeRiskyDash);

	public override void Apply(GameObject playerObject)
	{
		base.Apply(playerObject);

		// 플레이어에게 이미 동일한 효과 컴포넌트가 있는지 확인하고, 없다면 추가합니다.
		if (!playerObject.TryGetComponent<UpgradeRiskyDash>(out var effect))
		{
			effect = playerObject.AddComponent<UpgradeRiskyDash>();
			effect.SourceSO = this;
		}
	}

	public override void Remove(GameObject playerObject)
	{
		base.Remove(playerObject);

		if (playerObject != null)
		{
			Component componentToRemove = playerObject.GetComponent(m_effectComponentType);
			if (componentToRemove != null)
			{
				Destroy(componentToRemove);
			}
		}
	}
}