using UnityEngine;

public class UIAcquiredUpgradesDisplay : MonoBehaviour
{
	public GameObject m_iconPrefab; // UIAcquiredIcon 프리팹을 연결
	public Transform m_iconsParent; // 아이콘들이 생성될 부모 Transform

	public void AddIcon(UpgradeSO a_newUpgrade)
	{
		GameObject newIconObj = Instantiate(m_iconPrefab, m_iconsParent);
		newIconObj.GetComponent<UIAcquiredIcon>().Setup(a_newUpgrade);
	}

	public void RemoveIcon(UpgradeSO a_upgrade)
	{
		UIAcquiredIcon[] icons = m_iconsParent.GetComponentsInChildren<UIAcquiredIcon>();
		foreach (var icon in icons)
		{
			if (icon.RepresentedUpgrade.name == a_upgrade.name)
			{
				Destroy(icon.gameObject);
				return; // 첫 번째로 찾은 아이콘만 제거
			}
		}
		Debug.LogWarning($"업그레이드 아이콘을 찾을 수 없습니다: {a_upgrade.upgradeName}");
	}

	public void CleanIcons()
	{
		foreach (Transform child in m_iconsParent)
		{
			Destroy(child.gameObject);
		}
	}
}