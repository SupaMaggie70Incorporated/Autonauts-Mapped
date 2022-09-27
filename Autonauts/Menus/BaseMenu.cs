using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{
	[HideInInspector]
	public BaseGadget m_NewButtonClicked;

	protected Dictionary<BaseGadget, Action<BaseGadget>> m_GadgetActions;

	protected void Awake()
	{
		Init();
	}

	protected void Init()
	{
		if (m_GadgetActions == null)
		{
			m_GadgetActions = new Dictionary<BaseGadget, Action<BaseGadget>>();
		}
	}

	private void SetActionsForArray(BaseGadget[] NewGadgets)
	{
		foreach (BaseGadget baseGadget in NewGadgets)
		{
			baseGadget.SetAction(OnGadgetClick, baseGadget);
		}
	}

	protected void Start()
	{
		BaseGadget[] componentsInChildren = GetComponentsInChildren<BaseButton>();
		SetActionsForArray(componentsInChildren);
		componentsInChildren = GetComponentsInChildren<BaseToggle>();
		SetActionsForArray(componentsInChildren);
		componentsInChildren = GetComponentsInChildren<BaseSlider>();
		SetActionsForArray(componentsInChildren);
		componentsInChildren = GetComponentsInChildren<BaseDropdown>();
		SetActionsForArray(componentsInChildren);
		componentsInChildren = GetComponentsInChildren<BaseInputField>();
		SetActionsForArray(componentsInChildren);
		BaseButtonBack componentInChildren = base.transform.GetComponentInChildren<BaseButtonBack>();
		if ((bool)componentInChildren)
		{
			RegisterGadget(componentInChildren);
			m_GadgetActions.Add(componentInChildren, OnBackClicked);
		}
	}

	public void RegisterGadget(BaseGadget NewGadget)
	{
		NewGadget.SetAction(OnGadgetClick, NewGadget);
	}

	public void AddAction(BaseGadget NewGadget, Action<BaseGadget> NewAction)
	{
		m_GadgetActions.Add(NewGadget, NewAction);
	}

	public void RemoveAction(BaseGadget NewGadget)
	{
		if (m_GadgetActions.ContainsKey(NewGadget))
		{
			m_GadgetActions.Remove(NewGadget);
		}
	}

	public void OnGadgetClick(BaseGadget Target)
	{
		m_NewButtonClicked = Target;
	}

	public virtual void OnBackClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	protected void Update()
	{
		if (!(m_NewButtonClicked == null))
		{
			if (m_GadgetActions.ContainsKey(m_NewButtonClicked))
			{
				m_GadgetActions[m_NewButtonClicked](m_NewButtonClicked);
			}
			if (m_NewButtonClicked.m_OnClickEvent != null)
			{
				m_NewButtonClicked.m_OnClickEvent.Invoke(m_NewButtonClicked);
			}
			m_NewButtonClicked = null;
		}
	}
}
