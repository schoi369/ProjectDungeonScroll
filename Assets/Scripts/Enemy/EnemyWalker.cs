using UnityEngine;

public class EnemyWalker : EnemyBase
{
	public Color m_chargeAttackColor = Color.red;

	bool m_isChargingAttack = false;

	protected override void PerformTurnLogic()
	{
		var player = GameManager.Instance.m_player;
		var board = GameManager.Instance.m_boardManager;

		if (player == null || player.IsGameOver)
		{
			return;
		}

		Vector3Int playerPos = player.TilemapPos;
		Vector3Int enemyPos = m_cellPos;

		// 공격 조건 확인: 플레이어와 인접해 있는가?
		bool nextToPlayer = board.ArePositionsAdjacent(enemyPos, playerPos);

		// 1. 공격 실행 단계
		if (m_isChargingAttack)
		{
			if (nextToPlayer)
			{
				// 공격 실행
				VFXManager.Instance.PlaySlashEffect(player.transform.position, Color.red);
				player.TakeDamage(1);
			}
			// 플레이어가 피했든, 공격했든 '공격 준비' 상태는 해제
			StopChargeVisuals();
			return; // 턴 종료
		}

		// 2. 공격 준비 단계
		if (nextToPlayer)
		{
			// 공격하는 대신 '공격 준비' 상태로 진입
			StartChargeVisuals();
			return; // 턴 종료
		}

		// 3. 일반 이동 단계
		// (위의 두 경우에 해당하지 않을 때만 실행)
		int dx = playerPos.x - enemyPos.x;
		int dy = playerPos.y - enemyPos.y;

		Vector3Int horizontalTarget = new(enemyPos.x + (int)Mathf.Sign(dx), enemyPos.y);
		Vector3Int verticalTarget = new(enemyPos.x, enemyPos.y + (int)Mathf.Sign(dy));

		bool isHorizontalPrimary = Mathf.Abs(dx) > Mathf.Abs(dy);

		if (isHorizontalPrimary)
		{
			if (dx != 0 && board.IsCellWalkable(horizontalTarget)) board.MoveObjectOnBoard(this, horizontalTarget);
			else if (dy != 0 && board.IsCellWalkable(verticalTarget)) board.MoveObjectOnBoard(this, verticalTarget);
		}
		else
		{
			if (dy != 0 && board.IsCellWalkable(verticalTarget)) board.MoveObjectOnBoard(this, verticalTarget);
			else if (dx != 0 && board.IsCellWalkable(horizontalTarget)) board.MoveObjectOnBoard(this, horizontalTarget);
		}
	}
	
	/// <summary>
	/// 공격 준비 시각 효과를 시작합니다.
	/// </summary>
	private void StartChargeVisuals()
	{
		m_isChargingAttack = true;
		// 지금은 색상만 변경하지만, 나중에 이 부분에 어떤 연출이든 넣을 수 있습니다.
		m_spriteRenderer.color = m_chargeAttackColor;
	}

	/// <summary>
	/// 공격 준비 시각 효과를 종료합니다.
	/// </summary>
	private void StopChargeVisuals()
	{
		m_isChargingAttack = false;
		m_spriteRenderer.color = m_originalColor; // EnemyBase에 저장된 원래 색상으로 복구
	}
}