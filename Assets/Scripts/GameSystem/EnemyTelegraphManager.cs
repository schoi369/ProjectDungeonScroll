using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyTelegraphManager : MonoBehaviour
{
	public static EnemyTelegraphManager Instance;

	[SerializeField] private GameObject m_visualPrefab;
	[SerializeField] private BoardManager m_board;

	private IObjectPool<GameObject> m_pool;

	// 등록된 모든 타일 위치를 중복 없이 저장합니다.
	private readonly HashSet<Vector3Int> m_registeredTiles = new HashSet<Vector3Int>();

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

		m_pool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate(m_visualPrefab, transform),
			actionOnGet: (obj) => obj.SetActive(true),
			actionOnRelease: (obj) => obj.SetActive(false),
			actionOnDestroy: (obj) => { if (Application.isPlaying) Destroy(obj); },
			collectionCheck: false,
			defaultCapacity: 30,
			maxSize: 500);
	}

	/// <summary>
	/// 다음 턴에 공격할 타일들을 시스템에 등록합니다.
	/// </summary>
	public void RegisterTelegraph(List<Vector3Int> a_tilePositions)
	{
		m_registeredTiles.UnionWith(a_tilePositions);
	}

	/// <summary>
	/// 지금까지 등록된 모든 타일에 시각 효과를 표시합니다.
	/// </summary>
	public void DisplayTelegraphs()
	{
		foreach (var tilePos in m_registeredTiles)
		{
			GameObject visual = m_pool.Get();
			visual.transform.position = m_board.TilemapPosToWorldPos(tilePos);
		}
	}

	/// <summary>
	/// 등록된 타일 목록과 화면의 모든 시각 효과를 초기화합니다.
	/// </summary>
	public void ClearTelegraphs()
	{
		// ObjectPool은 활성화된 객체를 직접 추적하지 않으므로,
		// 자식으로 있는 모든 오브젝트를 비활성화하는 방식으로 풀에 반납합니다.
		foreach (Transform child in transform)
		{
			if (child.gameObject.activeSelf)
			{
				m_pool.Release(child.gameObject);
			}
		}
		m_registeredTiles.Clear();
	}
}