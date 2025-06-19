using UnityEngine;

#if OLD_UPGRADE
[CreateAssetMenu(fileName = "SerenityUpgrade", menuName = "Upgrades/Serenity")]
public class SerenityUpgradeSO : UpgradeSO
{
	public int m_turnsRequired = 3;
	public int m_healAmount = 1;

	private readonly System.Type m_effectComponentType = typeof(UpgradeSerenity);

	public override void Apply(GameObject playerObject)
	{
		// 컴포넌트가 이미 있는지 확인하고, 없다면 추가합니다.
		UpgradeSerenity effect = playerObject.GetComponent<UpgradeSerenity>();
		if (effect == null)
		{
			effect = playerObject.AddComponent<UpgradeSerenity>();
			// ★핵심: 생성된 컴포넌트에게 "너의 데이터는 나야" 라고 알려줍니다.
			effect.SourceSO = this;
		}

		base.Apply(playerObject);
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

		base.Remove(playerObject);
	}
}
#endif