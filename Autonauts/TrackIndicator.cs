using UnityEngine;

public class TrackIndicator : MonoBehaviour
{
	private bool m_Flash;

	private float m_FlashTimer;

	private MeshRenderer m_Renderer;

	private Transform m_Parent;

	private Vector3 m_Position;

	private void Start()
	{
	}

	public void SetFlash(bool Flash)
	{
		m_Flash = Flash;
		if (m_Renderer == null)
		{
			m_Renderer = GetComponent<MeshRenderer>();
		}
		if (m_Renderer != null)
		{
			m_Renderer.enabled = true;
		}
	}

	public void SetParent(Transform NewTransform)
	{
		m_Parent = NewTransform;
	}

	public void SetPosition(Vector3 Position)
	{
		m_Position = Position;
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		base.transform.position = m_Parent.TransformPoint(m_Position);
		base.transform.rotation = m_Parent.rotation;
		if (m_Flash)
		{
			m_FlashTimer += TimeManager.Instance.m_NormalDelta;
			bool flag = (int)(m_FlashTimer * 60f) % 20 > 7;
			if (m_Renderer != null)
			{
				m_Renderer.enabled = flag;
			}
		}
	}
}
