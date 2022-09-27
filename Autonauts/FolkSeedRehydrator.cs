using UnityEngine;

public class FolkSeedRehydrator : Converter
{
	private PlaySound m_PlaySound;

	private GameObject m_Funnel;

	private Wobbler m_FunnelWobbler;

	private bool m_Locked;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(-1, 0));
		SetResultToCreate(1);
		UpdateLocked();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Funnel = m_ModelRoot.transform.Find("Funnel").gameObject;
		UpdateFunnelActive();
		m_FunnelWobbler = new Wobbler();
	}

	public override void PostLoad()
	{
		base.PostLoad();
		UpdateFunnelActive();
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBlueprintMaking", this, Remember: true);
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		MoveIngredientsDown();
		ConvertVibrate();
		if ((int)(m_StateTimer * 60f) % 12 < 6)
		{
			m_Funnel.transform.localScale = new Vector3(1f, 1.25f, 1f);
		}
		else
		{
			m_Funnel.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	protected override void EndConverting()
	{
		EndIngredientsDown();
		EndVibrate();
		m_Funnel.transform.localScale = new Vector3(1f, 1f, 1f);
		m_Funnel.SetActive(value: false);
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		AudioManager.Instance.StartEvent("FolkAppear", this);
	}

	public bool GetFolkSeed()
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier == ObjectType.FolkSeed)
			{
				return true;
			}
		}
		return false;
	}

	public float GetFuel()
	{
		float num = 0f;
		foreach (Holdable ingredient in m_Ingredients)
		{
			num += Food.GetFoodEnergy(ingredient.m_TypeIdentifier);
		}
		float num2 = VariableManager.Instance.GetVariableAsInt("FolkSeedRehydrator_Food");
		float num3 = num / num2;
		if (num3 > 1f)
		{
			num3 = 1f;
		}
		return num3;
	}

	public override bool AreRequrementsMet()
	{
		if (GetFuel() != 1f)
		{
			return false;
		}
		return base.AreRequrementsMet();
	}

	protected ActionType GetActionFromFood(AFO Info)
	{
		Info.m_StartAction = StartAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_AbortAction = base.AbortAddAnything;
		Info.m_FarmerState = Farmer.State.Adding;
		if (GetFuel() == 1f)
		{
			return ActionType.Fail;
		}
		if (m_Ingredients.Count == 0)
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
			return ActionType.Total;
		}
		if (Info.m_ActionType == AFO.AT.Secondary && Food.GetIsTypeFood(Info.m_ObjectType))
		{
			return GetActionFromFood(Info);
		}
		return base.GetActionFromObject(Info);
	}

	protected override void EndAddAnything(AFO Info)
	{
		base.EndAddAnything(Info);
		if (!Food.GetIsTypeFood(Info.m_ObjectType))
		{
			UpdateFunnelActive();
			m_FunnelWobbler.Go(0.25f, 5f, 0.5f);
		}
	}

	protected override Vector3 GetIngredientPosition(BaseClass NewObject)
	{
		Vector3 vector = default(Vector3);
		float num = 0f;
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier != ObjectType.FolkSeed)
			{
				float height = ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier);
				num += height * ingredient.transform.localScale.y;
			}
		}
		vector = m_IngredientsRoot.transform.position;
		vector.y += num;
		return vector;
	}

	protected override void UpdateIngredients()
	{
		float num = 0f;
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier != ObjectType.FolkSeed)
			{
				ingredient.SendAction(new ActionInfo(ActionType.Show, default(TileCoord), this));
				ingredient.transform.parent = m_IngredientsRoot;
				ingredient.transform.localPosition = new Vector3(0f, num, 0f);
				ingredient.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
				float height = ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier);
				num += height * ingredient.transform.localScale.y;
			}
			else
			{
				ingredient.gameObject.SetActive(value: false);
			}
		}
		m_IngredientsHeight = num;
	}

	protected override BaseClass CreateNewItem()
	{
		if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative && FolkManager.Instance.IsFirstFolk(0))
		{
			CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.FirstFolk, m_UniqueID);
			FolkManager.Instance.RegisterFirstFolk(0);
		}
		return base.CreateNewItem();
	}

	protected new void Update()
	{
		base.Update();
		m_FunnelWobbler.Update();
		float num = 0.5f - m_FunnelWobbler.m_Height + 0.5f;
		m_Funnel.transform.localScale = new Vector3(num, num, num);
	}

	private void UpdateFunnelActive()
	{
		if (GetFolkSeed() && !m_Locked)
		{
			m_Funnel.SetActive(value: true);
		}
		else
		{
			m_Funnel.SetActive(value: false);
		}
	}

	public void UpdateLocked()
	{
		m_Locked = false;
		if ((bool)QuestManager.Instance && QuestManager.Instance.GetIsLastLevelActive())
		{
			m_Locked = true;
		}
		UpdateFunnelActive();
	}
}
