using UnityEngine;

public class UIAcquiredUpgradesDisplay : MonoBehaviour
{
	public GameObject m_iconPrefab; // UIAcquiredIcon 프리팹을 연결
	public Transform m_iconsParent; // 아이콘들이 생성될 부모 Transform

	void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.GameStarted, HandleGameStarted);

		// PlayerController가 존재하면 이벤트 구독
		if (GameManager.Instance != null && GameManager.Instance.m_player != null)
		{
			GameManager.Instance.m_player.OnUpgradeAdded += AddIcon;
		}
	}

	void OnDisable()
	{
		if (CustomEventManager.Instance != null)
		{
			CustomEventManager.Instance.Unsubscribe(CustomEventManager.CustomGameEvent.GameStarted, HandleGameStarted);
		}
		// 씬 전환 또는 파괴 시 이벤트 구독 해제
		if (GameManager.Instance != null && GameManager.Instance.m_player != null)
		{
			GameManager.Instance.m_player.OnUpgradeAdded -= AddIcon;
		}
	}

	private void AddIcon(UpgradeSO a_newUpgrade)
	{
		GameObject newIconObj = Instantiate(m_iconPrefab, m_iconsParent);
		newIconObj.GetComponent<UIAcquiredIcon>().Setup(a_newUpgrade);
	}

	private void HandleGameStarted(object a_payload)
	{
		// 아이콘 목록을 깨끗하게 비움
		foreach (Transform child in m_iconsParent)
		{
			Destroy(child.gameObject);
		}
	}
}