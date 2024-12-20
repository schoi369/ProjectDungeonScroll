using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomEventManager : MonoBehaviour
{
	public enum CustomGameEvent
	{
		None = 0,
		
		// Game System
		FloorChanged = 1,

		// Player Info
		PlayerMaxFoodAmountChanged = 100,
		PlayerCurrentFoodAmountChanged = 101,
	}

	static CustomEventManager _instance = null;

	private Dictionary<CustomGameEvent, Action<object>> _eventCallbackDictionary = new();

	public static CustomEventManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<CustomEventManager>();
			}

			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void Subscribe(CustomGameEvent customEvent, Action<object> newCallback)
	{
		if (_eventCallbackDictionary.TryGetValue(customEvent, out var correspondingCallbacks))
		{
			_eventCallbackDictionary[customEvent] = correspondingCallbacks + newCallback;
		}
		else
		{
			_eventCallbackDictionary[customEvent] = newCallback;
		}
	}

	public void Unsubscribe(CustomGameEvent customEvent, Action<object> newCallback)
	{
		if (_eventCallbackDictionary.TryGetValue(customEvent, out var correspondingCallbacks))
		{
			correspondingCallbacks -= newCallback;
			if (correspondingCallbacks == null)
			{
				_eventCallbackDictionary.Remove(customEvent);
			}
			else
			{
				_eventCallbackDictionary[customEvent] = correspondingCallbacks;
			}
		}
	}

	public void UnsubscribeAll(MonoBehaviour script)
	{
		foreach (var customEvent in _eventCallbackDictionary.Keys.ToList())
		{
			if (_eventCallbackDictionary.TryGetValue(customEvent, out var callbackGroup))
			{
				foreach (var callback in callbackGroup.GetInvocationList())
				{
					if (callback.Target == (object) script)
					{
						_eventCallbackDictionary[customEvent] -= (Action<object>) callback;
					}
				}

				if (_eventCallbackDictionary[customEvent] == null)
				{
					_eventCallbackDictionary.Remove(customEvent);
				}
			}
		}
	}

	public void KickEvent(CustomGameEvent customEvent, object parameter = null)
	{
		if (_eventCallbackDictionary.TryGetValue(customEvent, out var correspondingCallbacks))
		{
			correspondingCallbacks.Invoke(parameter);
		}
	}
}