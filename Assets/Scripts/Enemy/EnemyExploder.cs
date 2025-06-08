using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : EnemyBase
{
	public AttackAreaSO m_attackArea;

	int Counter { get; set; } = 0;

    protected override void PerformAction()
	{
		var board = GameManager.Instance.m_boardManager;
		var cellPosList = GameManager.Instance.m_boardManager.GetAttackAreaCellPositions(m_attackArea, m_cellPos, BoardManager.Direction.LEFT);

		Counter++;
		int remainder = Counter % 3;
		switch (remainder)
		{
			case 0:
				break;
			case 1:
				foreach (var targetCellPos in cellPosList)
				{
					var data = board.GetCellData(targetCellPos);
					data.m_groundTile.SetStatus(GroundTile.TileStatus.DANGER);
				}
				break;
			case 2:
				foreach (var targetCellPos in cellPosList)
				{
					var data = board.GetCellData(targetCellPos);
					data.m_groundTile.SetStatus(GroundTile.TileStatus.NONE);

					Vector3 cellWorldPos = board.CellPosToWorldPos(targetCellPos);
					AttackCellVisualPool.Instance.SpawnVisual(cellWorldPos);

					if (GameManager.Instance.IsPlayerAt(targetCellPos))
					{
						GameManager.Instance.m_player.TakeDamage(1);
					}
				}
				break;
		}
	}
}
