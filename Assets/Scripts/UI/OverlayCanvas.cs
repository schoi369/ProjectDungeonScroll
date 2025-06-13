using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayCanvas : MonoBehaviour
{
	// In Scene Singleton
	static OverlayCanvas _instance;
	public static OverlayCanvas Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<OverlayCanvas>();
			}
			return _instance;
		}
	}

	public UIPlayerInfo m_playerInfo;
	public UIGameOverPanel m_gameOverPanel;
	public UIUpgradeSelectionPanel m_upgradeSelectionPanel;
	public UIUpgradeIconTooltipPanel m_tooltipPanel;

	public Image m_fadeImage;

	public void ShowHideGameOverPanel(bool a_show)
	{
		m_gameOverPanel.gameObject.SetActive(a_show);
	}

	// Upgrade
	public void ShowUpgradeSelection(List<UpgradeSO> a_options)
	{
		m_upgradeSelectionPanel.ShowOptions(a_options);
	}

	public void HideUpgradeSelection()
	{
		m_upgradeSelectionPanel.Hide();
	}

	// Icon Tooltip
	public void ShowTooltip(UpgradeSO a_upgradeData)
	{
		m_tooltipPanel.ShowTooltip(a_upgradeData);
	}

	public void HideTooltip()
	{
		m_tooltipPanel.HideTooltip();
	}

	public void Fade(bool a_turnBlack)
	{
		m_fadeImage.color = a_turnBlack ? new Color(0f, 0f, 0f, 1f) : new Color(0f, 0f, 0f, 0f);
	}
}
