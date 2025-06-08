using UnityEngine;

public class EnemyWalker : EnemyBase
{
	protected override void PerformTurnLogic()
	{
		var player = GameManager.Instance.m_player;
		var board = GameManager.Instance.m_boardManager;

		if (player == null || player.IsGameOver)
		{
			return;
		}

		Vector2Int playerPos = player.CellPos;
		Vector2Int enemyPos = m_cellPos;

		// 공격 조건 확인: 플레이어와 인접해 있는가?
		int distance = Mathf.Abs(playerPos.x - enemyPos.x) + Mathf.Abs(playerPos.y - enemyPos.y);

		bool nextToPlayer = (distance == 1);
		if (nextToPlayer)
		{
			// 공격하고 턴 종료
			player.TakeDamage(1);
		}
		else
		{
			// 이동 조건 확인: 공격하지 않았다면 플레이어 방향으로 이동 시도
			int dx = playerPos.x - enemyPos.x;
			int dy = playerPos.y - enemyPos.y;

			Vector2Int horizontalTarget = new(enemyPos.x + (int)Mathf.Sign(dx), enemyPos.y);
			Vector2Int verticalTarget = new(enemyPos.x, enemyPos.y + (int)Mathf.Sign(dy));

			bool isHorizontalPrimary = Mathf.Abs(dx) > Mathf.Abs(dy);

			if (isHorizontalPrimary)
			{
				// 1순위: 수평 이동 시도
				if (dx != 0 && board.IsCellWalkable(horizontalTarget))
				{
					board.MoveObjectOnBoard(this, horizontalTarget);
				}
				// 2순위: 수직 이동 시도
				else if (dy != 0 && board.IsCellWalkable(verticalTarget))
				{
					board.MoveObjectOnBoard(this, verticalTarget);
				}
			}
			else
			{
				// 1순위: 수직 이동 시도
				if (dy != 0 && board.IsCellWalkable(verticalTarget))
				{
					board.MoveObjectOnBoard(this, verticalTarget);
				}
				// 2순위: 수평 이동 시도
				else if (dx != 0 && board.IsCellWalkable(horizontalTarget))
				{
					board.MoveObjectOnBoard(this, horizontalTarget);
				}
			}
		}
	}
}