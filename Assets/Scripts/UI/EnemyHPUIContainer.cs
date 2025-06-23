using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUIContainer : MonoBehaviour
{
	[SerializeField] Image m_fill;

	public void Setup()
	{
		m_fill.fillAmount = 1f;
	}

	public void UpdateDisplay(int a_currentHP, int a_maxHP)
	{
		float fillAmount = (float)a_currentHP / a_maxHP;
		m_fill.fillAmount = Mathf.Clamp01(fillAmount);
	}
}