using UnityEngine;
using TMPro;

public class UIDungeonInfo : MonoBehaviour
{
	public TextMeshProUGUI m_floorCountText;

	private void OnEnable()
	{
		// 층수 변경 이벤트만 구독
		CustomEventManager.Instance.Subscribe(CustomEventManager.CustomGameEvent.FloorChanged, OnFloorChanged);
	}

	private void OnDisable()
	{
		// 모든 구독을 안전하게 해제
		if (CustomEventManager.Instance != null)
		{
			CustomEventManager.Instance.UnsubscribeAll(this);
		}
	}

	private void OnFloorChanged(object a_floorCount)
	{
		m_floorCountText.text = $"Floor: B{(int)a_floorCount}";
	}
}