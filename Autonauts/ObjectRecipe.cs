using System.Collections.Generic;
using UnityEngine;

public class ObjectRecipe : BasePanel
{
	private static float m_BlankSpacing = 5f;

	private ObjectType m_ObjectType;

	private BaseButtonImage m_BackButton;

	private ObjectTypeButton m_DefaultButton;

	private ObjectTypeButton m_ObjectButton;

	private float m_ObjectButtonWidth;

	private Vector2 m_ObjectButtonPosition;

	private List<ObjectTypeButton> m_IngredientsButtons;

	private List<BaseImage> m_Pluses;

	private ObjectTypeButton m_ConverterButton;

	private BaseButtonImage m_DownButton;

	private BaseButtonImage m_UpButton;

	private Transform m_ButtonParent;

	private BaseImage m_EqualSign;

	private BaseImage m_Converter;

	private BaseImage m_DefaultPlus;

	private float m_SymbolY;

	private Vector3 m_Position;

	private int m_NumStages;

	private int m_CurrentStage;

	protected new void Awake()
	{
		base.Awake();
		CheckGadgets();
	}

	private void CheckGadgets()
	{
		if (!m_EqualSign)
		{
			m_EqualSign = base.transform.Find("Equals").GetComponent<BaseImage>();
			m_DefaultPlus = base.transform.Find("Plus").GetComponent<BaseImage>();
			m_DefaultPlus.SetActive(Active: false);
			m_Converter = base.transform.Find("Converter").GetComponent<BaseImage>();
			m_SymbolY = m_DefaultPlus.GetPosition().y;
			m_BackButton = base.transform.Find("BackButton").GetComponent<BaseButtonImage>();
			m_BackButton.SetAction(OnBackClicked, m_BackButton);
			m_DefaultButton = base.transform.Find("ObjectButton").GetComponent<ObjectTypeButton>();
			m_ButtonParent = m_DefaultButton.transform.parent;
			m_DefaultButton.SetActive(Active: false);
			m_ObjectButton = Object.Instantiate(m_DefaultButton, m_ButtonParent);
			m_ObjectButton.SetActive(Active: true);
			m_ObjectButtonWidth = m_ObjectButton.GetWidth();
			m_ObjectButtonPosition = m_ObjectButton.GetPosition();
			m_ConverterButton = Object.Instantiate(m_DefaultButton, m_ButtonParent);
			m_ConverterButton.SetActive(Active: true);
			m_ConverterButton.SetAction(OnObjectClicked, m_ConverterButton);
			m_DownButton = base.transform.Find("DownButton").GetComponent<BaseButtonImage>();
			m_DownButton.SetAction(OnDownClicked, m_BackButton);
			m_UpButton = base.transform.Find("UpButton").GetComponent<BaseButtonImage>();
			m_UpButton.SetAction(OnUpClicked, m_BackButton);
			m_IngredientsButtons = new List<ObjectTypeButton>();
			m_Pluses = new List<BaseImage>();
		}
	}

	private IngredientRequirement[] GetIngredients(ObjectType NewType, int CurrentStage)
	{
		IngredientRequirement[] array = VariableManager.Instance.GetSpecialIngredients(NewType);
		if (array.Length == 0)
		{
			array = ObjectTypeList.Instance.GetIngredientsFromIdentifier(NewType, CurrentStage);
		}
		return array;
	}

	private ObjectType GetConverterType(ObjectType NewType)
	{
		ObjectType objectType = VariableManager.Instance.GetSpecialConverterType(NewType);
		if (objectType == ObjectTypeList.m_Total)
		{
			objectType = VariableManager.Instance.GetConverterForObject(NewType);
		}
		return objectType;
	}

	private void DestroyIngredients()
	{
		foreach (ObjectTypeButton ingredientsButton in m_IngredientsButtons)
		{
			Object.Destroy(ingredientsButton.gameObject);
		}
		m_IngredientsButtons.Clear();
		foreach (BaseImage pluse in m_Pluses)
		{
			Object.Destroy(pluse.gameObject);
		}
		m_Pluses.Clear();
	}

	private float CreateIngredients(ObjectType NewType, float x, int CurrentStage)
	{
		IngredientRequirement[] ingredients = GetIngredients(NewType, CurrentStage);
		for (int i = 0; i < ingredients.Length; i++)
		{
			IngredientRequirement ingredientRequirement = ingredients[i];
			ObjectType type = ingredientRequirement.m_Type;
			int count = ingredientRequirement.m_Count;
			ObjectTypeButton objectTypeButton = Object.Instantiate(m_DefaultButton, m_ButtonParent);
			objectTypeButton.SetActive(Active: true);
			if (type < ObjectTypeList.m_Total)
			{
				objectTypeButton.SetAction(OnObjectClicked, objectTypeButton);
			}
			objectTypeButton.Init(type, CheckNew: false);
			objectTypeButton.SetCount(count);
			objectTypeButton.SetPosition(x, m_ObjectButtonPosition.y);
			x += m_ObjectButtonWidth + m_BlankSpacing;
			m_IngredientsButtons.Add(objectTypeButton);
			if (i != ingredients.Length - 1)
			{
				BaseImage baseImage = Object.Instantiate(m_DefaultPlus, m_ButtonParent);
				baseImage.SetActive(Active: true);
				baseImage.SetPosition(x, m_SymbolY);
				x += baseImage.GetWidth() + m_BlankSpacing;
				m_Pluses.Add(baseImage);
			}
		}
		return x;
	}

