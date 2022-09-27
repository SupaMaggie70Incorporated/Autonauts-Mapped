public class RecordingStamp
{
	public enum SpecialMessage
	{
		SetBotDrive = -1,
		SetBotFrame = -2,
		SetBotHead = -3,
		Total = 0
	}

	public int f;

	public int i;

	public int x;

	public int y;

	public int r;

	public object SpecialData1;

	public object SpecialData2;

	public RecordingStamp(int nf, int ni, int nx, int ny, int nr)
	{
		f = nf;
		i = ni;
		x = nx;
		y = ny;
		r = nr;
		SpecialData1 = null;
		SpecialData2 = null;
	}

	public RecordingStamp(int nf, int ni, SpecialMessage NewMessage, object Data1, object Data2)
	{
		f = nf;
		i = ni;
		x = (int)NewMessage;
		y = 0;
		r = 0;
		SpecialData1 = Data1;
		SpecialData2 = Data2;
	}

	public static bool GetIsSpecial(int nx)
	{
		return nx < 0;
	}

	public SpecialMessage GetSpecialMessage()
	{
		return (SpecialMessage)x;
	}

	public string GetSpecialData1AsString()
	{
		SpecialMessage specialMessage = (SpecialMessage)x;
		if ((uint)(specialMessage - -3) <= 2u)
		{
			return ObjectTypeList.Instance.GetSaveNameFromIdentifier((ObjectType)SpecialData1);
		}
		return "";
	}

	public string GetSpecialData2AsString()
	{
		SpecialMessage specialMessage = (SpecialMessage)x;
		if ((uint)(specialMessage - -3) <= 2u)
		{
			return ObjectTypeList.Instance.GetSaveNameFromIdentifier((ObjectType)SpecialData2);
		}
		return "";
	}

	public static object GetSpecialData1FromString(SpecialMessage x, string Data)
	{
		if ((uint)(x - -3) <= 2u)
		{
			return ObjectTypeList.Instance.GetIdentifierFromSaveName(Data);
		}
		return null;
	}

	public static object GetSpecialData2FromString(SpecialMessage x, string Data)
	{
		if ((uint)(x - -3) <= 2u)
		{
			return ObjectTypeList.Instance.GetIdentifierFromSaveName(Data);
		}
		return null;
	}
}
