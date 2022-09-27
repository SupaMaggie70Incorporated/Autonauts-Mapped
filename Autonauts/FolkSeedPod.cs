using System.Collections.Generic;
using UnityEngine;

public class FolkSeedPod : Converter
{
	private MyLight m_Light;

	private PlaySound m_PlaySound;

	private int m_LightColour;

	private bool m_Locked;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		SetResultToCreate(1);
		UpdateLocked();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		Transform transform = m_ModelRoot.transform.Find("FolkSeedPodTop");
		m_Light = LightManager.Instance.LoadLight("FolkSeedPodLight", transform.Find("FolkSeedPodLight"), new Vector3(0f, 0f, 1.5f));
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_Light);
		base.OnDestroy();
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.GetObjectType:
			return ObjectType.FolkSeed;
		case GetAction.IsDuplicatable:
			return false;
		case GetAction.IsDeletable:
		case GetAction.IsMovable:
			if (CeremonyManager.Instance.IsCeremonyInCeremonyQueue(CeremonyManager.CeremonyType.FolkSeedUnlocked))
			{
				return false;
			}
			break;
		}
		return base.GetActionInfo(Info);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingFolkSeedPodMaking", this, Remember: true);
	}

	public override void DoConvertAnimAction()
	{
		base.DoConvertAnimAction();
		int num = m_LightColour % 3;
		Color colour = new Color(1f, 0f, 0f, 1f);
		switch (num)
		{
		case 1:
			colour = new Color(0f, 1f, 0f, 1f);
			break;
		case 2:
			colour = new Color(0f, 0f, 1f, 1f);
			break;
		}
		m_LightColour++;
		m_Light.SetColour(colour);
	}

	protected override void EndConverting()
	{
		base.EndConverting();
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingMakingComplete", this);
		m_Light.SetColour(new Color(1f, 0.9f, 0f, 1f));
	}

	private void RemoveExcessFolkSeeds()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("FolkSeed");
		int num = 20;
		int num2 = 0;
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				if (!item.Key.GetComponent<FolkSeed>().m_BeingHeld && !SpawnAnimationManager.Instance.GetIsObjectSpawning(item.Key))
				{
					num2++;
				}
			}
		}
		int num3 = num2 - num;
		if (num3 <= 0)
		{
			return;
		}
		int num4 = 0;
		List<BaseClass> list = new List<BaseClass>();
		foreach (KeyValuePair<BaseClass, int> item2 in collection)
		{
			list.Add(item2.Key);
			num4++;
			if (num4 == num3)
			{
				break;
			}
		}
		foreach (BaseClass item3 in list)
		{
			item3.StopUsing();
		}
	}

	private void EndTakeNothing(AFO Info)
	{
		RemoveExcessFolkSeeds();
		StartConversion(Info.m_Actioner);
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_EndAction = EndTakeNothing;
		Info.m_FarmerState = Farmer.State.Taking;
		if (IsBusy())
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	private void StartAddFolkSeed(AFO Info)
	{
		Info.m_Object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
	}

	private void EndAddFolkSeed(AFO Info)
	{
		Info.m_Object.StopUsing();
	}

	private ActionType GetActionFromFolkSeed(AFO Info)
	{
		Info.m_StartAction = StartAddFolkSeed;
		Info.m_EndAction = EndAddFolkSeed;
		Info.m_FarmerState = Farmer.State.Adding;
		if (IsBusy())
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (m_Locked)
		{
			return ActionType.Total;
		}
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return GetActionFromNothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary && Info.m_ObjectType == ObjectType.FolkSeed)
		{
			return GetActionFromFolkSeed(Info);
		}
		return base.GetActionFromObject(Info);
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		ShowTop(Show: false);
		UpdateLightActive();
	}

	public void ShowTop(bool Show)
	{
		m_ModelRoot.transform.Find("FolkSeedPodTop").gameObject.SetActive(Show);
		m_Animator.Rebind();
	}

	private void UpdateLightActive()
	{
		if (m_Blueprint || m_Locked)
		{
			m_Light.SetActive(Active: false);
		}
		else
		{
			m_Light.SetActive(Active: true);
		}
	}

	public void UpdateLocked()
	{
		m_Locked = false;
		if ((bool)QuestManager.Instance && QuestManager.Instance.GetIsLastLevelActive())
		{
			m_Locked = true;
		}
		UpdateLightActive();
	}
}
