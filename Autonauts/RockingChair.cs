using UnityEngine;

public class RockingChair : Converter
{
	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingRockingChairMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 30 < 15)
		{
			m_ModelRoot.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
		}
		else
		{
			m_ModelRoot.transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);
		}
		if ((bool)m_ObjectInvolved)
		{
			m_ObjectInvolved.transform.position = m_ModelRoot.transform.TransformPoint(new Vector3(0f, 1f, 0f));
			m_ObjectInvolved.transform.rotation = m_ModelRoot.transform.rotation;
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if (Top.GetIsTypeTop(m_Results[m_ResultsToCreate][0].m_Type))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.UseRockingChairTop, m_LastEngagerType == ObjectType.Worker, 0, this);
		}
		else
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.UseRockingChairHat, m_LastEngagerType == ObjectType.Worker, 0, this);
		}
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		_ = Info.m_ActionType;
		return base.GetActionFromObject(Info);
	}
}
