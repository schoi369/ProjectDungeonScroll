using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthUIManager : MonoBehaviour
{
	public static EnemyHealthUIManager Instance { get; private set; }

	[Header("References")]
	public GameObject m_healthUIPrefab;    // EnemyHealthUI가 붙어있는 '하트 컨테이너' 프리팹
	public Transform m_canvasTransform;    // World Space Canvas의 Transform

	[Header("Settings")]
	public Vector3 m_offset = new Vector3(0, 0.5f, 0);

	private Dictionary<Transform, EnemyHealthUI> m_healthUIDictionary = new Dictionary<Transform, EnemyHealthUI>();
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
		foreach (var item in m_healthUIDictionary)
		{
			if (item.Key == null) continue; // 타겟이 파괴된 경우 대비

			Vector3 screenPos = m_mainCamera.WorldToScreenPoint(item.Key.position + m_offset);
			item.Value.transform.position = screenPos;
		}
	}

	public void AddHealthUI(Transform a_target, int a_maxHP)
	{
		if (a_target == null || m_healthUIDictionary.ContainsKey(a_target) || m_healthUIPrefab == null) return;

		GameObject newUIObj = Instantiate(m_healthUIPrefab, m_canvasTransform);
		var healthUI = newUIObj.GetComponent<EnemyHealthUI>();

		healthUI.Setup(a_maxHP);
		healthUI.UpdateDisplay(a_maxHP);

		m_healthUIDictionary.Add(a_target, healthUI);
	}

	public void UpdateHealth(Transform a_target, int a_currentHP)
	{
		if (a_target != null && m_healthUIDictionary.TryGetValue(a_target, out EnemyHealthUI healthUI))
		{
			healthUI.UpdateDisplay(a_currentHP);
		}
	}

	public void RemoveHealthUI(Transform a_target)
	{
		if (a_target != null && m_healthUIDictionary.TryGetValue(a_target, out EnemyHealthUI healthUI))
		{
			Destroy(healthUI.gameObject);
			m_healthUIDictionary.Remove(a_target);
		}
	}
}