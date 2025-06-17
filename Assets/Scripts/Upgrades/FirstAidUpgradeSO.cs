using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeFirstAid", menuName = "Upgrades/FirstAid")]
public class FirstAidUpgradeSO : UpgradeSO
{
	[Header("Effect Settings")]
	public int m_healAmount = 5; // 적 처치 시 회복량

	private readonly System.Type m_effectComponentType = typeof(UpgradeFirstAid);

	public override void Apply(GameObject playerObject)
	{
		UpgradeFirstAid effect = playerObject.GetComponent<UpgradeFirstAid>();
		if (effect == null)
		{
			effect = playerObject.AddComponent<UpgradeFirstAid>();
			effect.SourceSO = this;
		}
	}

	public override void Remove(GameObject playerObject)
	{
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