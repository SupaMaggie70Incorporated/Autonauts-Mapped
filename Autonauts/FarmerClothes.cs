using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class FarmerClothes : MonoBehaviour
{
	public enum Type
	{
		Hat,
		Gloves,
		Shoes,
		Top,
		Trousers,
		Total
	}

	private Farmer m_Farmer;

	private GameObject[] m_Parents;

	public List<Holdable> m_Clothes;

	private void Awake()
	{
		m_Farmer = GetComponent<Farmer>();
		m_Clothes = new List<Holdable>();
	}

	public void Restart()
	{
		RefreshParents();
		SwapPointAxis(Type.Top);
		SwapPointAxis(Type.Hat);
	}

	public void RefreshParents()
	{
		m_Parents = new GameObject[5];
		for (int i = 0; i < 5; i++)
		{
			string[] array = new string[5] { "HatPoint", "GlovesPoint", "ShoesPoint", "TopPoint", "TrousersPoint" };
			m_Parents[i] = FindChildByName(m_Farmer.m_ModelRoot.transform, array[i]);
			if ((bool)m_Parents[i])
			{
				m_Parents[i].transform.localRotation = Quaternion.identity;
			}
		}
	}

	public void UpdateScales()
	{
		for (int i = 0; i < 5; i++)
		{
			if ((bool)m_Parents[i])
			{
				Vector3 localScale = m_Parents[i].transform.parent.localScale;
				localScale.x = 1f / localScale.x;
				localScale.y = 1f / localScale.y;
				localScale.z = 1f / localScale.z;
				if ((bool)m_Parents[i])
				{
					m_Parents[i].transform.localScale = localScale;
				}
			}
		}
	}

	public void SwapPointAxis(Type NewType)
	{
		if ((bool)m_Parents[(int)NewType])
		{
			Vector3 localScale = m_Parents[(int)NewType].transform.localScale;
			m_Parents[(int)NewType].transform.localRotation = Quaternion.identity;
			m_Parents[(int)NewType].transform.localScale = new Vector3(localScale.x, localScale.z, localScale.y);
		}
	}

	private GameObject FindChildByName(Transform Parent, string Name)
	{
		if ((bool)Parent.transform.Find(Name))
		{
			return Parent.transform.Find(Name).gameObject;
		}
		foreach (Transform item in Parent.transform)
		{
			GameObject gameObject = FindChildByName(item, Name);
			if ((bool)gameObject)
			{
				return gameObject;
			}
		}
		return null;
	}

	public void StopUsing(bool AndDestroy = true)
	{
		for (int i = 0; i < m_Clothes.Count; i++)
		{
			m_Clothes[i].StopUsing();
		}
	}

	public void PreRefreshPlayerModel()
	{
		foreach (Clothing clothe in m_Clothes)
		{
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.DragInventorySlot)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().CheckObjectDropped(clothe);
			}
			clothe.transform.SetParent(null);
		}
	}

	public void RefreshPlayerModel()
	{
		foreach (Clothing clothe in m_Clothes)
		{
			if (clothe.GetComponent<Hat>() != null)
			{
				clothe.GetComponent<Clothing>().Wear(m_Farmer.gameObject, m_Parents[0].transform);
			}
			else if (clothe.GetComponent<Top>() != null)
			{
				clothe.GetComponent<Clothing>().Wear(m_Farmer.gameObject, m_Parents[3].transform);
			}
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["ClothesObjects"] = new JSONArray());
		for (int i = 0; i < m_Clothes.Count; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Clothes[i].GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			m_Clothes[i].GetComponent<Savable>().Save(jSONNode2);
			jSONArray[i] = jSONNode2;
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["ClothesObjects"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asObject);
				Add(baseClass.GetComponent<Holdable>());
			}
		}
	}

	public Holdable Get(Type NewType)
	{
		for (int i = 0; i < m_Clothes.Count; i++)
		{
			if (NewType == Type.Hat && (bool)m_Clothes[i].GetComponent<Hat>())
			{
				return m_Clothes[i];
			}
			if (NewType == Type.Top && (bool)m_Clothes[i].GetComponent<Top>())
			{
				return m_Clothes[i];
			}
		}
		return null;
	}

	public Type GetType(Holdable NewClothing)
	{
		if (Hat.GetIsTypeHat(NewClothing.m_TypeIdentifier))
		{
			return Type.Hat;
		}
		if (Top.GetIsTypeTop(NewClothing.m_TypeIdentifier))
		{
			return Type.Top;
		}
		return Type.Total;
	}

	private void UpdateAll()
	{
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().CheckTargetUpdated(m_Farmer);
		}
	}

	public void ReadyAdd(Holdable NewClothing)
	{
		Type type = GetType(NewClothing);
		NewClothing.transform.position = m_Parents[(int)type].transform.position;
	}

	public virtual bool Add(Holdable NewClothing)
	{
		Type type = GetType(NewClothing);
		Holdable holdable = Get(type);
		if ((bool)holdable)
		{
			holdable.GetComponent<Clothing>().Remove();
			m_Clothes.Remove(holdable);
			TileCoord tileCoord = m_Farmer.m_TileCoord;
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			holdable.ForceHighlight(Highlighted: false);
			holdable.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, m_Farmer));
			float yOffset = ObjectTypeList.Instance.GetYOffset(holdable.m_TypeIdentifier);
			float y = randomEmptyTile.ToWorldPositionTileCentered().y;
			SpawnAnimationManager.Instance.AddJump(holdable, tileCoord, randomEmptyTile, m_Farmer.transform.position.y, yOffset + y, 4f);
		}
		m_Clothes.Add(NewClothing);
		NewClothing.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), m_Farmer));
		NewClothing.GetComponent<Clothing>().Wear(m_Farmer.gameObject, m_Parents[(int)type].transform);
		AudioManager.Instance.StartEvent("ClothingApplied", m_Farmer);
		UpdateAll();
		return true;
	}

	public virtual bool Remove(Holdable NewClothing)
	{
		if (m_Clothes.Contains(NewClothing))
		{
			NewClothing.GetComponent<Clothing>().Remove();
			NewClothing.SendAction(new ActionInfo(ActionType.Dropped, m_Farmer.m_TileCoord, m_Farmer));
			m_Clothes.Remove(NewClothing);
			UpdateAll();
			return true;
		}
		return false;
	}

	public Holdable Remove(Type NewType)
	{
		Holdable holdable = Get(NewType);
		if ((bool)holdable)
		{
			Remove(holdable);
		}
		return holdable;
	}
}
