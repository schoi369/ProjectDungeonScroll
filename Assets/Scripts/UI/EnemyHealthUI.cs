using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
	[Header("Prefabs & Sprites")]
	public GameObject m_heartIconPrefab; // 하트 아이콘(Image)으로 사용할 프리팹
	public Sprite m_fullHeartSprite;
	public Sprite m_emptyHeartSprite;
	public Transform m_heartsParent; // 생성된 하트들이 위치할 부모 Transform (Horizontal Layout Group)

	private List<Image> m_heartImages = new List<Image>();

	public void Setup(int a_maxHP)
	{
		foreach (Transform child in m_heartsParent)
		{
			Destroy(child.gameObject);
		}
		m_heartImages.Clear();

		for (int i = 0; i < a_maxHP; i++)
		{
			GameObject newHeart = Instantiate(m_heartIconPrefab, m_heartsParent);
			m_heartImages.Add(newHeart.GetComponent<Image>());
		}
	}

	public void UpdateDisplay(int a_currentHP)
	{
		for (int i = 0; i < m_heartImages.Count; i++)
		{
			m_heartImages[i].sprite = (i < a_currentHP) ? m_fullHeartSprite : m_emptyHeartSprite;
		}
	}
}