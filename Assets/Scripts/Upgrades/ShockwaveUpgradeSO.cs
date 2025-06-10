using UnityEngine;

[CreateAssetMenu(fileName = "New Shockwave Upgrade", menuName = "Upgrades/Shockwave")]
public class ShockwaveUpgradeSO : UpgradeSO
{
	public override void Apply(GameObject playerObject)
	{
		PlayerController player = playerObject.GetComponent<PlayerController>();
		if (player != null)
		{
			// PlayerController가 공격 성공 시 발생시키는 이벤트에 'Knockback' 메서드를 구독
			player.OnAttackLanded += Knockback;
		}
	}

	public override void Remove(GameObject playerObject)
	{
		PlayerController player = playerObject.GetComponent<PlayerController>();
		if (player != null)
		{
			// 업그레이드가 제거될 때 이벤트 구독을 반드시 해지하여 메모리 누수 방지
			player.OnAttackLanded -= Knockback;
		}
	}

	private void Knockback(CellObject targetEnemy, BoardManager.Direction a_direction)
	{
		var board = GameManager.Instance.m_boardManager;
		var player = GameManager.Instance.m_player;

		if (targetEnemy == null || board == null || player == null) return;

		Vector2Int direction = Vector2Int.zero;
		switch (a_direction)
		{
			case BoardManager.Direction.UP: direction = Vector2Int.up; break;
			case BoardManager.Direction.DOWN: direction = Vector2Int.down; break;
			case BoardManager.Direction.LEFT: direction = Vector2Int.left; break;
			case BoardManager.Direction.RIGHT: direction = Vector2Int.right; break;
		}

		Vector2Int knockbackPos = targetEnemy.CellPos + direction;

		if (board.IsCellWalkable(knockbackPos))
		{
			board.MoveObjectOnBoard(targetEnemy, knockbackPos);
		}
	}
}