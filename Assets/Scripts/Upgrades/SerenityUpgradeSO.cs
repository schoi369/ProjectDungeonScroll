using UnityEngine;

[CreateAssetMenu(fileName = "Serenity Upgrade", menuName = "Upgrades/Serenity")]
public class SerenityUpgradeSO : UpgradeSO
{
	public int m_turnsRequired = 3;
	public int m_healAmount = 1;

	public override void Apply(GameObject playerObject)
	{
		StageManager.Instance.OnPlayerTurnEnded += CheckSerenity;
	}

	public override void Remove(GameObject playerObject)
	{
		if (StageManager.Instance != null)
		{
			StageManager.Instance.OnPlayerTurnEnded -= CheckSerenity;
		}
	}

	private void CheckSerenity()
	{
		var player = StageManager.Instance.m_player;

		if (player == null || player.IsGameOver) return;

		if (player.PeacefulTurns >= m_turnsRequired)
		{
			player.Heal(m_healAmount);
			player.PeacefulTurns = 0; // 회복 후 카운터 리셋
		}
	}
}