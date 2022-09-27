using SimpleJSON;
using UnityEngine;

public class Folk : GoTo
{
	public enum FaceType
	{
		Neutral,
		VeryHappy,
		Blink,
		Total
	}

	[HideInInspector]
	public enum State
	{
		Normal,
		Eating,
		BadObject,
		LevelUp,
		Total
	}

	public enum UnhappyState
	{
		None,
		Hungry,
		Housing,
		Clothing,
		Total
	}

	public enum FindReason
	{
		None,
		Hungry,
		NeedClothes,
		NeedHouse,
		Total
	}

	private static float m_EatingDelay = 2f;

	private static float m_BadObjectDelay = 1f;

	private static float m_LevelUpDelay = 1f;

	private static string[] m_Names = new string[134]
	{
		"Affogato", "Akumaki", "Amanatto", "Amaretti", "Anko", "Anmitsu", "Baba", "Babka", "Baklava", "Bamiyeh",
		"Barbajadav", "Baumkuchen", "Bombolone", "Botamochi", "Buchtel", "Bungeoppang", "Bupyeon", "Cannolo", "Cartellate", "Cassata",
		"Chapssaltteok", "Chinsuko", "Churro", "Ciambellone", "Clafoutis", "Cremeschnitte", "Crostata", "Crumble", "Daifuku", "Dampfnudel",
		"Dango", "Dorayaki", "Drømmekage", "Éclair", "Frappe", "Gaufre", "Gelato", "Granita", "Gyeongju", "Gyuhi",
		"Hanabiramochi", "Hakuto", "Halva", "Hangwa", "Hasma", "Higashi", "Hishimochi", "Hoppang", "Hotteok", "Ichigo",
		"Imagawayaki", "Ispahan", "Jiuniang", "Karinto", "Karukan", "Kasutera", "Kaiserschmarrn", "Kazandibi", "Kinkan", "Kkultarae",
		"Knedlíky", "Kouglof", "Krapfen", "Kuzumochi", "Lebkuchen", "Lokma", "Macaron", "Madeleine", "Manju", "Medovik",
		"Melona", "Meringue", "Mizuame", "Mochi", "Monaka", "Mousse", "Namagashi", "Oliebol", "Oshiruko", "Pandoro",
		"Panettone", "Panforte", "Parfait", "Pashmak", "Pastiera", "Patbingsu", "Pevarini", "Pfeffernuss", "Pignolo", "Pizzelle",
		"Poffertje", "Profiterole", "Religieuse", "Revani", "Sachertorte", "Sakuramochi", "Sambali", "Schneeball", "Semifreddo", "Sernik",
		"Sfingi", "Sfogliatella", "Shiruko", "Soufflé", "Streusel", "Stroopwafel", "Struffoli", "Taiyaki", "Tiramisù", "Tokoroten",
		"Torrone", "Tteok", "Twibap", "Uiro", "Varenye", "Wagashi", "Wasanbon", "Wibele", "Xiaodianxin", "Yakgwa",
		"Yatsuhashi", "Yoet", "Yokan", "Yubeshi", "Yumilgwa", "Zabaione", "Zeppola", "Zippula", "Zuccotto", "Zwetschgenkuchen",
		"Na", "JohnGames", "Koloxlo", "Keir"
	};

	[HideInInspector]
	public State m_State;

	[HideInInspector]
	public State m_PreviousState;

	private float m_StateTimer;

	[HideInInspector]
	public string m_FolkName;

	[HideInInspector]
	public float m_Energy;

	public int m_FoodTier;

	private ObjectType m_ObjectEaten;

	private int m_ObjectEatenUsageCount;

	public Housing m_Housing;

	[HideInInspector]
	private Clothing[] m_Clothes;

	private GameObject[] m_ClothesParents;

	private Toy m_Toy;

	private Transform m_ToyRoot;

	private Medicine m_Medicine;

	private Transform m_MedicineRoot;

	private Education m_Education;

	private Transform m_EducationRoot;

	private Art m_Art;

	private Transform m_ArtRoot;

	private Transform m_HeadRoot;

	private FolkStatusIndicator m_StatusIndicator;

	private LoveHeart m_LoveHeart;

	private string m_LastModelName;

	[HideInInspector]
	public float m_Happiness;

	[HideInInspector]
	public float m_HappyinessTimer;

	private int m_RefreshHutUID;

	private State m_RefreshState;

	private float m_RefreshStateTimer;

	private Animator m_Animator;

	private Material m_FaceMaterial;

	private int m_Size;

	private int m_Tier;

	private int m_OldTier;

	private bool m_Busy;

	private bool m_FeederBot;

	private MyParticles m_WuvParticles;

	private int m_Skin;

	private Clothing m_OldTop;

	private Clothing m_OldHat;

	private Toy m_OldToy;

	private Medicine m_OldMedicine;

	private Education m_OldEducation;

	private Art m_OldArt;

	private static float m_MaxEnergy;

	private static int m_FirstHousingTier;

	private static int m_FirstTopTier;

	private static int m_FirstToyTier;

	private static int m_FirstMedicineTier;

	private static int m_FirstEducationTier;

	private static int m_FirstArtTier;

	private static float m_HappinessDelay;

