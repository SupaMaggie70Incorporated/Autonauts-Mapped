using UnityEngine;

public class WorkerAssembler : Converter
{
	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(-1, 0));
		SetSpawnPoint(new TileCoord(2, 0));
		SetResultToCreate(1);
		m_DisplayIngredients = true;
	}

	public void CreateBot()
	{
		StartConversion(null);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWorkerAssemblerMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 15 < 7)
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
		AudioManager.Instance.StartEvent("BuildingWorkerAssemblerMakingComplete", this);
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private bool GetHaveFrame()
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (WorkerFrame.GetIsTypeFrame(ingredient.m_TypeIdentifier))
			{
				return true;
			}
		}
		return false;
	}

	private bool GetHaveHead()
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (WorkerHead.GetIsTypeHead(ingredient.m_TypeIdentifier))
			{
				return true;
			}
		}
		return false;
	}

	private bool GetHaveDrive()
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (WorkerDrive.GetIsTypeDrive(ingredient.m_TypeIdentifier))
			{
				return true;
			}
		}
		return false;
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if (IsBusy() && RightNow)
		{
			return false;
		}
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged && Info.m_Action != ActionType.SetValue)
		{
			return false;
		}
		ActionType action = Info.m_Action;
		if (action == ActionType.Engaged)
		{
			if (m_Ingredients.Count == 0 && GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>() == null && m_Engager == null)
			{
				return true;
			}
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	private bool IsSetToBot()
	{
		return m_Results[m_ResultsToCreate][0].m_Type == ObjectType.Worker;
	}

	public override bool CanAcceptIngredient(ObjectType NewType)
	{
		if (!IsSetToBot())
		{
			return base.CanAcceptIngredient(NewType);
		}
		if (IsBusy())
		{
			return false;
		}
		if (WorkerFrame.GetIsTypeFrame(NewType))
		{
			return !GetHaveFrame();
		}
		if (WorkerHead.GetIsTypeHead(NewType))
		{
			return !GetHaveHead();
		}
		if (WorkerDrive.GetIsTypeDrive(NewType))
		{
			return !GetHaveDrive();
		}
		return false;
	}

	protected override BaseClass CreateNewItem()
	{
		TileCoord spawnPoint = GetSpawnPoint();
		Vector3 position = spawnPoint.ToWorldPositionTileCentered();
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Worker, position, Quaternion.identity);
		FinishNewObject(baseClass);
		TileCoord spawnPointEject = GetSpawnPointEject();
		SpawnAnimationManager.Instance.AddJump(baseClass, spawnPointEject, spawnPoint, 0f, baseClass.transform.position.y, 4f);
		PlayResourceSound(m_Results[m_ResultsToCreate][0].m_Type);
		m_TypeCreated = 1;
		BadgeManager.Instance.AddEvent(BadgeEvent.Type.BotsMade);
		ObjectType type = m_Results[m_ResultsToCreate][0].m_Type;
		if (type != ObjectType.Worker)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Bot: false, type, baseClass);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Bot: false, ObjectType.Worker, baseClass);
		TileCoord direction = new TileCoord(0, 1);
		direction.Rotate(m_Rotation);
		baseClass.GetComponent<Worker>().Nudge(GetSpawnPoint(), direction);
		return baseClass;
	}

	protected override void FinishNewObject(BaseClass NewObject)
	{
		Worker component = NewObject.GetComponent<Worker>();
		if (!IsSetToBot())
		{
			component.SetFrame(ObjectType.WorkerFrameMk0);
			component.SetHead(ObjectType.WorkerHeadMk0);
			component.SetDrive(ObjectType.WorkerDriveMk0);
		}
		else
		{
			int num = 1;
			int num2 = 1;
			int num3 = 1;
			foreach (Holdable ingredient in m_Ingredients)
			{
				ObjectType typeIdentifier = ingredient.m_TypeIdentifier;
				if (WorkerFrame.GetIsTypeFrame(typeIdentifier))
				{
					component.SetFrame(typeIdentifier);
				}
				if (WorkerHead.GetIsTypeHead(typeIdentifier))
				{
					component.SetHead(typeIdentifier);
				}
				if (WorkerDrive.GetIsTypeDrive(typeIdentifier))
				{
					component.SetDrive(typeIdentifier);
				}
				if (WorkerFrameMk1.GetIsTypeFrameMk1(typeIdentifier))
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.MakeWorkerWithFrameMk1, m_LastEngagerType == ObjectType.Worker, 0, NewObject);
				}
				if (WorkerHeadMk1.GetIsTypeHeadMk1(typeIdentifier))
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.MakeWorkerWithHeadMk1, m_LastEngagerType == ObjectType.Worker, 0, NewObject);
				}
				if (WorkerDriveMk1.GetIsTypeDriveMk1(typeIdentifier))
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.MakeWorkerWithDriveMk1, m_LastEngagerType == ObjectType.Worker, 0, NewObject);
				}
				if (WorkerDriveMk2.GetIsTypeDriveMk2(typeIdentifier))
				{
					num = 2;
				}
				if (WorkerFrameMk2.GetIsTypeFrameMk2(typeIdentifier))
				{
					num2 = 2;
				}
				if (WorkerHeadMk2.GetIsTypeHeadMk2(typeIdentifier))
				{
					num3 = 2;
				}
			}
			if (num == 2 && num2 == 2 && num3 == 2)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.MakeWorkerMk2, m_LastEngagerType == ObjectType.Worker, 0, NewObject);
			}
		}
		component.UpdateModel();
		Worker component2 = NewObject.GetComponent<Worker>();
		component2.m_Energy = 0f;
		component2.m_WorkerIndicator.SetNoEnergy(NoEnergy: true);
		component2.m_WorkerIndicator.SetLowEnergy(LowEnergy: false);
	}
}
