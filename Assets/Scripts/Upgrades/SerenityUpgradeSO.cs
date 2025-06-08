using UnityEngine;

[CreateAssetMenu(fileName = "Serenity Upgrade", menuName = "Upgrades/Serenity")]
public class SerenityUpgradeSO : UpgradeSO
{
	public int m_turnsRequired = 3;
	public int m_healAmount = 1;

	public override void Apply(GameObject playerObject)
	{
		GameManager.Instance.OnPlayerTurnEnded += CheckSerenity;
	}

	public override void Remove(GameObject playerObject)
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnPlayerTurnEnded -= CheckSerenity;
		}
	}

	private void CheckSerenity()
	{
		var player = GameManager.Instance.m_player;

		if (player == null || player.IsGameOver) return;

		Debug.Log($"Peaceful turns: {player.PeacefulTurns}");

		if (player.PeacefulTurns >= m_turnsRequired)
		{
			Debug.Log($"평온 발동!");
			player.Heal(m_healAmount);
			player.PeacefulTurns = 0; // 회복 후 카운터 리셋
		}
	}
}