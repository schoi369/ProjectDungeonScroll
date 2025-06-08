using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Upgrades/Upgrade Database")]
public class UpgradeDatabase : ScriptableObject
{
	public List<UpgradeSO> m_allUpgrades;
}