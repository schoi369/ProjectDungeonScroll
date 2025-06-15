using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerInfo : MonoBehaviour
{
	[Header("HP References")]
	public Image m_hpBarFillImage;
	public TextMeshProUGUI m_hpText;

	[Header("Experience References")]
	public TextMeshProUGUI m_levelText;
	public TextMeshProUGUI m_expText;
	public Image m_expBarFillImage;

	private int m_currentHP;
	private int m_maxHP;

	private void OnEnable()
	{
		// 모든 플레이어 관련 이벤트를 구독
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerMaxHPChanged, OnMaxHPChanged);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, OnCurrentHPChanged);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerLevelChanged, OnLevelChanged);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerExpChanged, OnExpChanged);
	}

	private void OnDisable()
	{
		if (CustomEventManager.Instance != null)
		{
			CustomEventManager.Instance.UnsubscribeAll(this);
		}
	}

	// --- HP 관련 핸들러 ---
	private void OnCurrentHPChanged(object a_hp)
	{
		m_currentHP = (int)a_hp;
		UpdateHpBar();
	}

	private void OnMaxHPChanged(object a_maxHP)
	{
		m_maxHP = (int)a_maxHP;
		UpdateHpBar();
	}

	private void UpdateHpBar()
	{
		if (m_maxHP > 0)
		{
			m_hpBarFillImage.fillAmount = (float)m_currentHP / m_maxHP;
			m_hpText.text = $"{m_currentHP} / {m_maxHP}";
		}
	}

	// --- 경험치 관련 핸들러 ---
	void UpdateExpBar((int, int) a_expData)
	{
		(int currentExp, int expToNext) = a_expData;
		m_expText.text = $"{currentExp} / {expToNext}";

		if (expToNext > 0)
		{
			m_expBarFillImage.fillAmount = (float)currentExp / expToNext;
		}
	}

	void UpdateLevelText(int a_level)
	{
		m_levelText.text = $"Lv. {a_level}";
	}

	private void OnLevelChanged(object a_level)
	{
		UpdateLevelText((int)a_level);
	}

	private void OnExpChanged(object a_expData)
	{
		UpdateExpBar(((int, int))a_expData);
	}
}