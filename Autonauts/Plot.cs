using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

public class Plot : Actionable
{
	public static int m_PlotTilesWide = 21;

	public static int m_PlotTilesHigh = 12;

	public bool m_Visible;

	private bool m_MeshDirty;

	[HideInInspector]
	public int m_PlotX;

	[HideInInspector]
	public int m_PlotY;

	[HideInInspector]
	public int m_TileX;

	[HideInInspector]
	public int m_TileY;

	public GameObject m_Root;

	public GameObject m_Walls;

	public GameObject m_Water;

	[HideInInspector]
	public Dictionary<int, TileCoordObject> m_Objects;

	[HideInInspector]
	public Dictionary<ObjectType, List<TileCoordObject>> m_ObjectDictionary;

	[HideInInspector]
	public MeshRenderer m_Mesh;

	[HideInInspector]
	public MeshRenderer m_WaterMesh;

	[HideInInspector]
	public List<Vector3> m_WavesList;

	[HideInInspector]
	public List<PlaySound> m_AmbientObjects;

	private static int BestCount;

	private Dictionary<ObjectType, PlotObjectMerger> m_ObjectMergers;

	private List<PlotObjectMerger> m_ObjectMergersList;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Plot", m_TypeIdentifier);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Root = base.transform.Find("Root").gameObject;
		m_Walls = base.transform.Find("Walls").gameObject;
		m_Water = base.transform.Find("Water").gameObject;
		m_MeshDirty = false;
		m_Objects = new Dictionary<int, TileCoordObject>();
		m_ObjectDictionary = new Dictionary<ObjectType, List<TileCoordObject>>();
		m_ObjectMergers = new Dictionary<ObjectType, PlotObjectMerger>();
		m_ObjectMergersList = new List<PlotObjectMerger>();
		m_AmbientObjects = new List<PlaySound>();
		m_TypeIdentifier = ObjectType.Plot;
		m_UniqueID = ObjectTypeList.Instance.AddActionable(this);
		SetVisible(Visible: false);
		BestCount = 0;
		base.enabled = false;
		SetMeshDirty();
	}

	private void OnDestroy()
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (KeyValuePair<int, TileCoordObject> @object in m_Objects)
		{
			list.Add(@object.Value);
		}
		foreach (TileCoordObject item in list)
		{
			if ((bool)item && (bool)item.gameObject)
			{
				Object.DestroyImmediate(item.gameObject);
			}
		}
	}

	public void SetOffset(int x, int y)
	{
		m_PlotX = x;
		m_PlotY = y;
		m_TileX = m_PlotX * m_PlotTilesWide;
		m_TileY = m_PlotY * m_PlotTilesHigh;
	}

	public void SetVisible(bool Visible, bool DoUncoverEffect = false)
	{
		bool visible = m_Visible;
		m_Visible = Visible;
		PlotManager.Instance.UpdatePlotVisibility(this, Visible);
		m_Root.GetComponent<MeshRenderer>().sharedMaterial = MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapMaterial, !Visible);
		m_Walls.GetComponent<MeshRenderer>().sharedMaterials = new Material[2]
		{
			MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWalls, !Visible),
			MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapBoundary, !Visible)
		};
		Material[] array = new Material[3];
		MeshRenderer component = m_Water.GetComponent<MeshRenderer>();
		array[0] = MaterialManager.Instance.GetMisc(MaterialManager.MiscType.TileMapWaterMaterial, !Visible);
		array[1] = MaterialManager.Instance.GetMisc(MaterialManager.MiscType.WaterSurf, !Visible);
		array[2] = MaterialManager.Instance.GetMisc(MaterialManager.MiscType.WetSand, !Visible);
		component.sharedMaterials = array;
		foreach (KeyValuePair<int, TileCoordObject> @object in m_Objects)
		{
			TileCoordObject value = @object.Value;
			value.UpdatePlotVisibility();
			if (visible != m_Visible && (bool)FailSafeManager.Instance)
			{
				if (Visible)
				{
					FailSafeManager.Instance.AddVisibleType(value.m_TypeIdentifier);
				}
				else
				{
					FailSafeManager.Instance.RemoveVisibleType(value.m_TypeIdentifier);
				}
			}
		}
		if (m_Visible && (GeneralUtils.m_InGame || SaveLoadManager.m_MiniMap))
		{
			for (int i = 0; i < m_PlotTilesHigh; i++)
			{
				for (int j = 0; j < m_PlotTilesWide; j++)
				{
					RouteFinding.UpdateTileWalk(j + m_TileX, i + m_TileY);
				}
			}
		}
		foreach (PlotObjectMerger objectMergers in m_ObjectMergersList)
		{
			objectMergers.GetComponent<MeshRenderer>().enabled = m_Visible;
		}
		if (m_Visible && DoUncoverEffect)
		{
			Vector3 position = base.transform.position + new Vector3(Tile.m_Size * (float)m_PlotTilesWide * 0.5f, 0.1f, (0f - Tile.m_Size) * (float)m_PlotTilesHigh * 0.5f);
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.PlotUncover, position, Quaternion.identity);
		}
		if (m_Visible && !visible)
		{
			RecordingManager.Instance.ShowPlot(this);
		}
		if (m_Visible)
		{
			FailSafeManager.Instance.NewPlotVisible(this);
		}
	}

	public void SetMeshDirty()
	{
		m_MeshDirty = true;
		base.enabled = true;
	}

	public void AddObject(TileCoordObject NewObject)
	{
		if (m_Objects.ContainsKey(NewObject.m_UniqueID))
		{
			ErrorMessage.LogError(string.Concat("Plot ", m_PlotX, ",", m_PlotY, " already contains object ", NewObject.m_TypeIdentifier, " ", NewObject.m_UniqueID));
		}
		if (PlotManager.Instance.m_CapObjects)
		{
			CapObjectsAtTile(NewObject.m_TileCoord);
		}
		m_Objects.Add(NewObject.m_UniqueID, NewObject);
		if (!m_ObjectDictionary.ContainsKey(NewObject.m_TypeIdentifier))
		{
			m_ObjectDictionary.Add(NewObject.m_TypeIdentifier, new List<TileCoordObject>());
		}
		List<TileCoordObject> value = null;
		if (m_ObjectDictionary.TryGetValue(NewObject.m_TypeIdentifier, out value))
		{
			value.Add(NewObject);
		}
		NewObject.m_Plot = this;
		NewObject.UpdatePlotVisibility();
		if (ObjectTypeList.Instance.GetStackableFromIdentifier(NewObject.m_TypeIdentifier))
		{
			StackObjectsAtTile(NewObject.m_TileCoord);
		}
		if (ObjectTypeList.Instance.GetStackableFromIdentifier(NewObject.m_TypeIdentifier) && NewObject.m_TypeIdentifier != ObjectType.Folk && NewObject.m_TypeIdentifier != ObjectType.CertificateReward && !Decoration.GetIsTypeDecoration(NewObject.m_TypeIdentifier))
		{
			NewObject.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
		}
		if (m_Visible && (bool)FailSafeManager.Instance)
		{
			FailSafeManager.Instance.AddVisibleType(NewObject.m_TypeIdentifier);
		}
		if ((bool)FailSafeManager.Instance && NewObject.m_TypeIdentifier == ObjectType.FlowerWild)
		{
			FailSafeManager.Instance.AddFlower(NewObject.GetComponent<FlowerWild>());
		}
		AudioManager.Instance.PlotObjectAdded(this, NewObject);
	}

	public void RemoveObject(TileCoordObject NewObject)
	{
		NewObject.Wake();
		m_Objects.Remove(NewObject.m_UniqueID);
		List<TileCoordObject> value = null;
		if (m_ObjectDictionary.TryGetValue(NewObject.m_TypeIdentifier, out value))
		{
			value.Remove(NewObject);
		}
		NewObject.m_Plot = null;
		NewObject.UpdatePlotVisibility();
		if (ObjectTypeList.Instance.GetStackableFromIdentifier(NewObject.m_TypeIdentifier))
		{
			StackObjectsAtTile(NewObject.m_TileCoord);
		}
		if (m_Visible && (bool)FailSafeManager.Instance)
		{
			FailSafeManager.Instance.RemoveVisibleType(NewObject.m_TypeIdentifier);
		}
		if ((bool)FailSafeManager.Instance && NewObject.m_TypeIdentifier == ObjectType.FlowerWild)
		{
			FailSafeManager.Instance.RemoveFlower(NewObject.GetComponent<FlowerWild>());
		}
		AudioManager.Instance.PlotObjectRemoved(this, NewObject);
	}

	private void CapObjectsAtTile(TileCoord Position)
	{
		List<TileCoordObject> list = PlotManager.Instance.m_TileObjects[Position.GetIndex()];
		int num = 500;
		int num2 = list.Count - num;
		if (num2 <= 0)
		{
			return;
		}
		List<TileCoordObject> list2 = new List<TileCoordObject>();
		foreach (TileCoordObject item in list)
		{
			if (item.m_TypeIdentifier != ObjectType.FarmerPlayer && item.m_TypeIdentifier != ObjectType.Worker && (bool)item.GetComponent<Holdable>() && !Vehicle.GetIsTypeVehicle(item.m_TypeIdentifier) && !BaggedManager.Instance.IsObjectBagged(item))
			{
				list2.Add(item);
				if (list2.Count >= num2)
				{
					break;
				}
			}
		}
		foreach (TileCoordObject item2 in list2)
		{
			item2.StopUsing();
		}
	}

	public void StackObjectsAtTile(TileCoord Position)
	{
		int num = 0;
		List<TileCoordObject> obj = PlotManager.Instance.m_TileObjects[Position.GetIndex()];
		float num2 = Position.ToWorldPositionTileCentered().y;
		bool flag = false;
		foreach (TileCoordObject item in obj)
		{
			if (!item || !ObjectTypeList.Instance.GetStackableFromIdentifier(item.m_TypeIdentifier) || !item.DoesContainClass("Selectable") || !(item.GetComponent<Selectable>().m_TileCoord == Position))
			{
				continue;
			}
			float num3 = ObjectTypeList.Instance.GetHeight(item.m_TypeIdentifier) * item.transform.localScale.y;
			if (!flag && num2 < PlotMeshBuilderWater.m_WaterLevel)
			{
				if (num2 < PlotMeshBuilderWater.m_WaterLevel - num3 / 2f)
				{
					num2 = PlotMeshBuilderWater.m_WaterLevel - num3 / 2f;
				}
				flag = true;
			}
			Vector3 position = item.transform.position;
			float yOffset = ObjectTypeList.Instance.GetYOffset(item.m_TypeIdentifier);
			position.y = num2 + yOffset;
			item.transform.position = position;
			num2 += num3;
			num++;
			if (num == 20)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.Stack20Objects, Bot: false, 0, null);
			}
		}
	}

	public bool ShovelTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		Tile.TileType tileType = TileManager.Instance.GetTileType(TilePosition);
		switch (tileType)
		{
		case Tile.TileType.Soil:
		case Tile.TileType.SoilTilled:
		{
			BaseClass associatedObject = TileManager.Instance.GetTile(TilePosition).m_AssociatedObject;
			if ((bool)associatedObject)
			{
				associatedObject.StopUsing();
			}
			TileManager.Instance.SetTileType(TilePosition, Tile.TileType.SoilHole, NewFarmer);
			return true;
		}
		case Tile.TileType.SoilHole:
			TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Soil, NewFarmer);
			return true;
		case Tile.TileType.Clay:
		{
			BaseClass baseClass3 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Clay, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity);
			AudioManager.Instance.StartEvent("ObjectCreated", baseClass3.GetComponent<TileCoordObject>());
			TileUseManager.Instance.UseTile(TilePosition, tileType);
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Clay);
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineClay, NewFarmer.m_TypeIdentifier == ObjectType.Worker, 0, NewFarmer);
			return true;
		}
		case Tile.TileType.ClaySoil:
		{
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Clay, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity);
			AudioManager.Instance.StartEvent("ObjectCreated", baseClass2.GetComponent<TileCoordObject>());
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Clay);
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineClay, NewFarmer.m_TypeIdentifier == ObjectType.Worker, 0, NewFarmer);
			return true;
		}
		case Tile.TileType.Empty:
		case Tile.TileType.IronHidden:
		case Tile.TileType.ClayHidden:
		case Tile.TileType.CoalHidden:
		case Tile.TileType.StoneHidden:
		{
			int num = 1;
			if (Random.Range(1, 100) < VariableManager.Instance.GetVariableAsInt("TurfChance"))
			{
				num++;
			}
			BaseClass baseClass = null;
			for (int i = 0; i < num; i++)
			{
				baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Turf, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity);
				SpawnAnimationManager.Instance.AddJump(baseClass, TilePosition, TilePosition, 0f, baseClass.transform.position.y, 4f);
			}
			AudioManager.Instance.StartEvent("ObjectCreated", baseClass.GetComponent<TileCoordObject>());
			switch (tileType)
			{
			case Tile.TileType.ClayHidden:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.ClaySoil, NewFarmer);
				break;
			case Tile.TileType.IronHidden:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.IronSoil, NewFarmer);
				break;
			case Tile.TileType.IronSoil:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.IronSoil2, NewFarmer);
				break;
			case Tile.TileType.CoalHidden:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.CoalSoil, NewFarmer);
				break;
			case Tile.TileType.CoalSoil:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.CoalSoil2, NewFarmer);
				break;
			case Tile.TileType.CoalSoil2:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.CoalSoil3, NewFarmer);
				break;
			case Tile.TileType.StoneHidden:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.StoneSoil, NewFarmer);
				break;
			default:
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Soil, NewFarmer);
				break;
			}
			return true;
		}
		default:
			return false;
		}
	}

	public bool HoeTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		Tile.TileType tileType = TileManager.Instance.GetTileType(TilePosition);
		if (tileType == Tile.TileType.Soil || tileType == Tile.TileType.SoilHole)
		{
			TileManager.Instance.SetTileType(TilePosition, Tile.TileType.SoilTilled, NewFarmer);
			return true;
		}
		return false;
	}

	public bool ReapTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		Tile tile = TileManager.Instance.GetTile(TilePosition);
		if ((bool)tile.m_AssociatedObject)
		{
			if ((bool)tile.m_AssociatedObject.GetComponent<CropWheat>())
			{
				tile.m_AssociatedObject.GetComponent<CropWheat>().Cut(NewFarmer != null);
				return true;
			}
			if ((bool)tile.m_AssociatedObject.GetComponent<Grass>())
			{
				tile.m_AssociatedObject.GetComponent<Grass>().Cut(NewFarmer);
				return true;
			}
		}
		return false;
	}

	private void CreateNewObject(ObjectType NewType, TileCoord TilePosition, Farmer NewFarmer, string AppearNoise)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewType, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity);
		AudioManager.Instance.StartEvent(AppearNoise, baseClass.GetComponent<TileCoordObject>());
		SpawnAnimationManager.Instance.AddJump(baseClass, TilePosition, TilePosition, 0f, baseClass.transform.position.y, 4f);
		if (NewType == ObjectType.Rock)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineStone, NewFarmer.m_TypeIdentifier == ObjectType.Worker, 0, null);
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineStoneDeposits, NewFarmer.m_TypeIdentifier == ObjectType.Worker, 0, null);
		}
		if (NewType == ObjectType.IronOre)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineIron, NewFarmer.m_TypeIdentifier == ObjectType.Worker, 0, null);
		}
		if (NewType == ObjectType.Coal)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineCoal, NewFarmer.m_TypeIdentifier == ObjectType.Worker, 0, null);
		}
	}

	private bool RandomCreateNewObject(ObjectType NewType, TileCoord TilePosition, Farmer NewFarmer, string AppearNoise, int Chance)
	{
		if (Random.Range(0, 100) < Chance)
		{
			CreateNewObject(NewType, TilePosition, NewFarmer, AppearNoise);
			return true;
		}
		return false;
	}

	public bool MineTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		ObjectType heldObjectType = ObjectTypeList.m_Total;
		if ((bool)NewFarmer)
		{
			heldObjectType = NewFarmer.m_FarmerCarry.GetTopObjectType();
		}
		bool flag = false;
		Tile.TileType tileType = TileManager.Instance.GetTileType(TilePosition);
		switch (tileType)
		{
		case Tile.TileType.Iron:
			CreateNewObject(ObjectType.IronOre, TilePosition, NewFarmer, "ObjectCreated");
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, NewFarmer.m_TypeIdentifier == ObjectType.Worker, ObjectType.IronOre, null);
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Iron);
			TileUseManager.Instance.UseTile(TilePosition, tileType);
			flag = true;
			break;
		case Tile.TileType.Stone:
			CreateNewObject(ObjectType.Rock, TilePosition, NewFarmer, "ObjectCreated");
			TileUseManager.Instance.UseTile(TilePosition, tileType);
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Stones);
			flag = true;
			break;
		case Tile.TileType.Coal:
			CreateNewObject(ObjectType.Coal, TilePosition, NewFarmer, "ObjectCreated");
			TileUseManager.Instance.UseTile(TilePosition, tileType);
			flag = true;
			break;
		case Tile.TileType.ClaySoil:
			if (FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Clay, NewFarmer);
			}
			break;
		case Tile.TileType.IronSoil:
			if (!FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				flag = RandomCreateNewObject(ObjectType.IronOre, TilePosition, NewFarmer, "ObjectCreated", 100);
				if (flag)
				{
					StatsManager.Instance.AddEvent(StatsManager.StatEvent.Iron);
				}
			}
			else
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.IronSoil2, NewFarmer);
			}
			break;
		case Tile.TileType.IronSoil2:
			if (!FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				flag = RandomCreateNewObject(ObjectType.IronOre, TilePosition, NewFarmer, "ObjectCreated", 100);
				if (flag)
				{
					StatsManager.Instance.AddEvent(StatsManager.StatEvent.Iron);
				}
			}
			else
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Iron, NewFarmer);
			}
			break;
		case Tile.TileType.CoalSoil:
			if (!FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				flag = RandomCreateNewObject(ObjectType.Coal, TilePosition, NewFarmer, "ObjectCreated", 100);
			}
			else
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.CoalSoil2, NewFarmer);
			}
			break;
		case Tile.TileType.CoalSoil2:
			if (!FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				flag = RandomCreateNewObject(ObjectType.Coal, TilePosition, NewFarmer, "ObjectCreated", 100);
			}
			else
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.CoalSoil3, NewFarmer);
			}
			break;
		case Tile.TileType.CoalSoil3:
			if (!FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				flag = RandomCreateNewObject(ObjectType.Coal, TilePosition, NewFarmer, "ObjectCreated", 100);
			}
			else
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Coal, NewFarmer);
			}
			break;
		case Tile.TileType.StoneSoil:
			if (!FarmerStateMining.GetWillToolMineDown(heldObjectType, tileType))
			{
				flag = RandomCreateNewObject(ObjectType.Rock, TilePosition, NewFarmer, "ObjectCreated", 100);
				if (flag)
				{
					StatsManager.Instance.AddEvent(StatsManager.StatEvent.Stones);
				}
			}
			else
			{
				TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Stone, NewFarmer);
			}
			break;
		}
		return flag;
	}

	public bool StoneCutTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		Tile.TileType tileType = TileManager.Instance.GetTileType(TilePosition);
		if (tileType == Tile.TileType.Stone)
		{
			CreateNewObject(ObjectType.StoneBlockCrude, TilePosition, NewFarmer, "ObjectCreated");
			TileUseManager.Instance.UseTile(TilePosition, tileType, VariableManager.Instance.GetVariableAsInt("CrudeStoneBlockCost"));
			return true;
		}
		return false;
	}

	public bool DredgeTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		Tile.TileType tileType = TileManager.Instance.GetTileType(TilePosition);
		if (tileType == Tile.TileType.Soil || tileType == Tile.TileType.SoilTilled || tileType == Tile.TileType.SoilHole || tileType == Tile.TileType.Sand)
		{
			TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Dredged, NewFarmer);
			WaterManager.Instance.AddDredgedTile(TilePosition);
			return true;
		}
		return false;
	}

	public bool DropSoilTile(TileCoord TilePosition, Farmer NewFarmer = null)
	{
		Tile.TileType tileType = TileManager.Instance.GetTileType(TilePosition);
		if (TileHelpers.GetTileWater(tileType) || tileType == Tile.TileType.Dredged)
		{
			Holdable topObject = NewFarmer.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			ObjectType heldType = topObject.GetComponent<ToolBucket>().m_HeldType;
			SoilManager.Instance.AddSoil(TilePosition, 1, heldType, NewFarmer);
			topObject.GetComponent<ToolBucket>().Empty(1);
			return true;
		}
		return false;
	}

	public bool UseObjectOnTile(TileCoord TilePosition, Farmer NewFarmer, Holdable NewObject)
	{
		ObjectType typeIdentifier = NewObject.m_TypeIdentifier;
		switch (typeIdentifier)
		{
		case ObjectType.Fertiliser:
			TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Soil, NewFarmer);
			return true;
		case ObjectType.Turf:
			TileManager.Instance.SetTileType(TilePosition, Tile.TileType.Empty, NewFarmer);
			return true;
		case ObjectType.WheatSeed:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropWheat, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropWheat>().GetComponent<CropWheat>()
				.SetGrowing();
			return true;
		case ObjectType.CarrotSeed:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropCarrot, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropCarrot>().GetComponent<CropCarrot>()
				.SetGrowing();
			return true;
		case ObjectType.CottonSeeds:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropCotton, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropCotton>().GetComponent<CropCotton>()
				.SetGrowing();
			return true;
		case ObjectType.BullrushesSeeds:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Bullrushes, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Bullrushes>().GetComponent<Bullrushes>()
				.SetGrowing();
			return true;
		case ObjectType.GrassCut:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Grass, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Grass>().GetComponent<Grass>()
				.SetGrowing();
			return true;
		case ObjectType.TreeSeed:
		case ObjectType.Seedling:
		{
			MyTree component5 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreePine, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<MyTree>();
			component5.SetGrowing();
			if (NewFarmer.m_FarmerCarry.GetLastObjectType() == ObjectType.TreeSeed)
			{
				component5.SetSlow();
			}
			return true;
		}
		case ObjectType.SeedlingMulberry:
		{
			MyTree component4 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeMulberry, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<MyTree>();
			component4.SetGrowing();
			component4.SetSlow();
			return true;
		}
		case ObjectType.Coconut:
		{
			MyTree component3 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeCoconut, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<MyTree>();
			component3.SetGrowing();
			component3.SetSlow();
			return true;
		}
		case ObjectType.Apple:
		{
			MyTree component2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeApple, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<MyTree>();
			component2.SetGrowing();
			component2.SetSlow();
			return true;
		}
		case ObjectType.Berries:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Bush, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Bush>().SetState(Bush.State.GrowingNothing);
			return true;
		case ObjectType.PumpkinSeeds:
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropPumpkin, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropPumpkin>().SetGrowing();
			return true;
		default:
			if ((bool)NewObject.GetComponent<FlowerSeeds>())
			{
				FlowerWild component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FlowerWild, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<FlowerWild>();
				component.SetTypeFromSeed(typeIdentifier);
				component.SetGrowing();
				return true;
			}
			switch (typeIdentifier)
			{
			case ObjectType.MushroomDug:
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Mushroom, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Mushroom>().SetState(Mushroom.State.GrowingNothing);
				return true;
			case ObjectType.WeedDug:
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Weed, TilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Weed>().SetState(Weed.State.WeedGrow);
				return true;
			default:
				return false;
			}
		}
	}

	private void EndNewAction(AFO Info)
	{
		Actionable @object = Info.m_Object;
		Farmer component = Info.m_Actioner.GetComponent<Farmer>();
		if (Info.m_FarmerState == Farmer.State.Hoe)
		{
			HoeTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.Mining)
		{
			MineTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.StoneCutting)
		{
			StoneCutTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.Scythe)
		{
			ReapTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.Shovel)
		{
			ShovelTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.Dredging)
		{
			DredgeTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.DroppingSoil)
		{
			DropSoilTile(Info.m_Position, component);
		}
		else if (Info.m_FarmerState == Farmer.State.Seed)
		{
			UseObjectOnTile(Info.m_Position, component, @object.GetComponent<Holdable>());
		}
	}

	private bool GetHeldObjectState(AFO Info)
	{
		Tile tile = TileManager.Instance.GetTile(Info.m_Position);
		Tile.TileType tileType = tile.m_TileType;
		Info.m_FarmerState = Farmer.State.Total;
		if (tile.m_Floor != null || tile.m_BuildingFootprint != null || tile.m_Building != null)
		{
			return false;
		}
		ObjectType objectType = Info.m_ObjectType;
		Info.m_RequirementsOut = Tile.GetNameFromType(tileType);
		if (ModCheckUseObjectOnTile(tileType, Info))
		{
			Info.m_FarmerState = Farmer.State.ModAction;
			Info.m_EndAction = ModEndAction;
			return true;
		}
		switch (tileType)
		{
		case Tile.TileType.Empty:
		case Tile.TileType.IronHidden:
		case Tile.TileType.ClayHidden:
		case Tile.TileType.ClaySoil:
		case Tile.TileType.Clay:
		case Tile.TileType.CoalHidden:
		case Tile.TileType.StoneHidden:
			if (FarmerStateShovel.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Shovel;
				if (((tileType != Tile.TileType.Clay && tileType != Tile.TileType.ClaySoil) || !GetSelectableObjectAtTile(Info.m_Position)) && !SpawnAnimationManager.Instance.GetObjectAtTile(Info.m_Position))
				{
					return true;
				}
			}
			break;
		case Tile.TileType.Soil:
			if (FarmerStateShovel.GetIsToolAcceptable(objectType) || objectType == ObjectType.Stick)
			{
				Info.m_FarmerState = Farmer.State.Shovel;
				return true;
			}
			if (FarmerStateHoe.GetIsToolAcceptable(objectType))
			{
				BaseClass associatedObject = TileManager.Instance.GetTile(Info.m_Position).m_AssociatedObject;
				if (associatedObject == null || associatedObject.m_TypeIdentifier != ObjectType.TreeStump)
				{
					Info.m_FarmerState = Farmer.State.Hoe;
					return true;
				}
			}
			if (FarmerStateDredging.GetIsToolAcceptable(objectType))
			{
				BaseClass associatedObject2 = TileManager.Instance.GetTile(Info.m_Position).m_AssociatedObject;
				if (associatedObject2 == null || associatedObject2.m_TypeIdentifier != ObjectType.TreeStump)
				{
					Info.m_FarmerState = Farmer.State.Dredging;
					return true;
				}
			}
			if (ToolBucket.GetIsTypeBucket(objectType))
			{
				Info.m_FarmerState = Farmer.State.PickingUp;
				if ((bool)Info.m_Object)
				{
					ToolFillable component2 = Info.m_Object.GetComponent<ToolFillable>();
					if ((bool)component2 && component2.CanAcceptObjectType(ObjectType.Soil) && !component2.GetIsFull())
					{
						return true;
					}
					return false;
				}
				return true;
			}
			break;
		case Tile.TileType.SoilTilled:
			if (FarmerStateShovel.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Shovel;
				return true;
			}
			if (FarmerStateDredging.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Dredging;
				return true;
			}
			if (ToolBucket.GetIsTypeBucket(objectType))
			{
				Info.m_FarmerState = Farmer.State.PickingUp;
				if ((bool)Info.m_Object)
				{
					ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
					if ((bool)component && component.CanAcceptObjectType(ObjectType.Soil) && !component.GetIsFull())
					{
						return true;
					}
					return false;
				}
				return true;
			}
			break;
		}
		if (tileType == Tile.TileType.SoilHole)
		{
			if (FarmerStateShovel.GetIsToolAcceptable(objectType) || objectType == ObjectType.Stick)
			{
				Info.m_FarmerState = Farmer.State.Shovel;
				return true;
			}
			if (FarmerStateHoe.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Hoe;
				return true;
			}
			if (FarmerStateDredging.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Dredging;
				return true;
			}
		}
		if ((bool)tile.m_AssociatedObject && Crop.GetIsTypeCrop(tile.m_AssociatedObject.m_TypeIdentifier) && FarmerStateScythe.GetIsToolAcceptable(objectType))
		{
			Info.m_FarmerState = Farmer.State.Scythe;
			return true;
		}
		if (tileType == Tile.TileType.IronSoil || tileType == Tile.TileType.IronSoil2 || (tileType == Tile.TileType.ClaySoil && objectType != ObjectType.ToolPickStone) || tileType == Tile.TileType.CoalSoil || tileType == Tile.TileType.CoalSoil2 || tileType == Tile.TileType.CoalSoil3 || tileType == Tile.TileType.StoneSoil || tileType == Tile.TileType.Iron || tileType == Tile.TileType.Coal || tileType == Tile.TileType.Stone)
		{
			if (FarmerStateMining.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Mining;
				if (!GetSelectableObjectAtTile(Info.m_Position) && !SpawnAnimationManager.Instance.GetObjectAtTile(Info.m_Position))
				{
					return true;
				}
			}
			if (tileType == Tile.TileType.Stone && FarmerStateStoneCutting.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.StoneCutting;
				if (!GetSelectableObjectAtTile(Info.m_Position) && !SpawnAnimationManager.Instance.GetObjectAtTile(Info.m_Position))
				{
					return true;
				}
			}
			if (tileType == Tile.TileType.ClaySoil && FarmerStateShovel.GetIsToolAcceptable(objectType))
			{
				Info.m_FarmerState = Farmer.State.Shovel;
				if (!GetSelectableObjectAtTile(Info.m_Position) && !SpawnAnimationManager.Instance.GetObjectAtTile(Info.m_Position))
				{
					return true;
				}
			}
		}
		if (TileHelpers.GetTileWaterCollectable(tileType) || tileType == Tile.TileType.Sand)
		{
			if (ToolFillable.GetIsTypeFillable(objectType))
			{
				if (!Info.m_Object)
				{
					Info.m_FarmerState = Farmer.State.PickingUp;
					return true;
				}
				ObjectType newType = ObjectType.Water;
				if (tileType == Tile.TileType.Sand)
				{
					newType = ObjectType.Sand;
				}
				ToolFillable component3 = Info.m_Object.GetComponent<ToolFillable>();
				if ((bool)component3 && component3.CanAcceptObjectType(newType))
				{
					Info.m_FarmerState = Farmer.State.PickingUp;
					if (!component3.GetIsFull())
					{
						return true;
					}
				}
			}
			else if (FarmerStateDredging.GetIsToolAcceptable(objectType) && tileType == Tile.TileType.Sand)
			{
				Info.m_FarmerState = Farmer.State.Dredging;
				return true;
			}
		}
		if ((tileType == Tile.TileType.SeaWaterShallow || tileType == Tile.TileType.WaterShallow) && objectType == ObjectType.ToolFishingStick)
		{
			Info.m_FarmerState = Farmer.State.Fishing;
			return true;
		}
		if ((tileType == Tile.TileType.SeaWaterDeep || tileType == Tile.TileType.WaterDeep) && ToolFishingRod.GetIsTypeFishingRod(objectType))
		{
			Info.m_FarmerState = Farmer.State.Fishing;
			return true;
		}
		if ((tileType == Tile.TileType.Swamp || tileType == Tile.TileType.SeaWaterShallow || tileType == Tile.TileType.WaterShallow) && ToolNet.GetIsTypeNet(objectType))
		{
			Info.m_FarmerState = Farmer.State.Netting;
			return true;
		}
		Info.m_RequirementsOut = "";
		return false;
	}

	private bool GetAltHeldObjectState(AFO Info)
	{
		Tile tile = TileManager.Instance.GetTile(Info.m_Position);
		Tile.TileType tileType = tile.m_TileType;
		Info.m_FarmerState = Farmer.State.Total;
		Info.m_RequirementsOut = "";
		if (tile.m_Floor != null || tile.m_BuildingFootprint != null || tile.m_Building != null)
		{
			return false;
		}
		ObjectType objectType = Info.m_ObjectType;
		Info.m_RequirementsOut = Tile.GetNameFromType(tileType);
		if (tileType == Tile.TileType.SoilTilled && (objectType == ObjectType.WheatSeed || objectType == ObjectType.CottonSeeds || FlowerSeeds.GetIsTypeFlowerSeeds(objectType) || objectType == ObjectType.GrassCut || objectType == ObjectType.Turf || objectType == ObjectType.Fertiliser || objectType == ObjectType.CarrotSeed))
		{
			Info.m_FarmerState = Farmer.State.Seed;
			return true;
		}
		if (tileType == Tile.TileType.Soil && (objectType == ObjectType.WheatSeed || objectType == ObjectType.CottonSeeds || objectType == ObjectType.GrassCut || objectType == ObjectType.Turf || objectType == ObjectType.Fertiliser || objectType == ObjectType.WeedDug || objectType == ObjectType.CarrotSeed))
		{
			if (tile.m_AssociatedObject != null && tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.TreeStump)
			{
				Info.m_RequirementsOut = "";
				return false;
			}
			Info.m_FarmerState = Farmer.State.Seed;
			return true;
		}
		if (tileType == Tile.TileType.Swamp && objectType == ObjectType.BullrushesSeeds)
		{
			Info.m_FarmerState = Farmer.State.Seed;
			return true;
		}
		if (tileType == Tile.TileType.SoilHole && (objectType == ObjectType.Seedling || objectType == ObjectType.TreeSeed || objectType == ObjectType.Apple || objectType == ObjectType.PumpkinSeeds || objectType == ObjectType.Berries || objectType == ObjectType.PumpkinSeeds || objectType == ObjectType.Fertiliser || objectType == ObjectType.Coconut || objectType == ObjectType.SeedlingMulberry))
		{
			Info.m_FarmerState = Farmer.State.Seed;
			return true;
		}
		if (tileType == Tile.TileType.SoilHole && objectType == ObjectType.MushroomDug)
		{
			Info.m_FarmerState = Farmer.State.Seed;
			return true;
		}
		if ((TileHelpers.GetTileWaterShallow(tileType) || tileType == Tile.TileType.Dredged || tileType == Tile.TileType.Swamp) && (bool)Info.m_Object && ToolBucket.GetIsTypeBucket(Info.m_Object.m_TypeIdentifier) && !Info.m_Object.GetComponent<ToolFillable>().GetIsEmpty())
		{
			ObjectType heldType = Info.m_Object.GetComponent<ToolFillable>().m_HeldType;
			if (heldType == ObjectType.Soil || heldType == ObjectType.Sand)
			{
				Info.m_FarmerState = Farmer.State.DroppingSoil;
				return true;
			}
		}
		return false;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		Tile tile = TileManager.Instance.GetTile(Info.m_Position);
		BaseClass associatedObject = tile.m_AssociatedObject;
		if ((bool)associatedObject)
		{
			Info.m_Actioner.GetComponent<Farmer>();
			if (associatedObject.m_TypeIdentifier != ObjectType.SoilHolePile && associatedObject.m_TypeIdentifier != ObjectType.TreeStump)
			{
				return ActionType.Total;
			}
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.AnimalChicken)
		{
			if (tile.m_Floor == null && tile.m_BuildingFootprint == null)
			{
				return ActionType.Pickup;
			}
			return ActionType.Total;
		}
		if (Info.m_ObjectType != ObjectTypeList.m_Total)
		{
			Info.m_EndAction = EndNewAction;
			if (Info.m_ActionType == AFO.AT.Primary)
			{
				bool heldObjectState = GetHeldObjectState(Info);
				if (Info.m_FarmerState != Farmer.State.Total)
				{
					if (!heldObjectState)
					{
						return ActionType.Fail;
					}
					return ActionType.UseInHands;
				}
			}
			if (Info.m_ActionType == AFO.AT.Secondary)
			{
				bool altHeldObjectState = GetAltHeldObjectState(Info);
				if (!altHeldObjectState)
				{
					if (Info.m_RequirementsOut == "" && Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker)
					{
						return ActionType.Total;
					}
					Info.m_RequirementsOut = "";
					Tile.TileType tileType = tile.m_TileType;
					if ((TileHelpers.GetTileWaterShallow(tile.m_TileType) || tileType == Tile.TileType.Dredged || tileType == Tile.TileType.Swamp) && (bool)Info.m_Object && ToolBucket.GetIsTypeBucket(Info.m_Object.m_TypeIdentifier))
					{
						ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
						_ = component.m_LastHeldType;
						if (component.GetIsEmpty() && (component.m_LastHeldType == ObjectType.Sand || component.m_LastHeldType == ObjectType.Soil))
						{
							return ActionType.Fail;
						}
					}
				}
				else if (Info.m_FarmerState != Farmer.State.Total)
				{
					if (!altHeldObjectState)
					{
						return ActionType.Fail;
					}
					return ActionType.UseInHands;
				}
			}
		}
		return base.GetActionFromObject(Info);
	}

	private void Update()
	{
		if (m_MeshDirty)
		{
			m_MeshDirty = false;
			PlotMeshBuilder.Instance.BuildMesh(this);
			PlotMeshBuilderWater.m_Instance.BuildMesh(this);
			base.enabled = false;
		}
	}

	public Selectable GetSelectableObjectAtTile(TileCoord Position, Actionable ExcludedObject = null, bool ExcludeBuildings = false)
	{
		return PlotManager.Instance.GetSelectableObjectAtTile(Position, ExcludedObject, ExcludeBuildings);
	}

	public Selectable GetSelectableObjectAtTile(TileCoord Position, List<ObjectType> ExcludedTypes)
	{
		Selectable selectable = null;
		foreach (KeyValuePair<int, TileCoordObject> @object in m_Objects)
		{
			TileCoordObject value = @object.Value;
			if (value.m_TileCoord == Position && !value.GetComponent<Floor>() && (!value.GetComponent<Farmer>() || value.GetComponent<Farmer>().m_Energy == 0f) && (bool)value.GetComponent<Selectable>() && value.GetComponent<Selectable>().m_TileCoord == Position && value.GetComponent<Savable>().GetIsSavable() && !ExcludedTypes.Contains(value.m_TypeIdentifier))
			{
				if ((bool)value.GetComponent<Farmer>())
				{
					return value.GetComponent<Selectable>();
				}
				if (selectable == null)
				{
					selectable = value.GetComponent<Selectable>();
				}
			}
		}
		return selectable;
	}

	public TileCoordObject GetObjectTypeAtTile(ObjectType Type, TileCoord Position)
	{
		if (m_ObjectDictionary.ContainsKey(Type))
		{
			foreach (TileCoordObject item in m_ObjectDictionary[Type])
			{
				if (item.m_TypeIdentifier == Type && item.m_TileCoord == Position)
				{
					return item;
				}
			}
		}
		return null;
	}

	public TileCoordObject GetObjectTypesAtTile(List<ObjectType> Types, TileCoord Position)
	{
		foreach (ObjectType Type in Types)
		{
			if (!m_ObjectDictionary.ContainsKey(Type))
			{
				continue;
			}
			foreach (TileCoordObject item in m_ObjectDictionary[Type])
			{
				if (item.m_TypeIdentifier == Type && item.m_TileCoord == Position)
				{
					return item;
				}
			}
		}
		return null;
	}

	public List<TileCoordObject> GetObjectsTypeAtTile(ObjectType NewType, TileCoord Position)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		if (m_ObjectDictionary.ContainsKey(NewType))
		{
			foreach (TileCoordObject item in m_ObjectDictionary[NewType])
			{
				if (item.m_TypeIdentifier == NewType && item.m_TileCoord == Position)
				{
					list.Add(item);
				}
			}
			return list;
		}
		return list;
	}

	public List<TileCoordObject> GetObjectsTypeAtTile(TileCoord Position)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (KeyValuePair<int, TileCoordObject> @object in m_Objects)
		{
			TileCoordObject value = @object.Value;
			if (value.m_TileCoord == Position)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void AddWaves(TileCoord Position)
	{
		Vector3 item = Position.ToWorldPositionTileCentered();
		foreach (Vector3 waves in m_WavesList)
		{
			if (waves.x == item.x && waves.y == item.y)
			{
				return;
			}
		}
		m_WavesList.Add(item);
		if (m_WavesList.Count > BestCount)
		{
			BestCount = m_WavesList.Count;
		}
	}

	public void RemoveWaves(TileCoord Position)
	{
		Vector3 item = Position.ToWorldPositionTileCentered();
		foreach (Vector3 waves in m_WavesList)
		{
			if (waves.x == item.x && waves.y == item.y)
			{
				m_WavesList.Remove(item);
				break;
			}
		}
	}

	public float GetDistanceSqrToNearestWave(Vector3 TestPosition)
	{
		float num = 1E+08f;
		foreach (Vector3 waves in m_WavesList)
		{
			float sqrMagnitude = (waves - TestPosition).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
			}
		}
		return num;
	}

	public void UpdateObjectMerger(TileCoordObject NewObject, bool Immediate)
	{
		ObjectType typeIdentifier = NewObject.m_TypeIdentifier;
		if (!m_ObjectMergers.ContainsKey(typeIdentifier))
		{
			GameObject gameObject = Object.Instantiate((GameObject)Resources.Load("Prefabs/PlotObjectMerger", typeof(GameObject)), base.transform.position, Quaternion.identity, base.transform);
			gameObject.transform.localPosition = default(Vector3);
			m_ObjectMergers[typeIdentifier] = gameObject.GetComponent<PlotObjectMerger>();
			m_ObjectMergers[typeIdentifier].Init(NewObject, this);
			m_ObjectMergers[typeIdentifier].GetComponent<MeshRenderer>().enabled = m_Visible;
			m_ObjectMergersList.Add(m_ObjectMergers[typeIdentifier]);
		}
		m_ObjectMergers[typeIdentifier].MarkDirty(Immediate);
	}

	public void FinishMerge()
	{
		foreach (PlotObjectMerger objectMergers in m_ObjectMergersList)
		{
			objectMergers.FinishMerge();
		}
	}

	public void SetDesaturation(bool Desaturated)
	{
	}

	protected bool ModCheckUseObjectOnTile(Tile.TileType NewType, AFO Info)
	{
		if (ModManager.Instance.ModToolClass.CustomToolInfo.ContainsKey(Info.m_ObjectType) && ModManager.Instance.ModToolClass.CustomToolInfo[Info.m_ObjectType].TilesToUseOn.Contains(NewType))
		{
			return true;
		}
		return false;
	}

	public override void ModEndAction(AFO Info)
	{
		Tile.TileType tileType = TileManager.Instance.GetTile(Info.m_Position).m_TileType;
		if (!ModManager.Instance.ModToolClass.CustomToolInfo.ContainsKey(Info.m_ObjectType))
		{
			return;
		}
		ModTool.ModToolInfo modToolInfo = ModManager.Instance.ModToolClass.CustomToolInfo[Info.m_ObjectType];
		if (!modToolInfo.TilesToUseOn.Contains(tileType))
		{
			return;
		}
		int uniqueID = m_UniqueID;
		string str = m_TypeIdentifier.ToString();
		int num = 0;
		foreach (ObjectType item in modToolInfo.ObjectsToProduce)
		{
			for (int i = 0; i < modToolInfo.ObjectsToProduceAmount[num]; i++)
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(item, Info.m_Position.ToWorldPositionTileCentered(), Quaternion.identity);
				SpawnAnimationManager.Instance.AddJump(baseClass, Info.m_Position, Info.m_Position, 0f, baseClass.transform.position.y, 4f);
			}
			num++;
		}
		if (modToolInfo.Callback.Function != null)
		{
			DynValue[] args = new DynValue[5]
			{
				DynValue.NewNumber(Info.m_Actioner.m_UniqueID),
				DynValue.NewNumber(Info.m_Position.x),
				DynValue.NewNumber(Info.m_Position.y),
				DynValue.NewNumber(uniqueID),
				DynValue.NewString(str)
			};
			ModManager.Instance.Callback(modToolInfo.OwnerScript, modToolInfo.Callback, args);
		}
	}
}
