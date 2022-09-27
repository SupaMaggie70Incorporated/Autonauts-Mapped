using System.Collections.Generic;
using UnityEngine;

public class CeremonyFirstFolk : CeremonyGenericSpeechWithTitle
{
	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyFirstFolk");
		SetSpeech("CeremonyFirstFolkSpeech1");
		AudioManager.Instance.StartEvent("CeremonyFirstFolk");
	}

	private void Start()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Folk");
		if (collection == null)
		{
			return;
		}
		using Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator();
		if (enumerator.MoveNext())
		{
			PanTo(enumerator.Current.Key, new Vector3(0f, 2f, 0f), 10f, 1f);
		}
	}

	protected override void End()
	{
		base.End();
		ModeButton.Get(ModeButton.Type.Evolution).Show();
		ModeButton.Get(ModeButton.Type.Evolution).SetNew(New: true);
		GameStateManager.Instance.SetState(GameStateManager.State.Evolution);
		ReturnPanTo(1f);
	}
}
