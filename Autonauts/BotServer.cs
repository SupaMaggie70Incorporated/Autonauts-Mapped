using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class BotServer : Wonder
{
	public static BotServer m_FirstBotServer;

	private PlaySound m_PlaySound;

	private Transform m_ScreenTrans;

	private Material m_ScreenMaterial1;

	private Material m_ScreenMaterial2;

	private float m_ScreenTimer;

	private int m_ScreenIndex;

	private Transform m_Tape1;

	private Transform m_Tape2;

	private float m_TapeTimer;

	private int m_TapeState;

	public static void UpdateFirstServer()
	{
		m_FirstBotServer = null;
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("BotServer");
		if (collection == null || collection.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			BotServer component = item.Key.GetComponent<BotServer>();
			if ((bool)component && !component.m_FloorMissing && !component.m_WallMissing && !component.m_Blueprint)
			{
				m_FirstBotServer = component;
				break;
			}
		}
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("BotServer", this);
		}
		SetDimensions(new TileCoord(-1, -2), new TileCoord(1, 0), new TileCoord(0, 1));
		ChangeAccessPointToIn();
		m_PlaySound = AudioManager.Instance.StartEvent("BotServerIdle", this, Remember: true);
		UpdateFirstServer();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		base.StopUsing(AndDestroy);
		UpdateFirstServer();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_ScreenTrans = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "ScreenDatabase");
		if ((bool)m_ScreenTrans)
		{
			Material[] materials = m_ScreenTrans.GetComponent<MeshRenderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i].name.Contains("ScreenDatabase01"))
				{
					m_ScreenMaterial1 = materials[i];
				}
				if (materials[i].name.Contains("ScreenDatabase02"))
				{
					m_ScreenMaterial2 = materials[i];
				}
			}
		}
		m_Tape1 = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Tape.001");
		m_Tape2 = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Tape.002");
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("BotServer", this);
		UpdateFirstServer();
	}

	public void SetWorking(bool Working)
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		string eventName = "BotServerIdle";
		if (Working)
		{
			eventName = "BotServerWorking";
		}
		m_PlaySound = AudioManager.Instance.StartEvent(eventName, this, Remember: true);
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			GameStateManager.Instance.StartSelectBuilding(this);
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		case ActionType.RefreshFirst:
			CollectionManager.Instance.AddCollectable("BotServer", this);
			break;
		}
		base.SendAction(Info);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			Info.m_StartAction = null;
			Info.m_AbortAction = null;
			Info.m_FarmerState = Farmer.State.Total;
			if ((bool)TeachWorkerScriptEdit.Instance && TeachWorkerScriptEdit.Instance.m_State == TeachWorkerScriptEdit.State.Recording)
			{
				return ActionType.Fail;
			}
			if (!GetIsUsable())
			{
				return ActionType.Fail;
			}
			return ActionType.EngageObject;
		}
		return base.GetActionFromObject(Info);
	}

	private bool GetIsUsable()
	{
		if (m_FloorMissing || m_WallMissing)
		{
			return false;
		}
		return true;
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		if ((bool)m_ScreenTrans)
		{
			m_ScreenTrans.gameObject.SetActive(!Blueprint);
		}
		if (Blueprint)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
		}
		else
		{
			SetWorking(Working: false);
		}
	}

	private void UpdateScreen()
	{
		if (!(m_ScreenMaterial1 == null) && !(m_ScreenMaterial2 == null))
		{
			m_ScreenTimer += TimeManager.Instance.m_NormalDelta;
			float num = 0.4f;
			if (m_Engager != null)
			{
				num = 0.115f;
			}
			if (m_ScreenTimer > num)
			{
				m_ScreenTimer = 0f;
				int num2 = Random.Range(0, 3);
				string text = "ScreenDatabase0" + (num2 + 1);
				Texture2D value = (Texture2D)Resources.Load("Textures/WorldObjects/" + text, typeof(Texture2D));
				m_ScreenMaterial1.SetTexture("_MainTex", value);
				m_ScreenMaterial1.SetTexture("_EmissionMap", value);
				num2 = Random.Range(0, 3);
				text = "ScreenDatabase0" + (num2 + 1);
				value = (Texture2D)Resources.Load("Textures/WorldObjects/" + text, typeof(Texture2D));
				m_ScreenMaterial2.SetTexture("_MainTex", value);
				m_ScreenMaterial2.SetTexture("_EmissionMap", value);
			}
		}
	}

	private void UpdateTapes()
	{
		if (m_Tape1 == null)
		{
			return;
		}
		m_TapeTimer += TimeManager.Instance.m_NormalDelta;
		float num = 0.5f;
		if (m_TapeState == 0 || m_TapeState == 2)
		{
			num = 1f;
		}
		if (m_TapeTimer > num)
		{
			m_TapeTimer = 0f;
			m_TapeState++;
			if (m_TapeState >= 4)
			{
				m_TapeState = 0;
			}
		}
		float num2 = 0f;
		if (m_TapeState == 0)
		{
			num2 = 1f;
		}
		else if (m_TapeState == 2)
		{
			num2 = -2f;
		}
		num2 = num2 * TimeManager.Instance.m_NormalDelta * 360f;
		m_Tape1.localRotation = Quaternion.Euler(0f, num2, 0f) * m_Tape1.localRotation;
		m_Tape2.localRotation = Quaternion.Euler(0f, num2, 0f) * m_Tape2.localRotation;
	}

	public override void CheckWallsFloors()
	{
		base.CheckWallsFloors();
		if (!m_WallMissing && !m_FloorMissing && !m_Blueprint && GetIsSavable() && m_WallFloorIcon != null && (bool)QuestManager.Instance)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BotServerComplete, Bot: false, null, this);
		}
		UpdateFirstServer();
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			if (GetIsUsable())
			{
				UpdateScreen();
				UpdateTapes();
				AudioManager.Instance.SetEventVolume(m_PlaySound, 1f);
			}
			else
			{
				AudioManager.Instance.SetEventVolume(m_PlaySound, 0f);
			}
		}
	}
}
