using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void ShowHideGameOverPanel(bool a_show)
	{
		m_gameOverPanel.gameObject.SetActive(a_show);
	}
}
