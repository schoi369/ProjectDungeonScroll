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

	int m_attackWarningCounter = 0;


	// 물리적 상태를 나타내는 새로운 enum
	public enum PhysicalState { Normal, Warned, Destroyed }
	private PhysicalState m_physicalState;

	void Awake()
	{
		// 게임 시작 시 기본 상태로 설정
		SetPhysicalState(PhysicalState.Normal);
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
	 /// 이 타일에 대한 공격 예고를 하나 추가합니다.
	 /// </summary>
	public void AddAttackWarning()
	{
		m_attackWarningCounter++;
		UpdateAttackWarningVisuals();
	}

	/// <summary>
	/// 이 타일에 대한 공격 예고를 하나 제거합니다.
	/// </summary>
	public void RemoveAttackWarning()
	{
		m_attackWarningCounter--;
		if (m_attackWarningCounter < 0) m_attackWarningCounter = 0;
		UpdateAttackWarningVisuals();
	}

	// 카운터 값에 따라 색상을 업데이트합니다.
	private void UpdateAttackWarningVisuals()
	{
		// 카운터가 0보다 크면(하나라도 예고가 있으면) 위험색으로, 아니면 원래색으로
		m_spriteRenderer.color = (m_attackWarningCounter > 0) ? m_dangerColor : Color.white;
	}
}