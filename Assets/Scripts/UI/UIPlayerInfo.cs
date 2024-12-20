using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerInfo : MonoBehaviour
{
	// Floor
	public TextMeshProUGUI m_floorCountText;

	// Food Amount
	public TextMeshProUGUI m_foodAmountText;
	int CurrentFoodAmount { get; set; }
	int MaxFoodAmount { get; set; }

	private void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.FloorChanged, OnFloorChanged);

		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerCurrentFoodAmountChanged, OnCurrentFoodAmountChanged);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerMaxFoodAmountChanged, OnMaxFoodAmountChanged);
	}

	private void OnDisable()
	{
		CustomEventManager.Instance?.UnsubscribeAll(this);
	}

	void OnFloorChanged(object a_floorCount)
	{
		m_floorCountText.text = $"Floor: B{(int) a_floorCount}";
	}

	void OnCurrentFoodAmountChanged(object a_amount)
	{
		CurrentFoodAmount = (int) a_amount;
		RefreshFoodAmountText();
	}

	void OnMaxFoodAmountChanged(object a_max)
	{
		MaxFoodAmount = (int) a_max;
		RefreshFoodAmountText();
	}

	void RefreshFoodAmountText()
	{
		m_foodAmountText.text = $"Food: {CurrentFoodAmount} / {MaxFoodAmount}";
	}
}
