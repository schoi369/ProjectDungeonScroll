using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerInfo : MonoBehaviour
{
	public TextMeshProUGUI m_foodAmountText;
	int CurrentFoodAmount { get; set; }
	int MaxFoodAmount { get; set; }

	private void OnEnable()
	{
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerCurrentFoodAmountChanged, OnCurrentFoodAmountChanged);
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.PlayerMaxFoodAmountChanged, OnMaxFoodAmountChanged);
	}

	private void OnDisable()
	{
		CustomEventManager.Instance?.Unsubscribe(CustomEventManager.CustomGameEvent.PlayerCurrentFoodAmountChanged, OnCurrentFoodAmountChanged);
		CustomEventManager.Instance?.Unsubscribe(CustomEventManager.CustomGameEvent.PlayerMaxFoodAmountChanged, OnMaxFoodAmountChanged);
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
