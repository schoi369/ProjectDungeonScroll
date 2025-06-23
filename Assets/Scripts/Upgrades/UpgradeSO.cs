using UnityEngine;

public enum EIdolMember
{
	Common,
	Sakura,
	Yunjin,
	Kazuha,
}

public enum UpgradeCategory
{
	None,               // 카테고리 없음 (중복 제한을 받지 않음)
	Movement,           // 이동
	OnAttackHit,        // 공격 성공
	MainWeaponConfig,   // 메인 무기 설정 (센터 지정 등)
	OnDamaged,          // 피격
	OnKill,             // 적 처치
	OnTurnEnd,          // 턴 종료
	AdjacentEnvironment,// 인접 환경
	Passive,             // 패시브
}

// 이 클래스는 업그레이드의 데이터와 적용/제거 로직의 틀을 제공합니다.
// abstract 키워드로 인해 이 스크립트 자체는 에셋으로 생성할 수 없습니다.
public abstract class UpgradeSO : ScriptableObject
{
	[Header("멤버")]
	public EIdolMember m_idolMember = EIdolMember.Common;
	[Header("카테고리")]
	public UpgradeCategory m_category = UpgradeCategory.None;
	public string upgradeName;
	[Header("기타")]
	[TextArea]
	public string description;
	public Sprite icon;

	public enum CounterType
	{
		None,
		PeacefulTurns, // '평온' 스택 (안 쓰일지도)
		RiskyDashTurns = 2, // '위험한 질주' 스택
		SteadyGrowth = 3, // '꾸준한 성장' 스택
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