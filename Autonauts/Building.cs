using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Building : Selectable
{
	[HideInInspector]
	public int m_Rotation;

	[HideInInspector]
	public TileCoord m_TopLeft;

	[HideInInspector]
	public TileCoord m_BottomRight;

	[HideInInspector]
	public TileCoord m_AccessPoint;

	[HideInInspector]
	public int m_AccessPointRotation;

	[HideInInspector]
	public GameObject m_AccessModel;

	[HideInInspector]
	public bool m_AccessModelHidden;

	[HideInInspector]
	public ObjectType m_FloorRequired;

	[HideInInspector]
	public List<TileCoord> m_Tiles;

	protected static float m_AccessHeight = 0.01f;

	[HideInInspector]
	public int m_MaxLevels;

	[HideInInspector]
	public int m_NumLevels;

	[HideInInspector]
	public int m_TotalLevels;

	[HideInInspector]
	public List<Building> m_Levels;

	[HideInInspector]
	public int m_Level;

	[HideInInspector]
	public float m_LevelHeight;

	[HideInInspector]
	public Building m_ParentBuilding;

	[HideInInspector]
	public bool m_Blueprint;

	[HideInInspector]
	public Building m_ConnectedTo;

	[HideInInspector]
	public LinkedSystem m_LinkedSystem;

	private NewIcon m_NewIcon;

	public WallFloorIcon m_WallFloorIcon;

	public bool m_WallMissing;

	public bool m_FloorMissing;

	public bool m_PowerMissing;

	public bool m_ConnectedToTransmitter;

	private bool m_NewConnectedToTransmitter;

	public Dictionary<TileCoord, int> m_ExtraTiles;

	protected int m_CountIndex;

	private bool m_AccessIn;

	public ObjectType m_BuildingToUpgradeFrom;

	public ObjectType m_BuildingToUpgradeTo;

	protected string m_Name;

	public static bool GetIsTypeDragable(ObjectType NewType)
	{
		if (NewType == ObjectType.FencePost || NewType == ObjectType.StoneWall || NewType == ObjectType.StonePath || NewType == ObjectType.SandPath || (Bridge.GetIsTypeBridge(NewType) && NewType != ObjectType.CastleDrawbridge) || NewType == ObjectType.FencePicket || NewType == ObjectType.FlooringCrude || NewType == ObjectType.Workshop || NewType == ObjectType.LogWall || NewType == ObjectType.BlockWall || NewType == ObjectType.BrickWall || NewType == ObjectType.RoadCrude || ModManager.Instance.ModBuildingClass.IsItCustomType(NewType) || NewType == ObjectType.RoadGood || NewType == ObjectType.FlooringBrick || NewType == ObjectType.FlooringFlagstone || NewType == ObjectType.FlooringParquet || NewType == ObjectType.TrainTrack || NewType == ObjectType.TrainTrackBridge || NewType == ObjectType.CastleWall || NewType == ObjectType.FlooringChequer)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeRotatable(ObjectType NewType)
	{
		return true;
	}

	public static bool GetIsTypeWalkable(ObjectType NewType)
	{
		if (NewType == ObjectType.StoneArch || NewType == ObjectType.LogArch || NewType == ObjectType.BeltLinkage || Bridge.GetIsTypeBridge(NewType) || NewType == ObjectType.TrainTrackBridge || ModManager.Instance.ModBuildingClass.IsItWalkable(NewType) || VariableManager.Instance.GetVariableAsInt(NewType, "Walkable", CheckValid: false) == 1)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Building", m_TypeIdentifier);
	}

	public virtual void CheckAddCollectable(bool FromLoad)
	{
		if (!ObjectTypeList.m_Loading || FromLoad)
		{
			CollectionManager.Instance.AddCollectable("Building", this);
		}
	}

	public override void Restart()
	{
		base.Restart();
		m_CountIndex = 0;
		if (!ObjectTypeList.m_Loading && ((bool)GetComponent<Converter>() || TrainTrackStop.GetIsTypeTrainTrackStop(m_TypeIdentifier) || TrainTrackPoints.GetIsTypeTrainTrackPoints(m_TypeIdentifier)))
		{
			m_CountIndex = ObjectCountManager.Instance.RegisterNewObject(this);
		}
		CheckAddCollectable(FromLoad: false);
		m_Blueprint = false;
		m_Rotation = 0;
		m_NewIcon = null;
		m_FloorRequired = ObjectTypeList.m_Total;
		if (m_MaxLevels > 1)
		{
			m_Levels = new List<Building>();
		}
		else
		{
			m_Levels = null;
		}
		m_Level = 0;
		m_TotalLevels = m_NumLevels;
		m_ParentBuilding = null;
		CalcHeight();
		m_ConnectedTo = null;
		m_LinkedSystem = null;
		LinkedSystemManager.Instance.AddBuilding(this);
		m_ConnectedToTransmitter = true;
		SetConnectedToTransmitter(Connected: false);
		TestConnectedToTransmitter();
		m_ExtraTiles = null;
		m_BuildingToUpgradeFrom = ObjectTypeList.m_Total;
		m_BuildingToUpgradeTo = ObjectTypeList.m_Total;
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "UpgradeTo", CheckValid: false);
		if (variableAsInt != 0)
		{
			SetUpgradeTo((ObjectType)variableAsInt);
		}
		int variableAsInt2 = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "UpgradeFrom", CheckValid: false);
		if (variableAsInt2 != 0)
		{
			SetUpgradeFrom((ObjectType)variableAsInt2);
		}
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_AccessModel = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Buildings/BuildingAccessPoint", base.transform, RandomVariants: false);
		UpdateAccessModelPosition();
		m_WallMissing = false;
		m_FloorMissing = false;
		m_PowerMissing = false;
	}

	public void ChangeAccessPointToIn()
	{
		Object.Destroy(m_AccessModel.gameObject);
		m_AccessModel = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Buildings/BuildingAccessPointIn", base.transform, RandomVariants: false);
		UpdateAccessModelPosition();
		m_AccessIn = true;
	}

	public new void Awake()
	{
		base.Awake();
		m_Tiles = new List<TileCoord>();
		m_AccessPoint = new TileCoord(0, 0);
		m_TopLeft = new TileCoord(-1, -1);
		m_BottomRight = new TileCoord(1, 1);
		m_AccessModel = null;
		m_ConnectedTo = null;
		m_MaxLevels = 1;
		m_NumLevels = 1;
		m_Name = "";
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		if (m_NewIcon != null)
		{
			m_NewIcon.StopUsing();
			m_NewIcon = null;
		}
		if (m_WallFloorIcon != null)
		{
			m_WallFloorIcon.StopUsing();
			m_WallFloorIcon = null;
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		m_Name = "";
		if (!m_Blueprint)
		{
			RecordingManager.Instance.RemoveObject(this);
		}
		StartConnectionTo(null);
		if (m_LinkedSystem != null)
		{
			LinkedSystemManager.Instance.RemoveBuilding(this, AndDestroy);
		}
		if (m_NewIcon != null)
		{
			m_NewIcon.StopUsing();
			m_NewIcon = null;
		}
		if (m_WallFloorIcon != null)
		{
			m_WallFloorIcon.StopUsing();
			m_WallFloorIcon = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override void SetHighlight(bool Highlighted)
	{
		base.SetHighlight(Highlighted);
		if (m_BuildingToUpgradeTo == ObjectTypeList.m_Total || !(m_ParentBuilding == null))
		{
			return;
		}
		foreach (Building level in m_Levels)
		{
			level.SetHighlight(Highlighted);
		}
	}

	public virtual void PlayerDeleted()
	{
	}

	public virtual void UpdateTiles()
	{
		m_Tiles.Clear();
		GetBoundingRectangle(out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				m_Tiles.Add(new TileCoord(j, i));
			}
		}
		if ((bool)m_AccessModel && !m_AccessModelHidden)
		{
			m_Tiles.Add(GetAccessPosition());
		}
		if (m_ExtraTiles == null)
		{
			return;
		}
		foreach (KeyValuePair<TileCoord, int> extraTile in m_ExtraTiles)
		{
			TileCoord key = extraTile.Key;
			key.Rotate(m_Rotation);
			key += m_TileCoord;
			if (extraTile.Value == 1)
			{
				m_Tiles.Add(key);
			}
			else
			{
				m_Tiles.Remove(key);
			}
		}
	}

	public void AddTile(TileCoord NewTile)
	{
		if (m_ExtraTiles == null)
		{
			m_ExtraTiles = new Dictionary<TileCoord, int>();
		}
		m_ExtraTiles.Add(NewTile, 1);
	}

	public void RemoveTile(TileCoord NewTile)
	{
		if (m_ExtraTiles == null)
		{
			m_ExtraTiles = new Dictionary<TileCoord, int>();
		}
		m_ExtraTiles.Add(NewTile, 0);
	}

	private void UpdateAccessPosition()
	{
		if ((bool)m_AccessModel)
		{
			float heightOffGround = GetAccessPosition().GetHeightOffGround();
			Vector3 position = m_AccessModel.transform.position;
			position.y = heightOffGround + m_AccessHeight;
			m_AccessModel.transform.position = position;
		}
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		base.TileCoordChanged(Position);
		UpdateAccessPosition();
		UpdateTiles();
		if (!m_Blueprint)
		{
			RecordingManager.Instance.UpdateObject(this);
			Sleep();
		}
	}

	public void HideAccessModel(bool Hide = true)
	{
		m_AccessModelHidden = Hide;
		m_AccessModel.SetActive(!Hide);
		UpdateTiles();
	}

	public virtual void UpdateAccessModelPosition()
	{
		if (!m_AccessModel)
		{
			return;
		}
		if (m_Level == 0)
		{
			m_AccessModel.transform.localPosition = m_AccessPoint.ToWorldPosition() + new Vector3(0f, m_AccessHeight, 0f);
			m_AccessPointRotation = 1;
			if (m_AccessPoint.x < m_TopLeft.x)
			{
				m_AccessPointRotation = 2;
			}
			else if (m_AccessPoint.y > m_BottomRight.y)
			{
				m_AccessPointRotation = 1;
			}
			else if (m_AccessPoint.x > m_BottomRight.x)
			{
				m_AccessPointRotation = 0;
			}
			else if (m_AccessPoint.y < m_TopLeft.y)
			{
				m_AccessPointRotation = 3;
			}
			m_AccessModel.transform.localRotation = Quaternion.Euler(0f, 90 * (m_AccessPointRotation - 1), 0f);
		}
		else
		{
			m_AccessModel.SetActive(value: false);
		}
	}

	protected void SetDimensions(TileCoord TopLeft, TileCoord BottomRight, TileCoord AccessPoint)
	{
		m_TopLeft = TopLeft;
		m_BottomRight = BottomRight;
		m_AccessPoint = AccessPoint;
		UpdateAccessModelPosition();
		UpdateTiles();
		TileCoordChanged(m_TileCoord);
	}

	public override string GetHumanReadableName()
	{
		if (m_Name != "")
		{
			return m_Name;
		}
		return base.GetHumanReadableName();
	}

	public string GetName()
	{
		return m_Name;
	}

	public void SetName(string NewName)
	{
		bool flag = m_Name != NewName;
		m_Name = NewName;
		if (!SaveLoadManager.Instance.m_Loading && flag)
		{
			ModManager.Instance.CheckBuildingEditedCallback(this, "Rename", m_Name);
		}
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_CountIndex != 0)
		{
			JSONUtils.Set(Node, "Index", m_CountIndex);
		}
		JSONUtils.Set(Node, "Rotation", m_Rotation);
		if (m_Name != "")
		{
			JSONUtils.Set(Node, "Name", m_Name);
		}
		if (m_Levels != null && m_Levels.Count > 0)
		{
			JSONArray jSONArray = (JSONArray)(Node["Levels"] = new JSONArray());
			for (int i = 0; i < m_Levels.Count; i++)
			{
				JSONNode jSONNode2 = new JSONObject();
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Levels[i].m_TypeIdentifier);
				JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
				m_Levels[i].Save(jSONNode2);
				jSONArray[i] = jSONNode2;
			}
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CheckAddCollectable(FromLoad: true);
		m_CountIndex = JSONUtils.GetAsInt(Node, "Index", 0);
		if (m_CountIndex != 0)
		{
			ObjectCountManager.Instance.RegisterOldObject(this, m_CountIndex);
		}
		m_Rotation = JSONUtils.GetAsInt(Node, "Rotation", 0);
		SetRotation(m_Rotation);
		m_Name = JSONUtils.GetAsString(Node, "Name", "");
	}

	public void PostLoad(JSONNode Node)
	{
		MapManager.Instance.AddBuilding(this);
		JSONNode jSONNode = Node["Levels"];
		if (!(jSONNode != null) || jSONNode.IsNull)
		{
			return;
		}
		JSONArray asArray = jSONNode.AsArray;
		int count = asArray.Count;
		for (int i = 0; i < count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			if (asObject["ID"] != (object)"")
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				if ((bool)baseClass)
				{
					baseClass.GetComponent<Savable>().Load(asObject);
					baseClass.GetComponent<Building>().PostLoad(asObject);
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
		if (m_Levels != null)
		{
			for (int i = 0; i < m_Levels.Count; i++)
			{
				m_Levels[i].SendAction(Info);
			}
		}
		if (m_LinkedSystem != null)
		{
			LinkedSystemManager.Instance.UpdateBuilding(this);
		}
		if (Info.m_Action == ActionType.RefreshFirst)
		{
			RefreshConnected();
		}
		Refresh();
		if (Info.m_Action == ActionType.RefreshFirst)
		{
			RefreshConnected();
		}
		if (m_WallFloorIcon == null && (VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Walls", CheckValid: false) != 0 || VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Floors", CheckValid: false) != 0 || LinkedSystemConverter.GetIsTypeLinkedSystemConverter(m_TypeIdentifier)))
		{
			m_WallFloorIcon = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.WallFloorIcon, base.transform.position, Quaternion.identity).GetComponent<WallFloorIcon>();
			m_WallFloorIcon.SetScale(4f);
			m_WallFloorIcon.SetTarget(base.gameObject, new Vector3(0f, 3f, 0f));
			m_WallFloorIcon.gameObject.SetActive(value: false);
		}
		CheckWallsFloors();
		UpdateAccessPosition();
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsMovable:
		{
			if (m_BuildingToUpgradeTo == ObjectTypeList.m_Total || m_Levels.Count <= 0)
			{
				break;
			}
			for (int i = 0; i < m_Levels.Count; i++)
			{
				if (m_Levels[i].m_TypeIdentifier == ObjectType.ConverterFoundation && m_Levels[i].GetComponent<ConverterFoundation>().m_State != 0)
				{
					return false;
				}
			}
			break;
		}
		case GetAction.IsDuplicatable:
			if (m_BuildingToUpgradeTo == ObjectTypeList.m_Total || m_Levels.Count <= 0)
			{
				break;
			}
			foreach (Building level in m_Levels)
			{
				if (level.m_TypeIdentifier == ObjectType.ConverterFoundation && level.GetComponent<ConverterFoundation>().m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
				{
					return false;
				}
			}
			break;
		}
		return base.GetActionInfo(Info);
	}

	public virtual void CheckWallsFloors()
	{
		if (!m_WallFloorIcon)
		{
			return;
		}
		if (m_BuildingToUpgradeFrom != ObjectTypeList.m_Total && m_ParentBuilding != null)
		{
			m_WallFloorIcon.Set(WallsNeeded: false, FloorsNeeded: false, PowerNeeded: false);
			return;
		}
		m_WallMissing = false;
		m_FloorMissing = false;
		m_PowerMissing = false;
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Walls", CheckValid: false);
		int variableAsInt2 = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Floors", CheckValid: false);
		bool isTypeLinkedSystemConverter = LinkedSystemConverter.GetIsTypeLinkedSystemConverter(m_TypeIdentifier);
		if (GeneralUtils.m_InGame)
		{
			foreach (TileCoord tile2 in m_Tiles)
			{
				Tile tile = TileManager.Instance.GetTile(tile2);
				if (tile != null)
				{
					if (variableAsInt > 0 && tile.m_WalledArea == null)
					{
						m_WallMissing = true;
					}
					if (variableAsInt2 > 0 && tile.m_Floor == null)
					{
						m_FloorMissing = true;
					}
				}
			}
			if (isTypeLinkedSystemConverter && (m_LinkedSystem == null || ((LinkedSystemMechanical)m_LinkedSystem).m_TotalEnergyAvailable < (float)GetComponent<LinkedSystemConverter>().GetResultEnergyRequired()))
			{
				m_PowerMissing = true;
			}
		}
		m_WallFloorIcon.Set(m_WallMissing, m_FloorMissing, m_PowerMissing);
	}

	public bool GetAreRequirementsMet()
	{
		if (m_WallMissing || m_FloorMissing || m_PowerMissing)
		{
			return false;
		}
		return true;
	}

	public virtual void HideWallFloorIcon()
	{
		if ((bool)m_WallFloorIcon)
		{
			m_WallFloorIcon.gameObject.SetActive(value: false);
		}
	}

	public virtual void SetRotation(int Rotation)
	{
		bool flag = m_Rotation != Rotation;
		m_Rotation = Rotation;
		base.transform.localRotation = Quaternion.Euler(0f, (float)Rotation * 90f, 0f);
		UpdateTiles();
		TileCoordChanged(m_TileCoord);
		if (!SaveLoadManager.Instance.m_Loading && flag)
		{
			ModManager.Instance.CheckBuildingEditedCallback(this, "Rotate");
		}
	}

	public void GetBoundingRectangle(out TileCoord TopLeft, out TileCoord BottomRight)
	{
		TopLeft = m_TopLeft;
		BottomRight = m_BottomRight;
		TopLeft.Rotate(m_Rotation);
		BottomRight.Rotate(m_Rotation);
		TopLeft += m_TileCoord;
		BottomRight += m_TileCoord;
		if (TopLeft.y > BottomRight.y)
		{
			int y = BottomRight.y;
			BottomRight.y = TopLeft.y;
			TopLeft.y = y;
		}
		if (TopLeft.x > BottomRight.x)
		{
			int y = BottomRight.x;
			BottomRight.x = TopLeft.x;
			TopLeft.x = y;
		}
	}

	public virtual TileCoord GetAccessPosition()
	{
		if ((bool)m_ParentBuilding && m_ParentBuilding.m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			return m_ParentBuilding.GetAccessPosition();
		}
		if (m_AccessModelHidden)
		{
			return m_TileCoord;
		}
		TileCoord tileCoord = default(TileCoord);
		tileCoord.Copy(m_AccessPoint);
		tileCoord.Rotate(m_Rotation);
		return tileCoord + m_TileCoord;
	}

	public virtual TileCoord GetSpawnPoint()
	{
		return new TileCoord(-1, -1);
	}

	public float GetAccessRotationInDegrees()
	{
		return (float)((m_AccessPointRotation + m_Rotation + 2) % 4) * 360f / 4f;
	}

	public override List<TileCoord> GetAdjacentTiles()
	{
		TileCoord accessPosition = GetAccessPosition();
		List<TileCoord> list = new List<TileCoord>();
		foreach (TileCoord tile in m_Tiles)
		{
			if (!(tile != accessPosition) && !m_AccessModelHidden)
			{
				continue;
			}
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					TileCoord tileCoord = new TileCoord(j, i) + tile;
					if (tileCoord.x >= 0 && tileCoord.y >= 0 && tileCoord.x < TileManager.Instance.m_TilesWide && tileCoord.y < TileManager.Instance.m_TilesHigh && !list.Contains(tileCoord))
					{
						Building building = TileManager.Instance.GetTile(tileCoord).m_Building;
						if ((building == null || ((bool)building.GetComponent<ConverterFoundation>() && building != this)) && !TileManager.Instance.GetTileSolidToPlayer(tileCoord))
						{
							list.Add(tileCoord);
						}
					}
				}
			}
		}
		return list;
	}

	public override TileCoord GetNearestAdjacentTile(TileCoord Position)
	{
		TileCoord result = default(TileCoord);
		List<TileCoord> adjacentTiles = GetAdjacentTiles();
		float num = 1E+07f;
		foreach (TileCoord item in adjacentTiles)
		{
			float num2 = (Position - item).Magnitude();
			if (num2 < num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	public virtual void SetBlueprint(bool Blueprint, bool Register = false)
	{
		Wake();
		if (Blueprint)
		{
			GetComponent<Savable>().SetIsSavable(IsSavable: false);
			LinkedSystemManager.Instance.RemoveBuilding(this, Destroy: false);
			m_AccessModel.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.25f);
			if (m_CountIndex != 0)
			{
				ObjectCountManager.Instance.DeregisterObject(this, m_CountIndex);
			}
		}
		else
		{
			GetComponent<Savable>().SetIsSavable(IsSavable: true);
			LinkedSystemManager.Instance.AddBuilding(this);
			if (m_AccessIn)
			{
				ModelManager.Instance.RestoreStandardMaterials(ObjectTypeList.m_Total, m_AccessModel, "Models/Buildings/BuildingAccessPointIn");
			}
			else
			{
				ModelManager.Instance.RestoreStandardMaterials(ObjectTypeList.m_Total, m_AccessModel, "Models/Buildings/BuildingAccessPoint");
			}
			if (Register)
			{
				m_CountIndex = ObjectCountManager.Instance.RegisterNewObject(this);
			}
		}
		m_Blueprint = Blueprint;
		base.enabled = !Blueprint;
		if (Blueprint)
		{
			RecordingManager.Instance.IgnoreObject(this);
		}
		CheckWallsFloors();
		Sleep();
	}

	public virtual bool GetNewLevelAllowed(Building NewBuilding)
	{
		return false;
	}

	public virtual bool CanStack()
	{
		if (m_BuildingToUpgradeFrom == ObjectTypeList.m_Total)
		{
			return true;
		}
		return false;
	}

	public float GetBuildingHeightOffset()
	{
		float num = m_LevelHeight;
		if (m_Levels != null && CanStack())
		{
			for (int i = 0; i < m_Levels.Count; i++)
			{
				num += m_Levels[i].m_LevelHeight;
			}
		}
		return num;
	}

	private bool FindParent(Building TestBuilding)
	{
		if (TestBuilding == this)
		{
			return true;
		}
		if ((bool)m_ParentBuilding)
		{
			return m_ParentBuilding.FindParent(TestBuilding);
		}
		return false;
	}

	public virtual Vector3 AddBuilding(Building NewBuilding)
	{
		if ((bool)NewBuilding.m_ParentBuilding)
		{
			Debug.Log("Child building already has a parent " + m_UniqueID + " " + NewBuilding.m_UniqueID);
			NewBuilding.m_ParentBuilding.RemoveBuilding(NewBuilding);
		}
		if (FindParent(NewBuilding))
		{
			Debug.Log("Infinte recursion trying to add a building to another building " + m_UniqueID + " " + NewBuilding.m_UniqueID);
			return NewBuilding.transform.position;
		}
		if ((bool)m_ParentBuilding)
		{
			Debug.Log("Adding a child building when we have a parent " + m_UniqueID + " " + NewBuilding.m_UniqueID);
			return m_ParentBuilding.AddBuilding(NewBuilding);
		}
		if (NewBuilding.m_Levels != null && NewBuilding.m_Levels.Count > 0)
		{
			Debug.Log("Adding a child building that has children " + m_UniqueID + " " + NewBuilding.m_UniqueID);
		}
		Vector3 position = NewBuilding.transform.position;
		float num = 0f;
		if (NewBuilding.CanStack())
		{
			num = GetBuildingHeightOffset();
		}
		position.y = base.transform.position.y + num;
		m_Levels.Add(NewBuilding);
		NewBuilding.m_Level = m_TotalLevels;
		NewBuilding.m_ParentBuilding = this;
		NewBuilding.SetIsSavable(IsSavable: false);
		NewBuilding.m_AccessModel.SetActive(value: false);
		m_TotalLevels += NewBuilding.m_NumLevels;
		if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation && Storage.GetIsTypeStorage(NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier))
		{
			NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_AccessModel.SetActive(value: false);
		}
		return position;
	}

	public void RemoveBuilding(Building NewBuilding)
	{
		if (!m_Levels.Contains(NewBuilding))
		{
			return;
		}
		m_Levels.Remove(NewBuilding);
		m_TotalLevels -= NewBuilding.m_NumLevels;
		NewBuilding.m_Level = 0;
		NewBuilding.m_ParentBuilding = null;
		NewBuilding.SetIsSavable(IsSavable: true);
		if (!m_AccessModelHidden)
		{
			if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_AccessModel.SetActive(value: true);
			}
			else
			{
				NewBuilding.m_AccessModel.SetActive(value: true);
			}
		}
	}

	public void RemoveAllBuildings()
	{
		if (m_Levels == null || m_Levels.Count <= 0)
		{
			return;
		}
		if ((bool)m_ParentBuilding)
		{
			Debug.Log("Building has children and a parent " + m_UniqueID);
		}
		bool flag = false;
		do
		{
			flag = false;
			Building topBuilding = GetTopBuilding();
			if ((bool)topBuilding && topBuilding != this)
			{
				flag = true;
				RemoveBuilding(topBuilding);
			}
		}
		while (flag);
	}

	public Building GetTopBuilding()
	{
		if ((bool)m_ParentBuilding)
		{
			return m_ParentBuilding.GetTopBuilding();
		}
		if (m_Levels == null || m_Levels.Count == 0)
		{
			return this;
		}
		return m_Levels[m_Levels.Count - 1];
	}

	public Building GetTopFoundation()
	{
		if (m_Levels == null || m_Levels.Count == 0)
		{
			return this;
		}
		if (m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			return this;
		}
		for (int i = 0; i < m_Levels.Count; i++)
		{
			if (m_Levels[i].m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				return m_Levels[i];
			}
		}
		return this;
	}

	public Building GetBottomBuilding()
	{
		return TileManager.Instance.GetTile(m_TileCoord).m_Building;
	}

	public Building GetBuildingAtLevel(int Level)
	{
		int num = m_NumLevels;
		if (Level < num)
		{
			return this;
		}
		if (m_Levels == null)
		{
			return null;
		}
		foreach (Building level in m_Levels)
		{
			if (Level >= num && Level < num + level.m_NumLevels)
			{
				return level;
			}
			num += level.m_NumLevels;
		}
		return null;
	}

	public int GetBuildingLevelIndex()
	{
		if ((bool)m_ParentBuilding)
		{
			return m_ParentBuilding.m_Levels.IndexOf(this) + 1;
		}
		return 0;
	}

	public void SetHeight(float ExtraHeight)
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = ExtraHeight;
		base.transform.localPosition = localPosition;
		if (m_Levels == null)
		{
			return;
		}
		ExtraHeight += m_LevelHeight;
		foreach (Building level in m_Levels)
		{
			localPosition = level.transform.localPosition;
			if (level.CanStack())
			{
				localPosition.y = ExtraHeight;
			}
			else
			{
				localPosition.y = base.transform.position.y;
			}
			level.transform.localPosition = localPosition;
			ExtraHeight += level.m_LevelHeight;
		}
	}

	protected virtual void CalcHeight()
	{
		m_LevelHeight = ObjectTypeList.Instance.GetHeight(m_TypeIdentifier);
	}

	public void SetNew(bool New)
	{
		if (New)
		{
			if (m_NewIcon == null)
			{
				m_NewIcon = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.NewIcon, base.transform.position, Quaternion.identity).GetComponent<NewIcon>();
				m_NewIcon.SetScale(4f);
				m_NewIcon.SetTarget(base.gameObject, new Vector3(0f, 1f, 0f));
			}
		}
		else if ((bool)m_NewIcon)
		{
			m_NewIcon.StopUsing();
			m_NewIcon = null;
		}
	}

	public void SetBeingMoved(bool Moved)
	{
	}

	public void StartConnectionTo(Building NewBuilding)
	{
		if (m_ConnectedTo != NewBuilding)
		{
			if (m_ConnectedTo != null)
			{
				m_ConnectedTo.StopConnectionTo(this);
			}
			m_ConnectedTo = NewBuilding;
		}
	}

	public void StopConnection()
	{
		m_ConnectedTo = null;
	}

	protected void RefreshConnection()
	{
		if (m_ConnectedTo != null)
		{
			m_ConnectedTo.StopConnectionTo(this);
		}
		m_ConnectedTo = null;
		Building building = null;
		if (BeltLinkage.CanTypeConnectTo(m_TypeIdentifier))
		{
			building = BeltLinkage.TestConnection(this);
		}
		if ((bool)building)
		{
			building.ConnectBuilding(this);
		}
	}

	public virtual void StopConnectionTo(Building NewBuilding)
	{
	}

	public virtual void ConnectBuilding(Building NewBuilding)
	{
	}

	protected virtual void Refresh()
	{
	}

	protected virtual void RefreshConnected()
	{
	}

	public virtual void CopyFrom(Building OriginalBuilding)
	{
	}

	public void SetConnectedToTransmitter(bool Connected)
	{
		m_NewConnectedToTransmitter = Connected;
	}

	public void ChangeConnectionToTransmitter(bool Connected)
	{
	}

	public void UpdateConnectionToTransmitter()
	{
		if (!GetIsSavable())
		{
			return;
		}
		ChangeConnectionToTransmitter(m_ConnectedToTransmitter);
		if (m_Levels == null)
		{
			return;
		}
		foreach (Building level in m_Levels)
		{
			level.ChangeConnectionToTransmitter(m_ConnectedToTransmitter);
		}
	}

	protected virtual void ConnectionToTransmitterChanged()
	{
	}

	public void TestConnectedToTransmitter()
	{
		if (m_ConnectedToTransmitter == m_NewConnectedToTransmitter)
		{
			return;
		}
		m_ConnectedToTransmitter = m_NewConnectedToTransmitter;
		if (m_Levels != null)
		{
			foreach (Building level in m_Levels)
			{
				level.m_ConnectedToTransmitter = m_ConnectedToTransmitter;
			}
		}
		UpdateConnectionToTransmitter();
		ConnectionToTransmitterChanged();
	}

	public void DeleteFoundations()
	{
		if (m_Levels != null)
		{
			for (int num = m_Levels.Count - 1; num >= 0; num--)
			{
				Building building = m_Levels[num];
				if (building.m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					RemoveBuilding(building);
					building.StopUsing();
				}
			}
		}
		if (m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			MapManager.Instance.RemoveBuilding(this);
			StopUsing();
		}
	}

	public override void LoadNewModel(string ModelName, bool RandomVariants = false)
	{
		base.LoadNewModel(ModelName, RandomVariants);
		UpdateConnectionToTransmitter();
		if ((bool)m_ParentBuilding && m_ParentBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			m_ParentBuilding.GetComponent<ConverterFoundation>().MakeBuildingTransparent();
		}
	}

	public bool CanBeSearchedFor()
	{
		if (m_Blueprint)
		{
			return false;
		}
		if (m_ParentBuilding == null)
		{
			return true;
		}
		if (m_ParentBuilding.GetTopFoundation() == this)
		{
			return true;
		}
		return false;
	}

	protected virtual void EndAdd(AFO Info)
	{
	}

	protected virtual ActionType GetActionFromAnything(AFO Info)
	{
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			return GetActionFromAnything(Info);
		}
		return ActionType.Total;
	}

	public virtual bool CanBuildTypeUpon(ObjectType NewType)
	{
		if (m_MaxLevels != 1 && m_TypeIdentifier == NewType)
		{
			return true;
		}
		return false;
	}

	protected void SetUpgradeTo(ObjectType NewType)
	{
		m_BuildingToUpgradeTo = NewType;
		m_Levels = new List<Building>();
	}

	protected void SetUpgradeFrom(ObjectType NewType)
	{
		m_BuildingToUpgradeFrom = NewType;
	}

	protected bool IsBeingUpgraded()
	{
		if (m_BuildingToUpgradeTo != ObjectTypeList.m_Total && m_Levels.Count > 0)
		{
			return true;
		}
		return false;
	}

	public void BeginUpgrade()
	{
		bool flag = true;
		if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative && !CheatManager.Instance.m_InstantBuild)
		{
			ConverterFoundation component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ConverterFoundation, base.transform.position, Quaternion.identity).GetComponent<ConverterFoundation>();
			component.SetNewBuilding(m_BuildingToUpgradeTo);
			component.SetRotation(m_Rotation);
			AddBuilding(component);
			component.UpdateTiles();
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		Building component2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(m_BuildingToUpgradeTo, base.transform.position, Quaternion.identity).GetComponent<Building>();
		component2.SetRotation(m_Rotation);
		component2.HideAccessModel(Hide: false);
		MapManager.Instance.RemoveBuilding(this);
		ConverterFoundation.TransferToNewBuilding(this, component2);
		ConverterFoundation.TransferBotsToNewBuilding(this, component2);
		MapManager.Instance.AddBuilding(component2);
		RecordingManager.Instance.UpdateObject(component2);
		component2.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		foreach (Building level in m_Levels)
		{
			level.StopUsing();
		}
		StopUsing();
	}

	public virtual void SetLinkedSystem(LinkedSystem NewSystem)
	{
		m_LinkedSystem = NewSystem;
	}

	public void TestBuildingHeight()
	{
		float num = -100f;
		foreach (TileCoord tile in m_Tiles)
		{
			float heightOffGround = tile.GetHeightOffGround();
			if (num < heightOffGround)
			{
				num = heightOffGround;
			}
		}
		Vector3 position = base.transform.position;
		position.y = num;
		base.transform.position = position;
		if (m_Levels == null)
		{
			return;
		}
		num += m_LevelHeight;
		for (int i = 0; i < m_Levels.Count; i++)
		{
			position = m_Levels[i].transform.position;
			if (m_Levels[i].CanStack())
			{
				position.y = num;
			}
			else
			{
				position.y = base.transform.position.y;
			}
			m_Levels[i].transform.position = position;
			num += m_Levels[i].m_LevelHeight;
		}
	}

	public GameObject FindNode(string Name)
	{
		Transform transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, Name);
		if (transform == null)
		{
			return null;
		}
		return transform.gameObject;
	}

	private bool DoInstructionsReferenceThis(List<HighInstruction> NewInstructions)
	{
		foreach (HighInstruction NewInstruction in NewInstructions)
		{
			if (NewInstruction.m_ActionInfo.m_ObjectUID == m_UniqueID && NewInstruction.m_Type != HighInstruction.Type.Pickup)
			{
				return true;
			}
			if (NewInstruction.m_Children.Count > 0 && DoInstructionsReferenceThis(NewInstruction.m_Children))
			{
				return true;
			}
			if (NewInstruction.m_Children2.Count > 0 && DoInstructionsReferenceThis(NewInstruction.m_Children2))
			{
				return true;
			}
		}
		return false;
	}

	public List<Worker> GetReferencingWorkers()
	{
		List<Worker> list = new List<Worker>();
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				Worker component = item.Key.GetComponent<Worker>();
				HighInstructionList highInstructions = component.m_WorkerInterpreter.m_HighInstructions;
				if (DoInstructionsReferenceThis(highInstructions.m_List))
				{
					list.Add(component);
				}
			}
			return list;
		}
		return list;
	}

	public virtual void Moved()
	{
	}
}
