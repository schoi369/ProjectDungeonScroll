using UnityEngine;

public class VFXObject : MonoBehaviour
{
	public float m_lifetime = 0.5f;

	void Start()
	{
		Destroy(gameObject, m_lifetime);
	}
}