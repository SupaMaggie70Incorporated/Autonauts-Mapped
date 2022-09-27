using UnityEngine;
using UnityEngine.UI;

public class FarmerPlayerWorkerSelect : Indicator
{
	private Image m_Image;

	private FarmerPlayer m_FarmerPlayer;

	private float m_Timer;

	public void Restart()
	{
		base.gameObject.SetActive(value: false);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Image = GetComponentInChildren<Image>();
	}

	public void SetFarmerPlayer(FarmerPlayer NewFarmerPlayer)
	{
		m_FarmerPlayer = NewFarmerPlayer;
	}

	private void UpdatePosition()
	{
		UpdateTransform(m_FarmerPlayer.transform.position + new Vector3(0f, 1f, 0f));
	}

	public void SetActive(bool Active)
	{
		base.gameObject.SetActive(Active);
		UpdatePosition();
	}

	private void Update()
	{
		UpdatePosition();
		m_Timer += TimeManager.Instance.m_NormalDelta;
		if ((int)(m_Timer * 60f) % 30 < 15)
		{
			m_Image.enabled = true;
		}
		else
		{
			m_Image.enabled = false;
		}
	}
}
