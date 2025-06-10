using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
	public SpriteRenderer m_spriteRenderer;
	public Color m_dangerColor = Color.red;
	public bool m_passable;

	public Sprite _destroyedGroundSprite;

	public enum TileStatus
	{
		NONE,
		DANGER, // 다음 턴에 대미지가 들어오는 타일 (임시)
		DESTROYED, // 사용 불가가 된 타일
	}
	
	public void SetStatus(TileStatus a_status)
	{
		switch(a_status)
		{
			case TileStatus.NONE:
				m_spriteRenderer.color = Color.white;
				break;
			case TileStatus.DANGER:
				m_spriteRenderer.color = m_dangerColor;
				break;
			case TileStatus.DESTROYED:
				m_spriteRenderer.sprite = _destroyedGroundSprite;
				m_passable = false;
				break;
		}
	}


}
