using UnityEngine;
using System.Collections;

public abstract class EnemyBase : CellObject
{
	[Header("Enemy Base Stats")]
	public int m_maxHP = 1;
	protected int CurrentHP { get; private set; }
	public bool IsDead { get; private set; } = false;
	public bool IsStunned { get; set; } = false;
	public int m_expValue = 10;

	[Header("Visuals")]
	public SpriteRenderer m_spriteRenderer;
	public Color m_originalColor;
	public GameObject m_stunIcon;
	public float m_hitScaleMultiplier = 1.2f;
	public float m_hitScaleEffectDuration = 0.1f;
	Vector3 m_originalScale;
	Coroutine m_hitScaleEffectCoroutine;

	// GameManager에 자신을 등록/해제
	protected virtual void OnEnable()
	{
		if (StageManager.Instance != null)
		{
			StageManager.Instance.RegisterEnemy(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (StageManager.Instance != null)
		{
			StageManager.Instance.UnregisterEnemy(this);
		}
	}

	public override void Init(Vector3Int a_tilemapPos)
	{
		base.Init(a_tilemapPos);

		EnemyHPUIManager.Instance.AddHealthUI(transform, m_maxHP);

		CurrentHP = m_maxHP;
		IsDead = false;

		m_stunIcon.SetActive(false);

		m_originalScale = transform.localScale;
	}

	public override void GetAttacked(int a_damage)
	{
		if (m_hitScaleEffectCoroutine != null)
		{
			StopCoroutine(m_hitScaleEffectCoroutine);
		}
		transform.localScale = m_originalScale;
		m_hitScaleEffectCoroutine = StartCoroutine(HitEffectCoroutine());

		CurrentHP -= a_damage;
		EnemyHPUIManager.Instance.UpdateHealth(transform, CurrentHP);
		if (CurrentHP <= 0)
		{
			Die();
		}
	}

	IEnumerator HitEffectCoroutine()
	{
		Vector3 hitScale = m_originalScale * m_hitScaleMultiplier;
		float halfDuration = m_hitScaleEffectDuration / 2f;
		float timer = 0f;

		while (timer < halfDuration)
		{
			transform.localScale = Vector3.Lerp(m_originalScale, hitScale, timer / halfDuration);
			timer += Time.deltaTime;
			yield return null;
		}

		timer = 0f;

		while (timer < halfDuration)
		{
			transform.localScale = Vector3.Lerp(hitScale, m_originalScale, timer / halfDuration);
			timer += Time.deltaTime;
			yield return null;
		}

		transform.localScale = m_originalScale;
		m_hitScaleEffectCoroutine = null;
	}

	/// <summary>
	/// 적 파괴/사망을 담당하는 메소드.
	/// </summary>
	/// <param name="a_byGameSystem"> 보드 리셋, 바닥 파괴 등 게임 시스템 때문에 죽었는가.</param>
	protected virtual void Die(bool a_byGameSystem = false)
	{
		if (IsDead) return;
		IsDead = true;

		if (!a_byGameSystem)
		{
			StageManager.Instance.m_player.GainExp(m_expValue);
		}

		ClearTelegraphs(); // TODO: 오브젝트에서 타일맵 기반으로 넘어왔으니 텔레그래핑 방식 변경해야 함.

		StageManager.Instance.UnregisterEnemy(this);
		StageManager.Instance.KickEnemyDefeatedEvent(this);

		EnemyHPUIManager.Instance.RemoveHealthUI(transform);

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

	/// <summary>
	/// 이 적이 남긴 모든 타일 텔레그래핑 효과를 정리합니다. 자식 클래스에서 재정의할 수 있습니다.
	/// </summary>
	protected virtual void ClearTelegraphs()
	{
	}

	public override void GetDestroyedFromBoard()
	{
		Die(true);
	}

}