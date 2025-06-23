using UnityEngine;

/// <summary>
/// 디버그 설정에 따라 로그를 출력하는 커스텀 정적 로거 클래스입니다.
/// 사용법: Debug.Log("메시지") 대신 DLog.Log("메시지")를 사용하세요.
/// </summary>
public static class DLog
{
	// 로드된 설정 파일을 저장할 정적 변수
	private static IdolDebugSettingsSO s_settings;

	// 외부에서 현재 설정 값을 읽을 수 있도록 public 프로퍼티를 제공합니다. (예: 기즈모 표시 여부 확인용)
	public static IdolDebugSettingsSO Settings => s_settings;

	// 게임 시작 시 단 한 번 자동으로 호출되어 설정 파일을 로드하고 준비합니다.
	// [RuntimeInitializeOnLoadMethod] 덕분에 다른 매니저에서 따로 호출해줄 필요가 없습니다.
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		// Resources 폴더에서 "IdolDebugSettings"라는 이름의 에셋을 찾아 로드합니다.
		s_settings = Resources.Load<IdolDebugSettingsSO>("IdolDebugSettings");

		if (s_settings == null)
		{
			Debug.LogError("디버그 설정 파일(IdolDebugSettings.asset)을 Resources 폴더에서 찾을 수 없습니다! 지시에 따라 새로 생성해주세요.");
		}
	}

	// Debug.Log와 동일한 방식으로 사용할 수 있는 Log 메서드입니다.
	public static void Log(object message)
	{
		// 설정 파일이 로드되었고, 로그 활성화 플래그가 true일 때만 실제 Debug.Log를 호출합니다.
		if (s_settings != null && s_settings.enableDebugLogging)
		{
			Debug.Log(message);
		}
	}

	// 필요하다면 LogWarning, LogError 등도 같은 방식으로 추가하여 제어할 수 있습니다.
	public static void LogWarning(object message)
	{
		if (s_settings != null && s_settings.enableDebugLogging)
		{
			Debug.LogWarning(message);
		}
	}
}