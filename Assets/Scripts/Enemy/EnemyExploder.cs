using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : EnemyBase
{
	public AttackAreaSO m_attackArea;

	int Counter { get; set; } = 0;

	//List<GroundTile> m_warnedTiles = new();


	protected override void PerformTurnLogic()
	{
		var board = StageManager.Instance.m_boardManager;
		var tilemapPosList = board.GetAttackAreaCellPositions(m_attackArea, m_TilemapPos, BoardManager.Direction.LEFT); // 방향은 현재 무관

		Counter++;
		int remainder = Counter % 3;
		switch (remainder)
		{
			case 0:
				// 대기
				break;
			case 1: // 폭발 예고
				ClearTelegraphs();
				foreach (var targetTilemapPos in tilemapPosList)
				{
					var data = board.GetCellData(targetTilemapPos);
					//if (data != null && data.m_groundTile != null)
					//{
					//	data.m_groundTile.AddAttackWarning();
					//	m_warnedTiles.Add(data.m_groundTile);
					//}
				}
				break;
			case 2: // 폭발
				ClearTelegraphs();
				foreach (var targetTilemapPos in tilemapPosList)
				{
					Vector3 cellWorldPos = board.TilemapPosToWorldPos(targetTilemapPos);
					VFXManager.Instance.PlaySlashEffect(cellWorldPos, Color.red);

					if (StageManager.Instance.IsPlayerAt(targetTilemapPos))
					{
						StageManager.Instance.m_player.TakeDamage(1, true);
					}
				}
				break;
		}
	}

	protected override void ClearTelegraphs()
	{
		//foreach (var tile in m_warnedTiles)
		//{
		//	if (tile != null) // 타일이 이미 파괴되었을 경우 대비
		//	{
		//		tile.RemoveAttackWarning();
		//	}
		//}
		//m_warnedTiles.Clear(); // 리스트 비우기
	}

}