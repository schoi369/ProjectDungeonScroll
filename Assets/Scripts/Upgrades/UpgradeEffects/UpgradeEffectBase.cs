using UnityEngine;

// 'T'는 이 효과의 데이터 소스가 될 UpgradeSO의 자식 타입을 의미합니다.
// where T : UpgradeSO 제약 조건은 T가 반드시 UpgradeSO를 상속받는 타입이어야 함을 명시합니다.
public abstract class UpgradeEffectBase<T> : MonoBehaviour where T : UpgradeSO
{
	// 모든 자식 클래스는 이 프로퍼티를 통해 자신의 데이터 SO를 참조합니다.
	// 타입이 'T'로 명확하게 지정되므로, 자식 클래스에서 형변환 없이 바로 사용할 수 있습니다.
	public T SourceSO { get; set; }

	protected PlayerController m_player;

	protected virtual void Awake()
	{
		m_player = GetComponent<PlayerController>();
	}
}