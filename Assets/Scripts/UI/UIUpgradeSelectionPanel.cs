using System.Collections.Generic;
using UnityEngine;

public class UIUpgradeSelectionPanel : MonoBehaviour
{
	[Header("References")]
	public GameObject m_optionButtonPrefab; // UIUpgradeOptionButton 프리팹을 연결
	public Transform m_optionsParent; // 생성된 버튼들이 위치할 부모 오브젝트 (Layout Group 사용 권장)

	/// <summary>
	/// 업그레이드 선택지들을 받아와 화면에 표시합니다.
	/// </summary>
	public void ShowOptions(List<UpgradeSO> a_options)
	{
		// 기존에 있던 선택지들을 모두 삭제
		foreach (Transform child in m_optionsParent)
		{
			Destroy(child.gameObject);
		}

		// 새로운 선택지들을 생성
		foreach (var optionData in a_options)
		{
			GameObject newButtonObj = Instantiate(m_optionButtonPrefab, m_optionsParent);
			UIUpgradeOptionButton buttonScript = newButtonObj.GetComponent<UIUpgradeOptionButton>();
			buttonScript.Setup(optionData);
		}

		gameObject.SetActive(true); // 패널 활성화
	}

	/// <summary>
	/// 패널을 숨깁니다.
	/// </summary>
	public void Hide()
	{
		gameObject.SetActive(false); // 패널 비활성화
	}
}