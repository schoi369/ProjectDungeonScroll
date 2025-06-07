using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerInfo : MonoBehaviour
{
	// Floor
	public TextMeshProUGUI m_floorCountText;

	// Food Amount
	int CurrentFoodAmount { get; set; }
	int MaxFoodAmount { get; set; }

	// HP
	public TextMeshProUGUI m_hpText;
	int CurrentHP { get; set; }
	int MaxHP { get; set; }

	private void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.FloorChanged, OnFloorChanged);

		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerMaxHPChanged, OnMaxHPChanged);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerCurrentHPChanged, OnCurrentHPChanged);
	}

	private void OnDisable()
	{
		CustomEventManager.Instance?.UnsubscribeAll(this);
	}

	void OnFloorChanged(object a_floorCount)
	{
		m_floorCountText.text = $"Floor: B{(int) a_floorCount}";
	}

	void OnCurrentHPChanged(object a_hp)
	{
		CurrentHP = (int) a_hp;
		RefreshHPText();
	}

	void OnMaxHPChanged(object a_max)
	{
		MaxHP = (int) a_max;
		RefreshHPText();
	}

	void RefreshHPText()
	{
		m_hpText.text = $"HP: {CurrentHP} / {MaxHP}";
	}
}
