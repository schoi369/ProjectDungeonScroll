using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UIUpgradeIconTooltipPanel : MonoBehaviour
{
	public static UIUpgradeIconTooltipPanel Instance { get; private set; }
	
	public TextMeshProUGUI m_nameText;
	public TextMeshProUGUI m_descriptionText;

	public CanvasGroup m_canvasGroup;

	void Awake()
	{
		Instance = this;
		HideTooltip();
	}

	public void ShowTooltip(UpgradeSO a_upgradeData)
	{
		m_nameText.text = a_upgradeData.upgradeName;
		m_descriptionText.text = a_upgradeData.description;
		m_canvasGroup.alpha = 1f;
		m_canvasGroup.blocksRaycasts = true;
		// TODO: 마우스 커서 위치에 맞게 툴팁 위치를 조정하는 로직 추가
	}

	public void HideTooltip()
	{
		m_canvasGroup.alpha = 0f;
		// 마우스 이벤트가 툴팁 뒤로 통과하도록 설정
		m_canvasGroup.blocksRaycasts = false;
	}
}