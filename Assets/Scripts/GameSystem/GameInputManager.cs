using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
	public static GameInputManager Instance;

	public static event Action onMoveUp;
	public static event Action onMoveDown;
	public static event Action onMoveLeft;
	public static event Action onMoveRight;
	public static event Action onGameOverRestart; // MEMO: 나중에 UI 전용 조작으로 바꿀 수도.

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OnMoveUp(InputAction.CallbackContext a_context)
	{
		if (a_context.performed)
		{
			onMoveUp?.Invoke();
		}
	}
	public void OnMoveDown(InputAction.CallbackContext a_context)
	{
		if (a_context.performed)
		{
			onMoveDown?.Invoke();
		}
	}
	public void OnMoveLeft(InputAction.CallbackContext a_context)
	{
		if (a_context.performed)
		{
			onMoveLeft?.Invoke();
		}
	}
	public void OnMoveRight(InputAction.CallbackContext a_context)
	{
		if (a_context.performed)
		{
			onMoveRight?.Invoke();
		}
	}

	public void OnRestart(InputAction.CallbackContext a_context)
	{
		if (a_context.performed)
		{
			onGameOverRestart?.Invoke();
		}
	}
}