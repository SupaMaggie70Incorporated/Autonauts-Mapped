using UnityEngine;

public class Rollover : MonoBehaviour
{
	private static float m_StandardWidth = 250f;

	private static Vector2 m_StandardPosition = new Vector2(-10f, 10f);

	protected Wobbler m_Wobbler;

	public BasePanel m_Panel;

	private bool m_UpdatePosition = true;

	protected void Awake()
	{
		m_Wobbler = new Wobbler();
		CheckGadgets();
	}

	protected void Start()
	{
		SetWidth(m_StandardWidth);
	}

	protected virtual void CheckGadgets()
	{
		if (m_Panel == null)
		{
			m_Panel = base.transform.Find("BasePanel").GetComponent<BasePanel>();
		}
	}

	public void UpdatePosition(Vector2 Offset)
	{
		if (m_UpdatePosition)
		{
			Vector2 vector = new Vector2((0f - m_Panel.GetWidth()) / 2f, m_Panel.GetHeight() / 2f);
			GetComponent<RectTransform>().anchoredPosition = m_StandardPosition + vector + Offset;
		}
	}

	protected void SetWidth(float Width)
	{
		m_Panel.SetWidth(Width);
		UpdatePosition(Vector2.zero);
	}

	protected void SetHeight(float Height)
	{
		m_Panel.SetHeight(Height);
		UpdatePosition(Vector2.zero);
	}

	protected void Hide()
	{
		m_Panel.SetActive(Active: false);
	}

	public void UpdateWhilePaused(bool Update)
	{
		m_Wobbler.m_WobbleWhilePaused = Update;
	}

	public void ForceOpen()
	{
		m_Panel.SetActive(Active: true);
		UpdateScale();
		m_UpdatePosition = false;
	}

	protected void StartOpen()
	{
		m_Wobbler.Go(0.05f, 0.75f, -0.25f);
		UpdateScale();
	}

	protected void StartClose()
	{
		m_Wobbler.m_Height = 0f;
		UpdateScale();
	}

	protected void UpdateScale()
	{
		float num = 1f + m_Wobbler.m_Height;
		m_Panel.transform.localScale = new Vector3(num, num, num);
	}

	protected virtual bool GetTargettingSomething()
	{
		return false;
	}

	protected virtual void UpdateTarget()
	{
	}

	protected void Update()
	{
		m_Wobbler.Update();
		UpdateScale();
		if (GetTargettingSomething())
		{
			UpdateTarget();
			if (!m_Panel.GetActive())
			{
				AudioManager.Instance.StartEvent("UIRolloverPopup");
				m_Panel.SetActive(Active: true);
				StartOpen();
			}
			Vector3 vector = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
			float num = m_Panel.GetWidth() + 10f;
			m_Panel.GetHeight();
			float height = m_Panel.GetHeight();
			Vector2 offset = default(Vector2);
			if (vector.x > HudManager.Instance.m_CanvasWidth - num - 10f && vector.y < height + 10f)
			{
				offset.x = 0f - (num + 20f);
			}
			UpdatePosition(offset);
		}
		else if (m_Panel.GetActive())
		{
			StartClose();
			m_Panel.SetActive(Active: false);
		}
	}
}
