using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AttackCellVisualPool : MonoBehaviour
{
	static AttackCellVisualPool _instance;
	public static AttackCellVisualPool Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<AttackCellVisualPool>();
			}
			return _instance;
		}
	}

	public AttackCellVisual m_visualPrefab;
	ObjectPool<AttackCellVisual> m_pool;

	void Start()
	{
		m_pool = new ObjectPool<AttackCellVisual>(
			() =>
			{
				return Instantiate(m_visualPrefab, transform);
			},
			visual =>
			{
				visual.gameObject.SetActive(true);
			},
			visual =>
			{
				visual.gameObject.SetActive(false);
			},
			visual =>
			{
				Destroy(visual.gameObject);
			},
			false,
			10,
			100
		);
	}

	public void SpawnVisual(Vector3 a_pos)
	{
		var visual = m_pool.Get();
		visual.transform.position = a_pos;
		visual.Init();
	}

	public void RemoveVisual(AttackCellVisual a_visual)
	{
		m_pool.Release(a_visual);
	}
}
