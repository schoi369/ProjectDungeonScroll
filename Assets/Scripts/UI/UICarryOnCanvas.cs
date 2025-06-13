using UnityEngine;
using UnityEngine.UI;

public class UICarryOnCanvas : MonoBehaviour
{
	public static UICarryOnCanvas Instance;

	public Image m_transitionBoxImage;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Fade(bool a_turnBlack)
	{
		var anim = m_transitionBoxImage.GetComponent<Animator>();
		anim.SetBool("Show", a_turnBlack);
	}
}
