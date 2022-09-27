using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
	private List<Stat> m_Stats;

	private void Awake()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Stats/Stat", typeof(GameObject));
		float num = 200f;
		float num2 = 50f;
		m_Stats = new List<Stat>();
		for (int i = 0; i < 12; i++)
		{
			Stat component = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).GetComponent<Stat>();
			component.transform.localPosition = new Vector3(0f, num, 0f);
			component.SetStat((StatsManager.Stat)i);
			m_Stats.Add(component);
			num -= num2;
		}
	}

	private void OnDestroy()
	{
		foreach (Stat stat in m_Stats)
		{
			Object.Destroy(stat.gameObject);
		}
	}

	public void OnContinue()
	{
		AudioManager.Instance.StartEvent("UIUnpause");
		GameStateManager.Instance.PopState();
	}

	public void OnMouseEnter()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
	}
}
