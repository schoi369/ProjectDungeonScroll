using UnityEngine;

[CreateAssetMenu(fileName = "SteadyGrowthUpgrade", menuName = "Upgrades/SteadyGrowth")]
public class SteadyGrowthUpgradeSO : UpgradeSO
{
	[Header("Effect Settings")]
	public int m_movesRequired = 5;
	public int m_attackPowerBonus = 1;

	public override void Apply(GameObject playerObject)
	{
		// 효과 컴포넌트를 플레이어에게 부착합니다.
		if (playerObject.GetComponent<UpgradeSteadyGrowth>() == null)
		{
			var effect = playerObject.AddComponent<UpgradeSteadyGrowth>();
			effect.SourceSO = this;
		}

		base.Apply(playerObject);
	}

	public override void Remove(GameObject playerObject)
	{
		// 효과 컴포넌트를 플레이어에게서 제거합니다.
		var effect = playerObject.GetComponent<UpgradeSteadyGrowth>();
		if (effect != null)
		{
			Destroy(effect);
		}

		base.Remove(playerObject);
	}
}