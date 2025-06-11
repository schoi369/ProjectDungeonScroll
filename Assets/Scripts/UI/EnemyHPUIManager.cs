using System.Collections.Generic;
using UnityEngine;

public class EnemyHPUIManager : MonoBehaviour
{
	public static EnemyHPUIManager Instance { get; private set; }

	[Header("References")]
	public GameObject m_healthUIPrefab;
	public Transform m_canvasTransform;

	[Header("Settings")]
	public Vector3 m_offset = new Vector3(0, 0.5f, 0);

	private Dictionary<Transform, EnemyHPUIContainer> m_healthUIDictionary = new();
	private Camera m_mainCamera;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		m_mainCamera = Camera.main;
	}

	void LateUpdate()
	{
		// 모든 컨테이너의 위치 갱신
		foreach (var item in m_healthUIDictionary)
		{
			Vector3 screenPos = m_mainCamera.WorldToScreenPoint(item.Key.position + m_offset);
			item.Value.transform.position = screenPos;
		}
	}

	public void AddHealthUI(Transform a_target, int a_maxHP)
	{
		if (a_target == null || m_healthUIDictionary.ContainsKey(a_target) || m_healthUIPrefab == null) return;

		GameObject newUIObj = Instantiate(m_healthUIPrefab, m_canvasTransform);
		var healthUI = newUIObj.GetComponent<EnemyHPUIContainer>();

		healthUI.Setup(a_maxHP);
		healthUI.UpdateDisplay(a_maxHP);

		m_healthUIDictionary.Add(a_target, healthUI);
	}

	public void UpdateHealth(Transform a_target, int a_currentHP)
	{
		if (a_target != null && m_healthUIDictionary.TryGetValue(a_target, out EnemyHPUIContainer healthUI))
		{
			healthUI.UpdateDisplay(a_currentHP);
		}
	}

	public void RemoveHealthUI(Transform a_target)
	{
		if (a_target != null && m_healthUIDictionary.TryGetValue(a_target, out EnemyHPUIContainer healthUI))
		{
			Destroy(healthUI.gameObject);
			m_healthUIDictionary.Remove(a_target);
		}
	}
}