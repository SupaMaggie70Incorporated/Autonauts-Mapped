using UnityEngine;

public class ChoppingBlock : Converter
{
	private PlaySound m_PlaySound;

	private bool m_Bot;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
		m_DisplayIngredients = true;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBlueprintMaking", this, Remember: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
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
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		m_Bot = m_LastEngagerType == ObjectType.Worker;
		switch (m_Requirements[m_ResultsToCreate][0].m_Type)
		{
		case ObjectType.Log:
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChopLog, m_Bot, 0, this);
			break;
		case ObjectType.Plank:
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChopPlank, m_Bot, 0, this);
			break;
		case ObjectType.Pole:
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChopPole, m_Bot, 0, this);
			break;
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.ProcessWood, m_Bot, 0, this);
		ObjectType type = m_Results[m_ResultsToCreate][0].m_Type;
		if (type == ObjectType.Plank)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakePlank, m_Bot, 0, this);
		}
	}
}
