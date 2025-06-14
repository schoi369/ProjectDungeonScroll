using UnityEngine;

// 이 클래스는 업그레이드의 데이터와 적용/제거 로직의 틀을 제공합니다.
// abstract 키워드로 인해 이 스크립트 자체는 에셋으로 생성할 수 없습니다.
public abstract class UpgradeSO : ScriptableObject
{
	[Header("공통 정보")]
	public string upgradeName;
	[TextArea]
	public string description;
	public Sprite icon;

	public enum CounterType
	{
		None,
		PeacefulTurns // '평온' 스택
	}

	public CounterType m_counterType = CounterType.None;

	/// <summary>
	/// 업그레이드가 플레이어에게 적용될 때 호출될 함수
	/// </summary>
	/// <param name="playerObject">업그레이드를 적용받을 플레이어 게임오브젝트</param>
	public abstract void Apply(GameObject playerObject);

	/// <summary>
	/// 업그레이드가 플레이어에게서 제거될 때 호출될 함수
	/// </summary>
	/// <param name="playerObject">업그레이드를 제거할 플레이어 게임오브젝트</param>
	public abstract void Remove(GameObject playerObject);
}