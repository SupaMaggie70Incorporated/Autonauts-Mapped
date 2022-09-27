using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudInstructionChildren : HudInstruction
{
	private GameObject m_Title;

	private GameObject m_BottomPanel;

	protected GameObject m_MiddlePanel;

	public BaseDropdown m_Dropdown;

	private GameObject m_InputField2;

	private GameObject m_TextInputField2;

	private GameObject m_FailToggle;

	private int m_Value;

	private HighInstruction.ConditionType m_ConditionType;

	private List<HighInstruction.ConditionType> m_DropDownOptions;

	private int[] m_DropDownIndex;

	private bool m_FixDropdownWidth;

	protected new void Awake()
	{
		base.Awake();
		m_Title = base.transform.Find("Text").gameObject;
		m_BottomPanel = base.transform.Find("BottomPanel").gameObject;
		m_MiddlePanel = base.transform.Find("MiddlePanel").gameObject;
		m_Dropdown = base.transform.Find("Dropdown").GetComponent<BaseDropdown>();
		m_InputField2 = base.transform.Find("InputField").gameObject;
		m_TextInputField2 = base.transform.Find("TextInputField").gameObject;
		m_FailToggle = base.transform.Find("FailToggle").gameObject;
		m_Value = -1;
		m_ConditionType = HighInstruction.ConditionType.Forever;
	}

	private void OnDestroy()
	{
	}

	protected void UpdateFailToggleImage()
	{
		string text = "FailToggleOff";
		if (m_Instruction.m_ActionInfo.m_Value2 != "")
		{
			text = "FailToggleOn";
		}
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Script/" + text, typeof(Sprite));
		m_FailToggle.GetComponent<Image>().sprite = sprite;
	}

	public override void SetInvalidPosition(bool Invalid)
	{
		base.SetInvalidPosition(Invalid);
		foreach (HighInstruction child in m_Instruction.m_Children)
		{
			child.m_HudParent.SetInvalidPosition(Invalid);
		}
	}

	public override void SetInstruction(HighInstruction NewInstruction)
	{
		base.SetInstruction(NewInstruction);
		m_InputField2.SetActive(value: false);
		m_TextInputField2.SetActive(value: false);
		if (m_Instruction.m_Type == HighInstruction.Type.If || m_Instruction.m_Type == HighInstruction.Type.IfElse)
		{
			float num = m_FailToggle.GetComponent<RectTransform>().rect.width + 5f;
			Vector2 anchoredPosition = m_ObjectSelect.GetComponent<RectTransform>().anchoredPosition;
			anchoredPosition.x += num;
			m_ObjectSelect.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			anchoredPosition = m_TextInputField2.GetComponent<RectTransform>().anchoredPosition;
			anchoredPosition.x += num;
			m_TextInputField2.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			m_FailToggle.gameObject.SetActive(value: false);
		}
		m_DropDownOptions = new List<HighInstruction.ConditionType>();
		if (m_Instruction.m_Type == HighInstruction.Type.Repeat || m_Instruction.m_Type == HighInstruction.Type.If || m_Instruction.m_Type == HighInstruction.Type.IfElse)
		{
			m_Dropdown.ClearOptions();
			List<string> list = new List<string>();
			HighInstruction.ConditionType[] array = ((m_Instruction.m_Type != HighInstruction.Type.Repeat) ? HighInstruction.m_IfTypes : HighInstruction.m_RepeatTypes);
			HighInstruction.ConditionType[] array2 = array;
			foreach (HighInstruction.ConditionType conditionType in array2)
			{
				if ((conditionType != HighInstruction.ConditionType.InventoryFull && conditionType != HighInstruction.ConditionType.InventoryNotFull && conditionType != HighInstruction.ConditionType.InventoryEmpty && conditionType != HighInstruction.ConditionType.InventoryNotEmpty) || !m_Parent.m_CurrentTarget || m_Parent.m_CurrentTarget.m_FarmerCarry.m_TotalCapacity != 0)
				{
					m_DropDownOptions.Add(conditionType);
				}
			}
			m_DropDownIndex = new int[19];
			for (int j = 0; j < m_DropDownOptions.Count; j++)
			{
				string item = TextManager.Instance.Get(HighInstruction.GetNameFromConditionType(m_Instruction.m_Type, m_DropDownOptions[j]), "?");
				list.Add(item);
				m_DropDownIndex[(int)m_DropDownOptions[j]] = j;
			}
			m_Dropdown.SetOptions(list);
			if (m_Instruction.m_Argument != "")
			{
				SetDropdownValue(m_DropDownIndex[(int)HighInstruction.GetConditionTypeFromName(m_Instruction.m_Type, m_Instruction.m_Argument)]);
			}
			else
			{
				SetDropdownValue(1);
				SetDropdownValue(0);
			}
			m_ConditionType = m_DropDownOptions[m_Dropdown.GetValue()];
		}
		else if (m_Instruction.m_Type == HighInstruction.Type.Forever)
		{
			m_Dropdown.gameObject.SetActive(value: false);
		}
		if (m_ConditionType == HighInstruction.ConditionType.Times || m_ConditionType == HighInstruction.ConditionType.Hear)
		{
			m_TextInputField2.GetComponent<InputField>().text = m_Instruction.m_ActionInfo.m_Value;
		}
		UpdateFailToggleImage();
		UpdateWidth();
	}

	public override void SetColour(Color NewColour)
	{
		base.SetColour(NewColour);
		m_BottomPanel.GetComponent<Image>().color = NewColour;
		m_MiddlePanel.GetComponent<Image>().color = NewColour;
	}

	protected override void UpdateWidth()
	{
		float num = 20f;
		num += m_Dropdown.GetLabelWidth();
		float height = m_Dropdown.GetComponent<RectTransform>().rect.height;
		num += m_Dropdown.transform.Find("Arrow").GetComponent<RectTransform>().rect.width;
		m_Dropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(num, height);
		num = 20f;
		num += m_Title.GetComponent<BaseText>().m_Text.preferredWidth;
		if (num < 40f)
		{
			num = 40f;
		}
		Vector3 localPosition = m_Dropdown.transform.localPosition;
		localPosition.x = num - 8f;
		m_Dropdown.transform.localPosition = localPosition;
		num += m_Dropdown.GetComponent<RectTransform>().rect.width;
		if (m_FailToggle.activeSelf)
		{
			num += m_FailToggle.GetComponent<RectTransform>().rect.width + 5f;
		}
		if (m_ObjectSelect.gameObject.activeSelf)
		{
			num += m_ObjectSelect.GetComponent<RectTransform>().rect.width;
			num += 10f;
		}
		if (m_InputField2.gameObject.activeSelf)
		{
			num += m_InputField2.GetComponent<RectTransform>().rect.width;
		}
		if (m_TextInputField2.gameObject.activeSelf)
		{
			num += m_TextInputField2.GetComponent<RectTransform>().rect.width;
		}
		height = base.gameObject.GetComponent<RectTransform>().rect.height;
		base.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(num, height);
		UpdateBottomPanelWidth();
	}

	protected virtual void UpdateBottomPanelWidth()
	{
		Rect rect = base.transform.GetComponent<RectTransform>().rect;
		m_BottomPanel.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, rect.height);
	}

	public override void Refresh()
	{
		base.Refresh();
		OnValueChanged();
		float num = GetHeight(IncludeChildren2: false) - 30f;
		Vector3 localPosition = new Vector3(0f, 0f - num, 0f);
		m_BottomPanel.transform.localPosition = localPosition;
		Rect rect = m_MiddlePanel.transform.GetComponent<RectTransform>().rect;
		m_MiddlePanel.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.width, num - 30f + 12f);
		m_MiddlePanel.transform.localPosition = new Vector3(0f, (0f - (num - 30f)) / 2f - 30f, 0f);
		UpdateFailToggleImage();
	}

	public override float GetHeight(bool IncludeChildren2 = true)
	{
		float num = GetComponent<RectTransform>().rect.height + m_BottomPanel.GetComponent<RectTransform>().rect.height;
		foreach (HighInstruction child in m_Instruction.m_Children)
		{
			num += child.m_HudParent.GetHeight();
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
		return num;
	}

	public void OnValueChanged()
	{
		int value = m_Dropdown.GetValue();
		if (value == m_Value)
		{
			return;
		}
		SetDropdownValue(value);
		if (!m_Static)
		{
			if (m_ConditionType == HighInstruction.ConditionType.BuildingFull)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.UntilBuildingFullChosen, Bot: false, null, null);
			}
			if (m_ConditionType == HighInstruction.ConditionType.HandsEmpty)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.UntilHandsEmptyChosen, Bot: false, null, null);
			}
		}
		if (TutorialPanelController.Instance.GetActive())
		{
			TutorialScriptManager.Instance.TeachingInstructionsChanged();
		}
		TeachWorkerScriptEdit.Instance.UpdateContentSize();
	}

	private void SetDropdownValue(int Value)
	{
		m_Value = Value;
		m_Dropdown.SetValue(Value);
		m_ConditionType = m_DropDownOptions[Value];
		m_Instruction.m_Argument = HighInstruction.GetNameFromConditionType(m_Instruction.m_Type, m_ConditionType);
		m_ObjectSelect.SetActive(value: false);
		m_InputField2.SetActive(value: false);
		m_TextInputField2.SetActive(value: false);
		if (!m_Static)
		{
			if (HighInstruction.GetConditionRequireObject(m_ConditionType))
			{
				m_ObjectSelect.SetActive(value: true);
			}
			if (m_ConditionType == HighInstruction.ConditionType.Times)
			{
				m_InputField2.SetActive(value: true);
				m_InputField2.GetComponent<InputField>().text = m_Instruction.m_ActionInfo.m_Value;
			}
			if (m_ConditionType == HighInstruction.ConditionType.Hear)
			{
				m_TextInputField2.SetActive(value: true);
				m_TextInputField2.GetComponent<InputField>().text = m_Instruction.m_ActionInfo.m_Value;
			}
		}
		UpdateDropDownString();
		UpdateObjectSelectImage();
		UpdateWidth();
	}

	private void FixDropdownWidth()
	{
		Transform transform = m_Dropdown.transform.Find("Dropdown List");
		if (!transform)
		{
			return;
		}
		TextMeshProUGUI[] componentsInChildren = transform.GetComponentsInChildren<TextMeshProUGUI>();
		float num = 180f;
		TextMeshProUGUI[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			float num2 = array[i].preferredWidth + 50f;
			if (num < num2)
			{
				num = num2;
			}
		}
		Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = num;
		transform.GetComponent<RectTransform>().sizeDelta = sizeDelta;
	}

	private void UpdateDropDownString()
	{
		for (int i = 0; i < m_DropDownOptions.Count; i++)
		{
			string text = "";
			if (HighInstruction.GetConditionRequireObject(m_DropDownOptions[i]))
			{
				string val = "?";
				if (m_Instruction.m_ActionInfo.m_ObjectUID != 0)
				{
					BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_Instruction.m_ActionInfo.m_ObjectUID, ErrorCheck: false);
					if (objectFromUniqueID != null)
					{
						val = objectFromUniqueID.GetHumanReadableName();
					}
				}
				text = TextManager.Instance.Get(HighInstruction.GetNameFromConditionType(m_Instruction.m_Type, m_DropDownOptions[i]), val);
			}
			else if (m_DropDownOptions[i] == HighInstruction.ConditionType.Times)
			{
				string val2 = "?";
				if (m_Instruction.m_ActionInfo.m_Value != "")
				{
					val2 = m_Instruction.m_ActionInfo.m_Value;
				}
				text = TextManager.Instance.Get(HighInstruction.GetNameFromConditionType(m_Instruction.m_Type, m_DropDownOptions[i]), val2);
			}
			else
			{
				text = TextManager.Instance.Get(HighInstruction.GetNameFromConditionType(m_Instruction.m_Type, m_DropDownOptions[i]));
			}
			m_Dropdown.SetOption(i, text);
		}
		m_Dropdown.SetLabelText(m_Dropdown.GetOption(m_Dropdown.GetValue()));
		UpdateWidth();
	}

	public override void SetAssociatedObject(TileCoordObject AssociatedObject)
	{
		base.SetAssociatedObject(AssociatedObject);
		UpdateDropDownString();
		UpdateObjectSelectImage();
	}

	protected override void ObjectSelectPressed()
	{
		if (!HighInstruction.GetConditionRequireObject(m_ConditionType))
		{
			return;
		}
		GameStateManager.Instance.PushState(GameStateManager.State.SelectObject);
		List<ObjectType> list = new List<ObjectType>();
		foreach (ObjectType type in Storage.m_Types)
		{
			list.Add(type);
		}
		foreach (ObjectType type2 in Converter.m_Types)
		{
			if (type2 != ObjectType.ConverterFoundation)
			{
				list.Add(type2);
			}
		}
		list.Add(ObjectType.WheelBarrow);
		list.Add(ObjectType.Cart);
		list.Add(ObjectType.CartLiquid);
		list.Add(ObjectType.Carriage);
		list.Add(ObjectType.CarriageLiquid);
		list.Add(ObjectType.CarriageTrain);
		list.Add(ObjectType.CraneCrude);
		list.Add(ObjectType.StationaryEngine);
		list.Add(ObjectType.Minecart);
		list.Add(ObjectType.Train);
		list.Add(ObjectType.TrainRefuellingStation);
		list.Add(ObjectType.TrojanRabbit);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectObject>().SetSearchType(list);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectObject>().SetInstruction(this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseObjectArea, Bot: false, 0, null);
	}

	public override void OnInputFieldChanged()
	{
		string text = m_InputField2.GetComponent<InputField>().text;
		if (m_Instruction.m_Type == HighInstruction.Type.Repeat && m_ConditionType == HighInstruction.ConditionType.Times)
		{
			if (int.TryParse(text, out var result))
			{
				if (result < 1)
				{
					result = 1;
				}
			}
			else
			{
				result = 1;
			}
			text = result.ToString();
			m_InputField2.GetComponent<InputField>().text = text;
		}
		m_Instruction.m_ActionInfo.m_Value = text;
	}

	public override void OnTextInputFieldChanged()
	{
		string text = m_TextInputField2.GetComponent<InputField>().text;
		m_Instruction.m_ActionInfo.m_Value = text;
	}

	public void OnFailToggleChanged()
	{
		if (!m_Dragging)
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			if (m_Instruction.m_ActionInfo.m_Value2 == "")
			{
				m_Instruction.m_ActionInfo.m_Value2 = "1";
			}
			else
			{
				m_Instruction.m_ActionInfo.m_Value2 = "";
			}
			UpdateFailToggleImage();
		}
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		foreach (HighInstruction child in m_Instruction.m_Children)
		{
			if ((bool)child.m_HudParent)
			{
				child.m_HudParent.SetSelected(Selected);
			}
		}
	}

	public void OnFailEnter()
	{
		if (!m_Dragging)
		{
			HudManager.Instance.ActivateUIRollover(Activate: true, TextManager.Instance.Get("ScriptFailButton"), default(Vector3));
		}
	}

	public void OnFailExit()
	{
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	public override void OnObjectSelect()
	{
		if (!m_Dragging)
		{
			base.OnObjectSelect();
			ResetObject();
		}
	}

	public void OnObjectEnter()
	{
		if (!m_Dragging)
		{
			float num = 1.25f;
			m_ObjectSelect.transform.localScale = new Vector3(num, num, num);
			HudManager.Instance.ActivateUIRollover(Activate: true, TextManager.Instance.Get("ScriptObjectButton"), default(Vector3));
		}
	}

	private void ResetObject()
	{
		float num = 1f;
		m_ObjectSelect.transform.localScale = new Vector3(num, num, num);
	}

	public void OnObjectExit()
	{
		ResetObject();
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	private void Update()
	{
		FixDropdownWidth();
	}
}
