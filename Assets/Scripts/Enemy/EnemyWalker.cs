using UnityEngine;

public class EnemyWalker : EnemyBase
{
	protected override void PerformMovement()
	{
		var player = GameManager.Instance.m_player;
		var board = GameManager.Instance.m_boardManager;

		if (player == null || player.IsGameOver)
		{
			return;
		}

		Vector2Int playerPos = player.CellPos;
		Vector2Int enemyPos = m_cellPos;

		// 2. 이동 조건: 주 경로, 차선책 경로를 모두 고려하여 이동
		int dx = playerPos.x - enemyPos.x;
		int dy = playerPos.y - enemyPos.y;

		// 이동할 목표 지점을 미리 계산
		Vector2Int horizontalTarget = new Vector2Int(enemyPos.x + (int)Mathf.Sign(dx), enemyPos.y);
		Vector2Int verticalTarget = new Vector2Int(enemyPos.x, enemyPos.y + (int)Mathf.Sign(dy));

		// 수평 이동을 우선할지 결정
		bool isHorizontalPrimary = Mathf.Abs(dx) > Mathf.Abs(dy);

		if (isHorizontalPrimary)
		{
			// 1순위: 수평 이동 시도
			if (dx != 0 && board.IsCellWalkable(horizontalTarget))
			{
				board.MoveObjectOnBoard(this, horizontalTarget);
			}
			// 2순위: 1순위가 막혔을 경우, 수직 이동 시도
			else if (dy != 0 && board.IsCellWalkable(verticalTarget))
			{
				board.MoveObjectOnBoard(this, verticalTarget);
			}
			// (둘 다 막혔으면 턴 종료)
		}
		else // 수직 이동이 우선이거나 거리가 같을 때
		{
			// 1순위: 수직 이동 시도
			if (dy != 0 && board.IsCellWalkable(verticalTarget))
			{
				board.MoveObjectOnBoard(this, verticalTarget);
			}
			// 2순위: 1순위가 막혔을 경우, 수평 이동 시도
			else if (dx != 0 && board.IsCellWalkable(horizontalTarget))
			{
				board.MoveObjectOnBoard(this, horizontalTarget);
			}
			// (둘 다 막혔으면 턴 종료)
		}
	}

	protected override void PerformAction()
	{
		var player = GameManager.Instance.m_player;
		var board = GameManager.Instance.m_boardManager;

		if (player == null || player.IsGameOver)
		{
			return;
		}

		Vector2Int playerPos = player.CellPos;
		Vector2Int enemyPos = m_cellPos;

		// 1. 공격 조건: 플레이어와 인접 시 공격
		int distance = Mathf.Abs(playerPos.x - enemyPos.x) + Mathf.Abs(playerPos.y - enemyPos.y);
		if (distance == 1)
		{
			player.TakeDamage(1);
			return;
		}

	}
}