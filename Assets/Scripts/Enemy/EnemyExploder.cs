using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : EnemyBase
{
	[Header("Exploder Settings")]
	[SerializeField]
	private AttackAreaSO m_explosionArea;
	[SerializeField]
	private int m_initialCountdown = 3;
	[SerializeField]
	private int m_explosionDamage = 5;

	private int m_currentCountdown;

	// EnemyBase의 Init을 오버라이드하여 Exploder에 필요한 초기화를 추가합니다.
	public override void Init(Vector3Int a_tilemapPos)
	{
		base.Init(a_tilemapPos);

		m_currentCountdown = m_initialCountdown;

		// 여기에 카운트다운 숫자를 표시할 UI를 업데이트하는 코드를 넣을 수 있습니다.
		// 예: EnemyHPUIManager.Instance.UpdateExtraInfo(transform, m_currentCountdown.ToString());
	}

	protected override void PerformTurnLogic()
	{
		m_currentCountdown--;
		//Debug.Log($"{gameObject.name} countdown: {m_currentCountdown}");

		if (m_currentCountdown <= 0)
		{
			Explode();
		}
		else if (m_currentCountdown == 1)
		{
			TelegraphExplosion();
		}
	}

	private void TelegraphExplosion()
	{
		List<Vector3Int> explosionTiles = StageManager.Instance.m_boardManager.GetAttackAreaCellPositions(m_explosionArea, TilemapPos, BoardManager.Direction.RIGHT);
		EnemyTelegraphManager.Instance.RegisterTelegraph(explosionTiles);
	}

	// 실제 폭발 로직
	private void Explode()
	{
		List<Vector3Int> explosionTiles = StageManager.Instance.m_boardManager.GetAttackAreaCellPositions(m_explosionArea, TilemapPos, BoardManager.Direction.RIGHT);

		foreach (var tilePos in explosionTiles)
		{
			VFXManager.Instance.PlaySlashEffect(StageManager.Instance.m_boardManager.TilemapPosToWorldPos(tilePos), Color.red);

			if (StageManager.Instance.IsPlayerAt(tilePos))
			{
				StageManager.Instance.m_player.TakeDamage(m_explosionDamage, true);
			}
		}

		m_currentCountdown = m_initialCountdown; // 폭발 후 카운트 초기화
	}
}