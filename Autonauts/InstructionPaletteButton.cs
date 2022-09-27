using UnityEngine;
using UnityEngine.UI;

public class InstructionPaletteButton : MonoBehaviour
{
	private HighInstructionInfo.Category m_Category;

	private InstructionPalette m_Parent;

	public void SetParent(InstructionPalette Parent)
	{
		m_Parent = Parent;
	}

	public void SetCategory(HighInstructionInfo.Category NewCategory)
	{
		m_Category = NewCategory;
		string iconName = HighInstructionInfo.m_CategoryInfo[(int)m_Category].m_IconName;
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Script/Categories/" + iconName, typeof(Sprite));
		base.transform.Find("Image").GetComponentInChildren<Image>().sprite = sprite;
		Color colour = HighInstructionInfo.m_CategoryInfo[(int)m_Category].m_Colour;
		ColorBlock colors = base.gameObject.GetComponent<Button>().colors;
		colors.normalColor = colour;
		colour.a = 0.5f;
		colors.disabledColor = colour;
		base.gameObject.GetComponent<Button>().colors = colors;
		string rolloverName = HighInstructionInfo.m_CategoryInfo[(int)m_Category].m_RolloverName;
		GetComponent<ButtonRollover>().m_RolloverTag = rolloverName;
	}

	public void OnClick()
	{
		m_Parent.SetCurrentCategory(m_Category);
		AudioManager.Instance.StartEvent("UIOptionSelected");
	}
}
