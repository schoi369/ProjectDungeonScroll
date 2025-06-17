using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어의 영속적인 데이터를 담는 ScriptableObject.
/// 한 번의 게임 플레이(a run) 동안 유지됩니다.
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataSO", order = 1)]
public class PlayerDataSO : ScriptableObject
{
	[Header("플레이어 기본 스탯")]
	public int m_maxHP = 5;
	public int m_currentHP;
	public int m_attackPower = 1;

	[Header("레벨 및 경험치")]
	public int m_level;
	public int m_currentExp;
	public int m_expToNextLevel = 15; // 예시 값

	[Header("업그레이드")]
	public List<UpgradeSO> m_acquiredUpgrades = new List<UpgradeSO>();
	public List<UpgradeSO> m_testUpgrades = new(); // Debug


	[Header("기타")]
	public List<AttackAreaSO> m_memberAttackAreas;

	/// <summary>
	/// 새 게임을 시작할 때 데이터를 초기 상태로 리셋합니다.
	/// </summary>
	public void InitializeForNewRun()
	{
		m_currentHP = m_maxHP;
		m_level = 1;
		m_currentExp = 0;

		m_acquiredUpgrades.Clear();
	}

	public void AddUpgrade(UpgradeSO a_upgrade)
	{
		if (m_acquiredUpgrades.Contains(a_upgrade))
		{
			return; // 중복 획득 방지
		}
		m_acquiredUpgrades.Add(a_upgrade);

		a_upgrade.Apply(StageManager.Instance.m_player.gameObject);
		OverlayCanvas.Instance.AddUpgradeIcon(a_upgrade);

		Debug.Log($"업그레이드 획득: {a_upgrade.upgradeName}");
	}
}