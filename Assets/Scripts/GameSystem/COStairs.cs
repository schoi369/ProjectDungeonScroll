using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COStairs : CellObject
{
	public string m_sceneName;

	public override void Init(Vector3Int a_tilemapPos)
	{
		base.Init(a_tilemapPos);
	}

	public override void PlayerEntered()
	{
		Debug.Log("Player Entered Stairs");
		//GameManager.Instance.NewLevel();

		// TODO: 랜덤으로 스테이지(Scene)을 로드하기.
		GameManager.Instance.LoadNewScene(m_sceneName);
	}
}
