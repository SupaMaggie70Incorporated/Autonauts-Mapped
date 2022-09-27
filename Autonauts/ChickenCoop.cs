using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class ChickenCoop : Converter
{
	private PlaySound m_PlaySound;

	private ObjectType m_AnimalType;

	private List<AnimalChicken> m_Chickens;

	private AnimalStatusIndicator m_StatusIndicator;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 1), new TileCoord(0, 2));
		SetSpawnPoint(new TileCoord(2, 0));
		SetResultToCreate(1);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Chickens = new List<AnimalChicken>();
		GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/Animals/AnimalStatusIndicator", typeof(GameObject));
		Transform parent = null;
		if ((bool)HudManager.Instance)
		{
			parent = HudManager.Instance.m_IndicatorsRootTransform;
		}
		m_StatusIndicator = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<AnimalStatusIndicator>();
		m_StatusIndicator.GetComponent<Image>().enabled = false;
		m_StatusIndicator.SetParent(this);
		m_StatusIndicator.SetAllOff();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONArray jSONArray = (JSONArray)(Node["Chickens"] = new JSONArray());
		for (int i = 0; i < m_Chickens.Count; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(ObjectType.AnimalChicken);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			m_Chickens[i].GetComponent<Savable>().Save(jSONNode2);
			jSONArray[i] = jSONNode2;
		}
	}

	private bool FindChook(int UID)
	{
		foreach (AnimalChicken chicken in m_Chickens)
		{
			if (chicken.m_UniqueID == UID)
			{
				return true;
			}
		}
		return false;
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONArray asArray = Node["Chickens"].AsArray;
		if (!(asArray != null) || asArray.IsNull)
		{
			return;
		}
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			int asInt = JSONUtils.GetAsInt(asObject, "UID", -1);
			if (!FindChook(asInt))
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false);
				if (objectFromUniqueID != null && objectFromUniqueID.GetComponent<TileCoordObject>().m_Plot != null)
				{
					objectFromUniqueID.StopUsing();
				}
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				if ((bool)baseClass)
				{
					baseClass.GetComponent<Savable>().Load(asObject);
					AnimalChicken component = baseClass.GetComponent<AnimalChicken>();
					AddChicken(component);
					component.SetState(AnimalGrazer.State.WaitToExitBuilding);
				}
			}
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) > 1u)
		{
			return;
		}
		List<AnimalChicken> list = new List<AnimalChicken>();
		foreach (AnimalChicken chicken in m_Chickens)
		{
			list.Add(chicken);
		}
		foreach (AnimalChicken item in list)
		{
			item.SendAction(Info);
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Engaged && m_Chickens.Count > 0)
		{
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if ((uint)(action - 3) <= 1u && m_Chickens.Count > 0)
		{
			return false;
		}
		return base.GetActionInfo(Info);
	}

	public override bool CanAcceptIngredient(ObjectType NewType)
	{
		if (m_Chickens.Count > 0)
		{
			return false;
		}
		return base.CanAcceptIngredient(NewType);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (Info.m_Actioner.m_TypeIdentifier == ObjectType.AnimalChicken)
			{
				return ActionType.AddResource;
			}
			if (Info.m_ObjectType == ObjectType.AnimalChicken && ObjectTypeList.m_ObjectTypeCounts[199] >= 100)
			{
				return ActionType.Fail;
			}
		}
		return base.GetActionFromObject(Info);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWorkbenchMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void CreateAnimal()
	{
		Holdable component = m_Ingredients[0].GetComponent<Holdable>();
		m_Ingredients.RemoveAt(0);
		TileCoord spawnPoint = GetSpawnPoint();
		TileCoord spawnPointEject = GetSpawnPointEject();
		component.SendAction(new ActionInfo(ActionType.Dropped, spawnPointEject));
		component.UpdatePositionToTilePosition(GetSpawnPoint());
		PlotManager.Instance.RemoveObject(component);
		PlotManager.Instance.AddObject(component);
		SpawnAnimationManager.Instance.AddJump(component, spawnPointEject, spawnPoint, 0f, component.transform.position.y, 4f);
		component.gameObject.SetActive(value: true);
	}

	public void AddChicken(AnimalChicken NewChicken)
	{
		NewChicken.SendAction(new ActionInfo(ActionType.BeingHeld, NewChicken.m_TileCoord, this));
		m_Chickens.Add(NewChicken);
	}

	public void RemoveChicken(AnimalChicken NewChicken)
	{
		NewChicken.SendAction(new ActionInfo(ActionType.Dropped, NewChicken.m_TileCoord, this));
		m_Chickens.Remove(NewChicken);
		if (m_Chickens.Count == 0)
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
			m_StatusIndicator.SetSleeping(Sleeping: false);
		}
	}

	private void UpdateChickensLaying()
	{
		if (m_Chickens.Count > 0)
		{
			if (!DayNightManager.Instance.GetIsNightTime())
			{
				if ((int)(m_StateTimer * 60f) % 20 < 10)
				{
					m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
				}
				else
				{
					m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				m_StatusIndicator.SetSleeping(Sleeping: false);
			}
			else
			{
				m_StatusIndicator.SetSleeping(Sleeping: true);
				if ((int)(m_StateTimer * 60f) % 120 < 60)
				{
					m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
				}
				else
				{
					m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void RemoveFood()
	{
		List<Holdable> list = new List<Holdable>();
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (!Animal.GetIsTypeAnimal(ingredient.m_TypeIdentifier))
			{
				list.Add(ingredient);
			}
		}
		foreach (Holdable item in list)
		{
			m_Ingredients.Remove(item);
			item.StopUsing();
		}
	}

	protected new void Update()
	{
		if (m_Chickens.Count > 0)
		{
			UpdateChickensLaying();
		}
		else
		{
			switch (m_State)
			{
			case State.Converting:
			{
				UpdateConverting();
				if (!(m_StateTimer >= m_ConversionDelay))
				{
					break;
				}
				ObjectType type = m_Results[m_ResultsToCreate][0].m_Type;
				Vector3 position = m_TileCoord.ToWorldPositionTileCentered();
				Holdable component = ObjectTypeList.Instance.CreateObjectFromIdentifier(type, position, Quaternion.identity).GetComponent<Holdable>();
				component.gameObject.SetActive(value: false);
				List<Animal> list = new List<Animal>();
				foreach (Holdable ingredient in m_Ingredients)
				{
					if (Animal.GetIsTypeAnimal(ingredient.m_TypeIdentifier))
					{
						list.Add(ingredient.GetComponent<Animal>());
					}
				}
				component.GetComponent<Animal>().SetParents(list[0], list[1]);
				m_Ingredients.Add(component);
				component.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
				EndConverting();
				RemoveFood();
				SetState(State.Creating);
				m_StateTimer = m_CreateDelay;
				break;
			}
			case State.Creating:
				if (m_StateTimer >= m_CreateDelay)
				{
					m_StateTimer = 0f;
					CreateAnimal();
					AddAnimationManager.Instance.Add(this, Adding: false);
					if (m_Ingredients.Count == 0)
					{
						SetState(State.Idle);
					}
				}
				break;
			case State.Cancelling:
				if (m_StateTimer >= m_CancelDelay)
				{
					m_StateTimer = 0f;
					CreateCancelledItem();
					AddAnimationManager.Instance.Add(this, Adding: false);
					if (m_Ingredients.Count == 0)
					{
						SetState(State.Idle);
					}
				}
				break;
			}
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}

	public bool AnyChickenIngredients()
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier == ObjectType.AnimalChicken)
			{
				return true;
			}
		}
		return false;
	}
}
