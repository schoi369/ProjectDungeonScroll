using UnityEngine;

public class GroundTile : MonoBehaviour
{
	[Header("Sprites")]
	public Sprite m_defaultSprite;
	public Sprite m_warnedSprite;
	public Sprite m_destroyedSprite;

	[Header("Visuals")]
	public SpriteRenderer m_spriteRenderer;
	public Color m_dangerColor = Color.red;

	[Header("State")]
	public bool m_passable;

	// 물리적 상태를 나타내는 새로운 enum
	public enum PhysicalState { Normal, Warned, Destroyed }
	private PhysicalState m_physicalState;

	void Awake()
	{
		// 게임 시작 시 기본 상태로 설정
		SetPhysicalState(PhysicalState.Normal);
		SetAttackWarning(false);
	}

	/// <summary>
	/// 타일의 물리적 상태를 변경합니다. (스프라이트 교체)
	/// </summary>
	public void SetPhysicalState(PhysicalState a_newState)
	{
		m_physicalState = a_newState;
		switch (m_physicalState)
		{
			case PhysicalState.Normal:
				m_spriteRenderer.sprite = m_defaultSprite;
				m_passable = true;
				break;
			case PhysicalState.Warned:
				m_spriteRenderer.sprite = m_warnedSprite;
				m_passable = true;
				break;
			case PhysicalState.Destroyed:
				m_spriteRenderer.sprite = m_destroyedSprite;
				m_passable = false;
				break;
		}
	}

	/// <summary>
	/// 타일의 공격 위험 상태를 변경합니다. (색상 변경)
	/// TODO: AttackWarning 복수 해당 문제는? 카운터 형식으로 변경?
	/// </summary>
	public void SetAttackWarning(bool a_isUnderWarning)
	{
		if (a_isUnderWarning)
		{
			m_spriteRenderer.color = m_dangerColor;
		}
		else
		{
			m_spriteRenderer.color = Color.white;
		}
	}
}