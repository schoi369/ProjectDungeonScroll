// GenericButtonEditor.cs
using UnityEngine;
using UnityEditor;
using System.Reflection; // 리플렉션을 위해 필수

// MonoBehaviour를 상속하는 모든 컴포넌트에 대해 이 커스텀 에디터를 사용합니다.
[CustomEditor(typeof(MonoBehaviour), true)]
public class GenericButtonEditor : Editor
{
	public override void OnInspectorGUI()
	{
		// 기존 인스펙터를 그대로 그립니다.
		DrawDefaultInspector();

		// 현재 인스펙터에서 타겟으로 하는 오브젝트(스크립트)를 가져옵니다.
		var targetObject = target;

		// 타겟 오브젝트의 모든 메소드를 가져옵니다.
		// BindingFlags를 사용해 public, non-public, 인스턴스 메소드를 모두 포함시킵니다.
		var methods = targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		// 모든 메소드를 순회합니다.
		foreach (var method in methods)
		{
			// 메소드에서 InspectorButtonAttribute 속성이 있는지 찾습니다.
			var attributes = method.GetCustomAttributes(typeof(InspectorButtonAttribute), true);
			if (attributes.Length > 0)
			{
				// 속성이 있다면, 버튼을 생성합니다.
				var attribute = attributes[0] as InspectorButtonAttribute;
				string buttonName = attribute.ButtonLabel ?? method.Name; // 속성에 이름이 지정되었으면 그 이름을, 아니면 메소드 이름을 사용

				if (GUILayout.Button(buttonName))
				{
					// 버튼이 클릭되면, 리플렉션을 통해 해당 메소드를 실행합니다.
					// 현재 이 방법은 매개변수가 없는 메소드에만 작동합니다.
					method.Invoke(targetObject, null);
				}
			}
		}
	}
}