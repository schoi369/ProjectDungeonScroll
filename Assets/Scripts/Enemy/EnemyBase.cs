using UnityEngine;

public abstract class EnemyBase : CellObject
{
	[Header("Enemy Base Stats")]
	public int m_maxHP = 1;
	protected int CurrentHP { get; set; }

	protected bool m_isMovementOverridden = false;

	// 모든 적들은 턴 기반으로 행동하므로, EnemyBase에서 이벤트를 구독/해지
	protected virtual void OnEnable()
	{
		if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
		{
			GameManager.Instance.TurnManager.OnTick += HandleTurn;
		}
	}

	protected virtual void OnDisable()
	{
		if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
		{
			GameManager.Instance.TurnManager.OnTick -= HandleTurn;
		}
	}

	public override void Init(Vector2Int a_cellPos)
	{
		base.Init(a_cellPos);
		CurrentHP = m_maxHP;
	}

	// 모든 적들의 피격 로직을 표준화
	public override void GetAttacked(int a_damage)
	{
		CurrentHP -= a_damage;
		if (CurrentHP <= 0)
		{
			Die();
		}
	}

	// 사망 처리
	protected virtual void Die()
	{
		Destroy(gameObject);
	}

	// 플레이어는 적이 있는 칸으로 들어갈 수 없음
	public override bool PlayerWantsToEnter()
	{
		return false;
	}

	void HandleTurn()
	{
		if (m_isMovementOverridden)
		{
			m_isMovementOverridden = false;
		}
		else
		{
			PerformMovement();
		}

		PerformAction();
	}

	// 자식 클래스가 반드시 구현해야 할 턴 별 행동
	protected virtual void PerformMovement() {}
	protected virtual void PerformAction() {}

	public void OverrideNextMovement()
	{
		m_isMovementOverridden = true;
	}
}