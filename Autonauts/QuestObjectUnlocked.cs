using UnityEngine;

public class QuestObjectUnlocked : MonoBehaviour
{
	public BaseImage m_Image;

	public BaseText m_IngredientText;

	private void Awake()
	{
	}

	public void SetIngredient(ObjectType Type, bool WhiteText)
	{
		m_IngredientText = base.transform.Find("Ingredient").GetComponent<BaseText>();
		m_Image = base.transform.Find("Image").GetComponent<BaseImage>();
		m_IngredientText.SetText(ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(Type));
		m_IngredientText.SetColour(new Color(0f, 0f, 0f, 1f));
		m_Image.SetSprite(IconManager.Instance.GetIcon(Type));
	}
}
