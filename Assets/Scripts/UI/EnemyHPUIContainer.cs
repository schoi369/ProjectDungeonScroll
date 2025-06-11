using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUIContainer : MonoBehaviour
{
	public EnemyHPHeartEntity m_heartIconPrefab; // 하트 아이콘(Image)으로 사용할 프리팹
	List<EnemyHPHeartEntity> m_hearts = new();

	public void Setup(int a_maxHP)
	{
		// 추후 풀링 등 재사용을 위한 클린업.
		foreach (var heart in m_hearts)
		{
			Destroy(heart.gameObject);
		}
		m_hearts.Clear();

		for (int i = 0; i < a_maxHP; i++)
		{
			EnemyHPHeartEntity newHeart = Instantiate(m_heartIconPrefab, transform);
			m_hearts.Add(newHeart);
		}
	}

	public void UpdateDisplay(int a_currentHP)
	{
		for (int i = 0; i < m_hearts.Count; i++)
		{
			m_hearts[i].SetSprite(i < a_currentHP);
		}
	}
}