using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionPalette : MonoBehaviour
{
	private TeachWorkerScriptEdit m_Parent;

	private static HighInstructionInfo.Category m_CurrentCategory;

	private List<HudInstruction> m_CurrentCategoryInstructions;

	private List<InstructionPaletteButton> m_CategoryButtons;

	private void Awake()
	{
		Transform transform = base.transform.Find("Panel");
		float height = transform.GetComponent<RectTransform>().rect.height;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/InstructionPaletteButton", typeof(GameObject));
		m_CategoryButtons = new List<InstructionPaletteButton>();
		for (int i = 0; i < 6; i++)
		{
			GameObject obj = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
			float x = i % 3 * 47 + 28;
			float y = height - (float)(i / 3 * 32 + 12);
			obj.transform.localPosition = new Vector3(x, y, 0f);
			InstructionPaletteButton component = obj.GetComponent<InstructionPaletteButton>();
			component.SetParent(this);
			component.SetCategory((HighInstructionInfo.Category)i);
			m_CategoryButtons.Add(component);
		}
		m_CurrentCategoryInstructions = new List<HudInstruction>();
	}

	public void SetParent(TeachWorkerScriptEdit Parent)
	{
		m_Parent = Parent;
		SetCurrentCategory(m_CurrentCategory);
	}

	private void DeleteCurrentCategoryInstruction()
	{
		foreach (HudInstruction currentCategoryInstruction in m_CurrentCategoryInstructions)
		{
			Object.Destroy(currentCategoryInstruction.gameObject);
		}
		m_CurrentCategoryInstructions.Clear();
	}

	private void CreateCurrentCategoryInstructions()
	{
		Transform parent = base.transform.Find("Panel").Find("Scroll View").Find("Viewport")
			.Find("Content");
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/Instruction", typeof(GameObject));
		float num = -5f;
		float num2 = 32f;
		for (int i = 0; i < 30; i++)
		{
			if ((i == 2 || i == 21 || i == 1 || i == 24 || i == 17 || i == 19 || i == 20) && HighInstruction.m_Info[i].m_Category == m_CurrentCategory)
			{
				HudInstruction component = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<HudInstruction>();
				component.transform.localPosition = new Vector3(5f, num, 0f);
				component.SetParent(m_Parent);
				component.SetStatic();
				component.SetInstruction(new HighInstruction((HighInstruction.Type)i, null));
				m_CurrentCategoryInstructions.Add(component);
				num -= num2;
			}
		}
	}

	public void SetInstructionsInteractable(bool Interactable)
	{
		foreach (HudInstruction currentCategoryInstruction in m_CurrentCategoryInstructions)
		{
			currentCategoryInstruction.EnableInteraction(Interactable);
		}
	}

	public void SetCurrentCategory(HighInstructionInfo.Category Category)
	{
		DeleteCurrentCategoryInstruction();
		m_CurrentCategory = Category;
		CreateCurrentCategoryInstructions();
	}

	public void EnableInteraction(bool Interaction)
	{
		foreach (HudInstruction currentCategoryInstruction in m_CurrentCategoryInstructions)
		{
			currentCategoryInstruction.EnableInteraction(Interaction);
		}
		foreach (InstructionPaletteButton categoryButton in m_CategoryButtons)
		{
			categoryButton.GetComponent<Button>().interactable = Interaction;
		}
	}
}
