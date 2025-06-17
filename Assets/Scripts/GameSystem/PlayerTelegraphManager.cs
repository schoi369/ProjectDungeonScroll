using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerTelegraphManager : MonoBehaviour
{
	public static PlayerTelegraphManager Instance;

	[SerializeField] private GameObject m_visualPrefab;
	[SerializeField] private BoardManager m_board;

	// Unity의 공식 ObjectPool을 사용합니다.
	private IObjectPool<GameObject> m_pool;

	// 현재 화면에 활성화된 시각 효과들을 추적하기 위한 리스트
	private readonly List<GameObject> m_activeVisuals = new List<GameObject>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		// ObjectPool을 초기화합니다.
		m_pool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate(m_visualPrefab, transform), // 1. 풀에 오브젝트가 없을 때 새로 생성하는 방법
			actionOnGet: (obj) => obj.SetActive(true),               // 2. 풀에서 가져올 때 실행할 행동
			actionOnRelease: (obj) => obj.SetActive(false),          // 3. 풀에 반납할 때 실행할 행동
			actionOnDestroy: (obj) =>
			{
				// 에디터에서 플레이 모드일 때만 Destroy를 호출하도록 조건을 추가합니다.
				if (Application.isPlaying)
				{
					Destroy(obj);
				}
				// 플레이 모드가 아닐 때 (예: 게임 종료 시)는 Unity가 알아서 정리하므로,
				// 굳이 DestroyImmediate를 호출할 필요가 없습니다.
			},
			collectionCheck: false,  // (에러 체크) 이미 풀에 반납된 아이템인지 확인할지 여부
			defaultCapacity: 30,     // 기본적으로 생성해 둘 개수
			maxSize: 100);           // 풀이 가질 수 있는 최대 크기
	}

	// 지정된 타일 위치에 시각 효과를 표시합니다.
	public void ShowVisuals(List<Vector3Int> a_tilePositions)
	{
		HideAllVisuals(); // 기존 마커들을 모두 풀에 반납합니다.

		foreach (var tilePos in a_tilePositions)
		{
			GameObject visual = m_pool.Get(); // 풀에서 하나를 꺼내옵니다. (actionOnGet 실행됨)
			visual.transform.position = m_board.TilemapPosToWorldPos(tilePos);

			// 나중에 반납하기 위해 활성화된 오브젝트 목록에 추가해둡니다.
			m_activeVisuals.Add(visual);
		}
	}

	// 모든 시각 효과를 숨기고 풀에 반납합니다.
	public void HideAllVisuals()
	{
		// 현재 활성화된 오브젝트들을 순회하며 모두 풀에 반납합니다.
		foreach (var visual in m_activeVisuals)
		{
			m_pool.Release(visual); // 오브젝트를 풀에 반납 (actionOnRelease 실행됨)
		}

		// 활성화 목록을 깨끗하게 비웁니다.
		m_activeVisuals.Clear();
	}
}