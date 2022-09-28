using SimpleJSON;
using UnityEngine;

public class Boulder : Selectable
{
	protected Wobbler m_Wobbler;

	private float m_StartScale;

	protected float m_HeightScale;

	protected int m_AmountMined;

	protected int m_MaxMined;

	public static bool GetIsTypeBoulder(ObjectType NewType)
	{
		if (NewType == ObjectType.Boulder || NewType == ObjectType.TallBoulder)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		float y = Random.Range(0, 360);
		base.transform.rotation = Quaternion.Euler(0f, y, 0f);
		TileManager.Instance.SetAssociatedObject(m_TileCoord, this);
		m_AmountMined = 0;
		m_MaxMined = VariableManager.Instance.GetVariableAsInt(ObjectType.Boulder, "MaxMined");
		m_StartScale = Random.Range(0.75f, 1.25f);
		m_HeightScale = 1f;
		m_Wobbler.Restart();
		UpdateWobbler();
		Sleep();
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		RecordingManager.Instance.RemoveObject(this);
		base.StopUsing(AndDestroy);
		TileManager.Instance.ClearAssociatedObject(m_TileCoord, this);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "M", m_AmountMined);
	}

	public override void Load(JSONNode Node)
	{
		TileManager.Instance.ClearAssociatedObject(m_TileCoord, this);
		base.Load(Node);
		m_AmountMined = JSONUtils.GetAsInt(Node, "M", 0);
		UpdateWobbler();
		TileManager.Instance.SetAssociatedObject(m_TileCoord, this);
	}

	public void CreateRock(bool Ore = true)
	{
		TileCoord tileCoord = m_TileCoord;
		int num = 0;
		bool flag = false;
		do
		{
			tileCoord = m_TileCoord;
			tileCoord.x += Random.Range(-1, 2);
			tileCoord.y += Random.Range(-1, 2);
			flag = TileManager.Instance.GetTileSolidToPlayer(tileCoord);
			num++;
		}
		while (num < 100 && flag);
		if (num < 100)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Rock, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
			AudioManager.Instance.StartEvent("ObjectCreated", baseClass.GetComponent<TileCoordObject>());
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Stones);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, tileCoord, 0f, baseClass.transform.position.y, 4f);
		}
	}

	private void UsePick(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	private void EndPick(AFO Info)
	{
		CreateRock();
		m_AmountMined++;
		if (m_AmountMined == m_MaxMined)
		{
			StopUsing();
		}
		UpdateWobbler();
	}

	private ActionType GetActionFromPick(AFO Info)
	{
		Info.m_UseAction = UsePick;
		Info.m_EndAction = EndPick;
		Info.m_FarmerState = Farmer.State.Mining;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (FarmerStateMining.GetIsToolAcceptable(objectType) || objectType == ObjectType.Rock)
		{
			return GetActionFromPick(Info);
		}
		return base.GetActionFromObject(Info);
	}

	protected virtual void UpdateWobbler()
	{
		m_Wobbler.Update();
		float startScale = m_StartScale;
		startScale += m_Wobbler.m_Height * 0.125f;
		float num = (float)m_AmountMined / (float)m_MaxMined;
		startScale *= (1f - num) * 0.75f + 0.25f;
		base.transform.localScale = new Vector3(1f, m_HeightScale, 1f) * startScale;
	}

	private void Update()
	{
		UpdateWobbler();
		if (m_Wobbler.m_Height == 0f)
		{
			Sleep();
		}
	}
}
