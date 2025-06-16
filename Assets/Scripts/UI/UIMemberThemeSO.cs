using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UIMemberTheme", menuName = "ScriptableObjects/UI/Member Theme")]
public class UIMemberThemeSO : ScriptableObject
{
	// EIdolMember와 Color를 짝지어 인스펙터에서 편집할 수 있게 하는 내부 클래스
	[System.Serializable]
	public class MemberColor
	{
		public EIdolMember m_member;
		public Color m_color = Color.white;
	}

	[SerializeField]
	private List<MemberColor> m_memberColors;

	/// <summary>
	/// 특정 멤버의 색상을 가져옵니다. (캐시 없이 리스트를 직접 검색)
	/// </summary>
	public Color GetMemberColor(EIdolMember a_member)
	{
		// Linq를 사용해 리스트에서 m_member가 일치하는 첫 번째 항목을 찾습니다.
		var foundEntry = m_memberColors.FirstOrDefault(entry => entry.m_member == a_member);

		// 일치하는 항목을 찾았다면 해당 색상을 반환합니다.
		if (foundEntry != null)
		{
			return foundEntry.m_color;
		}

		// 리스트에 해당 멤버의 색상이 정의되지 않은 경우, 경고를 출력하고 기본 색상을 반환합니다.
		Debug.LogWarning($"Color for member '{a_member}' is not defined in {this.name}.");
		return Color.red;
	}
}