using UnityEngine;

public class VFXManager : MonoBehaviour
{
	public static VFXManager Instance { get; private set; }

	[Header("VFX Prefabs")]
	public GameObject m_slashEffectPrefab;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void PlaySlashEffect(Vector3 a_position, Color a_color)
	{
		if (m_slashEffectPrefab == null) return;

		GameObject effect = Instantiate(m_slashEffectPrefab, a_position, Quaternion.identity);

		// 이펙트의 SpriteRenderer를 찾아 색상 변경
		var renderer = effect.GetComponent<SpriteRenderer>();
		if (renderer != null)
		{
			renderer.color = a_color;
		}
	}
}