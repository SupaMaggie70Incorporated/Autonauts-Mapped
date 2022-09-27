using UnityEngine;

public class PlotUncovered : BaseClass
{
	private static float m_Delay = 0.25f;

	private float m_Timer;

	private Material m_Material;

	public override void Restart()
	{
		base.Restart();
		m_Timer = 0f;
		base.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		base.transform.localScale = new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, Tile.m_Size * (float)Plot.m_PlotTilesHigh, 1f);
		m_Material = GetComponent<MeshRenderer>().material;
		UpdateColour();
	}

	private void UpdateColour()
	{
		float num = m_Timer / m_Delay;
		Color color = new Color(1f, 1f, 1f, (1f - num) * 0.35f);
		m_Material.color = color;
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			m_Timer += TimeManager.Instance.m_NormalDelta;
			if (m_Timer >= m_Delay)
			{
				StopUsing();
			}
			UpdateColour();
		}
	}
}
