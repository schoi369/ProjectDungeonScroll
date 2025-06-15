using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 전체적인 흐름과 플레이어 데이터를 관리하는 싱글톤 매니저.
/// 씬이 바뀌어도 파괴되지 않습니다.
/// </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("게임 데이터")]
	[SerializeField]
	private PlayerDataSO m_playerDataAtStart; // 원본 데이터 SO (에디터에서 할당)
	public PlayerDataSO CurrentPlayerData { get; private set; }

	private void Awake()
	{
		// 일반적인 싱글톤 패턴 구현
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			// 게임 시작 시 데이터 초기화
			InitializeGame();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void InitializeGame()
	{
		// 원본 ScriptableObject를 Instantiate하여 런타임용 복제본을 만듭니다.
		// 이렇게 하면 플레이 중에 데이터가 변경되어도 원본 에셋에는 영향을 주지 않습니다.
		CurrentPlayerData = Instantiate(m_playerDataAtStart);
		CurrentPlayerData.InitializeForNewRun();
	}


	public void LoadTestStage001()
	{
		StartCoroutine(LoadSceneRoutine("TestStage_001"));
	}

	public void LoadNewScene(string a_sceneName)
	{
		StartCoroutine(LoadSceneRoutine(a_sceneName));
	}

	private IEnumerator LoadSceneRoutine(string a_sceneName)
	{
		// TODO: 여기에 화면을 어둡게 하는 페이드 아웃 효과를 넣으면 좋습니다.
		Debug.Log($"Loading scene: {a_sceneName}...");
		UICarryOnCanvas.Instance.Fade(true);

		yield return new WaitForSeconds(0.5f); // 페이드 아웃 효과를 위한 대기 시간

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(a_sceneName);

		// 씬 로딩이 완료될 때까지 대기
		while (!asyncLoad.isDone)
		{
			// TODO: 여기에 로딩 진행률(asyncLoad.progress)을 표시하는 UI를 업데이트할 수 있습니다.
			yield return null;
		}

		// TODO: 여기에 화면을 다시 밝게 하는 페이드 인 효과를 넣으면 좋습니다.
		Debug.Log("Scene loaded.");

		yield return new WaitForSeconds(0.5f);

		UICarryOnCanvas.Instance.Fade(false);
	}
}