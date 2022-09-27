using SimpleJSON;
using UnityEngine;

public class PlanningArea
{
	public static Color[] m_Colours;

	private static int m_ColourIndex;

	private TileCoord m_TopLeft;

	private TileCoord m_BottomRight;

	public string m_Name;

	public int m_Colour;

	public AreaIndicator m_AreaIndicator;

	private bool m_Deselected;

	public static void InitColours()
	{
		int[] obj = new int[16]
		{
			16724530, 16756786, 16777010, 3342130, 3342335, 5000447, 16711935, 13553325, 16752800, 10049551,
			11710983, 1613872, 2130048, 41215, 9726161, 8355691
		};
		int num = 0;
		m_Colours = new Color[obj.Length];
		int[] array = obj;
		foreach (int colour in array)
		{
			m_Colours[num] = GeneralUtils.ColorFromHex(colour);
			num++;
		}
	}

	public static string GetNewName()
	{
		return "Area";
	}

	public PlanningArea()
	{
		m_AreaIndicator = AreaIndicatorManager.Instance.Add();
		m_AreaIndicator.SetNameEnabled(Enabled: true);
		m_AreaIndicator.SetDimensionsEnabled(Enabled: true);
		m_AreaIndicator.SetActive(Active: false);
	}

	public void Delete()
	{
		AreaIndicatorManager.Instance.Remove(m_AreaIndicator);
	}

	public void SetDefaults()
	{
		SetName(GetNewName());
		SetColour(m_ColourIndex);
		m_ColourIndex++;
		if (m_ColourIndex == m_Colours.Length)
		{
			m_ColourIndex = 0;
		}
	}

	public void Save(JSONNode NewNode)
	{
		m_TopLeft.Save(NewNode, "TL");
		m_BottomRight.Save(NewNode, "BR");
		JSONUtils.Set(NewNode, "N", m_Name);
		JSONUtils.Set(NewNode, "C", m_Colour);
	}

	public void Load(JSONNode NewNode)
	{
		TileCoord topLeft = default(TileCoord);
		topLeft.Load(NewNode, "TL");
		TileCoord bottomRight = default(TileCoord);
		bottomRight.Load(NewNode, "BR");
		SetCoords(topLeft, bottomRight);
		string asString = JSONUtils.GetAsString(NewNode, "N", "");
		SetName(asString);
		int asInt = JSONUtils.GetAsInt(NewNode, "C", 0);
		SetColour(asInt);
	}

	public void UpdateColour()
	{
		Color color = m_Colours[m_Colour];
		if (m_Deselected)
		{
			float num = 0.25f;
			float num2 = (1f - num) * (color.r * 0.299f + color.g * 0.587f + color.b * 0.114f);
			color = num * color;
			color.r += num2;
			color.g += num2;
			color.b += num2;
		}
		m_AreaIndicator.SetColour(color);
		m_AreaIndicator.SetNameColour(color);
		m_AreaIndicator.SetFillColour(color);
	}

	public void SetCoords(TileCoord TopLeft, TileCoord BottomRight)
	{
		m_TopLeft = TopLeft;
		m_BottomRight = BottomRight;
		m_AreaIndicator.SetCoords(TopLeft, BottomRight);
	}

	public void SetName(string NewName)
	{
		m_Name = NewName;
		m_AreaIndicator.SetNameString(NewName);
	}

	public void SetColour(int NewColour)
	{
		m_Colour = NewColour;
		UpdateColour();
	}

	public void SetVisible(bool Visible)
	{
		m_AreaIndicator.SetVisible(Visible);
	}

	public void SetIndicated(bool Indicated)
	{
	}

	public void SetDeselected(bool Deselected)
	{
		m_Deselected = Deselected;
		UpdateColour();
	}

	public bool ContainsCoord(TileCoord NewCoord)
	{
		if (NewCoord.x < m_TopLeft.x || NewCoord.x > m_BottomRight.x || NewCoord.y < m_TopLeft.y || NewCoord.y > m_BottomRight.y)
		{
			return false;
		}
		return true;
	}

	public bool SetAnchorFromPosition(Vector3 Position)
	{
		m_AreaIndicator.SetAnchorFromPosition(Position);
		return m_AreaIndicator.m_AnchorActive;
	}
}
