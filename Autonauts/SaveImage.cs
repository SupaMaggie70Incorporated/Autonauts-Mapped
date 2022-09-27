using UnityEngine;
using UnityEngine.UI;

public class SaveImage : MonoBehaviour
{
	private float m_ActiveTimer;

	private void Awake()
	{
		m_ActiveTimer = 0f;
		base.gameObject.GetComponent<Image>().enabled = false;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 10f);
	}

	public void Activate()
	{
		m_ActiveTimer = 1.5f;
		base.gameObject.GetComponent<Image>().enabled = true;
	}

	private void Update()
	{
		if ((int)(m_ActiveTimer * 60f) % 20 < 10)
		{
			base.transform.localRotation = Quaternion.Euler(0f, 0f, 10f);
		}
		else
		{
			base.transform.localRotation = Quaternion.Euler(0f, 0f, -10f);
		}
		if (m_ActiveTimer > 0f)
		{
			m_ActiveTimer -= 1f / 60f;
			if (m_ActiveTimer <= 0f)
			{
				m_ActiveTimer = 0f;
				base.gameObject.GetComponent<Image>().enabled = false;
			}
		}
	}
}