	private float SetUpElements(int CurrentStage)
	{
		BaseText component = base.transform.Find("Title").GetComponent<BaseText>();
		string text = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_ObjectType);
		if (m_NumStages > 1)
		{
			string val = CurrentStage + 1 + "/" + m_NumStages;
			text += TextManager.Instance.Get("AutopediaRecipeStage", val);
		}
		component.SetText(text);
		float num = 20f;
		if (Objects.Instance.IsNavigationBackAvailable())
		{
			m_BackButton.SetActive(Active: true);
			m_BackButton.SetPosition(num, m_SymbolY);
			num += m_BackButton.GetWidth() + m_BlankSpacing;
		}
		else
		{
			m_BackButton.SetActive(Active: false);
		}
		ObjectType converterType = GetConverterType(m_ObjectType);
		if (!ObjectTypeList.Instance.GetIsBuilding(m_ObjectType) && converterType != ObjectTypeList.m_Total)
		{
			m_ConverterButton.SetActive(Active: true);
			m_ConverterButton.SetPosition(num, m_ObjectButtonPosition.y);
			m_ConverterButton.Init(converterType, CheckNew: false);
			num += m_ObjectButtonWidth + m_BlankSpacing;
			m_Converter.SetActive(Active: true);
			m_Converter.SetPosition(num, m_SymbolY);
			num += m_Converter.GetWidth() + m_BlankSpacing;
		}
		else
		{
			m_Converter.SetActive(Active: false);
			m_ConverterButton.SetActive(Active: false);
		}
		DestroyIngredients();
		IngredientRequirement[] ingredients = GetIngredients(m_ObjectType, CurrentStage);
		if (ingredients.Length != 0)
		{
			num = CreateIngredients(m_ObjectType, num, CurrentStage);
		}
		else
		{
			m_NumStages = 0;
		}
		if (ingredients.Length != 0)
		{
			m_EqualSign.SetActive(Active: true);
			m_EqualSign.SetPosition(num, m_SymbolY);
			num += m_EqualSign.GetWidth() + m_BlankSpacing;
		}
		else
		{
			m_EqualSign.SetActive(Active: false);
		}
		m_ObjectButton.SetPosition(num, m_ObjectButtonPosition.y);
		m_ObjectButton.Init(m_ObjectType, CheckNew: false);
		num += m_ObjectButtonWidth + m_BlankSpacing;
		if (m_NumStages > 1)
		{
			m_UpButton.SetActive(Active: true);
			m_DownButton.SetActive(Active: true);
			UpdateUpDownButtons();
		}
		else
		{
			m_UpButton.SetActive(Active: false);
			m_DownButton.SetActive(Active: false);
		}
		return num + (20f - m_BlankSpacing);
	}

	public void SetObjectType(ObjectType NewType)
	{
		if (m_ObjectType == NewType)
		{
			return;
		}
		CheckGadgets();
		m_ObjectType = NewType;
		if (NewType == ObjectTypeList.m_Total)
		{
			SetActive(Active: false);
			Objects.Instance.NavigateDestroy();
			return;
		}
		SetActive(Active: true);
		m_NumStages = ObjectTypeList.Instance.GetIngredientsStagesFromIdentifier(NewType);
		m_CurrentStage = 0;
		float num = SetUpElements(0);
		if (m_NumStages > 1)
		{
			for (int i = 1; i < m_NumStages; i++)
			{
				float num2 = SetUpElements(i);
				if (num < num2)
				{
					num = num2;
				}
			}
			SetUpElements(0);
		}
		SetWidth(num);
	}

	public void SetParent(Transform NewParent)
	{
		base.transform.SetParent(NewParent);
	}

	public void OnBackClicked(BaseGadget NewGadget)
	{
		Objects.Instance.NavigateBack();
	}

	private void UpdateUpDownButtons()
	{
		if (m_CurrentStage == 0)
		{
			m_DownButton.SetInteractable(Interactable: false);
		}
		else
		{
			m_DownButton.SetInteractable(Interactable: true);
		}
		if (m_CurrentStage == m_NumStages - 1)
		{
			m_UpButton.SetInteractable(Interactable: false);
		}
		else
		{
			m_UpButton.SetInteractable(Interactable: true);
		}
	}

	public void OnDownClicked(BaseGadget NewGadget)
	{
		m_CurrentStage--;
		UpdateUpDownButtons();
		SetUpElements(m_CurrentStage);
	}

	public void OnUpClicked(BaseGadget NewGadget)
	{
		m_CurrentStage++;
		UpdateUpDownButtons();
		SetUpElements(m_CurrentStage);
	}

	public void OnObjectClicked(BaseGadget NewGadget)
	{
		ObjectType objectType = NewGadget.GetComponent<ObjectTypeButton>().m_ObjectType;
		if (objectType != m_ObjectType)
		{
			switch (objectType)
			{
			case ObjectType.TallBoulder:
				return;
			case ObjectType.FishAny:
				objectType = ObjectType.FishSalmon;
				break;
			}
			if (objectType == ObjectType.HeartAny)
			{
				objectType = ObjectType.FolkHeart;
			}
			Objects.Instance.NavigateForward(m_ObjectType);
			Objects.Instance.SetObjectType(objectType, Flash: false, ScrollTo: true);
		}
	}

	public void UpdateLockedObjects()
	{
		m_ObjectButton.UpdateLocked();
	}
}
