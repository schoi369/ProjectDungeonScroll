using UnityEngine;

public class UIAcquiredUpgradesDisplay : MonoBehaviour
{
	public GameObject m_iconPrefab; // UIAcquiredIcon 프리팹을 연결
	public Transform m_iconsParent; // 아이콘들이 생성될 부모 Transform

	void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.GameStarted, OnGameStarted);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.NewStageLoaded, OnNewStageLoaded);

		GameManager.Instance.CurrentPlayerData.OnUpgradeAdded += AddIcon;
	}

	void OnDisable()
	{
		if (CustomEventManager.Instance != null)
		{
			CustomEventManager.Instance.Unsubscribe(CustomEventManager.CustomGameEvent.GameStarted, OnGameStarted);
			CustomEventManager.Instance.Unsubscribe(CustomEventManager.CustomGameEvent.NewStageLoaded, OnNewStageLoaded);
		}

		GameManager.Instance.CurrentPlayerData.OnUpgradeAdded -= AddIcon;
	}

	private void AddIcon(UpgradeSO a_newUpgrade)
	{
		GameObject newIconObj = Instantiate(m_iconPrefab, m_iconsParent);
		newIconObj.GetComponent<UIAcquiredIcon>().Setup(a_newUpgrade);
	}

	private void OnGameStarted(object a_payload)
	{
		// 아이콘 목록을 깨끗하게 비움
		foreach (Transform child in m_iconsParent)
		{
			Destroy(child.gameObject);
		}
	}

	void OnNewStageLoaded(object a_fromDeath)
	{
		bool fromDeath = (bool)a_fromDeath;

		if (fromDeath)
		{

		}
		else
		{
			// 플레이어 데이터를 확인하고 소지 중인 업그레이드 아이콘 추가
			if (StageManager.Instance.m_player)
			{
				foreach (var upgrade in StageManager.Instance.m_player.ActiveUpgrades)
				{
					AddIcon(upgrade);
				}
			}
		}
	}
}