using System.Collections.Generic;
using UnityEngine;

public class Barn : Converter
{
	private PlaySound m_PlaySound;

	private ObjectType m_AnimalType;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 1), new TileCoord(0, 2));
		SetSpawnPoint(new TileCoord(2, 0));
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

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (AnimalCow.GetIsTypeCow(Info.m_ObjectType) && ObjectTypeList.m_ObjectTypeCounts[195] + ObjectTypeList.m_ObjectTypeCounts[197] >= 100)
			{
				return ActionType.Fail;
			}
			if (AnimalSheep.GetIsTypeSheep(Info.m_ObjectType) && ObjectTypeList.m_ObjectTypeCounts[196] + ObjectTypeList.m_ObjectTypeCounts[198] >= 100)
			{
				return ActionType.Fail;
			}
		}
		return base.GetActionFromObject(Info);
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
			component.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord)));
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
				if (m_Ingredients.Count == 0)
				{
					SetState(State.Idle);
				}
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
