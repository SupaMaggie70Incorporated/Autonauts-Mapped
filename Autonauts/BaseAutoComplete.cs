using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseAutoComplete : MonoBehaviour
{
	public delegate List<BaseAutoCompletePossible> GetPossibleSearchObjects(string SearchName);

	private BaseInputField m_InputField;

	private GameObject m_DropDown;

	private RectTransform m_Template;

	private BaseAutoCompleteOption m_DefaultToggle;

	private float m_ToggleHeight;

	private List<BaseAutoCompletePossible> m_PossibleObjects;

	private List<BaseAutoCompleteOption> m_PossibleToggles;

	private float m_ToggleY;

	private object m_CurrentObject;

	private Action<BaseAutoComplete> m_Action;

	private BaseAutoComplete m_ActionGadget;

	private int m_RequestClose;

	private BaseImage m_Blocker;

	private GetPossibleSearchObjects m_GetPossibleSearchObjects;

	private void Awake()
	{
		m_InputField = base.transform.Find("SearchInput").GetComponent<BaseInputField>();
		m_InputField.m_OnValueChangedEvent = OnSearchInputChanged;
		m_InputField.m_OnEditEndEvent = OnSearchEditEnd;
		m_DropDown = base.transform.Find("SearchDropdown").gameObject;
		m_DropDown.SetActive(value: false);
		m_Blocker = base.transform.Find("Blocker").GetComponent<BaseImage>();
		m_Blocker.SetAction(OnBlockerClicked, m_Blocker);
		m_Blocker.SetActive(Active: false);
		m_Template = m_DropDown.transform.Find("Template").GetComponent<RectTransform>();
		m_DefaultToggle = m_DropDown.transform.Find("Template/Viewport/Content/Item").GetComponent<BaseAutoCompleteOption>();
		m_ToggleHeight = m_DefaultToggle.GetComponent<RectTransform>().rect.height;
		m_DefaultToggle.gameObject.SetActive(value: false);
		m_PossibleToggles = new List<BaseAutoCompleteOption>();
		SetMaxToggles(50);
	}

	public void SetObjectsCallback(GetPossibleSearchObjects GetPossibleSearchObjects)
	{
		m_GetPossibleSearchObjects = GetPossibleSearchObjects;
	}

	public void SetActive(bool Active)
	{
		base.gameObject.SetActive(Active);
	}

	protected void DoAction()
	{
		if (m_Action != null && !m_Action.Target.Equals(null))
		{
			m_Action(m_ActionGadget);
		}
	}

	public virtual void SetAction(Action<BaseAutoComplete> NewAction, BaseAutoComplete NewGadget)
	{
		m_Action = NewAction;
		m_ActionGadget = NewGadget;
	}

	private void SetMaxToggles(int NewMax)
	{
		if (NewMax > m_PossibleToggles.Count)
		{
			Transform parent = m_DefaultToggle.transform.parent;
			for (int i = m_PossibleToggles.Count; i < NewMax; i++)
			{
				BaseAutoCompleteOption baseAutoCompleteOption = UnityEngine.Object.Instantiate(m_DefaultToggle, parent);
				baseAutoCompleteOption.gameObject.SetActive(value: false);
				baseAutoCompleteOption.SetAction(OnToggleClicked, baseAutoCompleteOption);
				m_PossibleToggles.Add(baseAutoCompleteOption);
			}
		}
	}

	private void SetDropdownObjects(List<BaseAutoCompletePossible> PossibleTypes)
	{
		foreach (BaseAutoCompleteOption possibleToggle in m_PossibleToggles)
		{
			possibleToggle.gameObject.SetActive(value: false);
		}
		float num = (float)PossibleTypes.Count * m_ToggleHeight;
		m_DropDown.transform.Find("Template/Viewport/Content").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
		num += 5f;
		if (num > 150f)
		{
			num = 150f;
		}
		if (num < 10f)
		{
			num = 10f;
		}
		m_Template.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
		SetMaxToggles(PossibleTypes.Count);
		int num2 = 0;
		m_ToggleY = (0f - m_ToggleHeight) * 0.5f;
		foreach (BaseAutoCompletePossible PossibleType in PossibleTypes)
		{
			BaseAutoCompleteOption baseAutoCompleteOption = m_PossibleToggles[num2];
			baseAutoCompleteOption.SetText(PossibleType.m_Name);
			baseAutoCompleteOption.transform.localPosition = new Vector2(0f, m_ToggleY);
			m_ToggleY -= m_ToggleHeight;
			baseAutoCompleteOption.gameObject.SetActive(value: true);
			num2++;
		}
	}

	public void OnBlockerClicked(BaseGadget NewGadget)
	{
		m_DropDown.SetActive(value: false);
		m_Blocker.SetActive(Active: false);
	}

	public void OnSearchInputChanged(BaseGadget NewGadget)
	{
		m_PossibleObjects = m_GetPossibleSearchObjects(m_InputField.GetText());
		m_DropDown.SetActive(value: true);
		m_Blocker.SetActive(Active: true);
		SetDropdownObjects(m_PossibleObjects);
	}

	public void OnSearchEditEnd(BaseGadget NewGadget)
	{
	}

	private void SetCurrentObject(int Index)
	{
		m_CurrentObject = m_PossibleObjects[Index].m_Extra;
		string text = m_PossibleObjects[Index].m_Name;
		m_InputField.SetText(text);
		m_InputField.SetPlaceholderText(text);
		m_DropDown.SetActive(value: false);
		m_Blocker.SetActive(Active: false);
		DoAction();
	}

	public void OnToggleClicked(BaseGadget NewToggle)
	{
		m_RequestClose = 0;
		BaseAutoCompleteOption component = NewToggle.GetComponent<BaseAutoCompleteOption>();
		int currentObject = m_PossibleToggles.IndexOf(component);
		SetCurrentObject(currentObject);
	}

	private void CheckNameMatch()
	{
		string text = m_InputField.GetText().ToUpper().TrimStart(' ')
			.TrimEnd(' ');
		for (int i = 0; i < m_PossibleObjects.Count; i++)
		{
			if (m_PossibleObjects[i].m_Name == text)
			{
				SetCurrentObject(i);
				break;
			}
		}
	}

	public object GetCurrentObject()
	{
		return m_CurrentObject;
	}

	public void ForceFocus()
	{
		m_InputField.ForceFocus();
	}

	private void Update()
	{
		if (m_RequestClose != 0)
		{
			m_RequestClose--;
			if (m_RequestClose == 0)
			{
				m_DropDown.SetActive(value: false);
				m_Blocker.SetActive(Active: false);
				m_RequestClose = 0;
			}
		}
	}
}
