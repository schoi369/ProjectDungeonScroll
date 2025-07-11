using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAcquiredIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Image m_Image;
	public TextMeshProUGUI m_counterText;
	[SerializeField] Animator m_animator;

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
		StageManager.Instance.OnPlayerTurnEndedUI += UpdateCounterText;
	}

	void OnDisable()
	{
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnPlayerTurnEndedUI -= UpdateCounterText;
		}
	}

	private void UpdateCounterText()
	{
		if (m_representedUpgrade == null || m_representedUpgrade.m_counterType == UpgradeSO.CounterType.None) return;

		// 어떤 카운터 타입인지에 따라 적절한 값을 가져와 표시
		m_counterText.text = StageManager.Instance.m_player.GetCounterValueForUpgrade(m_representedUpgrade);
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

	public void PlayHighlightEffect()
	{
		if (m_animator != null)
		{
			m_animator.SetTrigger("Highlight");
		}
	}
}