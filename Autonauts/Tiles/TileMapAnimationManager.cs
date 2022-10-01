using UnityEngine;

public class TileMapAnimationManager : MonoBehaviour
{
	public static TileMapAnimationManager Instance;

	public Texture2D m_BlendMapTexture;

	private GameObject m_WaveAnimation;

	public GameObject m_WaveAnimator1;

	public GameObject m_WaveAnimator2;

	public GameObject m_WaveAnimator3;

	public float m_WaveOffset1;

	public float m_WaveOffset2;

	public float m_OldWaveOffset1;

	public float m_OldWaveOffset2;

	public float m_WaveDirection1;

	public float m_WaveDirection2;

	public float m_OldWaveDirection1;

	public float m_OldWaveDirection2;

	private bool m_LoadBlendMap;

	private float m_RippleOffset;

	private void Awake()
	{
		Instance = this;
		LoadBlendMap();
		m_LoadBlendMap = false;
		GameObject original = (GameObject)Resources.Load("WaveAnimation/WaveAnimation", typeof(GameObject));
		m_WaveAnimation = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		m_WaveAnimation.transform.Find("AnimatorCube").GetComponent<MeshRenderer>().enabled = false;
		m_WaveAnimator1 = m_WaveAnimation.transform.Find("WaveAnimator1").gameObject;
		m_WaveAnimator1.GetComponent<MeshRenderer>().enabled = false;
		m_WaveAnimator2 = m_WaveAnimation.transform.Find("WaveAnimator2").gameObject;
		m_WaveAnimator2.GetComponent<MeshRenderer>().enabled = false;
		m_WaveAnimator3 = m_WaveAnimation.transform.Find("WaveAnimator3").gameObject;
		m_WaveAnimator3.GetComponent<MeshRenderer>().enabled = false;
		Animator[] componentsInChildren = m_WaveAnimation.GetComponentsInChildren<Animator>();
		foreach (Animator newAnimator in componentsInChildren)
		{
			TimeManager.Instance.RegisterAnimator(newAnimator);
		}
		UpdateWaves();
		original = (GameObject)Resources.Load("Prefabs/Particles/WaterSparkles", typeof(GameObject));
		Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, null);
	}

	private void OnDestroy()
	{
		if (TimeManager.Instance != null)
		{
			Animator[] componentsInChildren = m_WaveAnimation.GetComponentsInChildren<Animator>();
			foreach (Animator newAnimator in componentsInChildren)
			{
				TimeManager.Instance.UnRegisterAnimator(newAnimator);
			}
			Object.DestroyImmediate(m_WaveAnimation.gameObject);
		}
	}

	public void StartLoadBlendMap()
	{
		m_LoadBlendMap = true;
	}

	private void LoadBlendMap()
	{
		int num = 1024;
		Texture2D src = (Texture2D)Resources.Load("Textures/Writable/BlendMap", typeof(Texture2D));
		m_BlendMapTexture = new Texture2D(num, num, TextureFormat.RGBA32, mipChain: false, linear: true);
		m_BlendMapTexture.filterMode = FilterMode.Point;
		for (int i = 0; i < num; i++)
		{
			Graphics.CopyTexture(src, 0, 0, 0, i, num, 1, m_BlendMapTexture, 0, 0, 0, num - 1 - i);
		}
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, Dark: false).SetTexture("_BlendTex", m_BlendMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, Dark: true).SetTexture("_BlendTex", m_BlendMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, Dark: false).SetTexture("_BlendTex", m_BlendMapTexture);
		MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, Dark: true).SetTexture("_BlendTex", m_BlendMapTexture);
	}

	public void ResetWaves()
	{
		UpdateWaves();
	}

	private void UpdateWaves()
	{
		m_OldWaveOffset1 = m_WaveOffset1;
		m_OldWaveOffset2 = m_WaveOffset2;
		m_WaveOffset1 = m_WaveAnimator1.transform.position.x;
		m_WaveOffset2 = m_WaveAnimator2.transform.position.x;
		m_OldWaveDirection1 = m_WaveDirection1;
		m_OldWaveDirection2 = m_WaveDirection2;
		if (m_WaveOffset1 > m_OldWaveOffset1)
		{
			m_WaveDirection1 = 1f;
		}
		else
		{
			m_WaveDirection1 = -1f;
		}
		if (m_WaveOffset2 > m_OldWaveOffset2)
		{
			m_WaveDirection2 = 1f;
		}
		else
		{
			m_WaveDirection2 = -1f;
		}
		float num = Tile.m_Size - PlotMeshBuilderWater.Bevel1 - PlotMeshBuilderWater.Bevel2;
		float value = (1f - m_WaveOffset1) * num + PlotMeshBuilderWater.Bevel2;
		Shader.SetGlobalFloat("_WaveOffset", value);
		float value2 = (1f - m_WaveOffset2) * num + PlotMeshBuilderWater.Bevel2;
		Shader.SetGlobalFloat("_SurfOffset", value2);
		int num2 = (int)(m_WaveAnimator2.transform.position.y / 0.1f * 255f);
		Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)num2);
		Shader.SetGlobalColor("_SurfColor", color);
		Shader.SetGlobalFloat("_WetSandOffset", value);
		num2 = (int)(m_WaveAnimator3.transform.position.y / 0.1f * 0.5f * 255f);
		color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)num2);
		Shader.SetGlobalColor("_WetSandColor", color);
		m_RippleOffset += TimeManager.Instance.m_NormalDelta;
		Shader.SetGlobalFloat("_RippleOffset", m_RippleOffset);
	}

	private void Update()
	{
		UpdateWaves();
		if (m_LoadBlendMap)
		{
			m_LoadBlendMap = false;
			LoadBlendMap();
		}
	}
}
