using UnityEngine;

public class Swoosh : MonoBehaviour
{
	private float m_Timer;

	private float m_Delay;

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Go(bool SideChop)
	{
		base.gameObject.SetActive(value: true);
		base.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
		if (SideChop)
		{
			base.transform.localPosition = new Vector3(-2.85f, 0.58f, -0.78f);
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		}
		else
		{
			base.transform.localPosition = new Vector3(-0.96f, 2.22f, -1.33f);
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 90f);
		}
		GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
		m_Delay = 1f / 12f;
		m_Timer = m_Delay;
		UpdateFrame();
	}

	private void UpdateFrame()
	{
		int num = (int)(m_Timer / m_Delay * 5f) + 1;
		if (num > 5)
		{
			num = 5;
		}
		if (num < 1)
		{
			num = 1;
		}
		Texture value = (Texture)Resources.Load("SwooshTest/Swoosh" + num, typeof(Texture));
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex", value);
	}

	private void Update()
	{
		m_Timer -= TimeManager.Instance.m_NormalDelta;
		if (m_Timer < 0f)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			UpdateFrame();
		}
	}
}