	public static bool GetWillFolkAcceptObjectType(ObjectType NewType)
	{
		if (Food.GetIsTypeFood(NewType) || Clothing.GetIsTypeClothing(NewType) || Toy.GetIsTypeToy(NewType) || Medicine.GetIsTypeMedicine(NewType) || Education.GetIsTypeEducation(NewType) || Art.GetIsTypeArt(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Folk/Folk2", ObjectType.Folk);
		ModelManager.Instance.AddModel("Models/Folk/Folk3", ObjectType.Folk);
		ModelManager.Instance.AddModel("Models/Folk/FolkSick", ObjectType.Folk);
		ModelManager.Instance.AddModel("Models/Folk/Folk2Sick", ObjectType.Folk);
		ModelManager.Instance.AddModel("Models/Folk/Folk3Sick", ObjectType.Folk);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Folk", this);
		}
		m_StatusIndicator.gameObject.SetActive(value: false);
		m_Housing = null;
		m_MaxEnergy = VariableManager.Instance.GetVariableAsFloat(ObjectType.Folk, "MaxEnergy");
		m_FirstHousingTier = VariableManager.Instance.GetVariableAsInt("FirstHousingTier");
		m_FirstTopTier = VariableManager.Instance.GetVariableAsInt("FirstTopTier");
		m_FirstToyTier = VariableManager.Instance.GetVariableAsInt("FirstToyTier");
		m_FirstMedicineTier = VariableManager.Instance.GetVariableAsInt("FirstMedicineTier");
		m_FirstEducationTier = VariableManager.Instance.GetVariableAsInt("FirstEducationTier");
		m_FirstArtTier = VariableManager.Instance.GetVariableAsInt("FirstArtTier");
		m_HappinessDelay = VariableManager.Instance.GetVariableAsFloat(ObjectType.Folk, "HappinessDelay");
		int num = Random.Range(0, m_Names.Length);
		m_FolkName = m_Names[num];
		m_LoveHeart = null;
		m_Energy = 0f;
		m_LastModelName = "";
		m_HappyinessTimer = 0f;
		UpdateHappiness();
		SetState(State.Normal);
		FolkManager.Instance.AddFolk(this);
		m_Size = 0;
		m_Skin = Random.Range(0, 3);
		UpdateSkin();
		SetTier(0);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/FolkStatusIndicator", typeof(GameObject));
		Transform parent = null;
		if ((bool)HudManager.Instance)
		{
			parent = HudManager.Instance.m_IndicatorsRootTransform;
		}
		m_StatusIndicator = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<FolkStatusIndicator>();
		m_StatusIndicator.SetFolk(this);
		m_StatusIndicator.gameObject.SetActive(value: false);
		m_Animator = GetComponent<Animator>();
		m_Clothes = new Clothing[2];
		for (int i = 0; i < 2; i++)
		{
			m_Clothes[i] = null;
		}
		GetModelNodes();
		if ((bool)ParticlesManager.Instance)
		{
			m_WuvParticles = ParticlesManager.Instance.CreateParticles("FolkWuv", default(Vector3), Quaternion.identity);
			m_WuvParticles.Stop();
		}
	}

	private void RemoveOldObjects()
	{
		m_OldTop = m_Clothes[1];
		if ((bool)m_OldTop)
		{
			m_OldTop.transform.SetParent(null);
		}
		m_OldHat = m_Clothes[0];
		if ((bool)m_OldHat)
		{
			m_OldHat.transform.SetParent(null);
		}
		m_OldToy = m_Toy;
		if ((bool)m_OldToy)
		{
			m_OldToy.transform.SetParent(null);
		}
		m_OldMedicine = m_Medicine;
		if ((bool)m_OldMedicine)
		{
			m_OldMedicine.transform.SetParent(null);
		}
		m_OldEducation = m_Education;
		if ((bool)m_OldEducation)
		{
			m_OldEducation.transform.SetParent(null);
		}
		m_OldArt = m_Art;
		if ((bool)m_OldArt)
		{
			m_OldArt.transform.SetParent(null);
		}
	}

	private void GetClothingNodes()
	{
		m_ClothesParents = new GameObject[2];
		m_ClothesParents[0] = m_ModelRoot.transform.Find("BodyHinge").Find("HeadHinge").Find("HatPoint")
			.gameObject;
		m_ClothesParents[0].transform.localRotation *= Quaternion.Euler(90f, 0f, 0f);
		m_ClothesParents[0].transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
		m_ClothesParents[1] = m_ModelRoot.transform.Find("BodyHinge").Find("TopPoint").gameObject;
		m_ClothesParents[1].transform.localRotation *= Quaternion.Euler(90f, 0f, 0f);
		m_ClothesParents[1].transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
		Vector3 localScale = m_ClothesParents[1].transform.localScale;
		m_ClothesParents[1].transform.localScale = new Vector3(localScale.x, localScale.z, localScale.y);
		if ((bool)m_OldTop)
		{
			m_OldTop.Wear(base.gameObject, m_ClothesParents[1].transform);
		}
		if ((bool)m_OldHat)
		{
			m_OldHat.Wear(base.gameObject, m_ClothesParents[0].transform);
		}
	}

	private void GetRequirementNodes()
	{
		m_ToyRoot = base.transform;
		if ((bool)m_OldToy)
		{
			m_OldToy.Hold(m_ToyRoot);
		}
		m_MedicineRoot = m_ModelRoot.transform.Find("BodyHinge").Find("Body").Find("RightArm")
			.Find("Hand.002");
		if ((bool)m_OldMedicine)
		{
			m_OldMedicine.Hold(m_MedicineRoot);
		}
		m_EducationRoot = m_ModelRoot.transform.Find("BodyHinge").Find("Body").Find("RightArm")
			.Find("Hand.002");
		if ((bool)m_OldEducation)
		{
			m_OldEducation.Hold(m_EducationRoot);
		}
		m_ArtRoot = m_ModelRoot.transform.Find("BodyHinge").Find("Body").Find("RightArm")
			.Find("Hand.002");
		if ((bool)m_OldArt)
		{
			m_OldArt.Hold(m_ArtRoot);
			UpdateArt(m_OldArt);
		}
	}

	private void GetModelNodes()
	{
		GetClothingNodes();
		GetRequirementNodes();
		m_HeadRoot = m_ModelRoot.transform.Find("BodyHinge").Find("HeadHinge");
		Material[] materials = m_HeadRoot.transform.Find("Face").gameObject.GetComponent<MeshRenderer>().materials;
		if (materials.Length != 0)
		{
			m_FaceMaterial = materials[0];
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_WuvParticles)
		{
			m_WuvParticles.Stop();
		}
		for (int i = 0; i < 2; i++)
		{
			if (m_Clothes[i] != null)
			{
				m_Clothes[i].StopUsing();
				m_Clothes[i] = null;
			}
		}
		if (m_Toy != null)
		{
			m_Toy.StopUsing();
			m_Toy = null;
		}
		if (m_Medicine != null)
		{
			m_Medicine.StopUsing();
			m_Medicine = null;
		}
		if (m_Education != null)
		{
			m_Education.StopUsing();
			m_Education = null;
		}
		if (m_Art != null)
		{
			m_Art.transform.SetParent(null);
			m_Art.StopUsing();
			m_Art = null;
		}
		base.StopUsing(AndDestroy);
	}

	protected new void OnDestroy()
	{
		if ((bool)m_WuvParticles)
		{
			ParticlesManager.Instance.DestroyParticles(m_WuvParticles);
		}
		if ((bool)m_StatusIndicator)
		{
			Object.Destroy(m_StatusIndicator.gameObject);
		}
		if ((bool)FolkManager.Instance)
		{
			FolkManager.Instance.RemoveFolk(this);
		}
		base.OnDestroy();
	}

	public override string GetHumanReadableName()
	{
		return m_FolkName;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
		JSONUtils.Set(Node, "NM", m_FolkName);
		JSONUtils.Set(Node, "EN", m_Energy);
		JSONUtils.Set(Node, "SZ", m_Size);
		JSONUtils.Set(Node, "TI", m_Tier);
		JSONUtils.Set(Node, "FT", m_FoodTier);
		JSONUtils.Set(Node, "HT", m_HappyinessTimer);
		if ((bool)m_Housing)
		{
			JSONUtils.Set(Node, "HutID", m_Housing.m_UniqueID);
		}
		else
		{
			JSONUtils.Set(Node, "HutID", 0);
		}
		JSONArray jSONArray = (JSONArray)(Node["Clothes"] = new JSONArray());
		for (int i = 0; i < m_Clothes.Length; i++)
		{
			Clothing clothing = m_Clothes[i];
			if (clothing != null)
			{
				JSONNode jSONNode2 = new JSONObject();
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(clothing.m_TypeIdentifier);
				JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
				clothing.Save(jSONNode2);
				jSONArray[i] = jSONNode2;
			}
		}
		if ((bool)m_Toy)
		{
			JSONNode jSONNode3 = new JSONObject();
			string saveNameFromIdentifier2 = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Toy.m_TypeIdentifier);
			JSONUtils.Set(jSONNode3, "ID", saveNameFromIdentifier2);
			m_Toy.Save(jSONNode3);
			JSONUtils.Set(Node, "Toy", jSONNode3);
		}
		if ((bool)m_Medicine)
		{
			JSONNode jSONNode4 = new JSONObject();
			string saveNameFromIdentifier3 = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Medicine.m_TypeIdentifier);
			JSONUtils.Set(jSONNode4, "ID", saveNameFromIdentifier3);
			m_Medicine.Save(jSONNode4);
			JSONUtils.Set(Node, "Medicine", jSONNode4);
		}
		if ((bool)m_Education)
		{
			JSONNode jSONNode5 = new JSONObject();
			string saveNameFromIdentifier4 = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Education.m_TypeIdentifier);
			JSONUtils.Set(jSONNode5, "ID", saveNameFromIdentifier4);
			m_Education.Save(jSONNode5);
			JSONUtils.Set(Node, "Education", jSONNode5);
		}
		if ((bool)m_Art)
		{
			JSONNode jSONNode6 = new JSONObject();
			string saveNameFromIdentifier5 = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Art.m_TypeIdentifier);
			JSONUtils.Set(jSONNode6, "ID", saveNameFromIdentifier5);
			m_Art.Save(jSONNode6);
			JSONUtils.Set(Node, "Art", jSONNode6);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Folk", this);
		m_RefreshState = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		m_RefreshStateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		m_FolkName = JSONUtils.GetAsString(Node, "NM", "Name");
		m_Energy = JSONUtils.GetAsFloat(Node, "EN", 0f);
		m_Size = JSONUtils.GetAsInt(Node, "SZ", 0);
		m_RefreshHutUID = JSONUtils.GetAsInt(Node, "HutID", 0);
		m_Tier = JSONUtils.GetAsInt(Node, "TI", 0);
		m_FoodTier = JSONUtils.GetAsInt(Node, "FT", 0);
		m_HappyinessTimer = JSONUtils.GetAsFloat(Node, "HT", 0f);
		JSONArray asArray = Node["Clothes"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(baseClass.m_UniqueID, ErrorCheck: false);
				if (objectFromUniqueID != null && objectFromUniqueID.GetComponent<TileCoordObject>().m_Plot != null)
				{
					objectFromUniqueID.StopUsing();
				}
				baseClass.GetComponent<Savable>().Load(asObject);
				ApplyClothing(baseClass.GetComponent<Clothing>(), Jump: false);
			}
		}
		JSONNode asNode = JSONUtils.GetAsNode(Node, "Toy");
		if (asNode != null)
		{
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromSaveName(asNode, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass2)
			{
				baseClass2.GetComponent<Savable>().Load(asNode);
				ApplyToy(baseClass2.GetComponent<Toy>(), Jump: false);
			}
		}
		JSONNode asNode2 = JSONUtils.GetAsNode(Node, "Medicine");
		if (asNode2 != null)
		{
			BaseClass baseClass3 = ObjectTypeList.Instance.CreateObjectFromSaveName(asNode2, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass3)
			{
				baseClass3.GetComponent<Savable>().Load(asNode2);
				ApplyMedicine(baseClass3.GetComponent<Medicine>(), Jump: false);
			}
		}
		JSONNode asNode3 = JSONUtils.GetAsNode(Node, "Education");
		if (asNode3 != null)
		{
			BaseClass baseClass4 = ObjectTypeList.Instance.CreateObjectFromSaveName(asNode3, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass4)
			{
				baseClass4.GetComponent<Savable>().Load(asNode3);
				ApplyEducation(baseClass4.GetComponent<Education>(), Jump: false);
			}
		}
		JSONNode asNode4 = JSONUtils.GetAsNode(Node, "Art");
		if (asNode4 != null)
		{
			BaseClass baseClass5 = ObjectTypeList.Instance.CreateObjectFromSaveName(asNode4, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass5)
			{
				baseClass5.GetComponent<Savable>().Load(asNode4);
				ApplyArt(baseClass5.GetComponent<Art>(), Jump: false);
			}
		}
		UpdateHappiness();
		SetTier(m_Tier);
		Evolution.SetLastLevelSeen(m_Tier);
	}

	public override void UpdatePlotVisibility()
	{
		base.UpdatePlotVisibility();
		UpdateIndicatorVisibility();
	}

	private void UpdateIndicatorVisibility()
	{
		if (m_StatusIndicator != null)
		{
			if ((bool)m_Housing)
			{
				m_StatusIndicator.gameObject.SetActive(value: false);
			}
			else
			{
				m_StatusIndicator.gameObject.SetActive(value: true);
			}
		}
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if ((bool)Holder && !ObjectTypeList.Instance.GetIsBuilding(Holder.m_TypeIdentifier))
		{
			AudioManager.Instance.StartEvent("FolkPickedUp", this);
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.parent = MapManager.Instance.m_FolksRootTransform;
	}

	public override void SendAction(ActionInfo Info)
	{
		if (m_State == State.BadObject || m_State == State.Eating)
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.BeingHeld:
			if ((bool)m_Housing)
			{
				m_Housing.ReleaseFolk(this);
			}
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			if (m_RefreshHutUID != 0)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_RefreshHutUID, ErrorCheck: false);
				if (objectFromUniqueID != null)
				{
					objectFromUniqueID.GetComponent<Housing>().AddFolk(this);
				}
				else
				{
					Debug.Log("Folk.Refresh : Couldn't find object with UID " + m_RefreshHutUID);
				}
				SetRotation(2);
			}
			SetState(m_RefreshState);
			m_StateTimer = m_RefreshStateTimer;
			break;
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.BeingHeld:
			if (m_State == State.Eating || m_State == State.BadObject)
			{
				return false;
			}
			return true;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			return true;
		default:
			return base.CanDoAction(Info, RightNow);
		}
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		if (m_State == State.Eating || m_State == State.BadObject)
		{
			return ActionType.Total;
		}
		Info.m_FarmerState = Farmer.State.PickingUp;
		FindReason[] array = new FindReason[2]
		{
			FindReason.NeedHouse,
			FindReason.None
		};
		string[] array2 = new string[2] { "FNOFolkNeedsHouse", "FNOFolkNone" };
		for (int i = 0; i < array.Length; i++)
		{
			FindReason findReason = array[i];
			if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == array2[i])
			{
				Info.m_RequirementsOut = array2[i];
				bool flag = false;
				switch (findReason)
				{
				case FindReason.NeedHouse:
					flag = GetNeedsHouse();
					break;
				case FindReason.None:
					flag = true;
					break;
				}
				if (flag)
				{
					return ActionType.Pickup;
				}
				if (Info.m_RequirementsIn != "")
				{
					return ActionType.Fail;
				}
				Info.m_RequirementsOut = "";
			}
		}
		return ActionType.Total;
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType)
		{
			if (Info.m_Value == AFO.AT.AltPrimary.ToString())
			{
				if ((bool)m_Clothes[0])
				{
					return m_Clothes[0].m_TypeIdentifier;
				}
				if ((bool)m_Clothes[1])
				{
					return m_Clothes[1].m_TypeIdentifier;
				}
			}
			return ObjectTypeList.m_Total;
		}
		return base.GetActionInfo(Info);
	}

	public int GetObjectTier(AFO Info)
	{
		if (Food.GetIsTypeFood(Info))
		{
			if (m_Energy == 0f)
			{
				return -1;
			}
			return GetFoodTier();
		}
		if (Top.GetIsTypeTop(Info.m_ObjectType))
		{
			return GetClothingTier();
		}
		if (Toy.GetIsTypeToy(Info.m_ObjectType))
		{
			return GetToyTier();
		}
		if (Medicine.GetIsTypeMedicine(Info.m_ObjectType))
		{
			return GetMedicineTier();
		}
		if (Education.GetIsTypeEducation(Info.m_ObjectType))
		{
			return GetEducationTier();
		}
		if (Art.GetIsTypeArt(Info.m_ObjectType))
		{
			return GetArtTier();
		}
		return 100000000;
	}

	private void StartAddFood(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if (ToolFillable.GetIsTypeFillable(@object.m_TypeIdentifier))
		{
			ToolFillable component = @object.GetComponent<ToolFillable>();
			m_ObjectEaten = component.m_HeldType;
			m_ObjectEatenUsageCount = 0;
			component.Empty(1);
		}
		else
		{
			m_ObjectEaten = @object.m_TypeIdentifier;
			m_ObjectEatenUsageCount = @object.GetComponent<Holdable>().m_UsageCount;
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.FoodConsumed, @object.m_TypeIdentifier, m_TileCoord, @object.m_UniqueID, m_UniqueID);
		}
		if (m_State == State.Eating)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.FeedFolk, m_FeederBot, null, null);
		}
		m_FeederBot = Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker;
	}

	private void EndAddFood(AFO Info)
	{
		Actionable @object = Info.m_Object;
		AddObjectEnergy();
		if (!ToolFillable.GetIsTypeFillable(@object.m_TypeIdentifier))
		{
			@object.StopUsing();
		}
		AudioManager.Instance.StartEvent("FolkEating", this);
		SetState(State.Eating);
	}

	private void AbortAddFood(AFO Info)
	{
	}

	private ActionType GetActionFromFood(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddFood;
		Info.m_EndAction = EndAddFood;
		Info.m_AbortAction = AbortAddFood;
		if (m_BeingHeld && m_Housing == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			if (GetFood() == 1f)
			{
				return ActionType.Fail;
			}
		}
		else
		{
			if (GetFood() == 1f)
			{
				return ActionType.Fail;
			}
			ObjectType newType = ((!ToolFillable.GetIsTypeFillable(Info.m_Object.m_TypeIdentifier)) ? Info.m_Object.m_TypeIdentifier : Info.m_Object.GetComponent<ToolFillable>().m_HeldType);
			int tierFromType = BaseClass.GetTierFromType(newType);
			if (m_Energy != 0f && tierFromType < m_FoodTier)
			{
				return ActionType.Fail;
			}
			float foodEnergy = Food.GetFoodEnergy(newType);
			if (m_Energy != 0f && m_Energy + foodEnergy > m_MaxEnergy)
			{
				return ActionType.Fail;
			}
		}
		return ActionType.AddResource;
	}

	private void StartAddClothing(AFO Info)
	{
		Actionable @object = Info.m_Object;
		ApplyClothing(@object.GetComponent<Clothing>());
		m_Busy = true;
	}

	private void EndAddClothing(AFO Info)
	{
		Actionable @object = Info.m_Object;
		Clothing.Type typeFromObjectType = Clothing.GetTypeFromObjectType(@object.m_TypeIdentifier);
		@object.GetComponent<Clothing>().Wear(base.gameObject, m_ClothesParents[(int)typeFromObjectType].transform);
		QuestManager.Instance.AddEvent(QuestEvent.Type.ClotheFolk, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, null, this);
		AudioManager.Instance.StartEvent("FolkClothed", this);
		switch (typeFromObjectType)
		{
		case Clothing.Type.Top:
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingTopAdded, @object.m_TypeIdentifier, m_TileCoord, @object.m_UniqueID, m_UniqueID);
			break;
		case Clothing.Type.Hat:
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, @object.m_TypeIdentifier, m_TileCoord, @object.m_UniqueID, m_UniqueID);
			break;
		}
		m_Busy = false;
	}

	private void AbortAddClothing(AFO Info)
	{
		m_Busy = false;
		Clothing.Type typeFromObjectType = Clothing.GetTypeFromObjectType(Info.m_Object.m_TypeIdentifier);
		RemoveClothing(typeFromObjectType, Jump: false);
	}

	private ActionType GetActionFromClothing(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddClothing;
		Info.m_EndAction = EndAddClothing;
		Info.m_AbortAction = AbortAddClothing;
		if (m_BeingHeld && m_Housing == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && !GetNeedsClothing(Info.m_Object.GetComponent<Clothing>()))
		{
			return ActionType.Total;
		}
		return ActionType.AddResource;
	}

	private void StartAddToy(AFO Info)
	{
		Actionable @object = Info.m_Object;
		ApplyToy(@object.GetComponent<Toy>());
		m_Busy = true;
	}

	private void EndAddToy(AFO Info)
	{
		Info.m_Object.GetComponent<Toy>().Hold(base.transform);
		QuestManager.Instance.AddEvent(QuestEvent.Type.ToyFolk, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, null, this);
		AudioManager.Instance.StartEvent("FolkGivenToy", this);
		m_Busy = false;
	}

	private void AbortAddToy(AFO Info)
	{
		m_Busy = false;
		RemoveToy(Jump: false);
	}

	private ActionType GetActionFromToy(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddToy;
		Info.m_EndAction = EndAddToy;
		Info.m_AbortAction = AbortAddToy;
		if (m_BeingHeld && m_Housing == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && !GetNeedsToy(Info.m_Object.GetComponent<Toy>()))
		{
			return ActionType.Total;
		}
		return ActionType.AddResource;
	}

	private void StartAddMedicine(AFO Info)
	{
		Actionable @object = Info.m_Object;
		ApplyMedicine(@object.GetComponent<Medicine>());
		m_Busy = true;
	}

	private void EndAddMedicine(AFO Info)
	{
		Info.m_Object.GetComponent<Medicine>().Hold(m_MedicineRoot);
		QuestManager.Instance.AddEvent(QuestEvent.Type.MedicineFolk, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, null, this);
		AudioManager.Instance.StartEvent("FolkGivenToy", this);
		m_Busy = false;
	}

	private void AbortAddMedicine(AFO Info)
	{
		m_Busy = false;
		RemoveMedicine(Jump: false);
	}

	private ActionType GetActionFromMedicine(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddMedicine;
		Info.m_EndAction = EndAddMedicine;
		Info.m_AbortAction = AbortAddMedicine;
		if (m_BeingHeld && m_Housing == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && !GetNeedsMedicine(Info.m_Object.GetComponent<Medicine>()))
		{
			return ActionType.Total;
		}
		return ActionType.AddResource;
	}

	private void StartAddEducation(AFO Info)
	{
		Actionable @object = Info.m_Object;
		ApplyEducation(@object.GetComponent<Education>());
		m_Busy = true;
	}

	private void EndAddEducation(AFO Info)
	{
		Info.m_Object.GetComponent<Education>().Hold(m_EducationRoot);
		QuestManager.Instance.AddEvent(QuestEvent.Type.EducateFolk, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, null, this);
		AudioManager.Instance.StartEvent("FolkGivenToy", this);
		m_Busy = false;
	}

	private void AbortAddEducation(AFO Info)
	{
		m_Busy = false;
		RemoveEducation(Jump: false);
	}

	private ActionType GetActionFromEducation(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddEducation;
		Info.m_EndAction = EndAddEducation;
		Info.m_AbortAction = AbortAddEducation;
		if (m_BeingHeld && m_Housing == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && !GetNeedsEducation(Info.m_Object.GetComponent<Education>()))
		{
			return ActionType.Total;
		}
		return ActionType.AddResource;
	}

	private void StartAddArt(AFO Info)
	{
		Actionable @object = Info.m_Object;
		ApplyArt(@object.GetComponent<Art>());
		m_Busy = true;
	}

	private void EndAddArt(AFO Info)
	{
		Actionable @object = Info.m_Object;
		@object.GetComponent<Art>().Hold(m_ArtRoot);
		UpdateArt(@object.GetComponent<Art>());
		((SpawnAnimationJump)SpawnAnimationManager.Instance.GetAnimation(@object)).m_EndPosition = m_Art.transform.position;
		QuestManager.Instance.AddEvent(QuestEvent.Type.ArtFolk, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, null, this);
		AudioManager.Instance.StartEvent("FolkGivenToy", this);
		m_Busy = false;
	}

	private void AbortAddArt(AFO Info)
	{
		m_Busy = false;
		RemoveArt(Jump: false);
	}

	private ActionType GetActionFromArt(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddArt;
		Info.m_EndAction = EndAddArt;
		Info.m_AbortAction = AbortAddArt;
		if (m_BeingHeld && m_Housing == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && !GetNeedsArt(Info.m_Object.GetComponent<Art>()))
		{
			return ActionType.Total;
		}
		return ActionType.AddResource;
	}

	public void StartActionNothingAlt(AFO Info)
	{
		Holdable holdable = RemoveClothing(Clothing.Type.Hat, Jump: false);
		if (holdable == null)
		{
			holdable = RemoveClothing(Clothing.Type.Top, Jump: false);
		}
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.TryAddCarry(holdable);
	}

	private void EndAddNothingAlt(AFO Info)
	{
	}

	private void AbortAddNothingAlt(AFO Info)
	{
		Holdable lastObject = Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetLastObject();
		ApplyClothing(lastObject.GetComponent<Clothing>());
		Clothing.Type typeFromObjectType = Clothing.GetTypeFromObjectType(lastObject.m_TypeIdentifier);
		lastObject.GetComponent<Clothing>().Wear(base.gameObject, m_ClothesParents[(int)typeFromObjectType].transform);
	}

	private ActionType GetActionFromNothingAlt(AFO Info)
	{
		Info.m_StartAction = StartActionNothingAlt;
		Info.m_EndAction = EndAddNothingAlt;
		Info.m_AbortAction = AbortAddNothingAlt;
		Info.m_FarmerState = Farmer.State.Taking;
		if (!m_Clothes[0] && !m_Clothes[1])
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_ObjectType == ObjectTypeList.m_Total)
		{
			return GetActionFromNothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (Food.GetIsTypeFood(Info))
			{
				return GetActionFromFood(Info);
			}
			if (Clothing.GetIsTypeClothing(Info.m_ObjectType))
			{
				return GetActionFromClothing(Info);
			}
			if (Toy.GetIsTypeToy(Info.m_ObjectType))
			{
				return GetActionFromToy(Info);
			}
			if (Medicine.GetIsTypeMedicine(Info.m_ObjectType))
			{
				return GetActionFromMedicine(Info);
			}
			if (Education.GetIsTypeEducation(Info.m_ObjectType))
			{
				return GetActionFromEducation(Info);
			}
			if (Art.GetIsTypeArt(Info.m_ObjectType))
			{
				return GetActionFromArt(Info);
			}
		}
		if (Info.m_ActionType == AFO.AT.AltSecondary && Clothing.GetIsTypeClothing(Info.m_ObjectType))
		{
			return GetActionFromClothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary && Info.m_ObjectType == ObjectTypeList.m_Total && ((bool)m_Clothes[0] || (bool)m_Clothes[1]))
		{
			return GetActionFromNothingAlt(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void UpdateHappiness()
	{
		if (!m_Busy)
		{
			float happiness = m_Happiness;
			m_Happiness = 0f;
			if (m_Tier == GetHeartTier())
			{
				m_Happiness = 1f;
			}
			if (m_Happiness == 1f)
			{
				m_StatusIndicator.SetState(FolkStatusIndicator.State.None);
			}
			else
			{
				m_StatusIndicator.SetState(FolkStatusIndicator.State.Unhappy);
			}
			if (happiness != m_Happiness)
			{
				UpdateAnimation();
			}
			if ((m_Happiness == 1f || happiness == 1f) && m_Happiness != happiness)
			{
				FolkManager.Instance.StartUpdateHappy();
			}
		}
	}

	public void UpdateAnimation()
	{
		string text = "FolkNeutral";
		if (m_State == State.Eating)
		{
			text = "FolkEating";
		}
		else if (m_Happiness == 1f)
		{
			text = "FolkHappy";
		}
		if (text != m_LastModelName)
		{
			m_LastModelName = text;
			m_Animator.Play(text);
		}
	}

	public void SetFace(FaceType NewType)
	{
		string text = "FolkFace" + NewType;
		Texture value = (Texture)Resources.Load("Textures/WorldObjects/Folk/" + text, typeof(Texture));
		if ((bool)m_FaceMaterial)
		{
			m_FaceMaterial.SetTexture("_MainTex", value);
		}
	}

	public bool GetIsUnhappyState(UnhappyState State)
	{
		if (State == UnhappyState.Hungry && GetIsHungry())
		{
			return true;
		}
		if (State == UnhappyState.Housing && m_Housing == null)
		{
			return true;
		}
		if (State == UnhappyState.Clothing && m_Clothes[1] == null)
		{
			return true;
		}
		return false;
	}

	private void SetState(State NewState)
	{
		State state = m_State;
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Eating:
		{
			m_StatusIndicator.SetState(FolkStatusIndicator.State.None);
			float scale = 2f;
			if (Food.GetFoodEnergy(m_ObjectEaten) > 60f)
			{
				scale = 4f;
			}
			m_LoveHeart = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.LoveHeart, base.transform.position, Quaternion.identity).GetComponent<LoveHeart>();
			m_LoveHeart.SetScale(scale);
			m_LoveHeart.SetWorldPosition(base.transform.position);
			break;
		}
		case State.BadObject:
			m_StatusIndicator.SetState(FolkStatusIndicator.State.BadObject);
			m_PreviousState = state;
			break;
		}
		UpdateAnimation();
	}

	public void Feed()
	{
		AudioManager.Instance.StartEvent("FolkEating", this);
		SetState(State.Eating);
	}

	public void Housed(Housing NewHut)
	{
		m_Housing = NewHut;
		SetRotation(2);
		UpdateHappiness();
		UpdateIndicatorVisibility();
		QuestManager.Instance.AddEvent(QuestEvent.Type.UpdateHousedFolk, Bot: false, 0, this);
		int housingTier = GetHousingTier(IgnoreUsed: true);
		if (housingTier != m_Tier)
		{
			SetTier(housingTier);
			if (!SaveLoadManager.Instance.m_Loading)
			{
				SetState(State.LevelUp);
				AudioManager.Instance.StartEvent("FolkLevelUp", this);
				if (FolkManager.Instance.IsFirstFolk(m_Tier))
				{
					CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.FolkLevelUp, m_UniqueID);
					FolkManager.Instance.RegisterFirstFolk(m_Tier);
				}
				FolkManager.Instance.UpdateFolkTiers();
			}
		}
		else if (!SaveLoadManager.Instance.m_Loading)
		{
			AudioManager.Instance.StartEvent("FolkHappy", this);
			m_LoveHeart = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.LoveHeart, base.transform.position, Quaternion.identity).GetComponent<LoveHeart>();
			m_LoveHeart.SetScale(4f);
			m_LoveHeart.SetWorldPosition(base.transform.position);
		}
		UpdateSkin();
	}

	public void Homeless()
	{
		m_Housing = null;
		AudioManager.Instance.StartEvent("FolkSad", this);
		UpdateHappiness();
		UpdateIndicatorVisibility();
		QuestManager.Instance.AddEvent(QuestEvent.Type.UpdateHousedFolk, Bot: false, 0, this);
		if (m_Tier != 0)
		{
			SetTier(0);
			if (!SaveLoadManager.Instance.m_Loading)
			{
				SetState(State.LevelUp);
				AudioManager.Instance.StartEvent("FolkLevelUp", this);
				FolkManager.Instance.UpdateFolkTiers();
			}
		}
		UpdateSkin();
	}

	public void Show(bool Show)
	{
		m_ModelRoot.SetActive(Show);
	}

	private Holdable RemoveClothing(Clothing.Type NewType, bool Jump)
	{
		Clothing clothing = m_Clothes[(int)NewType];
		if (clothing != null)
		{
			switch (NewType)
			{
			case Clothing.Type.Top:
				ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingTopAdded, clothing.m_TypeIdentifier, m_TileCoord, clothing.m_UniqueID, m_UniqueID);
				break;
			case Clothing.Type.Hat:
				ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, clothing.m_TypeIdentifier, m_TileCoord, clothing.m_UniqueID, m_UniqueID);
				break;
			}
			m_Clothes[(int)NewType] = null;
			clothing.Remove();
			TileCoord tileCoord = m_TileCoord;
			if ((bool)m_Housing)
			{
				tileCoord = m_Housing.GetAccessPosition();
			}
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			clothing.SetHighlight(Highlighted: false);
			clothing.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				float yOffset = ObjectTypeList.Instance.GetYOffset(clothing.m_TypeIdentifier);
				SpawnAnimationManager.Instance.AddJump(clothing, tileCoord, randomEmptyTile, 0f, yOffset, 4f);
			}
			return clothing;
		}
		return null;
	}

	private void ApplyClothing(Clothing NewClothing, bool Jump = true)
	{
		Clothing.Type typeFromObjectType = Clothing.GetTypeFromObjectType(NewClothing.m_TypeIdentifier);
		RemoveClothing(typeFromObjectType, Jump);
		if ((bool)NewClothing)
		{
			m_Clothes[(int)typeFromObjectType] = NewClothing;
			NewClothing.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			if (Jump)
			{
				NewClothing.ReadyWear(base.gameObject, m_ClothesParents[(int)typeFromObjectType].transform);
			}
			else
			{
				NewClothing.Wear(base.gameObject, m_ClothesParents[(int)typeFromObjectType].transform);
			}
		}
	}

	private void RemoveToy(bool Jump)
	{
		Toy toy = m_Toy;
		if (toy != null)
		{
			m_Toy = null;
			TileCoord tileCoord = m_TileCoord;
			if ((bool)m_Housing)
			{
				tileCoord = m_Housing.GetAccessPosition();
			}
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			toy.SetHighlight(Highlighted: false);
			toy.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				SpawnAnimationManager.Instance.AddJump(toy, tileCoord, randomEmptyTile, 0f, 0f, 4f);
			}
			toy = null;
		}
	}

	private void ApplyToy(Toy NewToy, bool Jump = true)
	{
		RemoveToy(Jump);
		if ((bool)NewToy)
		{
			m_Toy = NewToy;
			NewToy.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			if (Jump)
			{
				NewToy.ReadyHold(base.transform);
			}
			else
			{
				NewToy.Hold(base.transform);
			}
		}
	}

	private void RemoveMedicine(bool Jump)
	{
		Medicine medicine = m_Medicine;
		if (medicine != null)
		{
			m_Medicine = null;
			TileCoord tileCoord = m_TileCoord;
			if ((bool)m_Housing)
			{
				tileCoord = m_Housing.GetAccessPosition();
			}
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			medicine.gameObject.SetActive(value: true);
			medicine.SetHighlight(Highlighted: false);
			medicine.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				SpawnAnimationManager.Instance.AddJump(medicine, tileCoord, randomEmptyTile, 0f, 0f, 4f);
			}
			medicine = null;
			UpdateSkin();
		}
	}

	private void ApplyMedicine(Medicine NewMedicine, bool Jump = true)
	{
		RemoveMedicine(Jump);
		if ((bool)NewMedicine)
		{
			m_Medicine = NewMedicine;
			NewMedicine.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			if (Jump)
			{
				NewMedicine.ReadyHold(m_ModelRoot.transform);
			}
			else
			{
				NewMedicine.Hold(m_ModelRoot.transform);
			}
			UpdateSkin();
		}
	}

	private void UpdateSkin()
	{
		string text = "Folk";
		if (m_Skin > 0)
		{
			text += m_Skin + 1;
		}
		if (GetIsMedicineRequirement() && m_Medicine == null)
		{
			text += "Sick";
		}
		RemoveOldObjects();
		LoadNewModel("Models/Folk/" + text);
		m_Animator.Rebind();
		GetModelNodes();
		float num = 1f;
		if (m_Education == null && GetIsEducationRequirement())
		{
			num = 0.8f;
		}
		m_HeadRoot.localScale = new Vector3(num, num, num);
	}

	private void RemoveEducation(bool Jump)
	{
		Education education = m_Education;
		if (education != null)
		{
			m_Education = null;
			TileCoord tileCoord = m_TileCoord;
			if ((bool)m_Housing)
			{
				tileCoord = m_Housing.GetAccessPosition();
			}
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			education.SetHighlight(Highlighted: false);
			education.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				SpawnAnimationManager.Instance.AddJump(education, tileCoord, randomEmptyTile, 0f, 0f, 4f);
			}
			education = null;
			UpdateSkin();
		}
	}

	private void ApplyEducation(Education NewEducation, bool Jump = true)
	{
		RemoveEducation(Jump);
		if ((bool)NewEducation)
		{
			m_Education = NewEducation;
			NewEducation.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			if (Jump)
			{
				NewEducation.ReadyHold(m_EducationRoot);
			}
			else
			{
				NewEducation.Hold(m_EducationRoot);
			}
			UpdateSkin();
		}
	}

	private void RemoveArt(bool Jump)
	{
		Art art = m_Art;
		if (art != null)
		{
			m_Art = null;
			TileCoord tileCoord = m_TileCoord;
			if ((bool)m_Housing)
			{
				tileCoord = m_Housing.GetAccessPosition();
			}
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			art.SetHighlight(Highlighted: false);
			art.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				SpawnAnimationManager.Instance.AddJump(art, tileCoord, randomEmptyTile, 0f, 0f, 4f);
			}
			art = null;
		}
	}

	private void ApplyArt(Art NewArt, bool Jump = true)
	{
		RemoveArt(Jump);
		if ((bool)NewArt)
		{
			m_Art = NewArt;
			NewArt.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			if (Jump)
			{
				NewArt.ReadyHold(m_ArtRoot);
				return;
			}
			NewArt.Hold(m_ArtRoot);
			UpdateArt(m_Art);
		}
	}

	private void UpdateArt(Art NewArt)
	{
		if ((bool)NewArt)
		{
			if ((bool)m_Housing && m_Housing.m_TypeIdentifier == ObjectType.Castle)
			{
				_ = NewArt.transform.parent;
				Transform parent = ObjectUtils.FindDeepChild(m_Housing.transform, "ArtPoint");
				NewArt.gameObject.SetActive(value: true);
				NewArt.transform.SetParent(parent);
				NewArt.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
				NewArt.transform.localScale = Vector3.one;
				NewArt.transform.localPosition = Vector3.zero;
			}
			else
			{
				NewArt.transform.localPosition = default(Vector3);
				NewArt.transform.localRotation = Quaternion.identity;
				NewArt.UpdateTierScale();
				NewArt.gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateStateUnOwned()
	{
	}

	private void UpdateEnergy()
	{
		if (GetHeartTier() != -1 && m_Energy > 0f)
		{
			float tierTimeDelta = GetTierTimeDelta();
			m_Energy -= tierTimeDelta;
			if (m_Energy <= 0f)
			{
				m_Energy = 0f;
				m_FoodTier = 0;
				QuestManager.Instance.AddEvent(QuestEvent.Type.UpdateFedFolk, Bot: false, 0, this);
			}
		}
		UpdateHappiness();
	}

	private void AddObjectEnergy()
	{
		int tierFromType = BaseClass.GetTierFromType(m_ObjectEaten);
		if (m_Energy == 0f || tierFromType > m_FoodTier)
		{
			m_FoodTier = tierFromType;
		}
		m_Energy += Food.GetFoodEnergy(m_ObjectEaten);
		float maxEnergy = m_MaxEnergy;
		if (m_Energy > maxEnergy)
		{
			m_Energy = maxEnergy;
		}
		UpdateHappiness();
		QuestManager.Instance.AddEvent(QuestEvent.Type.UpdateFedFolk, Bot: false, 0, this);
	}

	private ObjectType GetClayContainer(ObjectType BaseObject)
	{
		if (ObjectTypeList.Instance.GetInifinteRecursionFromType(BaseObject))
		{
			Mod modContainingItem = ModManager.Instance.GetModContainingItem(BaseObject);
			if (modContainingItem != null && modContainingItem.IsLocal)
			{
				string descriptionOverride = "Error: Recipe for '" + ModManager.Instance.m_ModStrings[BaseObject] + "' contains recursion within ingredients - cannot allow clay dish to be returned.";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			}
			return ObjectTypeList.m_Total;
		}
		IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(BaseObject);
		for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
		{
			if (ingredientsFromIdentifier[i].m_Type == ObjectType.PotClay || ingredientsFromIdentifier[i].m_Type == ObjectType.LargeBowlClay)
			{
				return ingredientsFromIdentifier[i].m_Type;
			}
		}
		for (int j = 0; j < ingredientsFromIdentifier.Length; j++)
		{
			ObjectType clayContainer = GetClayContainer(ingredientsFromIdentifier[j].m_Type);
			if (clayContainer == ObjectType.PotClay || clayContainer == ObjectType.LargeBowlClay)
			{
				return clayContainer;
			}
		}
		return ObjectTypeList.m_Total;
	}

	private void UpdateStateEating()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (!(m_StateTimer > m_EatingDelay))
		{
			return;
		}
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		AudioManager.Instance.StartEvent("FolkHappy", this);
		SetState(State.Normal);
		QuestManager.Instance.AddEvent(QuestEvent.Type.FeedFolk, m_FeederBot, null, null);
		ObjectType clayContainer = GetClayContainer(m_ObjectEaten);
		if (clayContainer == ObjectTypeList.m_Total)
		{
			return;
		}
		m_ObjectEatenUsageCount++;
		if (m_ObjectEatenUsageCount != VariableManager.Instance.GetVariableAsInt(clayContainer, "MaxUsage"))
		{
			TileCoord tileCoord = m_TileCoord;
			if ((bool)m_Housing)
			{
				tileCoord = m_Housing.GetAccessPosition();
			}
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(clayContainer, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, tileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
			baseClass.GetComponent<Holdable>().m_UsageCount = m_ObjectEatenUsageCount;
		}
	}

	private void UpdateStateBadObject()
	{
		if (m_StateTimer > m_BadObjectDelay)
		{
			SetState(m_PreviousState);
		}
	}

	private void UpdateStateLevelUp()
	{
		float tierScale = BaseClass.GetTierScale(m_Tier);
		if ((int)(m_StateTimer * 60f) % 8 < 4)
		{
			tierScale = BaseClass.GetTierScale(m_OldTier);
		}
		SetScale(tierScale);
		if (m_StateTimer > m_LevelUpDelay)
		{
			SetState(State.Normal);
			tierScale = BaseClass.GetTierScale(m_Tier);
			SetScale(tierScale);
		}
	}

	public bool GetIsHoused()
	{
		return m_Housing != null;
	}

	private bool GetIsHungry()
	{
		if (GetFood() < 0.01f && m_State != State.Eating && m_State != State.BadObject)
		{
			return true;
		}
		return false;
	}

	public bool GetIsHappy()
	{
		if (m_Tier == GetHeartTier() && GetFood() > 0f)
		{
			return true;
		}
		return false;
	}

	public float GetFood()
	{
		return m_Energy / m_MaxEnergy;
	}

	public float GetHousing()
	{
		if ((bool)m_Housing)
		{
			return 1f - m_Housing.GetUsed();
		}
		return 0f;
	}

	public float GetClothing()
	{
		if ((bool)m_Clothes[1])
		{
			return 1f - m_Clothes[1].GetUsed();
		}
		return 0f;
	}

	public float GetToy()
	{
		if ((bool)m_Toy)
		{
			return 1f - m_Toy.GetUsed();
		}
		return 0f;
	}

	public float GetMedicine()
	{
		if ((bool)m_Medicine)
		{
			return 1f - m_Medicine.GetUsed();
		}
		return 0f;
	}

	public float GetEducation()
	{
		if ((bool)m_Education)
		{
			return 1f - m_Education.GetUsed();
		}
		return 0f;
	}

	public float GetArt()
	{
		if ((bool)m_Art)
		{
			return 1f - m_Art.GetUsed();
		}
		return 0f;
	}

	public int GetFoodTier()
	{
		if (GetFood() == 0f)
		{
			return -1;
		}
		return m_FoodTier;
	}

	public int GetHousingTier(bool IgnoreUsed = false)
	{
		if (m_Housing == null)
		{
			return 0;
		}
		if (!IgnoreUsed && m_Housing.GetUsed() == 1f)
		{
			return 0;
		}
		return BaseClass.GetTierFromType(m_Housing.m_TypeIdentifier);
	}

	public int GetClothingTier()
	{
		if (m_Clothes[1] == null)
		{
			return 0;
		}
		if (m_Clothes[1].GetUsed() == 1f)
		{
			return 0;
		}
		return BaseClass.GetTierFromType(m_Clothes[1].m_TypeIdentifier);
	}

	public int GetToyTier()
	{
		if (m_Toy == null)
		{
			return 0;
		}
		if (m_Toy.GetUsed() == 1f)
		{
			return 0;
		}
		return BaseClass.GetTierFromType(m_Toy.m_TypeIdentifier);
	}

	public int GetMedicineTier()
	{
		if (m_Medicine == null)
		{
			return 0;
		}
		if (m_Medicine.GetUsed() == 1f)
		{
			return 0;
		}
		return BaseClass.GetTierFromType(m_Medicine.m_TypeIdentifier);
	}

	public int GetEducationTier()
	{
		if (m_Education == null)
		{
			return 0;
		}
		if (m_Education.GetUsed() == 1f)
		{
			return 0;
		}
		return BaseClass.GetTierFromType(m_Education.m_TypeIdentifier);
	}

	public int GetArtTier()
	{
		if (m_Art == null)
		{
			return 0;
		}
		if (m_Art.GetUsed() == 1f)
		{
			return 0;
		}
		return BaseClass.GetTierFromType(m_Art.m_TypeIdentifier);
	}

	public bool GetIsFoodRequirement()
	{
		return true;
	}

	public bool GetIsHousingRequirement()
	{
		return m_Tier >= m_FirstHousingTier;
	}

	public bool GetIsClothingRequirement()
	{
		return m_Tier >= m_FirstTopTier;
	}

	public bool GetIsToyRequirement()
	{
		return m_Tier >= m_FirstToyTier;
	}

	public bool GetIsMedicineRequirement()
	{
		return m_Tier >= m_FirstMedicineTier;
	}

	public bool GetIsEducationRequirement()
	{
		return m_Tier >= m_FirstEducationTier;
	}

	public bool GetIsArtRequirement()
	{
		return m_Tier >= m_FirstArtTier;
	}

	public int GetHeartTier()
	{
		int num = GetFoodTier();
		if (GetIsHousingRequirement())
		{
			int num2 = GetHousingTier();
			if (num2 == 0)
			{
				num2 = -1;
			}
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (GetIsClothingRequirement())
		{
			int num3 = GetClothingTier();
			if (num3 == 0)
			{
				num3 = -1;
			}
			if (num3 < num)
			{
				num = num3;
			}
		}
		if (GetIsToyRequirement())
		{
			int num4 = GetToyTier();
			if (num4 == 0)
			{
				num4 = -1;
			}
			if (num4 < num)
			{
				num = num4;
			}
		}
		if (GetIsMedicineRequirement())
		{
			int num5 = GetMedicineTier();
			if (num5 == 0)
			{
				num5 = -1;
			}
			if (num5 < num)
			{
				num = num5;
			}
		}
		if (GetIsEducationRequirement())
		{
			int num6 = GetEducationTier();
			if (num6 == 0)
			{
				num6 = -1;
			}
			if (num6 < num)
			{
				num = num6;
			}
		}
		if (GetIsArtRequirement())
		{
			int num7 = GetArtTier();
			if (num7 == 0)
			{
				num7 = -1;
			}
			if (num7 < num)
			{
				num = num7;
			}
		}
		if (num > m_Tier)
		{
			num = m_Tier;
		}
		return num;
	}

	public void SetTier(int NewTier)
	{
		m_OldTier = m_Tier;
		m_Tier = NewTier;
		m_Weight = VariableManager.Instance.GetVariableAsInt(ObjectType.Folk, "Weight" + m_Tier);
		float tierScale = BaseClass.GetTierScale(m_Tier);
		SetScale(tierScale);
		UpdateParticles();
	}

	public int GetTier()
	{
		return m_Tier;
	}

	private float GetTierTimeDelta()
	{
		float normalDelta = TimeManager.Instance.m_NormalDelta;
		int heartTier = GetHeartTier();
		normalDelta *= 1f / (float)(heartTier * 2 + 1);
		if (CheatManager.Instance.m_FastEat)
		{
			normalDelta *= 20f;
		}
		return normalDelta;
	}

	private void UseHouse(ObjectType NewType)
	{
		if (!(m_Housing == null))
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Usage");
			m_Housing.Use(variableAsInt);
		}
	}

	private void UseClothing(ObjectType NewType)
	{
		Clothing clothing = m_Clothes[1];
		if (!(clothing == null) && GetIsClothingRequirement())
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Usage");
			clothing.Use(variableAsInt);
			if (clothing.m_UsageCount == clothing.m_MaxUsageCount)
			{
				RemoveClothing(Clothing.Type.Top, Jump: false);
				clothing.StopUsing();
			}
		}
	}

	private void UseToy(ObjectType NewType)
	{
		if (!(m_Toy == null) && GetIsToyRequirement())
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Usage");
			m_Toy.Use(variableAsInt);
			if (m_Toy.m_UsageCount == m_Toy.m_MaxUsageCount)
			{
				Toy toy = m_Toy;
				RemoveToy(Jump: false);
				toy.StopUsing();
			}
		}
	}

	private void UseMedicine(ObjectType NewType)
	{
		if (!(m_Medicine == null) && GetIsMedicineRequirement())
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Usage");
			m_Medicine.Use(variableAsInt);
			if (m_Medicine.m_UsageCount == m_Medicine.m_MaxUsageCount)
			{
				Medicine medicine = m_Medicine;
				RemoveMedicine(Jump: false);
				medicine.StopUsing();
			}
		}
	}

	private void UseEducation(ObjectType NewType)
	{
		if (!(m_Education == null) && GetIsEducationRequirement())
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Usage");
			m_Education.Use(variableAsInt);
			if (m_Education.m_UsageCount == m_Education.m_MaxUsageCount)
			{
				Education education = m_Education;
				RemoveEducation(Jump: false);
				education.StopUsing();
			}
		}
	}

	private void UseArt(ObjectType NewType)
	{
		if (!(m_Art == null) && GetIsArtRequirement())
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Usage");
			m_Art.Use(variableAsInt);
			if (m_Art.m_UsageCount == m_Art.m_MaxUsageCount)
			{
				Art art = m_Art;
				RemoveArt(Jump: false);
				art.StopUsing();
			}
		}
	}

	private void CreateNewHeart()
	{
		TileCoord tileCoord = m_TileCoord;
		if ((bool)m_Housing)
		{
			tileCoord = m_Housing.GetAccessPosition();
		}
		TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(tileCoord);
		ObjectType objectTypeFromTier = FolkHeart.GetObjectTypeFromTier(GetHeartTier());
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(objectTypeFromTier, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
		SpawnAnimationManager.Instance.AddJump(baseClass, tileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
		QuestManager.Instance.AddEvent(QuestEvent.Type.MakeFolkHeart, Bot: false, 0, this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Bot: false, objectTypeFromTier, this);
		AudioManager.Instance.StartEvent("FolkCreateHeart", this);
		UseHouse(objectTypeFromTier);
		UseClothing(objectTypeFromTier);
		UseToy(objectTypeFromTier);
		UseMedicine(objectTypeFromTier);
		UseEducation(objectTypeFromTier);
		UseArt(objectTypeFromTier);
	}

	private void UpdateHeartGeneration()
	{
		if (GetHeartTier() != -1 && GetFood() > 0f)
		{
			float tierTimeDelta = GetTierTimeDelta();
			m_HappyinessTimer += tierTimeDelta;
			if (m_HappyinessTimer > m_HappinessDelay)
			{
				m_HappyinessTimer = 0f;
				CreateNewHeart();
			}
		}
	}

	private void SetScale(float NewScale)
	{
		base.transform.localScale = new Vector3(NewScale, NewScale, NewScale);
	}

	private void UpdateParticlesPosition()
	{
		m_WuvParticles.transform.position = base.transform.position + Vector3.Scale(new Vector3(0f, 2f, 0f), base.transform.localScale);
		m_WuvParticles.transform.rotation = Quaternion.Euler(-90f, 0f, 90f);
		m_WuvParticles.transform.localScale = base.transform.localScale;
	}

	private void UpdateParticles()
	{
		if (!m_WuvParticles)
		{
			return;
		}
		if (!m_WuvParticles.GetIsPlaying())
		{
			if (m_Happiness == 1f && m_Tier != 7)
			{
				UpdateParticlesPosition();
				m_WuvParticles.Play();
			}
		}
		else if (m_Happiness != 1f || m_Tier == 7)
		{
			m_WuvParticles.Stop();
		}
		else
		{
			UpdateParticlesPosition();
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Normal:
			if (!m_BeingHeld || m_Housing != null)
			{
				UpdateEnergy();
				UpdateHeartGeneration();
			}
			UpdateParticles();
			break;
		case State.Eating:
			UpdateStateEating();
			break;
		case State.BadObject:
			UpdateStateBadObject();
			break;
		case State.LevelUp:
			UpdateStateLevelUp();
			break;
		}
		if ((bool)TimeManager.Instance)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
		if (m_StatusIndicator.gameObject.activeSelf)
		{
			m_StatusIndicator.UpdateIndicator();
		}
		if ((bool)TimeManager.Instance && TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
	}

	public bool GetNeedsClothing(Clothing NewClothing)
	{
		if (NewClothing == null)
		{
			return false;
		}
		Clothing.Type typeFromObjectType = Clothing.GetTypeFromObjectType(NewClothing.m_TypeIdentifier);
		Clothing clothing = m_Clothes[(int)typeFromObjectType];
		if (clothing == null)
		{
			return true;
		}
		if (clothing.m_TypeIdentifier == NewClothing.m_TypeIdentifier)
		{
			return false;
		}
		if (Top.GetIsTypeTop(NewClothing.m_TypeIdentifier) && BaseClass.GetTierFromType(clothing.m_TypeIdentifier) >= BaseClass.GetTierFromType(NewClothing.m_TypeIdentifier))
		{
			return false;
		}
		return true;
	}

	public bool GetNeedsToy(Toy NewToy)
	{
		if (NewToy == null)
		{
			return false;
		}
		if (m_Toy == null)
		{
			return true;
		}
		if (Toy.GetIsTypeToy(NewToy.m_TypeIdentifier) && BaseClass.GetTierFromType(m_Toy.m_TypeIdentifier) >= BaseClass.GetTierFromType(NewToy.m_TypeIdentifier))
		{
			return false;
		}
		return true;
	}

	public bool GetNeedsMedicine(Medicine NewMedicine)
	{
		if (NewMedicine == null)
		{
			return false;
		}
		if (m_Medicine == null)
		{
			return true;
		}
		if (Medicine.GetIsTypeMedicine(NewMedicine.m_TypeIdentifier) && BaseClass.GetTierFromType(m_Medicine.m_TypeIdentifier) >= BaseClass.GetTierFromType(NewMedicine.m_TypeIdentifier))
		{
			return false;
		}
		return true;
	}

	public bool GetNeedsEducation(Education NewEducation)
	{
		if (NewEducation == null)
		{
			return false;
		}
		if (m_Education == null)
		{
			return true;
		}
		if (Education.GetIsTypeEducation(NewEducation.m_TypeIdentifier) && BaseClass.GetTierFromType(m_Education.m_TypeIdentifier) >= BaseClass.GetTierFromType(NewEducation.m_TypeIdentifier))
		{
			return false;
		}
		return true;
	}

	public bool GetNeedsArt(Art NewArt)
	{
		if (NewArt == null)
		{
			return false;
		}
		if (m_Art == null)
		{
			return true;
		}
		if (Art.GetIsTypeArt(NewArt.m_TypeIdentifier) && BaseClass.GetTierFromType(m_Art.m_TypeIdentifier) >= BaseClass.GetTierFromType(NewArt.m_TypeIdentifier))
		{
			return false;
		}
		return true;
	}

	public bool GetNeedsHouse()
	{
		return m_Housing == null;
	}

	public bool ModAddObject(Actionable ActionObject)
	{
		if (GetWillFolkAcceptObjectType(ActionObject.m_TypeIdentifier))
		{
			if (Food.GetIsTypeFood(ActionObject.m_TypeIdentifier) && ActionObject.GetComponent<Food>() != null)
			{
				if (ToolFillable.GetIsTypeFillable(ActionObject.m_TypeIdentifier))
				{
					ToolFillable component = ActionObject.GetComponent<ToolFillable>();
					m_ObjectEaten = component.m_HeldType;
					m_ObjectEatenUsageCount = 0;
					component.Empty(1);
				}
				else
				{
					m_ObjectEaten = ActionObject.m_TypeIdentifier;
					m_ObjectEatenUsageCount = ActionObject.GetComponent<Holdable>().m_UsageCount;
					ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.FoodConsumed, ActionObject.m_TypeIdentifier, m_TileCoord, ActionObject.m_UniqueID, m_UniqueID);
				}
				AddObjectEnergy();
				if (!ToolFillable.GetIsTypeFillable(ActionObject.m_TypeIdentifier))
				{
					ActionObject.StopUsing();
				}
				AudioManager.Instance.StartEvent("FolkEating", this);
				SetState(State.Eating);
				return true;
			}
			if (Clothing.GetIsTypeClothing(ActionObject.m_TypeIdentifier))
			{
				Clothing component2 = ActionObject.GetComponent<Clothing>();
				if (component2 != null && GetNeedsClothing(component2))
				{
					ApplyClothing(component2, Jump: false);
					return true;
				}
			}
			if (Toy.GetIsTypeToy(ActionObject.m_TypeIdentifier))
			{
				Toy component3 = ActionObject.GetComponent<Toy>();
				if (component3 != null && GetNeedsToy(component3))
				{
					ApplyToy(component3);
					return true;
				}
			}
			if (Medicine.GetIsTypeMedicine(ActionObject.m_TypeIdentifier))
			{
				Medicine component4 = ActionObject.GetComponent<Medicine>();
				if (component4 != null && GetNeedsMedicine(component4))
				{
					ApplyMedicine(component4);
					return true;
				}
			}
			if (Education.GetIsTypeEducation(ActionObject.m_TypeIdentifier))
			{
				Education component5 = ActionObject.GetComponent<Education>();
				if (component5 != null && GetNeedsEducation(component5))
				{
					ApplyEducation(component5);
					return true;
				}
			}
			if (Art.GetIsTypeArt(ActionObject.m_TypeIdentifier))
			{
				Art component6 = ActionObject.GetComponent<Art>();
				if (component6 != null && GetNeedsArt(component6))
				{
					ApplyArt(component6, Jump: false);
					return true;
				}
			}
		}
		return false;
	}
}
