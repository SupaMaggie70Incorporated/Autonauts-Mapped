using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class MyInputManager : MonoBehaviour
{
	public static MyInputManager Instance;

	public static Player m_Rewired;

	public Dictionary<ActionElementMap, InputBinding> m_ControlsActions;

	private List<StandardAcceptButton> m_AcceptButtons;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		Debug.Log("Loading Input Manager");
		GameObject obj = (GameObject)Resources.Load("Prefabs/Rewired Input Manager", typeof(GameObject));
		if (obj == null)
		{
			Debug.Log("Can't find Input Manager prefab");
		}
		if (Object.Instantiate(obj, base.transform) == null)
		{
			Debug.Log("Couldn't instantiate Input Manager");
		}
		m_Rewired = ReInput.players.GetPlayer(0);
		if (m_Rewired == null)
		{
			Debug.Log("Couldn't get Player");
		}
		m_ControlsActions = new Dictionary<ActionElementMap, InputBinding>();
		List<ControllerMap> list = new List<ControllerMap>();
		m_Rewired.controllers.maps.GetAllMaps(ControllerType.Keyboard, list);
		foreach (ControllerMap item in list)
		{
			foreach (ActionElementMap allMap in item.AllMaps)
			{
				InputAction action = ReInput.mapping.GetAction(allMap.actionId);
				if (action != null && action.userAssignable)
				{
					InputBinding value = new InputBinding(allMap.keyCode, allMap.modifierKey1);
					m_ControlsActions.Add(allMap, value);
				}
			}
		}
		m_AcceptButtons = new List<StandardAcceptButton>();
	}

	public void AddAcceptButton(StandardAcceptButton NewButton)
	{
		m_AcceptButtons.Add(NewButton);
	}

	public void RemoveAcceptButton(StandardAcceptButton NewButton)
	{
		if (m_AcceptButtons.Contains(NewButton))
		{
			m_AcceptButtons.Remove(NewButton);
		}
	}

	public string GetKeyFromRewiredName(string RewiredName)
	{
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			if (ReInput.mapping.GetAction(key.actionId).name == RewiredName)
			{
				return ControllerBinding.GetControllerText(key);
			}
		}
		return "";
	}

	private void Update()
	{
		if (CustomStandaloneInputModule.Instance == null || CustomStandaloneInputModule.Instance.IsUIInUse() || !m_Rewired.GetButtonDown("Accept") || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
		{
			return;
		}
		for (int num = m_AcceptButtons.Count - 1; num >= 0; num--)
		{
			if (m_AcceptButtons[num].GetActive() && m_AcceptButtons[num].GetInteractable() && m_AcceptButtons[num].GetVisible())
			{
				m_AcceptButtons[num].KeyBindingPressed();
				break;
			}
		}
	}

	public bool GetCTRLHeld()
	{
		if (!Input.GetKey(KeyCode.LeftControl))
		{
			return Input.GetKey(KeyCode.RightControl);
		}
		return true;
	}
}
