using System.Collections.Generic;
using UnityEngine;

public class CeremonyEraComplete : CeremonyQuestEnded
{
	private CeremonyBlueprint m_Blueprint;

	private void Awake()
	{
		AudioManager.Instance.StartEvent("CeremonyEraEnd");
	}

	private void Start()
	{
	}

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		m_Quest = NewQuest;
		CreateBlueprints();
		CreateNew();
		AttachNewToTopBlueprint();
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
	}

	private void Update()
	{
	}
}
