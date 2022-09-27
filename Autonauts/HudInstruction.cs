using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HudInstruction : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Color m_Colour;

	[HideInInspector]
	public bool m_Dragging;

	private Vector3 m_DragMouseOffset;

	[HideInInspector]
	public bool m_Static;

	protected TeachWorkerScriptEdit m_Parent;

	public HighInstruction m_Instruction;

	private bool m_Invalid;

	public GameObject m_ObjectSelect;

	public GameObject m_EditArea;

	private float m_EditAreaTimer;

	private bool m_EditAreaFlash;

	private bool m_Selected;

	private GameObject m_InputField;

	private GameObject m_TextInputField;

	private GameObject m_FillToggle;

	protected void Awake()
	{
		m_Dragging = false;
		m_Static = false;
		m_Parent = null;
		m_Invalid = false;
		m_Selected = false;
	}

	private void OnDestroy()
	{
		if (m_Instruction != null)
		{
			m_Instruction.m_HudParent = null;
		}
	}

	public void SetParent(TeachWorkerScriptEdit Parent)
	{
		m_Parent = Parent;
		m_ObjectSelect = base.transform.Find("ObjectSelect").gameObject;
		if ((bool)base.transform.Find("EditArea"))
		{
			m_EditArea = base.transform.Find("EditArea").gameObject;
		}
		m_InputField = base.transform.Find("InputField").gameObject;
		m_TextInputField = base.transform.Find("TextInputField").gameObject;
		if ((bool)base.transform.Find("FillToggle"))
		{
			m_FillToggle = base.transform.Find("FillToggle").gameObject;
		}
	}

	public void ScaleAreaIndicator(bool Up)
	{
		AreaIndicator areaIndicatorFromInstruction = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(m_Instruction);
		if ((bool)areaIndicatorFromInstruction)
		{
			areaIndicatorFromInstruction.Scale(Up);
		}
	}

	public void CancelScale()
	{
		AreaIndicator areaIndicatorFromInstruction = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(m_Instruction);
		if ((bool)areaIndicatorFromInstruction)
		{
			areaIndicatorFromInstruction.CancelScale();
		}
	}

	public void SetVisible(bool Visible)
	{
		AreaIndicator areaIndicatorFromInstruction = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(m_Instruction);
		if ((bool)areaIndicatorFromInstruction)
		{
			areaIndicatorFromInstruction.SetVisible(Visible);
		}
	}

	public virtual void SetStatic()
	{
		m_Static = true;
	}

	public virtual void SetColour(Color NewColour)
	{
		GetComponent<Image>().color = NewColour;
	}

	private string GetName()
	{
		if (m_Instruction.m_Type == HighInstruction.Type.IfElse && m_Static)
		{
			return TextManager.Instance.Get("InstructionIfElseStatic");
		}
		return m_Instruction.GetHumanReadableInstruction();
	}

	public virtual void SetInstruction(HighInstruction NewInstruction)
	{
		m_ObjectSelect.SetActive(value: false);
		if ((bool)m_EditArea)
		{
			m_EditArea.SetActive(value: false);
		}
		m_Instruction = NewInstruction;
		NewInstruction.m_HudParent = this;
		if (NewInstruction.m_ActionInfo == null)
		{
			NewInstruction.m_ActionInfo = new ActionInfo(ActionType.Total, default(TileCoord));
		}
		HighInstructionInfo obj = HighInstruction.m_Info[(int)m_Instruction.m_Type];
		base.transform.Find("Text").GetComponent<BaseText>().SetText(GetName());
		HighInstructionInfo.Category category = obj.m_Category;
		m_Colour = HighInstructionInfo.m_CategoryInfo[(int)category].m_Colour;
		SetColour(m_Colour);
		UpdateObjectedSelect();
		UpdateWidth();
		UpdateFillToggleImage();
	}

	private void UpdateObjectedSelect()
	{
		if ((bool)GetComponent<HudInstructionChildren>())
		{
			return;
		}
		m_ObjectSelect.SetActive(value: false);
		if ((bool)m_FillToggle)
		{
			m_FillToggle.SetActive(value: false);
		}
		if ((bool)m_EditArea)
		{
			m_EditArea.SetActive(value: false);
		}
		m_InputField.SetActive(value: false);
		m_TextInputField.SetActive(value: false);
		if (!m_Static)
		{
			if (IsObjectSelectAvailable())
			{
				m_ObjectSelect.SetActive(value: true);
			}
			if ((bool)m_FillToggle && IsFillHandsAvailable())
			{
				m_FillToggle.SetActive(value: true);
			}
			if ((bool)m_EditArea && m_Instruction.GetIsGetNearest())
			{
				m_EditArea.SetActive(value: true);
			}
			if (m_Instruction.m_Type == HighInstruction.Type.Wait)
			{
				m_InputField.SetActive(value: true);
				m_InputField.GetComponent<InputField>().text = m_Instruction.m_ActionInfo.m_Value;
			}
			if (m_Instruction.m_Type == HighInstruction.Type.Shout)
			{
				m_TextInputField.SetActive(value: true);
				m_TextInputField.GetComponent<InputField>().text = m_Instruction.m_ActionInfo.m_Value;
			}
		}
		UpdateObjectSelectImage();
	}

	protected void UpdateObjectSelectImage()
	{
		string text = "ObjectSelector";
		if (m_Instruction.m_ActionInfo.m_ObjectUID != 0 && (bool)ObjectTypeList.Instance.GetObjectFromUniqueID(m_Instruction.m_ActionInfo.m_ObjectUID, ErrorCheck: false))
		{
			text = "ObjectSelectorSelected";
		}
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Script/" + text, typeof(Sprite));
		m_ObjectSelect.GetComponent<Image>().sprite = sprite;
	}

	protected virtual void UpdateWidth()
	{
		float num = 20f;
		num += GetComponentInChildren<BaseText>().m_Text.preferredWidth;
		if (m_ObjectSelect.gameObject.activeSelf)
		{
			num += m_ObjectSelect.GetComponent<RectTransform>().rect.width;
			num += 10f;
		}
		if ((bool)m_FillToggle && m_FillToggle.gameObject.activeSelf)
		{
			if (m_ObjectSelect.gameObject.activeSelf)
			{
				float num2 = m_ObjectSelect.GetComponent<RectTransform>().rect.width + 10f;
				Vector2 anchoredPosition = m_FillToggle.GetComponent<RectTransform>().anchoredPosition;
				anchoredPosition.x -= num2;
				m_FillToggle.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			}
			num += m_FillToggle.GetComponent<RectTransform>().rect.width;
			num += 10f;
		}
		if ((bool)m_EditArea && m_EditArea.gameObject.activeSelf)
		{
			num += m_EditArea.GetComponent<RectTransform>().rect.width;
			num += 10f;
		}
		if (m_InputField.gameObject.activeSelf)
		{
			num += m_InputField.GetComponent<RectTransform>().rect.width;
		}
		if (m_TextInputField.gameObject.activeSelf)
		{
			num += m_TextInputField.GetComponent<RectTransform>().rect.width;
		}
		float height = base.transform.GetComponent<RectTransform>().rect.height;
		base.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(num, height);
	}

	public virtual void Refresh()
	{
		UpdateObjectedSelect();
		UpdateFillToggleImage();
	}

	public virtual void SetInvalidPosition(bool Invalid)
	{
		m_Invalid = Invalid;
		if (Invalid)
		{
			SetColour(new Color(1f, 0f, 0f));
			return;
		}
		Color colour = (new Color(1f, 1f, 1f) - m_Colour) * 0.5f + m_Colour;
		SetColour(colour);
	}

	public bool GetInvalid()
	{
		return m_Invalid;
	}

	private void HighLightObject(bool Highlight)
	{
		BaseClass baseClass = null;
		if (m_Instruction.m_ActionInfo.m_ObjectUID != 0)
		{
			baseClass = ObjectTypeList.Instance.GetObjectFromUniqueID(m_Instruction.m_ActionInfo.m_ObjectUID, ErrorCheck: false);
		}
		if (m_Instruction.GetIsGetNearest())
		{
			m_Parent.SetPointAtObjectSearch(baseClass, Highlight);
		}
		else if (m_Instruction.m_Type == HighInstruction.Type.MoveTo || m_Instruction.m_Type == HighInstruction.Type.MoveToLessOne || m_Instruction.m_Type == HighInstruction.Type.MoveToRange || m_Instruction.m_Type == HighInstruction.Type.AddResource || m_Instruction.m_Type == HighInstruction.Type.TakeResource)
		{
			if ((bool)baseClass)
			{
				if (Highlight)
				{
					if (Vehicle.GetIsTypeVehicle(baseClass.m_TypeIdentifier))
					{
						HudManager.Instance.PointToTile(m_Instruction.m_ActionInfo.m_Position);
					}
					else
					{
						HudManager.Instance.PointToObject(baseClass);
					}
				}
				else
				{
					HudManager.Instance.StopPointingToObject(baseClass);
					HudManager.Instance.PointToTile(new TileCoord(-1, -1));
				}
			}
			else
			{
				bool flag = m_Instruction.m_ActionInfo.m_Action == ActionType.MoveTo || m_Instruction.m_ActionInfo.m_Action == ActionType.MoveToLessOne || m_Instruction.m_ActionInfo.m_Action == ActionType.MoveToRange;
				if (Highlight && flag && (m_Instruction.m_Type == HighInstruction.Type.MoveTo || m_Instruction.m_Type == HighInstruction.Type.MoveToLessOne || m_Instruction.m_Type == HighInstruction.Type.MoveToRange))
				{
					HudManager.Instance.PointToTile(m_Instruction.m_ActionInfo.m_Position);
				}
				else
				{
					HudManager.Instance.PointToTile(new TileCoord(-1, -1));
				}
			}
		}
		else if ((bool)baseClass)
		{
			if (Highlight && (m_Instruction.GetIsGetNearest() || ((m_Instruction.m_Type == HighInstruction.Type.Repeat || m_Instruction.m_Type == HighInstruction.Type.If || m_Instruction.m_Type == HighInstruction.Type.IfElse) && (m_Instruction.m_Argument == "RepeatBuildingFull" || m_Instruction.m_Argument == "RepeatBuildingNotFull" || m_Instruction.m_Argument == "RepeatBuildingEmpty" || m_Instruction.m_Argument == "RepeatBuildingNotEmpty" || m_Instruction.m_Argument == "IfBuildingFull" || m_Instruction.m_Argument == "IfBuildingNotFull" || m_Instruction.m_Argument == "IfBuildingEmpty" || m_Instruction.m_Argument == "IfBuildingNotEmpty"))))
			{
				HudManager.Instance.PointToObject(baseClass);
				return;
			}
			HudManager.Instance.StopPointingToObject(baseClass);
			HudManager.Instance.PointToTile(new TileCoord(-1, -1));
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (m_Parent.IsDragging())
		{
			return;
		}
		m_Parent.InstructionHover(this, Hover: true);
		if (!m_Parent.IsDragging())
		{
			AudioManager.Instance.StartEvent("ScriptingInstructionIndicated");
			if (!m_Selected)
			{
				SetColour(m_Colour * new Color(0.75f, 0.75f, 0.75f));
			}
			HighLightObject(Highlight: true);
			AreaIndicatorManager.Instance.ActivateInstructionArea(m_Instruction, Active: true);
			HudManager.Instance.ActivateUIRollover(Activate: true, TextManager.Instance.Get("ScriptInstruction"), default(Vector3));
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (m_Parent.IsDragging())
		{
			return;
		}
		m_Parent.InstructionHover(this, Hover: false);
		if (!m_Parent.IsDragging())
		{
			if (!m_Selected)
			{
				SetColour(m_Colour);
			}
			HighLightObject(Highlight: false);
			AreaIndicatorManager.Instance.ActivateInstructionArea(m_Instruction, Active: false);
			HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			m_Parent.InstructionHover(this, Hover: false);
			m_Parent.OnInstructionClick(this);
			if (!m_Selected)
			{
				SetColour(m_Colour);
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Right && !m_Parent.IsDragging() && !m_Instruction.m_HudParent.m_Static)
		{
			OnPointerExit(eventData);
			m_Parent.InstructionDelete(this);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (!m_Static)
			{
				m_Dragging = false;
			}
		}
		else
		{
			_ = eventData.button;
			_ = 1;
		}
	}

	public void SetDragging(bool Dragging)
	{
		if (Dragging)
		{
			Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
			base.transform.SetParent(menusRootTransform);
		}
		else
		{
			Transform parent = m_Parent.m_ScrollView.GetContent().transform;
			base.transform.SetParent(parent);
		}
		m_Dragging = Dragging;
		GetComponent<CanvasGroup>().interactable = GetComponent<Image>().raycastTarget;
		GetComponent<CanvasGroup>().blocksRaycasts = GetComponent<Image>().raycastTarget;
		if (Dragging)
		{
			Color colour = (new Color(1f, 1f, 1f) - m_Colour) * 0.5f + m_Colour;
			SetColour(colour);
		}
		else
		{
			SetColour(m_Colour);
		}
	}

	public virtual void SetSelected(bool Selected)
	{
		m_Selected = Selected;
		if (m_Selected)
		{
			Color colour = (new Color(1f, 1f, 1f) - m_Colour) * 0.5f + m_Colour;
			SetColour(colour);
		}
		else
		{
			SetColour(m_Colour);
		}
	}

	public void EnableInteraction(bool Interaction)
	{
		GetComponent<CanvasGroup>().interactable = Interaction;
		GetComponent<CanvasGroup>().blocksRaycasts = Interaction;
		if (m_Static)
		{
			GetComponent<Button>().interactable = Interaction;
			ColorBlock colors = GetComponent<Button>().colors;
			Color disabledColor = colors.disabledColor;
			disabledColor.a = 0.25f;
			colors.disabledColor = disabledColor;
			GetComponent<Button>().colors = colors;
		}
	}

	public virtual float GetHeight(bool IncludeChildren2 = true)
	{
		return GetComponent<RectTransform>().rect.height;
	}

	public virtual float GetWidth()
	{
		return GetComponent<RectTransform>().rect.width;
	}

	protected virtual void ObjectSelectPressed()
	{
		HighLightObject(Highlight: false);
		if (m_Instruction.GetIsGetNearest())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.SelectObject);
			List<ObjectType> list = new List<ObjectType>();
			list.Add(ObjectType.Sign);
			list.Add(ObjectType.Sign2);
			list.Add(ObjectType.Sign3);
			list.Add(ObjectType.Billboard);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectObject>().SetSearchType(list);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectObject>().SetInstruction(this);
		}
		if (m_Instruction.m_ActionInfo.m_ObjectUID == 0 || (m_Instruction.m_Type != HighInstruction.Type.MoveTo && m_Instruction.m_Type != HighInstruction.Type.MoveToLessOne && m_Instruction.m_Type != HighInstruction.Type.MoveToRange && m_Instruction.m_Type != HighInstruction.Type.AddResource && m_Instruction.m_Type != HighInstruction.Type.TakeResource))
		{
			return;
		}
		GameStateManager.Instance.PushState(GameStateManager.State.SelectObject);
		List<ObjectType> list2 = new List<ObjectType>();
		if (m_Instruction.m_Type != HighInstruction.Type.TakeResource)
		{
			foreach (ObjectType type in Converter.m_Types)
			{
				if (type != ObjectType.ConverterFoundation)
				{
					list2.Add(type);
				}
			}
			list2.Add(ObjectType.StoneHeads);
			list2.Add(ObjectType.ResearchStationCrude);
			list2.Add(ObjectType.ResearchStationCrude2);
			list2.Add(ObjectType.ResearchStationCrude3);
			list2.Add(ObjectType.ResearchStationCrude4);
			list2.Add(ObjectType.ResearchStationCrude5);
			list2.Add(ObjectType.ResearchStationCrude6);
			list2.Add(ObjectType.StationaryEngine);
			list2.Add(ObjectType.Aquarium);
			list2.Add(ObjectType.AquariumGood);
			list2.Add(ObjectType.SpacePort);
		}
		foreach (ObjectType type2 in Storage.m_Types)
		{
			list2.Add(type2);
		}
		list2.Add(ObjectType.WheelBarrow);
		list2.Add(ObjectType.Cart);
		list2.Add(ObjectType.CartLiquid);
		list2.Add(ObjectType.Carriage);
		list2.Add(ObjectType.CarriageLiquid);
		list2.Add(ObjectType.CarriageTrain);
		list2.Add(ObjectType.TrainRefuellingStation);
		list2.Add(ObjectType.SilkwormStation);
		list2.Add(ObjectType.TrojanRabbit);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectObject>().SetSearchType(list2);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectObject>().SetInstruction(this);
	}

	public virtual void OnObjectSelect()
	{
		if (!m_Dragging)
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			ObjectSelectPressed();
		}
	}

	private void EditActivePressed()
	{
		QuestManager.Instance.AddEvent(QuestEvent.Type.EditSearchArea, Bot: false, 0, null);
		SetColour(m_Colour);
		HighLightObject(Highlight: false);
		ResetEditArea();
		GameStateManager.Instance.PushState(GameStateManager.State.EditArea);
		AreaIndicator areaIndicatorFromInstruction = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(m_Instruction);
		if (areaIndicatorFromInstruction == null)
		{
			TeachWorkerScriptEdit.Instance.RemakeAreaIndicators();
			areaIndicatorFromInstruction = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(m_Instruction);
		}
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEditArea>().SetIndicator(areaIndicatorFromInstruction, m_Parent.m_CurrentTarget.GetTotalSearchRange(), m_Instruction);
	}

	public void OnEditActive()
	{
		if (!m_Dragging)
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			EditActivePressed();
		}
	}

	public void OnEditEnter()
	{
		if (!m_Dragging)
		{
			float num = 1.25f;
			m_EditArea.transform.localScale = new Vector3(num, num, num);
			AudioManager.Instance.StartEvent("ScriptingInstructionAreaIndicated");
			HudManager.Instance.ActivateUIRollover(Activate: true, TextManager.Instance.Get("ScriptEditAreaButton"), default(Vector3));
		}
	}

	private void ResetEditArea()
	{
		float num = 1f;
		m_EditArea.transform.localScale = new Vector3(num, num, num);
	}

	public void OnEditExit()
	{
		ResetEditArea();
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	private bool IsObjectSelectAvailable()
	{
		if ((bool)m_Parent && (bool)m_Parent.m_CurrentTarget && (WorkerHeadMk0.GetIsTypeHeadMk0(m_Parent.m_CurrentTarget.m_Head) || WorkerHeadMk1.GetIsTypeHeadMk1(m_Parent.m_CurrentTarget.m_Head)))
		{
			return false;
		}
		return m_Instruction.IsObjectSelectAvailable();
	}

	private bool IsFillHandsAvailable()
	{
		return false;
	}

	public virtual void OnInputFieldChanged()
	{
		string text = m_InputField.GetComponent<InputField>().text;
		if (m_Instruction.m_Type == HighInstruction.Type.Wait)
		{
			if (float.TryParse(text, out var result))
			{
				if (result < 0f)
				{
					result = 0f;
				}
			}
			else
			{
				result = 1f;
			}
			text = result.ToString();
			m_InputField.GetComponent<InputField>().text = text;
		}
		m_Instruction.m_ActionInfo.m_Value = text;
	}

	public virtual void OnTextInputFieldChanged()
	{
		string text = m_TextInputField.GetComponent<InputField>().text;
		m_Instruction.m_ActionInfo.m_Value = text;
	}

	public virtual void SetAssociatedObject(TileCoordObject AssociatedObject)
	{
		if (m_Instruction.m_Type == HighInstruction.Type.AddResource || m_Instruction.m_Type == HighInstruction.Type.TakeResource || m_Instruction.m_Type == HighInstruction.Type.MoveTo || m_Instruction.m_Type == HighInstruction.Type.MoveToLessOne || m_Instruction.m_Type == HighInstruction.Type.MoveToRange)
		{
			if (AssociatedObject != null)
			{
				m_Instruction.m_ActionInfo.m_ObjectUID = AssociatedObject.m_UniqueID;
				m_Instruction.m_ActionInfo.m_ObjectType = AssociatedObject.m_TypeIdentifier;
				if ((bool)AssociatedObject.GetComponent<Building>())
				{
					m_Instruction.m_ActionInfo.m_Position = AssociatedObject.GetComponent<Building>().GetAccessPosition();
				}
			}
		}
		else if (AssociatedObject == null)
		{
			m_Instruction.m_ActionInfo.m_ObjectUID = 0;
		}
		else
		{
			m_Instruction.m_ActionInfo.m_ObjectUID = AssociatedObject.m_UniqueID;
		}
		m_Parent.RefreshInstructions();
		if (TutorialPanelController.Instance.GetActive())
		{
			TutorialScriptManager.Instance.TeachingInstructionsChanged();
		}
	}

	public void SetFlashEditArea(bool Flash)
	{
		m_EditAreaFlash = Flash;
		if ((bool)m_EditArea)
		{
			m_EditArea.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		}
	}

	private void UpdateFlashEditArea()
	{
		if (m_EditAreaFlash && !(m_EditArea == null))
		{
			m_EditAreaTimer += TimeManager.Instance.m_NormalDelta;
			float a = 1f;
			if ((int)(m_EditAreaTimer * 60f) % 20 < 10)
			{
				a = 0.2f;
			}
			m_EditArea.GetComponent<Image>().color = new Color(1f, 1f, 1f, a);
		}
	}

	protected void UpdateFillToggleImage()
	{
		if (m_FillToggle == null)
		{
			return;
		}
		if (m_Parent == null || m_Parent.m_CurrentTarget == null)
		{
			m_FillToggle.SetActive(value: false);
			return;
		}
		string text = "FailToggleOff";
		if (m_Instruction.m_ActionInfo.m_Value2 != "")
		{
			text = "FillToggleOn";
		}
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Script/" + text, typeof(Sprite));
		m_FillToggle.GetComponent<Image>().sprite = sprite;
		if (!WorkerFrameMk3.GetIsTypeFrameMk3(m_Parent.m_CurrentTarget.m_Frame))
		{
			m_FillToggle.SetActive(value: false);
		}
	}

	public void OnFillToggleChanged()
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
			UpdateFillToggleImage();
		}
	}

	public void OnFillEnter()
	{
		if (!m_Dragging)
		{
			string text = "ScriptFillHandsButton";
			if (m_Instruction.m_Type == HighInstruction.Type.AddResource)
			{
				text = ((!m_Instruction.m_ActionInfo.m_Object || !Converter.GetIsTypeConverter(m_Instruction.m_ActionInfo.m_Object.m_TypeIdentifier)) ? "ScriptEmptyHandsButton" : "ScriptLoadConverterButton");
			}
			HudManager.Instance.ActivateUIRollover(Activate: true, TextManager.Instance.Get(text), default(Vector3));
		}
	}

	public void OnFillExit()
	{
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	private void Update()
	{
		UpdateFlashEditArea();
	}
}
