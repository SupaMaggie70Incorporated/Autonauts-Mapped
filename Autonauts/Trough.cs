using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Trough : Building
{
	public float m_Hay;

	public float m_Capacity;

	private GameObject m_HayModel;

	private List<TileCoord> m_GrazerPositions;

	private Wobbler m_Wobbler;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		m_GrazerPositions = new List<TileCoord>();
		m_Capacity = 50f;
		m_Hay = 0f;
		UpdateModel();
		m_Wobbler.Restart();
		ChangeAccessPointToIn();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)m_ModelRoot.transform.Find("Hay"))
		{
			m_HayModel = m_ModelRoot.transform.Find("Hay").gameObject;
		}
		m_Wobbler = new Wobbler();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Hay", m_Hay);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Hay = JSONUtils.GetAsFloat(Node, "Hay", 0f);
		UpdateModel();
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		switch (Info.m_Action)
		{
		case ActionType.Bump:
			m_Wobbler.Go(0.5f, 5f, 0.2f);
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			GenerateCowPositions();
			break;
		}
	}

	public float GetHayPercent()
	{
		return m_Hay / m_Capacity;
	}

	private void UpdateModel()
	{
		if (!(m_HayModel == null))
		{
			if (m_Hay == 0f)
			{
				m_HayModel.SetActive(value: false);
			}
			else
			{
				m_HayModel.SetActive(value: true);
			}
			m_HayModel.GetComponent<MeshRenderer>().enabled = m_HayModel.activeSelf;
			ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, "Idle");
		}
	}

	public bool EatHay()
	{
		if (m_Hay == 0f)
		{
			return false;
		}
		m_Hay -= 1f;
		if (m_Hay == 0f)
		{
			UpdateModel();
		}
		return true;
	}

	private void GenerateCowPositions()
	{
		m_GrazerPositions.Clear();
		TileCoord accessPosition = GetAccessPosition();
		foreach (TileCoord adjacentTile in GetAdjacentTiles())
		{
			if (adjacentTile != accessPosition)
			{
				m_GrazerPositions.Add(adjacentTile);
			}
		}
	}

	public TileCoord GetNewGrazerPosition(TileCoord GrazerPosition)
	{
		List<ObjectType> list = new List<ObjectType>();
		list.Add(ObjectType.AnimalSheep);
		list.Add(ObjectType.AnimalCow);
		list.Add(ObjectType.AnimalAlpaca);
		list.Add(ObjectType.AnimalCowHighland);
		TileCoord result = m_GrazerPositions[0];
		float num = 1000000f;
		foreach (TileCoord grazerPosition in m_GrazerPositions)
		{
			if (PlotManager.Instance.GetPlotAtTile(grazerPosition).GetObjectTypesAtTile(list, grazerPosition) == null)
			{
				float num2 = (grazerPosition - GrazerPosition).Magnitude();
				if (num2 < num)
				{
					num = num2;
					result = grazerPosition;
				}
			}
		}
		return result;
	}

	private ActionType GetActionFromGrazer(AFO Info)
	{
		if (m_Hay == 0f)
		{
			return ActionType.Fail;
		}
		if (m_GrazerPositions.Count == 0)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	private void StartAddHay(AFO Info)
	{
		Actionable @object = Info.m_Object;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		m_Hay += BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		if (m_Hay > m_Capacity)
		{
			m_Hay = m_Capacity;
		}
	}

	private void EndAddHay(AFO Info)
	{
		Info.m_Object.StopUsing();
		UpdateModel();
	}

	private void AbortAddHay(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_Hay -= BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	private ActionType GetActionFromHay(AFO Info)
	{
		Info.m_StartAction = StartAddHay;
		Info.m_EndAction = EndAddHay;
		Info.m_AbortAction = AbortAddHay;
		Info.m_FarmerState = Farmer.State.Adding;
		float fuelEnergy = BurnableFuel.GetFuelEnergy(Info.m_ObjectType);
		if (m_Hay + fuelEnergy >= m_Capacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && (bool)Info.m_Actioner.GetComponent<AnimalGrazer>())
		{
			return GetActionFromGrazer(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary && GetIsObjectAcceptable(Info.m_ObjectType))
		{
			ActionType actionFromHay = GetActionFromHay(Info);
			if (actionFromHay != ActionType.Total)
			{
				return actionFromHay;
			}
		}
		return base.GetActionFromObject(Info);
	}

	public static bool GetIsObjectAcceptable(ObjectType NewType)
	{
		if (BurnableFuel.GetIsBurnableFuel(NewType) && BurnableFuel.GetFuelTier(NewType) == BurnableInfo.Tier.Hay)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		m_Wobbler.Update();
		float y = 1f + m_Wobbler.m_Height;
		base.transform.localScale = new Vector3(1f, y, 1f);
	}

	public void ModAddHay(float Amount)
	{
		m_Hay += Amount;
		if (m_Hay > m_Capacity)
		{
			m_Hay = m_Capacity;
		}
		UpdateModel();
	}
}
