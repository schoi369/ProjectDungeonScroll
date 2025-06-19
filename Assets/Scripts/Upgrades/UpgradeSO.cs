using UnityEngine;

public enum EIdolMember
{
	Common,
	Sakura,
	Yunjin,
	Kazuha,
}

// 이 클래스는 업그레이드의 데이터와 적용/제거 로직의 틀을 제공합니다.
// abstract 키워드로 인해 이 스크립트 자체는 에셋으로 생성할 수 없습니다.
public abstract class UpgradeSO : ScriptableObject
{
	public EIdolMember m_idolMember = EIdolMember.Common;
	public string upgradeName;
	[TextArea]
	public string description;
	public Sprite icon;

	public enum CounterType
	{
		None,
		PeacefulTurns, // '평온' 스택
		RiskyDashTurns, // '위험한 질주' 스택
	}

	public CounterType m_counterType = CounterType.None;

	/// <summary>
	/// 업그레이드가 플레이어에게 적용될 때 호출될 함수
	/// </summary>
	/// <param name="playerObject">업그레이드를 적용받을 플레이어 게임오브젝트</param>
	public virtual void Apply(GameObject playerObject)
	{
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerUpgradeAdded, this);
	}

	/// <summary>
	/// 업그레이드가 플레이어에게서 제거될 때 호출될 함수
	/// </summary>
	/// <param name="playerObject">업그레이드를 제거할 플레이어 게임오브젝트</param>
	public virtual void Remove(GameObject playerObject)
	{
		CustomEventManager.Instance.KickEvent(CustomEventManager.CustomGameEvent.PlayerUpgradeRemoved, this);
	}
}