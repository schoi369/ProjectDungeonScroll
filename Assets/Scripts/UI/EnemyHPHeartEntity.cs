using UnityEngine;
using UnityEngine.UI;

public class EnemyHPHeartEntity : MonoBehaviour
{
	[Header("Components")]
	public Image m_Image;

	[Header("Sprites")]
	public Sprite m_fullSprite;
	public Sprite m_emptySprite;

	public void SetSprite(bool a_full)
	{
		m_Image.sprite = a_full ? m_fullSprite : m_emptySprite;
	}
}
