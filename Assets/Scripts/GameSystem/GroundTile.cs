using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
	public SpriteRenderer m_spriteRenderer;
	public Color m_dangerColor = Color.red;
	public bool m_passable;

	public enum TileStatus
	{
		NONE,
		DANGER,

	}

	public void SetSpriteAlpha(float a_alpha)
	{
		var currentColor = m_spriteRenderer.color;
		m_spriteRenderer.color = new(currentColor.r, currentColor.g, currentColor.b, a_alpha);
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
		}
	}
}
