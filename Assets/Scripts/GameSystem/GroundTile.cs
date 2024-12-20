using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
	public SpriteRenderer m_spriteRenderer;
	public bool m_passable;

	public void SetSpriteAlpha(float a_alpha)
	{
		var currentColor = m_spriteRenderer.color;
		m_spriteRenderer.color = new(currentColor.r, currentColor.g, currentColor.b, a_alpha);
	}
}
