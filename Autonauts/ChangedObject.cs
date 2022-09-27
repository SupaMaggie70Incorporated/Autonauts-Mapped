using System.Collections.Generic;

public class ChangedObject
{
	public TileCoordObject m_Object;

	public List<ChangedObjectSpecial> m_Specials;

	public ChangedObject(TileCoordObject Object)
	{
		m_Object = Object;
		m_Specials = new List<ChangedObjectSpecial>();
	}

	public void AddSpecialMessage(RecordingStamp.SpecialMessage NewMessage, object Data1, object Data2)
	{
		ChangedObjectSpecial item = default(ChangedObjectSpecial);
		item.m_SpecialMessage = NewMessage;
		item.m_Data1 = Data1;
		item.m_Data2 = Data2;
		m_Specials.Add(item);
	}
}
