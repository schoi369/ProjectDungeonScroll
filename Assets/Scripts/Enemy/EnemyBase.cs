using UnityEngine;

public abstract class EnemyBase : CellObject
{
	[Header("Enemy Base Stats")]
	public int m_maxHP = 1;
	protected int CurrentHP { get; private set; }
	public bool IsDead { get; private set; } = false;

	// GameManager에 자신을 등록/해제
	protected virtual void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.RegisterEnemy(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.UnregisterEnemy(this);
		}
	}

	public override void Init(Vector2Int a_cellPos)
	{
		base.Init(a_cellPos);
		CurrentHP = m_maxHP;
		IsDead = false;
	}

	public override void GetAttacked(int a_damage)
	{
		CurrentHP -= a_damage;
		if (CurrentHP <= 0)
		{
			Die();
		}
	}

	protected virtual void Die()
	{
		if (IsDead) return;
		IsDead = true;
		GameManager.Instance.UnregisterEnemy(this);
		Destroy(gameObject);
	}

	public override bool PlayerWantsToEnter()
	{
		return false;
	}

	// 자식 클래스가 자신의 턴에 수행할 모든 로직을 이 메서드 안에서 구현합니다.
	public abstract void ExecuteTurn();
}