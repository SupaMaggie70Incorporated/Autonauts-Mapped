using System.Collections.Generic;
using SimpleJSON;

public class StatValuesRate : StatValues
{
	private static int m_RecordingLengthInMinutes = 10;

	private List<int> m_Recordings;

	public StatValuesRate(StatsManager.Stat ID)
		: base(ID, Type.Rate)
	{
		m_Recordings = new List<int>();
	}

	public override void Add()
	{
		int item = (int)TimeManager.Instance.m_TotalTime;
		m_Recordings.Add(item);
	}

	public override string GetAsString()
	{
		return ((float)m_Recordings.Count / (float)m_RecordingLengthInMinutes).ToString();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONArray jSONArray = new JSONArray();
		JSONUtils.Set(Node, "Recordings", jSONArray);
		int num = 0;
		foreach (int recording in m_Recordings)
		{
			jSONArray[num] = recording;
			num++;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONArray asArray = JSONUtils.GetAsArray(Node, "Recordings");
		for (int i = 0; i < asArray.Count; i++)
		{
			m_Recordings.Add(asArray[i].AsInt);
		}
	}

	public override void Update()
	{
		int num = (int)TimeManager.Instance.m_TotalTime - m_RecordingLengthInMinutes * 60;
		int num2 = 0;
		using (List<int>.Enumerator enumerator = m_Recordings.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current < num)
			{
				num2++;
			}
		}
		if (num2 > 0)
		{
			m_Recordings.RemoveRange(0, num2);
		}
	}
}
