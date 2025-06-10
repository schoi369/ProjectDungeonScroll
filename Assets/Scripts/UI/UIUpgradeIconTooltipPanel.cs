using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIUpgradeIconTooltipPanel : MonoBehaviour
{	
	public TextMeshProUGUI m_nameText;
	public TextMeshProUGUI m_descriptionText;
	public Vector2 m_positionOffset; // 아이콘 위치로부터 얼마나 떨어져서 표시될지

	private CanvasGroup m_canvasGroup;
	private RectTransform m_rectTransform;
	private Canvas m_rootCanvas;

	void Awake()
	{
		m_canvasGroup = GetComponent<CanvasGroup>();
		m_rectTransform = GetComponent<RectTransform>();
		m_rootCanvas = GetComponentInParent<Canvas>();

		HideTooltip();
	}

	private void Update()
	{
		if (m_canvasGroup.alpha != 0)
		{
			Vector3 scaledOffset = m_positionOffset * m_rootCanvas.scaleFactor;

			m_rectTransform.position = Input.mousePosition + scaledOffset;
		}
	}

	public void ShowTooltip(UpgradeSO a_upgradeData)
	{
		m_nameText.text = a_upgradeData.upgradeName;
		m_descriptionText.text = a_upgradeData.description;

		m_canvasGroup.alpha = 1f;
	}

	public void HideTooltip()
	{
		m_canvasGroup.alpha = 0f;
	}
}