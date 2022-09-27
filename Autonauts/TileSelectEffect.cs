using UnityEngine;

public class TileSelectEffect : BaseClass
{
	private float m_Timer;

	private Material m_Material;

	private Vector3 m_StartScale;

	public override void Restart()
	{
		base.Restart();
		m_Timer = 0f;
		base.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		m_Material = GetComponent<MeshRenderer>().material;
		UpdateAnimation(1f);
	}

	public void SetScale(Vector3 StartScale)
	{
		m_StartScale = StartScale;
	}

	private void UpdateAnimation(float Percent)
	{
		float num = 1f + Percent * 0.5f;
		base.transform.localScale = m_StartScale * num * Tile.m_Size;
		m_Material.color = new Color(1f, 1f, 1f, 1f - Percent);
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			m_Timer += TimeManager.Instance.m_NormalDelta;
			float num = 0.25f;
			float percent = m_Timer / num;
			UpdateAnimation(percent);
			if (m_Timer >= num)
			{
				StopUsing();
			}
		}
	}
}
