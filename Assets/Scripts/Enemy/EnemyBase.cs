using UnityEngine;

public abstract class EnemyBase : CellObject
{
	[Header("Enemy Base Stats")]
	public int m_maxHP = 1;
	protected int CurrentHP { get; private set; }
	public bool IsDead { get; private set; } = false;
	public bool IsStunned { get; set; } = false;
	public int m_expValue = 10;

	[Header("Visuals")]
	public GameObject m_stunIcon;

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

		m_stunIcon.SetActive(false);
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

		GameManager.Instance.m_player.GainExp(m_expValue);

		GameManager.Instance.UnregisterEnemy(this);
		Destroy(gameObject);
	}

	public override bool PlayerWantsToEnter()
	{
		return false;
	}

	public void ExecuteTurn()
	{
		if (IsStunned)
		{
			Debug.Log($"{name} is stunned and skips a turn.");
			SetStun(false);
			return;
		}

		PerformTurnLogic();
	}

	protected abstract void PerformTurnLogic();

	public void SetStun(bool a_stun)
	{
		IsStunned = a_stun;
		if (m_stunIcon != null)
		{
			m_stunIcon.SetActive(a_stun);
		}
	}
}