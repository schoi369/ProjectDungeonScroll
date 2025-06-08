using UnityEngine;

[CreateAssetMenu(fileName = "Static Discharge Upgrade", menuName = "Upgrades/Static Discharge")]
public class StaticDischargeUpgradeSO : UpgradeSO
{
	[Range(0, 1)]
	public float m_stunRate = 0.25f;

	public override void Apply(GameObject playerObject)
	{
		GameManager.Instance.OnPlayerTurnEnded += StunNearbyEnemies;
	}

	public override void Remove(GameObject playerObject)
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnPlayerTurnEnded -= StunNearbyEnemies;
		}
	}

	private void StunNearbyEnemies()
	{
		var player = GameManager.Instance.m_player;
		var board = GameManager.Instance.m_boardManager;

		// 플레이어 주변 8칸을 순회
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0) continue; // 플레이어 자신은 제외

				Vector2Int targetPos = player.CellPos + new Vector2Int(x, y);
				var cellData = board.GetCellData(targetPos);

				if (cellData?.m_containedObject is EnemyBase enemy)
				{
					if (enemy != null)
					{
						if (Random.value < m_stunRate)
						{
							Debug.Log($"{enemy.name} 스턴 성공!");
							enemy.SetStun(true);
						}
					}
				}
			}
		}
	}
}