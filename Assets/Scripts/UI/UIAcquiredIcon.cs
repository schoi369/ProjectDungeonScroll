using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAcquiredIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Image m_Image;
	public TextMeshProUGUI m_counterText;

	private UpgradeSO m_representedUpgrade;
	public UpgradeSO RepresentedUpgrade => m_representedUpgrade;


	public void Setup(UpgradeSO a_upgradeData)
	{
		m_representedUpgrade = a_upgradeData;
		m_Image.sprite = a_upgradeData.icon;

		// 카운터 타입에 따라 텍스트 활성화/비활성화 및 초기값 설정
		if (m_representedUpgrade.m_counterType != UpgradeSO.CounterType.None)
		{
			m_counterText.gameObject.SetActive(true);
			UpdateCounterText(); // 초기값 표시
		}
		else
		{
			m_counterText.gameObject.SetActive(false);
		}
	}

	void OnEnable()
	{
		StageManager.Instance.OnPlayerTurnEnded += UpdateCounterText;
	}

	void OnDisable()
	{
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnPlayerTurnEnded -= UpdateCounterText;
		}
	}

	private void UpdateCounterText()
	{
		if (m_representedUpgrade == null || m_representedUpgrade.m_counterType == UpgradeSO.CounterType.None) return;

		// 어떤 카운터 타입인지에 따라 적절한 값을 가져와 표시
		switch (m_representedUpgrade.m_counterType)
		{
			case UpgradeSO.CounterType.PeacefulTurns:
				int currentPeacefulTurns = StageManager.Instance.m_player.PeacefulTurns;
				m_counterText.text = currentPeacefulTurns.ToString();
				break;
				// 추후 다른 카운터 타입이 추가될 수 있음
		}
	}


	public void OnPointerEnter(PointerEventData a_eventData)
	{
		if (m_representedUpgrade != null)
		{
			OverlayCanvas.Instance.ShowTooltip(m_representedUpgrade);
		}
	}

	public void OnPointerExit(PointerEventData a_eventData)
	{
		OverlayCanvas.Instance.HideTooltip();
	}
}