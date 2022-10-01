using System.Collections.Generic;
using UnityEngine;

public class CreativeTools : ObjectSelect
{
	private ObjectInfo[] m_Containers = new ObjectInfo[8]
	{
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.Water),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.SeaWater),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.Milk),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.Honey),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.Sand),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.Soil),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolBucket, ObjectType.Mortar),
		new ObjectInfo(ObjectInfo.Type.Container, ObjectType.ToolWateringCan, ObjectType.Water)
	};

	private ObjectInfo[] m_Bots = new ObjectInfo[4]
	{
		new ObjectInfo(ObjectInfo.Type.Bot, ObjectType.Worker, ObjectType.Nothing),
		new ObjectInfo(ObjectInfo.Type.Bot, ObjectType.Worker, ObjectType.Empty),
		new ObjectInfo(ObjectInfo.Type.Bot, ObjectType.Worker, ObjectType.Plot),
		new ObjectInfo(ObjectInfo.Type.Bot, ObjectType.Worker, ObjectType.FarmerPlayer)
	};

	private ObjectInfo[] m_Folk = new ObjectInfo[6]
	{
		new ObjectInfo(ObjectInfo.Type.Folk, ObjectType.Folk, ObjectType.Nothing),
		new ObjectInfo(ObjectInfo.Type.Folk, ObjectType.Folk, ObjectType.Empty),
		new ObjectInfo(ObjectInfo.Type.Folk, ObjectType.Folk, ObjectType.Plot),
		new ObjectInfo(ObjectInfo.Type.Folk, ObjectType.Folk, ObjectType.FarmerPlayer),
		new ObjectInfo(ObjectInfo.Type.Folk, ObjectType.Folk, ObjectType.BasicWorker),
		new ObjectInfo(ObjectInfo.Type.Folk, ObjectType.Folk, ObjectType.Worker)
	};

	private Farmer m_Farmer;

	public override void Init(bool JustBuildings, bool Everything)
	{
		base.Init(JustBuildings, Everything);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		m_Farmer = players[0].GetComponent<Farmer>();
	}

	protected override List<ObjectInfo> GetObjectsInCategory(ObjectCategory NewCategory)
	{
		List<ObjectInfo> objectsInCategory = base.GetObjectsInCategory(NewCategory);
		List<ObjectInfo> list = new List<ObjectInfo>();
		foreach (ObjectInfo item4 in objectsInCategory)
		{
			if (item4.m_ObjectType == ObjectType.Folk)
			{
				list.Add(item4);
			}
		}
		foreach (ObjectInfo item5 in list)
		{
			objectsInCategory.Remove(item5);
		}
		switch (NewCategory)
		{
		case ObjectCategory.Tools:
		{
			ObjectInfo[] folk = m_Containers;
			foreach (ObjectInfo item2 in folk)
			{
				objectsInCategory.Add(item2);
			}
			break;
		}
		case ObjectCategory.Bots:
		{
			ObjectInfo[] folk = m_Bots;
			foreach (ObjectInfo item3 in folk)
			{
				objectsInCategory.Add(item3);
			}
			break;
		}
		case ObjectCategory.Misc:
		{
			ObjectInfo[] folk = m_Folk;
			foreach (ObjectInfo item in folk)
			{
				objectsInCategory.Add(item);
			}
			break;
		}
		}
		return objectsInCategory;
	}

	protected override void InitButtonWithObjectInfo(StandardButtonObject NewButton, ObjectInfo NewInfo)
	{
		base.InitButtonWithObjectInfo(NewButton, NewInfo);
		if (NewInfo.m_Type == ObjectInfo.Type.Container)
		{
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(NewInfo.m_ObjectType);
			string humanReadableNameFromIdentifier2 = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(NewInfo.m_Extra);
			string rollover = TextManager.Instance.Get("ObjectSelectContainer", humanReadableNameFromIdentifier, humanReadableNameFromIdentifier2);
			NewButton.SetRollover(rollover);
			NewButton.UseGadgetRollover();
		}
		else if (NewInfo.m_Type == ObjectInfo.Type.Bot)
		{
			int extra = (int)NewInfo.m_Extra;
			string val = extra.ToString();
			string rollover2 = TextManager.Instance.Get("ObjectSelectBot", val);
			NewButton.SetRollover(rollover2);
			NewButton.UseGadgetRollover();
		}
		else if (NewInfo.m_Type == ObjectInfo.Type.Folk)
		{
			string val2 = ((int)(NewInfo.m_Extra + 1)).ToString();
			string rollover3 = TextManager.Instance.Get("ObjectSelectFolk", val2);
			NewButton.SetRollover(rollover3);
			NewButton.UseGadgetRollover();
		}
	}

	private void CreateBot(int Mark)
	{
		TileCoord tileCoord = m_Farmer.m_TileCoord + new TileCoord(0, 1);
		if (tileCoord.GetIsValid())
		{
			Worker component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Worker, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Worker>();
			switch (Mark)
			{
			case 0:
				component.SetFrame(ObjectType.WorkerFrameMk0);
				component.SetDrive(ObjectType.WorkerDriveMk0);
				component.SetHead(ObjectType.WorkerHeadMk0);
				break;
			case 1:
				component.SetFrame(ObjectType.WorkerFrameMk1);
				component.SetDrive(ObjectType.WorkerDriveMk1);
				component.SetHead(ObjectType.WorkerHeadMk1);
				break;
			case 2:
				component.SetFrame(ObjectType.WorkerFrameMk2);
				component.SetDrive(ObjectType.WorkerDriveMk2);
				component.SetHead(ObjectType.WorkerHeadMk2);
				break;
			case 3:
				component.SetFrame(ObjectType.WorkerFrameMk3);
				component.SetDrive(ObjectType.WorkerDriveMk3);
				component.SetHead(ObjectType.WorkerHeadMk3);
				break;
			}
			component.UpdateModel();
			GameStateManager.Instance.PopState();
		}
	}

	public override void OnObjectClicked(BaseGadget NewGadget)
	{
		int index = m_ObjectButtons.IndexOf(NewGadget.GetComponent<BaseButtonImage>());
		ObjectType objectType = m_ObjectList[index].m_ObjectType;
		ObjectInfo.Type type = m_ObjectList[index].m_Type;
		if (type == ObjectInfo.Type.Bot)
		{
			CreateBot((int)m_ObjectList[index].m_Extra);
		}
		else
		{
			if (m_Farmer.m_State != 0 || (GetComponent<CheatTools>() == null && QuestManager.Instance.m_ObjectsLocked.ContainsKey(objectType)))
			{
				return;
			}
			bool flag = false;
			if (Input.GetKey(KeyCode.LeftShift) && m_Farmer.m_State != Farmer.State.Moving)
			{
				flag = true;
			}
			do
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(objectType, m_Farmer.m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
				if (type == ObjectInfo.Type.Container)
				{
					int capacity = baseClass.GetComponent<ToolFillable>().m_Capacity;
					baseClass.GetComponent<ToolFillable>().Fill(m_ObjectList[index].m_Extra, capacity);
				}
				if (type == ObjectInfo.Type.Folk)
				{
					baseClass.GetComponent<Folk>().SetTier((int)m_ObjectList[index].m_Extra);
				}
				if ((bool)baseClass.GetComponent<Vehicle>() || baseClass.m_TypeIdentifier == ObjectType.AnimalSilkmoth)
				{
					break;
				}
				if (!ToolFillable.GetIsTypeFillable(m_Farmer.m_FarmerCarry.GetTopObjectType()))
				{
					m_Farmer.SendAction(new ActionInfo(ActionType.SetTool, m_Farmer.m_TileCoord, baseClass.GetComponent<Actionable>()));
				}
				else
				{
					flag = false;
				}
			}
			while (flag && m_Farmer.m_FarmerCarry.CanAddCarry(objectType));
			GameStateManager.Instance.PopState();
		}
	}
}
