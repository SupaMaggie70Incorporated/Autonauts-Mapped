using UnityEngine;

public class StorageSeedlings : StorageFueler
{
	private GameObject[] m_Seedlings;

	private GameObject m_Seed;

	private static int m_NumWide = 5;

	private static int m_NumHigh = 3;

	private static float Spacing = 1.5f;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 0), new TileCoord(0, 1));
	}

	protected new void Awake()
	{
		base.Awake();
		m_Capacity = 150;
		m_Tier = BurnableInfo.Tier.Fertiliser;
		m_FuelCapacity = 9f;
		m_RequiredType = ObjectType.Nothing;
	}

	protected new void OnDestroy()
	{
		DestroySeedingModels();
		DestroySeedModel();
		base.OnDestroy();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (AndDestroy)
		{
			DestroySeedingModels();
			DestroySeedModel();
		}
		base.StopUsing(AndDestroy);
	}

	private void DestroySeedingModels()
	{
		if (m_Seedlings == null)
		{
			return;
		}
		for (int i = 0; i < m_Seedlings.Length; i++)
		{
			if ((bool)m_Seedlings[i])
			{
				Object.Destroy(m_Seedlings[i].gameObject);
				m_Seedlings[i] = null;
			}
		}
	}

	private void CreateSeedlingModels()
	{
		Transform parent = m_ModelRoot.transform;
		int num = m_NumWide * m_NumHigh;
		string text = "Seedling";
		if (m_ObjectType == ObjectType.SeedlingMulberry)
		{
			text = "SeedlingMulberry";
		}
		GameObject original = (GameObject)Resources.Load("Models/Other/" + text, typeof(GameObject));
		m_Seedlings = new GameObject[num];
		for (int i = 0; i < m_NumHigh; i++)
		{
			for (int j = 0; j < m_NumWide; j++)
			{
				GameObject gameObject = Object.Instantiate(original, Vector3.zero, Quaternion.Euler(0f, 0f, 90f), null);
				gameObject.transform.SetParent(parent);
				Vector3 localPosition = new Vector3((float)j * Spacing - 3f, 0.6f, (float)i * Spacing);
				gameObject.transform.localPosition = localPosition;
				gameObject.SetActive(value: false);
				float num2 = Random.Range(0.75f, 1f);
				gameObject.transform.localScale = new Vector3(num2, num2, num2);
				gameObject.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 90f);
				m_Seedlings[i * m_NumWide + j] = gameObject;
			}
		}
	}

	private void DestroySeedModel()
	{
		if ((bool)m_Seed)
		{
			Object.Destroy(m_Seed.gameObject);
			m_Seed = null;
		}
	}

	private void CreateSeedModel()
	{
		Transform parent = m_ModelRoot.transform;
		string text = "TreeSeed";
		float z = 90f;
		float num = 0.7f;
		if (m_ObjectType == ObjectType.SeedlingMulberry)
		{
			text = "MulberrySeed";
			z = 0f;
			num = 0.5f;
		}
		GameObject original = (GameObject)Resources.Load("Models/Other/" + text, typeof(GameObject));
		m_Seed = Object.Instantiate(original, Vector3.zero, Quaternion.Euler(0f, 0f, z), null);
		m_Seed.transform.SetParent(parent);
		m_Seed.transform.localScale = new Vector3(num, num, num);
		m_Seed.SetActive(value: false);
		UpdateSeed();
	}

	private void UpdateSeed()
	{
		if (m_RequiredCount == 0 || GetStored() >= m_NumWide * m_NumHigh)
		{
			m_Seed.SetActive(value: false);
			return;
		}
		int stored = GetStored();
		int num = stored % m_NumWide;
		int num2 = stored / m_NumWide;
		float y = 0.4f;
		if (m_ObjectType == ObjectType.SeedlingMulberry)
		{
			y = 0.6f;
		}
		Vector3 localPosition = new Vector3((float)num * Spacing - 3f, y, (float)num2 * Spacing);
		m_Seed.transform.localPosition = localPosition;
		m_Seed.SetActive(value: true);
	}

	public override void SetObjectType(ObjectType NewType)
	{
		base.SetObjectType(NewType);
		if (m_ObjectType == ObjectType.Seedling && m_RequiredType != ObjectType.TreeSeed)
		{
			m_RequiredType = ObjectType.TreeSeed;
		}
		else if (m_ObjectType == ObjectType.SeedlingMulberry && m_RequiredType != ObjectType.MulberrySeed)
		{
			m_RequiredType = ObjectType.MulberrySeed;
		}
		DestroySeedingModels();
		CreateSeedlingModels();
		DestroySeedModel();
		CreateSeedModel();
		UpdateStored();
	}

	public override void UpdateStored()
	{
		base.UpdateStored();
		if (m_ObjectType == ObjectType.Seedling && m_RequiredType != ObjectType.TreeSeed)
		{
			m_RequiredType = ObjectType.TreeSeed;
		}
		else if (m_ObjectType == ObjectType.SeedlingMulberry && m_RequiredType != ObjectType.MulberrySeed)
		{
			m_RequiredType = ObjectType.MulberrySeed;
		}
		int stored = GetStored();
		for (int i = 0; i < m_Seedlings.Length; i++)
		{
			if (i < stored)
			{
				MeshRenderer[] componentsInChildren = m_Seedlings[i].GetComponentsInChildren<MeshRenderer>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].enabled = true;
				}
				m_Seedlings[i].SetActive(value: true);
			}
			else
			{
				m_Seedlings[i].SetActive(value: false);
			}
		}
		UpdateSeed();
	}

	protected override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (NewType != ObjectType.Seedling && NewType != ObjectType.SeedlingMulberry)
		{
			return false;
		}
		if (NewType != m_ObjectType && m_Stored != 0)
		{
			return false;
		}
		if (m_Stored == 0 && m_RequiredCount != 0)
		{
			if (NewType == ObjectType.Seedling && m_RequiredType == ObjectType.MulberrySeed)
			{
				return false;
			}
			if (NewType == ObjectType.SeedlingMulberry && m_RequiredType == ObjectType.TreeSeed)
			{
				return false;
			}
		}
		return base.CanAcceptObject(NewObject, NewType);
	}

	protected override void DoConversion(Actionable Actioner)
	{
		base.DoConversion(Actioner);
		bool bot = false;
		if ((bool)Actioner && Actioner.m_TypeIdentifier == ObjectType.Worker)
		{
			bot = true;
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, bot, ObjectType.Seedling, this);
		UpdateSeed();
	}

	protected override bool CanAcceptRequiredType(ObjectType NewType)
	{
		if (!base.CanAcceptRequiredType(NewType))
		{
			return false;
		}
		if (NewType != ObjectType.TreeSeed && NewType != ObjectType.MulberrySeed)
		{
			return false;
		}
		return true;
	}

	protected override void StartAddRequiredType(AFO Info)
	{
		base.StartAddRequiredType(Info);
		if (Info.m_ObjectType == ObjectType.TreeSeed)
		{
			SetObjectType(ObjectType.Seedling);
		}
		else if (Info.m_ObjectType == ObjectType.MulberrySeed)
		{
			SetObjectType(ObjectType.SeedlingMulberry);
		}
		UpdateSeed();
	}
}
