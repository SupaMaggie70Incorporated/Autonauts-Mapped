using UnityEngine;

public class MouseActions : BaseGadget
{
	private int m_Timer;

	private MouseAction[] m_Actions;

	private BaseText m_Reserved;

	private int m_ActionsActive;

	protected new void Awake()
	{
		base.Awake();
		m_Actions = new MouseAction[4];
		m_Actions[0] = base.transform.Find("MouseAction").GetComponent<MouseAction>();
		m_Actions[1] = Object.Instantiate(m_Actions[0], base.transform);
		m_Actions[1].SetRMB();
		m_Actions[2] = base.transform.Find("MouseActionAlt").GetComponent<MouseAction>();
		m_Actions[3] = Object.Instantiate(m_Actions[2], base.transform);
		m_Actions[3].SetRMB();
		MouseAction[] actions = m_Actions;
		for (int i = 0; i < actions.Length; i++)
		{
			actions[i].SetActive(Active: false);
		}
		m_Reserved = base.transform.Find("Reserved").GetComponent<BaseText>();
	}

	private void SetAction(MouseAction NewMouseAction, ActionInfo NewInfo, ActionType NewAction)
	{
		NewMouseAction.SetAction(NewInfo, NewAction);
		NewMouseAction.SetActive(Active: true);
		m_ActionsActive++;
	}

	public void SetActions(ActionInfo NewInfo, ActionType NewAction, ActionInfo NewInfo2, ActionType NewAction2, ActionInfo NewAltInfo, ActionType NewAltAction, ActionInfo NewAltInfo2, ActionType NewAltAction2)
	{
		MouseAction[] actions = m_Actions;
		for (int i = 0; i < actions.Length; i++)
		{
			actions[i].SetActive(Active: false);
		}
		m_ActionsActive = 0;
		if (NewAction != ActionType.Total)
		{
			SetAction(m_Actions[0], NewInfo, NewAction);
		}
		if (NewAction2 != ActionType.Total && NewAction2 != ActionType.DropAll)
		{
			SetAction(m_Actions[1], NewInfo2, NewAction2);
		}
		if (NewAltAction != ActionType.Total)
		{
			SetAction(m_Actions[2], NewAltInfo, NewAltAction);
		}
		if (NewAltAction2 != ActionType.Total)
		{
			SetAction(m_Actions[3], NewAltInfo2, NewAltAction2);
		}
		bool flag = BaggedManager.Instance.IsObjectBagged(NewInfo.m_Object) || BaggedManager.Instance.IsTileBagged(NewInfo.m_Position);
		if (m_ActionsActive > 0 || flag)
		{
			if (m_Timer == 0)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.AltHover, Bot: false, null, null);
			}
			m_Timer = 3;
			SetActive(Active: true);
			float height = m_Actions[0].GetHeight();
			float num = (float)m_ActionsActive * height + 20f;
			if (flag)
			{
				num += m_Reserved.GetHeight();
			}
			SetHeight(num);
			float num2 = -10f;
			actions = m_Actions;
			foreach (MouseAction mouseAction in actions)
			{
				if (mouseAction.GetActive())
				{
					mouseAction.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, num2);
					num2 -= height;
				}
			}
			if (flag)
			{
				BaseClass baseClass = null;
				baseClass = ((!BaggedManager.Instance.IsObjectBagged(NewInfo.m_Object)) ? BaggedManager.Instance.GetTileBagger(NewInfo.m_Position) : BaggedManager.Instance.GetObjectBagger(NewInfo.m_Object));
				string val = "";
				if ((bool)baseClass)
				{
					val = baseClass.GetHumanReadableName();
				}
				string text = TextManager.Instance.Get("MouseActionsReserved", val);
				m_Reserved.SetText(text);
				m_Reserved.SetActive(Active: true);
			}
			else
			{
				m_Reserved.SetActive(Active: false);
			}
			Vector3 localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
			base.transform.localPosition = localPosition;
		}
		else
		{
			m_Timer = 0;
			SetActive(Active: false);
		}
	}

	private void Update()
	{
		m_Timer--;
		if (m_Timer == 0)
		{
			SetActive(Active: false);
		}
	}
}
