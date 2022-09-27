using System.Collections.Generic;

public class ObjectRecording
{
	public string m_ObjectType;

	public List<RecordingStamp> m_Stamps;

	public int cx;

	public int cy;

	public int cr;

	public ObjectRecording(string ObjectType)
	{
		m_ObjectType = ObjectType;
		m_Stamps = new List<RecordingStamp>();
	}

	public void AddStamp(int f, int i, int x, int y, int r)
	{
		m_Stamps.Add(new RecordingStamp(f, i, x, y, r));
	}

	public void AddStamp(int f, int i, RecordingStamp.SpecialMessage NewMessage, string Data1, string Data2)
	{
		object specialData1FromString = RecordingStamp.GetSpecialData1FromString(NewMessage, Data1);
		object specialData1FromString2 = RecordingStamp.GetSpecialData1FromString(NewMessage, Data2);
		m_Stamps.Add(new RecordingStamp(f, i, NewMessage, specialData1FromString, specialData1FromString2));
	}

	public void AddStamp(int f, int i, RecordingStamp.SpecialMessage NewMessage, object NewData1, object NewData2)
	{
		m_Stamps.Add(new RecordingStamp(f, i, NewMessage, NewData1, NewData2));
	}

	public void SetLatestChange(int x, int y, int r)
	{
		cx = x;
		cy = y;
		cr = r;
	}

	public void UpdateLatestChange(int f, int i)
	{
		RecordingStamp recordingStamp = m_Stamps[m_Stamps.Count - 1];
		if (cx != recordingStamp.x || cr != recordingStamp.r || cr != recordingStamp.r)
		{
			m_Stamps.Add(new RecordingStamp(f, i, cx, cy, cr));
		}
	}
}
