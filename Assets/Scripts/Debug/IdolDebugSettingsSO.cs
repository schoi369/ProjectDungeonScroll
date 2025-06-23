using UnityEngine;

// 에디터의 Create > ScriptableObjects > Idol Debug Settings 메뉴를 통해 에셋을 생성할 수 있습니다.
[CreateAssetMenu(fileName = "IdolDebugSettings", menuName = "ScriptableObjects/Idol Debug Settings")]
public class IdolDebugSettingsSO : ScriptableObject
{
	[Header("Logging")]
	[Tooltip("체크하면 DLog.Log()를 통한 로그가 콘솔에 표시됩니다.")]
	public bool enableDebugLogging = true;

	[Header("Gizmos")]
	[Tooltip("체크하면 공격 범위 기즈모 등 디버그용 기즈모가 씬 뷰에 표시됩니다.")]
	public bool showDebugGizmos = true;

	// 나중에 여기에 '적 무적 모드', 'UI 디버그 정보 표시' 등
	// 필요한 bool, int, string 변수를 계속 추가하여 중앙에서 관리할 수 있습니다.
}