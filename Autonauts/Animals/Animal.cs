using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class Animal : GoTo
{
	[HideInInspector]
	public AnimalStatusIndicator m_Indicator;

	[HideInInspector]
	public float m_Scale;

	[HideInInspector]
	public Actionable m_BaggedObject;

	protected int m_BaggedObjectUID;

	private TileCoord m_BaggedTile;

	public static bool GetIsTypeAnimal(ObjectType NewType)
	{
		if (NewType == ObjectType.AnimalBee || NewType == ObjectType.AnimalBird || NewType == ObjectType.AnimalLeech || NewType == ObjectType.AnimalSilkworm || NewType == ObjectType.AnimalSilkmoth || AnimalPet.GetIsTypePet(NewType) || NewType == ObjectType.FishBait || Fish.GetIsTypeFish(NewType) || AnimalGrazer.GetIsTypeAnimalGrazer(NewType))
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeAnimalDespawn(ObjectType NewType)
	{
		if (NewType == ObjectType.AnimalBee || NewType == ObjectType.AnimalBird || NewType == ObjectType.AnimalSilkmoth || AnimalPet.GetIsTypePet(NewType) || Fish.GetIsTypeFish(NewType) || AnimalGrazer.GetIsTypeAnimalGrazer(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Animal", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		if ((bool)m_Indicator)
		{
			m_Indicator.Restart();
			m_Indicator.GetComponent<Image>().enabled = true;
		}
		UpdateSize();
	}

	protected new void Awake()
	{
		base.Awake();
		m_Scale = Random.Range(0.5f, 1f);
		if ((bool)GetComponent<AnimalGrazer>() || (bool)GetComponent<AnimalBee>() || (bool)GetComponent<AnimalPet>())
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/Animals/AnimalStatusIndicator", typeof(GameObject));
			Transform parent = null;
			if ((bool)HudManager.Instance)
			{
				parent = HudManager.Instance.m_IndicatorsRootTransform;
			}
			m_Indicator = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<AnimalStatusIndicator>();
			m_Indicator.SetParent(this);
			m_Indicator.GetComponent<Image>().enabled = false;
		}
	}

	protected new void OnDestroy()
	{
		if ((bool)m_Indicator)
		{
			Object.Destroy(m_Indicator.gameObject);
		}
		base.OnDestroy();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		if ((bool)m_Indicator)
		{
			m_Indicator.gameObject.SetActive(value: false);
		}
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "S", m_Scale);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		float asFloat = JSONUtils.GetAsFloat(Node, "S", 1f);
		SetSize(asFloat);
	}

	private void SetSize(float Scale)
	{
		m_Scale = Scale;
		UpdateSize();
	}

	private void UpdateSize()
	{
		m_ModelRoot.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
	}

	public void SetParents(Animal Parent1, Animal Parent2)
	{
		float num = Parent1.m_Scale + Parent2.m_Scale;
		num /= 2f;
		num = num * 0.9f + Random.Range(0f, num * 0.2f);
		SetSize(num);
	}

	public void SetBaggedObject(TileCoordObject NewObject)
	{
		if ((bool)m_BaggedObject)
		{
			BaggedManager.Instance.Remove(m_BaggedObject);
			m_BaggedObject.SendAction(new ActionInfo(ActionType.SetUnbagged, default(TileCoord), this));
		}
		m_BaggedObject = NewObject;
		if ((bool)m_BaggedObject)
		{
			m_BaggedObjectUID = m_BaggedObject.m_UniqueID;
			BaggedManager.Instance.Add(m_BaggedObject, this);
			NewObject.SendAction(new ActionInfo(ActionType.SetBagged, default(TileCoord), this));
		}
	}

	public void SetBaggedTile(TileCoord NewPosition)
	{
		if (m_BaggedTile.x != 0 && m_BaggedTile.y != 0)
		{
			BaggedManager.Instance.Remove(m_BaggedTile);
		}
		m_BaggedTile = NewPosition;
		if (m_BaggedTile.x != 0 && m_BaggedTile.y != 0)
		{
			BaggedManager.Instance.Add(m_BaggedTile, this);
		}
	}
}
