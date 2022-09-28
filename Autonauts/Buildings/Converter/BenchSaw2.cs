using UnityEngine;

public class BenchSaw2 : LinkedSystemConverter
{
	private GameObject m_Blade;

	private PlaySound m_PlaySound;

	private MeshRenderer m_Mesh;

	private Material[] m_Materials;

	private int m_MaterialIndex;

	private Material m_FlashOnMaterial;

	private Material m_FlashOffMaterial;

	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, 0), new TileCoord(1, 0), new TileCoord(-2, 0));
		SetSpawnPoint(new TileCoord(2, 0));
		m_Blade = m_ModelRoot.transform.Find("Blade").gameObject;
		m_PulleySide = 1;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Mesh = m_ModelRoot.transform.Find("Bench").GetComponent<MeshRenderer>();
		m_Materials = m_Mesh.materials;
		m_MaterialIndex = 0;
		Material[] materials = m_Materials;
		foreach (Material material in materials)
		{
			if (material.name.Contains("SharedYellowGlow") || material.name.Contains("SharedYellow"))
			{
				break;
			}
			m_MaterialIndex++;
		}
		Material original = (Material)Resources.Load("Models/Materials/SharedYellowGlow", typeof(Material));
		m_FlashOnMaterial = Object.Instantiate(original);
		original = (Material)Resources.Load("Models/Materials/SharedYellow", typeof(Material));
		m_FlashOffMaterial = Object.Instantiate(original);
		m_Materials[m_MaterialIndex] = m_FlashOffMaterial;
		m_Mesh.materials = m_Materials;
	}

	protected override void UpdateIngredients()
	{
		if (m_Ingredients.Count > 0)
		{
			m_Ingredients[0].gameObject.SetActive(value: true);
			m_Ingredients[0].transform.parent = m_IngredientsRoot;
			m_Ingredients[0].transform.localPosition = new Vector3(0f, 0f, 0f);
			m_Ingredients[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBenchSaw2Making", this, Remember: true);
		m_Ingredients[0].transform.localRotation = Quaternion.identity;
		m_Particles = ParticlesManager.Instance.CreateParticles("Chips", base.transform.localPosition + new Vector3(-1.26f, -1.08f, 0.03f), base.transform.rotation * Quaternion.Euler(-60f, 90f, 0f));
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true);
	}

	protected override void UpdateConverting()
	{
		m_Blade.transform.localRotation = Quaternion.Euler(0f, 0f, m_StateTimer * 360f * 20f) * ObjectUtils.m_ModelRotator;
		float num = m_StateTimer / m_ConversionDelay;
		m_Ingredients[0].transform.localPosition = new Vector3(num * 6f, 0f, 0f);
		if ((int)(m_StateTimer * 60f) % 10 < 5)
		{
			m_Materials[m_MaterialIndex] = m_FlashOnMaterial;
		}
		else
		{
			m_Materials[m_MaterialIndex] = m_FlashOffMaterial;
		}
		m_Mesh.materials = m_Materials;
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_Materials[m_MaterialIndex] = m_FlashOffMaterial;
		m_Mesh.materials = m_Materials;
		m_Blade.transform.localRotation = Quaternion.Euler(0f, 0f, 35f) * ObjectUtils.m_ModelRotator;
		m_Particles.Stop();
		QuestManager.Instance.AddEvent(QuestEvent.Type.ProcessWood, m_LastEngagerType == ObjectType.Worker, 0, this);
	}
}
