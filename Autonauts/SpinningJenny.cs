using System;
using UnityEngine;

public class SpinningJenny : LinkedSystemConverter
{
	private PlaySound m_PlaySound;

	private Transform[] m_Wool;

	private GameObject[] m_WoolModel;

	private Transform m_Carriage;

	private float m_CarriageStartX;

	private static float m_CarriageTravel = -3.5f;

	private Transform m_Threads;

	private float m_ThreadsWidth;

	private Transform m_RawMaterial;

	private float m_Timer;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 0), new TileCoord(-1, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_PulleySide = 1;
		SetResultToCreate(1);
		if (m_Wool == null)
		{
			m_Wool = new Transform[4];
			m_WoolModel = new GameObject[4];
			for (int i = 0; i < 4; i++)
			{
				m_Wool[i] = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "ThreadPoint" + (i + 1));
				m_WoolModel[i] = ModelManager.Instance.Instantiate(ObjectType.Wool, "", m_Wool[i], RandomVariants: true);
				m_WoolModel[i].transform.localPosition = default(Vector3);
				m_WoolModel[i].transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				m_WoolModel[i].transform.localScale = default(Vector3);
			}
			m_Carriage = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Carriage");
			m_CarriageStartX = m_Carriage.transform.localPosition.x;
			m_Threads = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Threads");
			m_ThreadsWidth = GetThreadLength();
			m_RawMaterial = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "RawMaterial");
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("SpinningJennyMaking", this, Remember: true);
	}

	private float GetThreadLength()
	{
		return Mathf.Abs(((m_Wool[1].transform.position + m_Wool[2].transform.position) / 2f).x - m_Threads.transform.position.x);
	}

	protected override void UpdateConverting()
	{
		m_Timer += TimeManager.Instance.m_NormalDelta;
		float num = 1f - (Mathf.Cos(m_Timer * 3f * (float)Math.PI * 2f) * 0.5f + 0.5f);
		Vector3 localPosition = m_Carriage.transform.localPosition;
		localPosition.x = num * m_CarriageTravel + m_CarriageStartX;
		m_Carriage.transform.localPosition = localPosition;
		Vector3 localScale = m_Threads.transform.localScale;
		float threadLength = GetThreadLength();
		localScale.x = threadLength / m_ThreadsWidth;
		m_Threads.transform.localScale = localScale;
		m_RawMaterial.transform.localRotation = Quaternion.Euler(0f, 0f, 1000f * TimeManager.Instance.m_NormalDelta) * m_RawMaterial.transform.localRotation;
		float conversionPercent = GetConversionPercent();
		for (int i = 0; i < 4; i++)
		{
			m_WoolModel[i].transform.localScale = new Vector3(conversionPercent, 1f, conversionPercent);
			m_WoolModel[i].transform.localPosition = new Vector3(0f, 0f, (0f - conversionPercent) * ObjectTypeList.Instance.GetHeight(ObjectType.Wool) * 0.5f);
			m_Wool[i].transform.localRotation = Quaternion.Euler(0f, 0f, 1400f * TimeManager.Instance.m_NormalDelta) * m_Wool[i].transform.localRotation;
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		for (int i = 0; i < 4; i++)
		{
			m_WoolModel[i].transform.localScale = default(Vector3);
		}
	}
}
