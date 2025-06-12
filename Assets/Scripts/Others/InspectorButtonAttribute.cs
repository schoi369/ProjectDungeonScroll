using System;

// 이 속성은 메소드에만 사용할 수 있도록 지정합니다.
[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : Attribute
{
	public readonly string ButtonLabel;

	// 속성에 이름을 지정할 수 있도록 생성자를 만듭니다.
	// 예: [InspectorButton("체력 회복")]
	public InspectorButtonAttribute(string buttonLabel = null)
	{
		ButtonLabel = buttonLabel;
	}
}