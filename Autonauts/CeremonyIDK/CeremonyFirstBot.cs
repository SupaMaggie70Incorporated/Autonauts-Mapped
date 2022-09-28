using System.Collections.Generic;
using UnityEngine;

public class CeremonyFirstBot : CeremonyGenericSpeechWithTitle
{
	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyFirstBot");
		SetSpeech("CeremonyFirstBotSpeech1");
		AudioManager.Instance.StartEvent("CeremonyFirstBot");
	}

	private void Start()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
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
		ReturnPanTo(1f);
	}
}
