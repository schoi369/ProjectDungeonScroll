using UnityEngine;

public class UIAcquiredUpgradesDisplay : MonoBehaviour
{
	public GameObject m_iconPrefab; // UIAcquiredIcon 프리팹을 연결
	public Transform m_iconsParent; // 아이콘들이 생성될 부모 Transform

	private void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerUpgradeAdded, OnUpgradeAdded);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerUpgradeRemoved, OnUpgradeRemoved);
	}

	private void OnDisable()
	{
		if (CustomEventManager.Instance)
		{
			CustomEventManager.Instance.UnsubscribeAll(this);
		}
	}

	void OnUpgradeAdded(object a_upgrade)
	{
		if (a_upgrade is UpgradeSO upgrade)
		{
			AddIcon(upgrade);
		}
		else
		{
			Debug.LogWarning("PlayerUpgradeAdded 이벤트에 잘못된 타입이 전달되었습니다. UpgradeSO가 필요합니다.");
		}
	}

	void OnUpgradeRemoved(object a_upgrade)
	{
		if (a_upgrade is UpgradeSO upgrade)
		{
			RemoveIcon(upgrade);
		}
		else
		{
			Debug.LogWarning("PlayerUpgradeRemoved 이벤트에 잘못된 타입이 전달되었습니다. UpgradeSO가 필요합니다.");
		}
	}

	void AddIcon(UpgradeSO a_newUpgrade)
	{
		GameObject newIconObj = Instantiate(m_iconPrefab, m_iconsParent);
		newIconObj.GetComponent<UIAcquiredIcon>().Setup(a_newUpgrade);
	}

	void RemoveIcon(UpgradeSO a_upgrade)
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