using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class MyTree : Flora
{
	public enum State
	{
		Growing,
		GrowingFruit,
		Waiting,
		FallWait,
		Falling,
		WaitingEmpty,
		Total
	}

	public static List<ObjectType> m_TreeTypes;

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	[HideInInspector]
	public float m_WobbleTimer;

	private float m_WobbleDelay;

	private GameObject m_Trunk;

	protected Wobbler m_GrownWobble;

	private bool m_Slow;

	private float m_GrowDelay;

	private float m_PreviousGrowPercent;

	[HideInInspector]
	public BeesNest m_BeesNest;

	public static bool GetIsTree(ObjectType NewType)
	{
		if (NewType == ObjectType.TreePine || NewType == ObjectType.TreeApple || NewType == ObjectType.TreeCoconut || NewType == ObjectType.TreeMulberry)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Tree", m_TypeIdentifier);
		m_TreeTypes = new List<ObjectType>();
		m_TreeTypes.Add(ObjectType.TreePine);
		m_TreeTypes.Add(ObjectType.TreeApple);
		m_TreeTypes.Add(ObjectType.TreeCoconut);
		m_TreeTypes.Add(ObjectType.TreeMulberry);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TreePine);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TreeApple);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TreeCoconut);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TreeMulberry);
		ModelManager.Instance.AddModel("Models/Crops/TreePine2", ObjectType.TreePine, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Crops/TreePine3", ObjectType.TreePine, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Crops/TreePineGrowing", ObjectType.TreePine);
		ModelManager.Instance.AddModel("Models/Crops/TreeAppleGrowing", ObjectType.TreeApple);
		ModelManager.Instance.AddModel("Models/Crops/TreePalmGrowing", ObjectType.TreeCoconut);
		ModelManager.Instance.AddModel("Models/Crops/TreeMulberryGrowing", ObjectType.TreeMulberry);
		ModelManager.Instance.AddModel("Models/Crops/TreePineCultivated", ObjectType.TreePine);
		ModelManager.Instance.AddModel("Models/Crops/TreeAppleCultivated", ObjectType.TreeApple);
		ModelManager.Instance.AddModel("Models/Crops/TreePalmCultivated", ObjectType.TreeCoconut);
		ModelManager.Instance.AddModel("Models/Crops/TreeMulberryCultivated", ObjectType.TreeMulberry);
	}

	public override void Restart()
	{
		m_State = State.Waiting;
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Tree", this);
		}
		if ((bool)m_ModelRoot.transform.Find("Trunk"))
		{
			m_Trunk = m_ModelRoot.transform.Find("Trunk").gameObject;
		}
		else
		{
			m_Trunk = m_ModelRoot;
		}
		m_GrowDelay = 50f;
		m_PreviousGrowPercent = 0f;
		m_Slow = false;
		m_WobbleTimer = 0f;
		m_WobbleDelay = 0.075f;
		m_BeesNest = null;
		m_GrownWobble.Restart();
		m_State = State.Total;
		SetState(State.Waiting);
		UpdateScale();
		UpdateModel();
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.Growing)
		{
			text = text + " (" + TextManager.Instance.Get("Growing") + ")";
		}
		return text;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
		if (m_Slow)
		{
			JSONUtils.Set(Node, "SL", 1);
		}
		else
		{
			JSONUtils.Set(Node, "SL", 0);
		}
		if ((bool)m_BeesNest)
		{
			JSONNode jSONNode = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_BeesNest.m_TypeIdentifier);
			JSONUtils.Set(jSONNode, "ID", saveNameFromIdentifier);
			m_BeesNest.Save(jSONNode);
			JSONUtils.Set(Node, "BeesNest", jSONNode);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Tree", this);
		State asInt = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		SetState(asInt);
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		if (m_State == State.Growing && JSONUtils.GetAsInt(Node, "SL", 0) == 1)
		{
			SetSlow();
		}
		JSONNode asNode = JSONUtils.GetAsNode(Node, "BeesNest");
		if (asNode != null)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asNode, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asNode);
				AddBeesNest(baseClass.GetComponent<BeesNest>());
			}
		}
	}

	protected new void Awake()
	{
		base.Awake();
		m_GrownWobble = new Wobbler();
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Bump)
		{
			m_WobbleTimer = 0.25f;
			Wake();
		}
		base.SendAction(Info);
	}

	private void UseAxe(AFO Info)
	{
		m_WobbleTimer = 0.25f;
		Wake();
		Vector3 vector = base.transform.position + new Vector3(0f, 1f, 0f);
		Vector3 vector2 = Info.m_Object.transform.position - vector;
		float y = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("ChopChips", vector, Quaternion.Euler(0f, y, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
		vector = base.transform.position + new Vector3(0f, 5f, 0f);
		newParticles = ParticlesManager.Instance.CreateParticles("ChopLeaves", vector, Quaternion.Euler(90f, 0f, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
	}

	private void EndAxe(AFO Info)
	{
		TileCoord tileCoord = TileHelpers.GetRandomEmptyTile(m_TileCoord) - m_TileCoord;
		m_Heading = Mathf.Atan2(tileCoord.y, tileCoord.x) * 57.29578f + 180f;
		base.transform.localRotation = Quaternion.Euler(0f, m_Heading, 0f);
		SetState(State.FallWait);
	}

	private void AbortAxe(AFO Info)
	{
		UpdateChops(0f);
	}

	private ActionType GetActionFromAxe(AFO Info)
	{
		Info.m_UseAction = UseAxe;
		Info.m_EndAction = EndAxe;
		Info.m_AbortAction = AbortAxe;
		Info.m_FarmerState = Farmer.State.Chopping;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Waiting || m_State == State.WaitingEmpty || m_State == State.GrowingFruit)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	protected void UseMallet(AFO Info)
	{
		m_WobbleTimer = 0.25f;
		Wake();
		Vector3 localPosition = base.transform.position + new Vector3(0f, 5f, 0f);
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("ChopLeaves", localPosition, Quaternion.Euler(90f, 0f, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
	}

	protected void EndMallet(AFO Info)
	{
		CreateHammeredGoodies(m_TileCoord);
	}

	protected virtual ActionType GetActionFromMallet(AFO Info)
	{
		Info.m_UseAction = UseMallet;
		Info.m_EndAction = EndMallet;
		Info.m_FarmerState = Farmer.State.Hammering;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Waiting || m_State == State.WaitingEmpty)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_Actioner.m_TypeIdentifier == ObjectType.AnimalBird)
		{
			if (m_State == State.Waiting)
			{
				return ActionType.AddResource;
			}
			return ActionType.Total;
		}
		if (FarmerStateChopping.GetIsToolAcceptable(objectType) || objectType == ObjectType.Rock)
		{
			return GetActionFromAxe(Info);
		}
		if (FarmerStateHammering.GetIsToolAcceptable(objectType))
		{
			return GetActionFromMallet(Info);
		}
		return base.GetActionFromObject(Info);
	}

	public void SetSlow()
	{
		m_Slow = true;
		SetScale(0f);
	}

	protected virtual void UpdateModel()
	{
		string text = ObjectTypeList.Instance.GetModelNameFromIdentifier(m_TypeIdentifier);
		bool randomVariants = true;
		if (m_State == State.Growing)
		{
			text += "Growing";
			randomVariants = false;
		}
		else if (!m_WorldCreated)
		{
			text += "Cultivated";
			randomVariants = false;
		}
		LoadNewModel(text, randomVariants);
		UpdatePlotVisibility();
		if ((bool)m_ModelRoot.transform.Find("Trunk"))
		{
			m_Trunk = m_ModelRoot.transform.Find("Trunk").gameObject;
		}
		else
		{
			m_Trunk = m_ModelRoot;
		}
	}

	protected void SetState(State NewState)
	{
		if (m_State != NewState)
		{
			State state = m_State;
			m_State = NewState;
			m_StateTimer = 0f;
			if (NewState != State.Waiting)
			{
				Wake();
			}
			if (NewState == State.Growing || state == State.Growing || NewState == State.GrowingFruit || state == State.GrowingFruit)
			{
				UpdateModel();
			}
		}
	}

	public void SetGrowing()
	{
		SetState(State.Growing);
		UpdateStateGrowing();
		Wake();
		SetScale(0f);
	}

	public virtual void UpdateChops(float Percent)
	{
	}

	public void AddBeesNest(BeesNest NewNest)
	{
		UpdateScale();
		m_BeesNest = NewNest;
		m_BeesNest.SendAction(new ActionInfo(ActionType.BeingHeld, m_TileCoord, this));
		m_BeesNest.transform.SetParent(base.transform);
		Vector3 position = m_ModelRoot.transform.TransformPoint(new Vector3(-0.49f, 4.79f, -1.41f));
		m_BeesNest.transform.position = position;
		m_BeesNest.CreatedInTree(this);
	}

	public BeesNest DetachBeesNest()
	{
		Vector3 position = m_BeesNest.transform.position;
		m_BeesNest.transform.SetParent(base.transform);
		m_BeesNest.SendAction(new ActionInfo(ActionType.Dropped, m_TileCoord, this));
		m_BeesNest.transform.position = position;
		BeesNest beesNest = m_BeesNest;
		m_BeesNest = null;
		return beesNest;
	}

	protected override void UpdateScale()
	{
		float num = m_StartingScale * m_Scale;
		m_ModelRoot.transform.localScale = new Vector3(num, (m_GrownWobble.m_Height + 1f) * num, num);
	}

	private void UpdateStateWaiting()
	{
		m_GrownWobble.Update();
		UpdateScale();
		UpdateWobble();
		if (m_GrownWobble.m_Height == 0f && m_WobbleTimer == 0f)
		{
			Sleep();
		}
	}

	protected void UpdateWobble()
	{
		if (m_WobbleTimer > 0f)
		{
			m_WobbleTimer -= TimeManager.Instance.m_NormalDelta;
			float x = Mathf.Sin(m_WobbleTimer / (m_WobbleDelay / 1f) * (float)Math.PI * 2f) * 10f;
			if (m_WobbleTimer <= 0f)
			{
				m_WobbleTimer = 0f;
				x = 0f;
			}
			base.transform.rotation = Quaternion.Euler(x, m_Heading, 0f);
		}
	}

	private void CreateBeesNest()
	{
		if ((bool)m_BeesNest)
		{
			BeesNest beesNest = DetachBeesNest();
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			SpawnAnimationManager.Instance.AddJump(beesNest, m_TileCoord, randomEmptyTile, beesNest.transform.position.y, 0f, 4f);
		}
	}

	protected virtual void CreateChoppedGoodies(TileCoord StartPosition)
	{
		CreateBeesNest();
	}

	protected virtual void CreateHammeredGoodies(TileCoord StartPosition)
	{
		CreateBeesNest();
	}

	private void UpdateStateFalling()
	{
		float num = m_StateTimer / 0.1f;
		if (num >= 1f)
		{
			num = 1f;
			AudioManager.Instance.StartEvent("CropTreeFallen", this);
			StopUsing();
			Vector3 worldPosition = m_Trunk.transform.TransformPoint(new Vector3(0f, 0f, 0f));
			TileCoord tileCoord = new TileCoord(worldPosition);
			Tile tile = TileManager.Instance.GetTile(tileCoord);
			if (tile == null || tile.m_Building != null || tile.m_BuildingFootprint != null || TileManager.Instance.GetTileSolidToPlayer(tileCoord))
			{
				tileCoord = m_TileCoord;
			}
			ObjectType identifier = ObjectType.Log;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(identifier, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
			baseClass.transform.localRotation = Quaternion.Euler(0f, m_Heading, 0f);
			TileCoord tileCoord2 = baseClass.GetComponent<TileCoordObject>().m_TileCoord;
			CreateChoppedGoodies(tileCoord2);
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.TreeCut);
			if (!SaveLoadManager.Instance.m_Loading)
			{
				ModManager.Instance.CheckItemTypeSpawnedCallback(baseClass.m_UniqueID, baseClass.m_TypeIdentifier, baseClass.GetComponent<TileCoordObject>().m_TileCoord);
			}
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(0f, m_Heading, 90f * num);
		}
	}

	private void NewScale(float Scale)
	{
		SetScale(Scale);
		m_GrownWobble.Go(0.5f, 5f, 0.25f);
		AudioManager.Instance.StartEvent("CropGrowing", this);
	}

	protected virtual void GrowingFinished()
	{
		SetState(State.Waiting);
	}

	private void UpdateStateGrowing()
	{
		m_GrownWobble.Update();
		UpdateScale();
		float num = m_StateTimer / m_GrowDelay;
		if (num >= 1f)
		{
			m_GrownWobble.Go(0.5f, 5f, 0.25f);
			GrowingFinished();
			AudioManager.Instance.StartEvent("CropGrown", this);
			if (!m_WorldCreated)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.GrowTree, Bot: false, 0, this);
			}
		}
		else if (num >= 0.75f && m_PreviousGrowPercent < 0.75f)
		{
			NewScale(0.8f);
		}
		else if (num >= 0.5f && m_PreviousGrowPercent < 0.5f)
		{
			NewScale(0.6f);
		}
		else if (num >= 0.25f && m_PreviousGrowPercent < 0.25f)
		{
			NewScale(0.3f);
		}
		else if (num >= 0.05f && m_PreviousGrowPercent < 0.05f)
		{
			NewScale(0.1f);
		}
		m_PreviousGrowPercent = num;
	}

	public virtual void UpdateStateGrowingFruit()
	{
		m_GrownWobble.Update();
		UpdateScale();
		UpdateWobble();
		float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "GrowFruitDelay", CheckValid: false);
		if (m_StateTimer > variableAsFloat)
		{
			m_GrownWobble.Go(0.5f, 5f, 0.25f);
			SetState(State.Waiting);
			AudioManager.Instance.StartEvent("CropGrown", this);
		}
	}

	public override void UpdatePlotVisibility()
	{
		base.UpdatePlotVisibility();
		if ((bool)m_BeesNest)
		{
			m_BeesNest.UpdatePlotVisibility();
		}
	}

	protected void Update()
	{
		switch (m_State)
		{
		case State.Waiting:
			UpdateStateWaiting();
			break;
		case State.Growing:
			UpdateStateGrowing();
			break;
		case State.GrowingFruit:
			UpdateStateGrowingFruit();
			break;
		case State.FallWait:
			if (m_StateTimer > 0.25f)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeStump, base.transform.position, Quaternion.identity).transform.localRotation = Quaternion.Euler(0f, m_Heading, 0f);
				TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
				if (randomEmptyTile != m_TileCoord)
				{
					TileCoord tileCoord = randomEmptyTile - m_TileCoord;
					m_Heading = Mathf.Atan2(tileCoord.y, tileCoord.x) * 57.29578f + 180f;
				}
				SetState(State.Falling);
			}
			break;
		case State.Falling:
			UpdateStateFalling();
			break;
		}
		if (m_State == State.Growing && m_Slow)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta * 0.3333f;
		}
		else
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
	}

	protected override void UpdateWorldCreated()
	{
		UpdateModel();
		base.UpdateWorldCreated();
		if ((bool)m_BeesNest)
		{
			Vector3 position = m_ModelRoot.transform.TransformPoint(new Vector3(-0.49f, 4.79f, -1.41f));
			m_BeesNest.transform.position = position;
		}
	}
}
