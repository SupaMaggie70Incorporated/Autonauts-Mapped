using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMissionPanel : MonoBehaviour
{
	public IndustryTreeNode m_MainNode;

	private bool m_Selected;

	private List<IndustryTreeNode> m_Nodes;

	public int m_XIndex;

	public int m_YIndex;

	public void SetMissionNode(IndustryTreeNode NewNode)
	{
		m_MainNode = NewNode;
		m_Nodes = new List<IndustryTreeNode>();
		Transform parent = base.transform.Find("Mission");
		m_MainNode.transform.SetParent(parent);
		string newText = m_MainNode.m_Level.m_Quest.m_ID.ToString();
		base.transform.Find("Title").GetComponent<BaseText>().SetTextFromID(newText);
		base.transform.Find("Banner").GetComponent<MainMissionBanner>().SetParent(this);
		SetSelected(Selected: false, Force: true);
	}

	public void SetPosition(int x, int y)
	{
		m_XIndex = x;
		m_YIndex = y;
		UpdatePosition(x, y);
	}

	public void UpdatePosition(int x, int y)
	{
		Vector3 localPosition = new Vector3((float)x * IndustryTreeNode.m_Spacing, (float)(-y) * IndustryTreeNode.m_Spacing);
		base.transform.localPosition = localPosition;
		m_MainNode.transform.localPosition = default(Vector3);
	}

	public void AddNode(IndustryTreeNode NewNode)
	{
		m_Nodes.Add(NewNode);
	}

	private float GetWidth()
	{
		float num = 0f;
		foreach (IndustryTreeNode node in m_Nodes)
		{
			float num2 = node.transform.localPosition.x - base.transform.localPosition.x;
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num + IndustryTreeNode.m_Spacing;
	}

	private float GetHeight()
	{
		float num = 0f;
		foreach (IndustryTreeNode node in m_Nodes)
		{
			float num2 = base.transform.localPosition.y - node.transform.localPosition.y;
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num + (IndustryTreeNode.m_Spacing - 20f);
	}

	public void SetSelected(bool Selected, bool Force = false)
	{
		if (Force || m_Selected != Selected)
		{
			m_Selected = Selected;
			float num = 0f;
			float num2 = 0f;
			if (m_Selected)
			{
				num = GetWidth();
				num2 = GetHeight();
			}
			if (num < 3200f)
			{
				num = 3200f;
			}
			if (num2 < 300f)
			{
				num2 = 300f;
			}
			GetComponent<BasePanel>().SetWidth(num);
			GetComponent<BasePanel>().SetHeight(num2);
		}
	}

	public int GetIntHeight()
	{
		int num = 0;
		if (m_Selected)
		{
			foreach (IndustryTreeNode node in m_Nodes)
			{
				int num2 = node.m_YIndex - m_YIndex;
				if (num2 > num)
				{
					num = num2;
				}
			}
			num++;
		}
		if (num < 2)
		{
			num = 2;
		}
		return num;
	}

	public void OnMouseEnter(PointerEventData eventData)
	{
		m_MainNode.SetIndicated(Indicated: true);
	}

	public void OnMouseExit(PointerEventData eventData)
	{
		m_MainNode.SetIndicated(Indicated: false);
	}

	public void OnMouseClick(PointerEventData eventData)
	{
		m_MainNode.OnPointerClick(eventData);
	}
}
