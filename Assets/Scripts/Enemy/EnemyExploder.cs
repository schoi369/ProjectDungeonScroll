using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExploder : CellObject
{
	public int m_maxHP = 1;
	int CurrentHP { get; set; }

	public AttackAreaSO m_attackArea;

	int Counter { get; set; } = 0;

	void Awake()
	{
		GameManager.Instance.TurnManager.OnTick += TurnHappened;
	}

	void OnDestroy()
	{
		GameManager.Instance.TurnManager.OnTick -= TurnHappened;
	}

	public override void Init(Vector2Int a_cellPos)
	{
		base.Init(a_cellPos);
		CurrentHP = m_maxHP;
	}

	public override void GetAttacked(int a_damage)
	{
		CurrentHP -= a_damage;
		if (CurrentHP <= 0)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	public override bool PlayerWantsToEnter()
    {
		return false;
    }

    void TurnHappened()
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
				}
				break;
		}
	}
}
