using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COStairs : CellObject
{
	public override void Init(Vector2Int a_cellPos)
	{
		base.Init(a_cellPos);
	}

	public override void PlayerEntered()
	{
		GameManager.Instance.NewLevel();
	}
}
