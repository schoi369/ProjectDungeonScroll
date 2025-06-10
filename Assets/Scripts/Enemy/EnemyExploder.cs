using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : EnemyBase
{
	public AttackAreaSO m_attackArea;

	int Counter { get; set; } = 0;

	protected override void PerformTurnLogic()
	{
		var board = GameManager.Instance.m_boardManager;
		var cellPosList = board.GetAttackAreaCellPositions(m_attackArea, m_cellPos, BoardManager.Direction.LEFT); // 방향은 현재 무관

		Counter++;
		int remainder = Counter % 3;
		switch (remainder)
		{
			case 0:
				// 대기
				break;
			case 1: // 폭발 예고
				foreach (var targetCellPos in cellPosList)
				{
					var data = board.GetCellData(targetCellPos);
					if (data != null && data.m_groundTile != null)
					{
						data.m_groundTile.SetStatus(GroundTile.TileStatus.DANGER);
					}
				}
				break;
			case 2: // 폭발
				foreach (var targetCellPos in cellPosList)
				{
					var data = board.GetCellData(targetCellPos);
					if (data != null && data.m_groundTile != null)
					{
						data.m_groundTile.SetStatus(GroundTile.TileStatus.NONE);
					}

					Vector3 cellWorldPos = board.CellPosToWorldPos(targetCellPos);
					VFXManager.Instance.PlaySlashEffect(cellWorldPos, Color.red);

					if (GameManager.Instance.IsPlayerAt(targetCellPos))
					{
						GameManager.Instance.m_player.TakeDamage(1);
					}
				}
				break;
		}
	}
}