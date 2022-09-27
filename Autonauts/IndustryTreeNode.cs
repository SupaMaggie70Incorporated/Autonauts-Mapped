using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class IndustryTreeNode : BaseButtonImage
{
	private Quest.State m_State;

	public static float m_Width = 100f;

	public static float m_Spacing = 160f;

	public Quest m_Quest;

	public int m_XIndex;

	public int m_YIndex;

	public int m_LastXIndex;

	public int m_LastYIndex;

	private IndustryTree m_Parent;

	private Transform m_TransParent;

	private Transform m_LineParent;

	public List<UILineRenderer> m_Lines;

	public List<bool> m_LineFlashing;

	public List<IndustryTreeNode> m_LineNode;

	public IndustryLevel m_Level;

	private bool m_Flashing;

	public IndustryTreeNode m_NodeParent;

	public bool m_Visible;

	private GameObject m_Star;

	private GameObject m_RedDot;

	private GameObject m_Pin;

	protected new void Awake()
	{
		base.Awake();
		m_Star = base.transform.Find("Star").gameObject;
		m_RedDot = base.transform.Find("RedDot").gameObject;
		m_Pin = base.transform.Find("Pin").gameObject;
		m_Lines = new List<UILineRenderer>();
		m_LineFlashing = new List<bool>();
		m_LineNode = new List<IndustryTreeNode>();
		m_Visible = true;
	}

	public void SetParents(IndustryLevel NewLevel, IndustryTree Parent, Transform TransParent, Transform LineParent)
	{
		m_Level = NewLevel;
		m_Parent = Parent;
		m_TransParent = TransParent;
		m_LineParent = LineParent;
	}

	public void AddLine(UILineRenderer NewLine, IndustryTreeNode LineNode)
	{
		m_Lines.Add(NewLine);
		m_LineFlashing.Add(item: false);
		m_LineNode.Add(LineNode);
	}

	public void ClearLines()
	{
		foreach (UILineRenderer line in m_Lines)
		{
			Object.Destroy(line.gameObject);
		}
		m_Lines.Clear();
		m_LineFlashing.Clear();
		m_LineNode.Clear();
	}

	public void SetLinesVisible(bool Visible)
	{
		foreach (UILineRenderer line in m_Lines)
		{
			line.gameObject.SetActive(Visible);
		}
		UpdateLines();
	}

	public void SetVisible(bool Visible)
	{
		m_Visible = Visible;
		base.gameObject.SetActive(Visible);
		SetLinesVisible(Visible);
	}

	public void SetHide(bool OriginalHide)
	{
		bool flag = OriginalHide;
		if (m_Quest.m_Active || m_Quest.m_Complete)
		{
			flag = false;
		}
		base.gameObject.SetActive(!flag);
		int num = 0;
		foreach (UILineRenderer line in m_Lines)
		{
			Quest.ID newID = m_Quest.m_QuestsUnlocked[num];
			Quest quest = QuestData.Instance.GetQuest(newID);
			bool flag2 = flag;
			if (OriginalHide && !quest.m_Active && !quest.m_Complete)
			{
				flag2 = true;
			}
			line.gameObject.SetActive(!flag2);
			num++;
		}
	}

	public void SetHighLight(bool HighLight)
	{
		foreach (UILineRenderer line in m_Lines)
		{
			if (HighLight)
			{
				line.LineThickness = 10f;
				line.color = new Color(1f, 0f, 0f);
				line.transform.SetParent(m_TransParent);
			}
			else
			{
				line.LineThickness = 6f;
				line.color = new Color(1f, 1f, 1f);
				line.transform.SetParent(m_LineParent);
			}
		}
	}

	public void SetParentHighLight(bool HighLight, int LineIndex)
	{
		UILineRenderer uILineRenderer = m_Lines[LineIndex];
		if (HighLight)
		{
			uILineRenderer.LineThickness = 10f;
			uILineRenderer.color = new Color(0f, 1f, 0f);
			uILineRenderer.transform.SetParent(m_TransParent);
		}
		else
		{
			uILineRenderer.LineThickness = 6f;
			uILineRenderer.color = new Color(1f, 1f, 1f);
			uILineRenderer.transform.SetParent(m_LineParent);
		}
	}

	private void UpdatePanelColour()
	{
		Color backingColour = new Color(1f, 1f, 1f, 1f);
		if (m_Quest.m_Type == Quest.Type.Industry || m_Quest.m_Type == Quest.Type.Infrastructure || m_Quest.m_Type == Quest.Type.Important)
		{
			Color panelColour = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(m_Quest).m_Parent.m_Parent.m_PanelColour;
			backingColour *= panelColour;
		}
		if (!m_Quest.m_MajorNode && m_State == Quest.State.Unavailable)
		{
			float num = 0.35f;
			backingColour.r *= num;
			backingColour.g *= num;
			backingColour.b *= num;
		}
		if (m_Flashing && m_Parent.m_FlashOn)
		{
			backingColour = new Color(1f, 0f, 0f, 1f);
		}
		SetBackingColour(backingColour);
	}

	public void UpdateStatus()
	{
		if (m_State != Quest.State.Completed || m_Quest.m_MajorNode)
		{
			m_Star.SetActive(value: false);
		}
		if (m_State != Quest.State.Incompletable || m_Quest.m_MajorNode)
		{
			m_RedDot.SetActive(value: false);
		}
		m_Pin.SetActive(value: false);
		UpdatePanelColour();
		UpdateLines();
		UpdateScale();
	}

	private void SetPanelImage()
	{
		if (m_Quest.m_Type == Quest.Type.Research)
		{
			SetImageEnabled(Enabled: true);
			SetSprite(IconManager.Instance.GetIcon(m_Quest.m_ObjectTypeRequired));
		}
		else if (m_Quest.m_Type == Quest.Type.Industry || m_Quest.m_Type == Quest.Type.Infrastructure || m_Quest.m_Type == Quest.Type.Important || m_Quest.m_Type == Quest.Type.Tutorial)
		{
			string icon = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(m_Quest).GetIcon();
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + icon, typeof(Sprite));
			if (sprite != null)
			{
				SetSprite(sprite);
				SetImageEnabled(Enabled: true);
			}
			else
			{
				SetImageEnabled(Enabled: false);
			}
		}
	}

	private void UpdateScale()
	{
		float num = 1f;
		if (m_Quest.m_Type != Quest.Type.Research)
		{
			IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(m_Quest);
			if (industryLevelFromQuest != null && industryLevelFromQuest.m_MajorNode)
			{
				num = 2f;
			}
		}
		if (m_Selected)
		{
			num *= 1.25f;
		}
		base.transform.localScale = new Vector3(num, num, num);
	}

	public void SetObjectType(Quest NewQuest)
	{
		m_Quest = NewQuest;
		m_State = m_Quest.GetState();
		SetPanelImage();
		UpdateStatus();
		UpdateScale();
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		GameStateIndustry.SetSelectedQuest(m_Quest);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
		m_Parent.SetHighLight(this, Indicated);
		if (Indicated)
		{
			HudManager.Instance.ActivateQuestRollover(Indicated, m_Quest);
		}
		else
		{
			HudManager.Instance.ActivateQuestRollover(Activate: false, null);
		}
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		UpdateStatus();
	}

	public void SetPosition(int NewX, int NewY)
	{
		m_XIndex = NewX;
		m_YIndex = NewY;
		base.transform.localPosition = new Vector3((float)m_XIndex * m_Spacing, (float)(-m_YIndex) * m_Spacing, 0f);
	}

	public void SetFlashing(bool Flashing)
	{
		m_Flashing = Flashing;
	}

	public void SetLineFlashing(int Index, bool Flashing)
	{
		m_LineFlashing[Index] = Flashing;
		m_Lines[Index].color = new Color(1f, 1f, 1f, 1f);
	}

	private void UpdateLines()
	{
		for (int i = 0; i < m_LineNode.Count; i++)
		{
			bool flag = m_LineNode[i].m_Quest.GetState() == Quest.State.Unavailable;
			Color color = new Color(1f, 1f, 1f, 1f);
			if (m_LineFlashing[i] && m_Parent.m_FlashOn)
			{
				color = new Color(1f, 0f, 0f, 1f);
			}
			else if (flag)
			{
				color = new Color(0.35f, 0.35f, 0.35f, 1f);
			}
			m_Lines[i].color = color;
		}
	}

	private void Update()
	{
		UpdatePanelColour();
		UpdateLines();
	}
}
