using UnityEngine;

public class ConverterRequirementIngredient : MonoBehaviour
{
	private ObjectType m_Type;

	private BaseImage m_Image;

	private BaseText m_IngredientText;

	private BaseText m_CountText;

	private bool m_WhiteText;

	private bool m_Locked;

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (m_IngredientText == null)
		{
			m_IngredientText = base.transform.Find("Ingredient").GetComponent<BaseText>();
			m_Image = base.transform.Find("Image").GetComponent<BaseImage>();
			m_CountText = base.transform.Find("Count").GetComponent<BaseText>();
		}
	}

	public void SetIngredient(string IngredientID, ObjectType IconType, bool WhiteText)
	{
		m_IngredientText.SetTextFromID(IngredientID);
		m_Image.SetSprite(IconManager.Instance.GetIcon(IconType));
		m_WhiteText = WhiteText;
	}

	public void SetIngredient(ObjectType Type, bool WhiteText)
	{
		Init();
		if (Type == ObjectType.FolkHeart)
		{
			m_IngredientText.SetTextFromID("FolkHeart");
		}
		else
		{
			m_IngredientText.SetText(ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(Type));
		}
		m_Type = Type;
		m_Image.SetSprite(IconManager.Instance.GetIcon(Type));
		m_WhiteText = WhiteText;
	}

	public void SetLocked()
	{
		m_Locked = true;
	}

	public void OnMouseEnter()
	{
		string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_Type);
		HudManager.Instance.ActivateUIRollover(Activate: true, humanReadableNameFromIdentifier, default(Vector3));
	}

	public void OnMouseExit()
	{
		HudManager.Instance.ActivateUIRollover(Activate: false, "", new Vector3(0f, 0f, 0f));
	}

	public void SetRequired(int Count, int Max, bool ShowCount = true)
	{
		Init();
		string text = GeneralUtils.FormatBigInt(Count);
		string text2 = GeneralUtils.FormatBigInt(Max);
		if (ShowCount)
		{
			m_CountText.SetText(text + "/" + text2);
		}
		else
		{
			m_CountText.SetText(text2);
		}
		Color colour = new Color(1f, 1f, 1f);
		if (!m_WhiteText)
		{
			colour = new Color(0f, 0f, 0f);
		}
		if (Count == Max)
		{
			colour.a = 0.35f;
		}
		m_Image.SetColour(new Color(1f, 1f, 1f, colour.a));
		m_IngredientText.SetColour(colour);
		m_CountText.SetColour(colour);
	}

	private void Update()
	{
		if (m_Locked)
		{
			Color colour = new Color(1f, 1f, 1f, 0.5f);
			if (!m_WhiteText)
			{
				colour = new Color(0f, 0f, 0f);
			}
			if (HudManager.Instance.m_ConverterRollover.m_MissingFlashOn)
			{
				colour = new Color(1f, 0f, 0f);
			}
			m_IngredientText.SetColour(colour);
		}
	}
}
