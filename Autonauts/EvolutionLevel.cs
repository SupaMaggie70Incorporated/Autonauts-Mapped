using UnityEngine;

public class EvolutionLevel
{
	public Color m_BaseColour;

	public Color m_FadedColour;

	public EvolutionLevel(Color BaseColour, Color FadedColour)
	{
		m_BaseColour = BaseColour;
		m_FadedColour = FadedColour;
	}

	public EvolutionLevel(int BaseColour)
	{
		m_BaseColour = GeneralUtils.ColorFromHex(BaseColour);
		m_BaseColour.a = 1f;
		m_FadedColour = m_BaseColour;
		m_FadedColour.a = 0.5f;
	}
}
