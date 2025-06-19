using UnityEngine;
using System.Collections.Generic;

public class UIAcquiredUpgradesDisplay : MonoBehaviour
{
	public UIAcquiredIcon m_iconPrefab; // UIAcquiredIcon 프리팹을 연결
	public Transform m_iconsParent; // 아이콘들이 생성될 부모 Transform

	private readonly Dictionary<UpgradeSO, UIAcquiredIcon> m_spawnedIcons = new();


	private void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerUpgradeAdded, OnUpgradeAdded);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerUpgradeRemoved, OnUpgradeRemoved);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.UpgradeEffectTriggered, OnUpgradeEffectTriggered);
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

	void OnUpgradeEffectTriggered(object a_object)
	{
		HighlightIcon(a_object as UpgradeSO);
	}

	void AddIcon(UpgradeSO a_newUpgrade)
	{
		UIAcquiredIcon icon = Instantiate(m_iconPrefab, m_iconsParent);
		icon.Setup(a_newUpgrade);
		m_spawnedIcons.Add(a_newUpgrade, icon); // 딕셔너리에 추가
	}

	void RemoveIcon(UpgradeSO a_upgrade)
	{
		if (m_spawnedIcons.TryGetValue(a_upgrade, out UIAcquiredIcon icon))
		{
			m_spawnedIcons.Remove(a_upgrade);

			// iconScript가 null이 아닐 때만 gameObject에 접근하도록 안전장치 추가
			if (icon != null) Destroy(icon.gameObject);
		}
	}

	void HighlightIcon(UpgradeSO a_upgrade)
	{
		if (a_upgrade == null) return;

		if (m_spawnedIcons.TryGetValue(a_upgrade, out UIAcquiredIcon iconScript))
		{
			iconScript.PlayHighlightEffect();
		}
	}

	public void CleanIcons()
	{
		foreach (Transform child in m_iconsParent)
		{
			Destroy(child.gameObject);
		}
	}
}