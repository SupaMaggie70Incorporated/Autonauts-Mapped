using UnityEngine;

public class WheatHammer : Converter
{
	private GameObject m_Handle;

	private GameObject m_Funnel;

	private PlaySound m_PlaySound;

	private MyParticles m_ChaffParticles;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		SetResultToCreate(1);
		m_Handle = m_ModelRoot.transform.Find("Handle").gameObject;
		if ((bool)m_ModelRoot.transform.Find("Funnel"))
		{
			m_Funnel = m_ModelRoot.transform.Find("Funnel").gameObject;
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWheatHammerMaking", this, Remember: true);
		m_ChaffParticles = ParticlesManager.Instance.CreateParticles("Chaff", m_IngredientsRoot.position, Quaternion.Euler(-70f, 60f, 0f));
		StartIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		ConvertVibrate();
		MoveIngredientsDown();
		m_Handle.transform.localRotation = m_Handle.transform.localRotation * Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f);
		m_ChaffParticles.transform.rotation = Quaternion.Euler(0f, 1400f * TimeManager.Instance.m_NormalDelta, 0f) * m_ChaffParticles.transform.rotation;
		UpdateFunnel(m_Funnel);
	}

	protected override void EndConverting()
	{
		EndVibrate();
		AudioManager.Instance.StopEvent(m_PlaySound);
		ParticlesManager.Instance.DestroyParticles(m_ChaffParticles, WaitUntilNoParticles: true, StopSpawning: true);
		if (m_Ingredients[0].m_TypeIdentifier == ObjectType.Wheat)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ThreshWheat, m_LastEngagerType == ObjectType.Worker, 0, this);
		}
		if (m_Ingredients[0].m_TypeIdentifier == ObjectType.CottonBall)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ThreshCottonBalls, m_LastEngagerType == ObjectType.Worker, 0, this);
		}
		if (m_Ingredients[0].m_TypeIdentifier == ObjectType.BullrushesStems)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ThreshBullrushesStems, m_LastEngagerType == ObjectType.Worker, 0, this);
		}
		EndFunnel(m_Funnel);
	}
}
