using System.Collections.Generic;
using UnityEngine;

public class CeremonyFirstTool : CeremonyGenericSpeech
{
	protected new void Awake()
	{
		base.Awake();
		SetSpeech("TutorialToolsComplete");
		AudioManager.Instance.StartEvent("CeremonyFirstResearch");
	}

	private void Start()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		foreach (KeyValuePair<int, TileCoordObject> @object in PlotManager.Instance.GetPlotAtTile(players[0].GetComponent<Farmer>().m_TileCoord).m_Objects)
		{
			if (MyTool.GetIsTypeTool(@object.Value.m_TypeIdentifier))
			{
				PanTo(@object.Value, new Vector3(0f, 2f, 0f), 10f, 1f);
			}
		}
	}

	protected override void End()
	{
		base.End();
		ReturnPanTo(1f);
		TutorialPanelController.Instance.NextQuest();
	}
}
