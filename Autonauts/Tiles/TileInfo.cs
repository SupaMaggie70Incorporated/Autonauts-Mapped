using UnityEngine;

public class TileInfo
{
	public bool m_Active;

	public bool m_Solid;

	public int m_TextureNum;

	public string m_Name;

	public float m_Speed;

	public string m_IconName;

	public Color32 m_MapColour;

	public bool m_CanReveal;

	public int m_MaxUse;

	public float m_RecoverDelay;

	public int m_VariantsIndex;

	public TileInfo(bool Active, bool Solid, int TextureNum, string Name, float Speed, string IconName, Color MapColour, bool CanReveal, int MaxUse, float RecoverDelay, int VariantsIndex)
	{
		m_Active = Active;
		m_Solid = Solid;
		m_TextureNum = TextureNum;
		m_Name = Name;
		m_Speed = Speed;
		m_IconName = IconName;
		m_MapColour = MapColour;
		m_CanReveal = CanReveal;
		m_MaxUse = MaxUse;
		m_RecoverDelay = RecoverDelay;
		m_VariantsIndex = VariantsIndex;
	}
}
