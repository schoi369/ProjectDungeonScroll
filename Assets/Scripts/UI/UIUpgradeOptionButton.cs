using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUpgradeOptionButton : MonoBehaviour
{
	[Header("UI References")]
	public TextMeshProUGUI m_nameText;
	public TextMeshProUGUI m_descriptionText;

	private UpgradeSO m_representedUpgrade;

	[SerializeField] UIMemberThemeSO m_memberTheme;
	[SerializeField] Image m_memberColorBoarder;

	/// <summary>
	/// 이 버튼이 어떤 업그레이드를 표시할지 설정하고 UI를 업데이트합니다.
	/// </summary>
	public void Setup(UpgradeSO a_upgradeData)
	{
		m_representedUpgrade = a_upgradeData;

		m_nameText.text = m_representedUpgrade.upgradeName;
		m_descriptionText.text = m_representedUpgrade.description;

		m_memberColorBoarder.color = m_memberTheme.GetMemberColor(m_representedUpgrade.m_idolMember);
	}

	/// <summary>
	/// 이 버튼이 클릭되었을 때 호출될 메서드입니다.
	/// Inspector의 OnClick()에 할당됨.
	/// </summary>
	public void OnSelect()
	{
		if (m_representedUpgrade == null) return;

		// 1. 플레이어에게 업그레이드 적용
		GameManager.Instance.CurrentPlayerData.AddUpgrade(m_representedUpgrade);

		// 2. StageManager에게 선택이 끝났음을 알림
		StageManager.Instance.EndUpgradeSelection();
	}
}