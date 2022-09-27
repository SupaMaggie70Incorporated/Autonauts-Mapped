using UnityEngine;
using UnityEngine.UI;

public class CameraCrossFade : MonoBehaviour
{
	private static float m_FadingDelay = 0.5f;

	private bool m_Fading;

	private bool m_FadingUp;

	private float m_FadingTimer;

	private RawImage m_Image;

	private Material m_SkyBoxMaterial;

	private Material m_OldSkyBox;

	private void Awake()
	{
		m_Image = GameObject.Find("CrossFadeImage").GetComponent<RawImage>();
		Material original = (Material)Resources.Load("Materials/MainMenuSkyBox", typeof(Material));
		m_SkyBoxMaterial = Object.Instantiate(original);
	}

	public void StartFade(bool FadeUp)
	{
		m_Fading = true;
		m_FadingUp = FadeUp;
		m_FadingTimer = 0f;
		if ((bool)SpaceAnimation.Instance)
		{
			SpaceAnimation.Instance.UseAudioListener(Use: true);
		}
	}

	private void Update()
	{
		if (!m_Fading)
		{
			return;
		}
		if ((bool)TimeManager.Instance)
		{
			m_FadingTimer += TimeManager.Instance.m_NormalDelta;
		}
		if (m_FadingTimer > m_FadingDelay)
		{
			m_FadingTimer = m_FadingDelay;
			if (m_FadingUp && (bool)SpaceAnimation.Instance)
			{
				SpaceAnimation.Instance.UseAudioListener(Use: false);
			}
			m_Fading = false;
		}
		float num = m_FadingTimer / m_FadingDelay;
		if (m_FadingUp)
		{
			num = 1f - num;
		}
		if ((bool)SpaceAnimation.Instance)
		{
			SpaceAnimation.Instance.SetCrossFadeValue(num);
		}
		Color color = new Color(1f, 1f, 1f, num);
		m_Image.color = color;
	}

	private void OnPreRender()
	{
		m_OldSkyBox = RenderSettings.skybox;
		RenderSettings.skybox = m_SkyBoxMaterial;
	}

	private void OnPostRender()
	{
		RenderSettings.skybox = m_OldSkyBox;
	}
}
