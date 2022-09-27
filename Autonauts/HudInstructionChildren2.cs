using UnityEngine;
using UnityEngine.UI;

public class HudInstructionChildren2 : HudInstructionChildren
{
	private GameObject m_BottomPanel2;

	private GameObject m_MiddlePanel2;

	protected new void Awake()
	{
		base.Awake();
		m_BottomPanel2 = base.transform.Find("BottomPanel2").gameObject;
		m_MiddlePanel2 = base.transform.Find("MiddlePanel2").gameObject;
	}

	public override void SetColour(Color NewColour)
	{
		base.SetColour(NewColour);
		m_BottomPanel2.GetComponent<Image>().color = NewColour;
		m_MiddlePanel2.GetComponent<Image>().color = NewColour;
	}

	public override void SetInvalidPosition(bool Invalid)
	{
		base.SetInvalidPosition(Invalid);
		foreach (HighInstruction item in m_Instruction.m_Children2)
		{
			item.m_HudParent.SetInvalidPosition(Invalid);
		}
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		foreach (HighInstruction item in m_Instruction.m_Children2)
		{
			if ((bool)item.m_HudParent)
			{
				item.m_HudParent.SetSelected(Selected);
			}
		}
	}

	protected override void UpdateBottomPanelWidth()
	{
		base.UpdateBottomPanelWidth();
		Rect rect = base.transform.GetComponent<RectTransform>().rect;
		m_BottomPanel2.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
	}

	public override void Refresh()
	{
		base.Refresh();
		float height = GetHeight(IncludeChildren2: false);
		float num = GetBottomHeight() - 30f;
		float num2 = num + height;
		Vector3 localPosition = new Vector3(0f, 0f - num2, 0f);
		m_BottomPanel2.transform.localPosition = localPosition;
		Rect rect = m_MiddlePanel2.transform.GetComponent<RectTransform>().rect;
		m_MiddlePanel2.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, num + 12f);
		m_MiddlePanel2.transform.localPosition = new Vector3(0f, (0f - num) / 2f - height, 0f);
	}

	private float GetBottomHeight()
	{
		float num = m_BottomPanel2.GetComponent<RectTransform>().rect.height;
		foreach (HighInstruction item in m_Instruction.m_Children2)
		{
			num += item.m_HudParent.GetHeight();
		}
		return num;
	}

	public override float GetHeight(bool IncludeChildren2 = true)
	{
		float num = base.GetHeight(IncludeChildren2: true);
		if (IncludeChildren2)
		{
			num += GetBottomHeight();
		}
		return num;
	}

	public override float GetWidth()
	{
		float num = GetComponent<RectTransform>().rect.width;
		float width = m_MiddlePanel.GetComponent<RectTransform>().rect.width;
		foreach (HighInstruction child in m_Instruction.m_Children)
		{
			float num2 = width + child.m_HudParent.GetWidth();
			if (num2 > num)
			{
				num = num2;
			}
		}
		foreach (HighInstruction item in m_Instruction.m_Children2)
		{
			float num3 = width + item.m_HudParent.GetWidth();
			if (num3 > num)
			{
				num = num3;
			}
		}
		return num;
	}
}
