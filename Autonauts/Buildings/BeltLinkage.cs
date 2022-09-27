using System.Collections.Generic;
using UnityEngine;

public class BeltLinkage : Building
{
	private static bool m_Log = false;

	private static int m_MaxBeltLength = 6;

	private static int m_MaxRodLength = 6;

	private static int m_MaxBuildingLength = 4;

	private static int m_MaxBuildingDeviation = 3;

	[HideInInspector]
	public MechanicalBelt m_Belt;

	[HideInInspector]
	public BeltLinkage m_BeltConnectTo;

	[HideInInspector]
	public MechanicalRod m_Rod;

	[HideInInspector]
	public BeltLinkage m_RodConnectTo;

	private Dictionary<Building, MechanicalBelt> m_ConnectedBuildings;

	[HideInInspector]
	public Transform m_ConnectPoint;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		HideAccessModel();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Belt = null;
		m_Rod = null;
		m_ConnectPoint = FindNode("BeltPoint").transform;
		m_ConnectedBuildings = new Dictionary<Building, MechanicalBelt>();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_Log)
		{
			Debug.Log("Destroy " + m_UniqueID);
		}
		CleanHighlight();
		DestroyBuildingBelts();
		DestroyBelt();
		DestroyBeltConnectTo();
		DestroyRod();
		DestroyRodConnectTo();
		base.StopUsing(AndDestroy);
	}

	public override string GetCheatRolloverText()
	{
		string text = base.GetCheatRolloverText() + "\n\r";
		text = text + m_UniqueID + " - ";
		if ((bool)m_Rod)
		{
			text += m_Rod.m_ConnectedTo.m_UniqueID;
			if ((bool)m_Rod.m_Parent)
			{
				text = text + "(" + m_Rod.m_Parent.m_UniqueID + ")";
			}
		}
		else
		{
			text += "None";
		}
		text += "-";
		if ((bool)m_RodConnectTo)
		{
			return text + m_RodConnectTo.m_UniqueID;
		}
		return text + "None";
	}

	public override void SetRotation(int Rotation)
	{
		UpdateTiles();
		TileCoordChanged(m_TileCoord);
	}

	private void DestroyBuildingBelts()
	{
		foreach (KeyValuePair<Building, MechanicalBelt> connectedBuilding in m_ConnectedBuildings)
		{
			connectedBuilding.Value.StopUsing();
			RefreshManager.Instance.AddObject(connectedBuilding.Key);
		}
		m_ConnectedBuildings.Clear();
	}

	private void DestroyRod()
	{
		if ((bool)m_Rod)
		{
			m_Rod.StopUsing();
			m_Rod = null;
			DestroyBuildingBelts();
		}
	}

	public void RefreshRod()
	{
		if (m_Log)
		{
			Debug.Log("RefreshRod " + m_UniqueID);
		}
		DestroyRod();
		for (int i = 1; i < m_MaxRodLength; i++)
		{
			Tile tile = TileManager.Instance.GetTile(m_TileCoord + new TileCoord(0, i));
			if (tile != null && (bool)tile.m_Building && tile.m_Building.m_TypeIdentifier == ObjectType.BeltLinkage)
			{
				m_Rod = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.MechanicalRod, default(Vector3), base.transform.localRotation).GetComponent<MechanicalRod>();
				m_Rod.ConnectTo(this, tile.m_Building);
				if (m_Log)
				{
					Debug.Log("Rod Connect to " + tile.m_Building.m_UniqueID);
				}
				break;
			}
		}
	}

	private void DestroyRodConnectTo()
	{
		if ((bool)m_RodConnectTo)
		{
			BeltLinkage rodConnectTo = m_RodConnectTo;
			m_RodConnectTo = null;
			rodConnectTo.RefreshRod();
		}
	}

	private void RefreshRodConnectTo()
	{
		if (m_Log)
		{
			Debug.Log("RefreshRodConnectTo " + m_UniqueID);
		}
		DestroyRodConnectTo();
		for (int i = 1; i < m_MaxRodLength; i++)
		{
			Tile tile = TileManager.Instance.GetTile(m_TileCoord + new TileCoord(0, -i));
			if (tile != null && (bool)tile.m_Building && tile.m_Building.m_TypeIdentifier == ObjectType.BeltLinkage)
			{
				BeltLinkage component = tile.m_Building.GetComponent<BeltLinkage>();
				component.RefreshRod();
				component.RefreshBuildings();
				break;
			}
		}
	}

	public void SetRodConnectedTo(BeltLinkage NewBelt)
	{
		if ((bool)m_RodConnectTo)
		{
			m_RodConnectTo.DestroyRod();
		}
		m_RodConnectTo = NewBelt;
	}

	public void DestroyBelt()
	{
		if ((bool)m_Belt)
		{
			m_Belt.StopUsing();
			m_Belt = null;
		}
	}

	public void RefreshBelt()
	{
		if (m_Log)
		{
			Debug.Log("RefreshBelt " + m_UniqueID);
		}
		DestroyBelt();
		for (int i = 1; i < m_MaxBeltLength; i++)
		{
			Tile tile = TileManager.Instance.GetTile(m_TileCoord + new TileCoord(i, 0));
			if (tile != null && (bool)tile.m_Building && tile.m_Building.m_TypeIdentifier == ObjectType.BeltLinkage)
			{
				m_Belt = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.MechanicalBelt, default(Vector3), base.transform.localRotation).GetComponent<MechanicalBelt>();
				m_Belt.transform.SetParent(m_ConnectPoint);
				m_Belt.transform.localPosition = default(Vector3);
				m_Belt.ConnectTo(this, tile.m_Building);
				if (m_Log)
				{
					Debug.Log("Belt Connect to " + tile.m_Building.m_UniqueID);
				}
				break;
			}
		}
	}

	private void DestroyBeltConnectTo()
	{
		if ((bool)m_BeltConnectTo)
		{
			BeltLinkage beltConnectTo = m_BeltConnectTo;
			m_BeltConnectTo = null;
			beltConnectTo.RefreshBelt();
		}
	}

	private void RefreshBeltConnectTo()
	{
		if (m_Log)
		{
			Debug.Log("RefreshBeltConnectTo " + m_UniqueID);
		}
		DestroyBeltConnectTo();
		for (int i = 1; i < m_MaxBeltLength; i++)
		{
			Tile tile = TileManager.Instance.GetTile(m_TileCoord + new TileCoord(-i, 0));
			if (tile != null && (bool)tile.m_Building && tile.m_Building.m_TypeIdentifier == ObjectType.BeltLinkage)
			{
				tile.m_Building.GetComponent<BeltLinkage>().RefreshBelt();
				break;
			}
		}
	}

	public void SetBeltConnectedTo(BeltLinkage NewBelt)
	{
		if ((bool)m_BeltConnectTo)
		{
			m_BeltConnectTo.DestroyBelt();
		}
		m_BeltConnectTo = NewBelt;
	}

	public void RefreshBuildings()
	{
		DestroyBuildingBelts();
		TileCoord tileCoord = m_TileCoord + new TileCoord(-m_MaxBuildingLength, -m_MaxBuildingDeviation);
		TileCoord tileCoord2 = m_TileCoord + new TileCoord(m_MaxBuildingLength, m_MaxBuildingDeviation);
		if (m_Rod != null)
		{
			tileCoord2.y += m_Rod.m_Length;
		}
		for (int i = tileCoord.y; i <= tileCoord2.y; i++)
		{
			for (int j = tileCoord.x; j <= tileCoord2.x; j++)
			{
				Tile tile = TileManager.Instance.GetTile(new TileCoord(j, i));
				if (tile != null)
				{
					Building buildingFootprint = tile.m_BuildingFootprint;
					if ((bool)buildingFootprint && CanTypeConnectTo(buildingFootprint.m_TypeIdentifier))
					{
						RefreshManager.Instance.AddObject(buildingFootprint);
					}
				}
			}
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			if (m_Log)
			{
				Debug.Log("Refresh " + m_UniqueID);
			}
			DestroyRod();
			RefreshRodConnectTo();
			RefreshRod();
			DestroyBelt();
			RefreshBeltConnectTo();
			RefreshBelt();
			RefreshBuildings();
		}
	}

	public override void StopConnectionTo(Building NewBuilding)
	{
		if (m_ConnectedBuildings.ContainsKey(NewBuilding))
		{
			m_ConnectedBuildings[NewBuilding].StopUsing();
			m_ConnectedBuildings.Remove(NewBuilding);
		}
	}

	public override void ConnectBuilding(Building NewBuilding)
	{
		if (NewBuilding.m_ConnectedTo != this)
		{
			if ((bool)NewBuilding.m_ConnectedTo)
			{
				NewBuilding.m_ConnectedTo.StopConnectionTo(NewBuilding);
			}
			MechanicalBelt component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.MechanicalBelt, default(Vector3), base.transform.localRotation).GetComponent<MechanicalBelt>();
			component.ConnectTo(this, NewBuilding);
			m_ConnectedBuildings.Add(NewBuilding, component);
		}
	}

	public override void SetLinkedSystem(LinkedSystem NewSystem)
	{
		base.SetLinkedSystem(NewSystem);
		if ((bool)m_Belt)
		{
			m_Belt.UpdateLinkedSystem();
		}
		foreach (KeyValuePair<Building, MechanicalBelt> connectedBuilding in m_ConnectedBuildings)
		{
			connectedBuilding.Value.UpdateLinkedSystem();
		}
	}

	private static bool GetIsPulleyOnBadSide(int PulleySide, TileCoord BeltCoord, TileCoord BuildingTopLeft, TileCoord BuildingBottomRight)
	{
		bool result = false;
		switch (PulleySide)
		{
		case 0:
			if (BeltCoord.x > BuildingBottomRight.x)
			{
				result = true;
			}
			break;
		case 1:
			if (BeltCoord.y > BuildingBottomRight.y)
			{
				result = true;
			}
			break;
		case 2:
			if (BeltCoord.x < BuildingTopLeft.x)
			{
				result = true;
			}
			break;
		case 3:
			if (BeltCoord.y < BuildingBottomRight.y)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public static BeltLinkage TestConnection(Building NewBuilding)
	{
		if (NewBuilding == null)
		{
			return null;
		}
		int pulleySide = 0;
		if (LinkedSystemConverter.GetIsTypeLinkedSystemConverter(NewBuilding.m_TypeIdentifier))
		{
			pulleySide = NewBuilding.GetComponent<LinkedSystemConverter>().GetPulleySide();
		}
		else if (LinkedSystemEngine.GetIsTypeLinkedSystemEngine(NewBuilding.m_TypeIdentifier))
		{
			pulleySide = NewBuilding.GetComponent<LinkedSystemEngine>().GetPulleySide();
		}
		NewBuilding.GetBoundingRectangle(out var TopLeft, out var BottomRight);
		TileCoord tileCoord = NewBuilding.m_TileCoord;
		if (NewBuilding.FindNode("BeltPoint") != null)
		{
			tileCoord = new TileCoord(NewBuilding.FindNode("BeltPoint").transform.position);
		}
		TileCoord tileCoord2 = tileCoord + new TileCoord(-m_MaxBuildingLength, -m_MaxRodLength - m_MaxBuildingDeviation);
		TileCoord tileCoord3 = tileCoord + new TileCoord(m_MaxBuildingLength, m_MaxRodLength + m_MaxBuildingDeviation);
		BeltLinkage result = null;
		float num = 1E+07f;
		for (int i = tileCoord2.y; i <= tileCoord3.y; i++)
		{
			for (int j = tileCoord2.x; j <= tileCoord3.x; j++)
			{
				Tile tile = TileManager.Instance.GetTile(new TileCoord(j, i));
				if (tile == null || !tile.m_Building || tile.m_Building.m_TypeIdentifier != ObjectType.BeltLinkage)
				{
					continue;
				}
				BeltLinkage component = tile.m_Building.GetComponent<BeltLinkage>();
				TileCoord tileCoord4 = tile.m_Building.m_TileCoord;
				if ((bool)component.m_Rod && tileCoord.y > tileCoord4.y)
				{
					int length = component.m_Rod.m_Length;
					tileCoord4.y += length;
					if (tileCoord4.y > tileCoord.y)
					{
						tileCoord4.y = tileCoord.y;
					}
				}
				if (GetIsPulleyOnBadSide(pulleySide, tileCoord4, TopLeft, BottomRight))
				{
					continue;
				}
				int num2 = tileCoord4.y - tileCoord.y;
				if (num2 >= -m_MaxBuildingDeviation && num2 <= m_MaxBuildingDeviation)
				{
					float num3 = (tileCoord4 - tileCoord).Magnitude();
					if (num > num3)
					{
						num = num3;
						result = component;
					}
				}
			}
		}
		return result;
	}

	public static bool CanTypeConnectTo(ObjectType NewType)
	{
		if (LinkedSystemEngine.GetIsTypeLinkedSystemEngine(NewType) || LinkedSystemConverter.GetIsTypeLinkedSystemConverter(NewType))
		{
			return true;
		}
		return false;
	}
}
