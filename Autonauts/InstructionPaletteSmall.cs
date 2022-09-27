using System.Collections.Generic;
using UnityEngine;

public class InstructionPaletteSmall : MonoBehaviour
{
	private static bool m_Expanded = true;

	private TeachWorkerScriptEdit m_Parent;

	private List<HudInstruction> m_Instructions;

	private Vector3 m_StartPosition;

	private float m_Width;

	private BaseButtonImage m_ExpandButton;

	private void Awake()
	{
	}

	public void SetParent(TeachWorkerScriptEdit Parent)
	{
		m_Parent = Parent;
		CreateInstructions();
		m_ExpandButton = base.transform.Find("ExpandButton").GetComponent<BaseButtonImage>();
		m_ExpandButton.SetAction(OnExpandClicked, m_ExpandButton);
		m_StartPosition = GetComponent<RectTransform>().anchoredPosition;
		m_Width = GetComponent<RectTransform>().sizeDelta.x;
		UpdateExpanded();
	}

	private void CreateInstructions()
	{
		BaseScrollView component = base.transform.Find("BasePanel/BaseScrollView").GetComponent<BaseScrollView>();
		Transform parent = component.GetContent().transform;
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/Hud/Scripting/Instruction", typeof(GameObject));
		float num = -15f;
		float num2 = gameObject.GetComponent<RectTransform>().sizeDelta.y + 5f;
		HighInstruction.Type[] array = new HighInstruction.Type[5]
		{
			HighInstruction.Type.Repeat,
			HighInstruction.Type.ExitRepeat,
			HighInstruction.Type.If,
			HighInstruction.Type.IfElse,
			HighInstruction.Type.Wait
		};
		component.SetScrollSize(num2 * (float)array.Length);
		m_Instructions = new List<HudInstruction>();
		HighInstruction.Type[] array2 = array;
		foreach (HighInstruction.Type newType in array2)
		{
			HudInstruction component2 = Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<HudInstruction>();
			component2.transform.localPosition = new Vector3(15f, num, 0f);
			component2.SetParent(m_Parent);
			component2.SetStatic();
			component2.SetInstruction(new HighInstruction(newType, null));
			m_Instructions.Add(component2);
			num -= num2;
		}
		float num3 = 0f;
		foreach (HudInstruction instruction in m_Instructions)
		{
			if (instruction.GetWidth() > num3)
			{
				num3 = instruction.GetWidth();
			}
		}
		num3 += 20f;
		float y = GetComponent<RectTransform>().sizeDelta.y;
		GetComponent<RectTransform>().sizeDelta = new Vector2(num3, y);
	}

	public void SetInstructionsInteractable(bool Interactable)
	{
		foreach (HudInstruction instruction in m_Instructions)
		{
			instruction.EnableInteraction(Interactable);
		}
	}

	public void EnableInteraction(bool Interaction)
	{
		foreach (HudInstruction instruction in m_Instructions)
		{
			instruction.EnableInteraction(Interaction);
		}
	}

	public void OnExpandClicked(BaseGadget NewGadget)
	{
		m_Parent.OnClickPalette();
	}

	private void UpdateExpandPosition()
	{
		Vector3 startPosition = m_StartPosition;
		if (!m_Expanded)
		{
			startPosition.x -= m_Width - 5f;
		}
		GetComponent<RectTransform>().anchoredPosition = startPosition;
	}

	private void UpdateExpandButton()
	{
		string rolloverFromID = "ScriptPaletteExpandButton";
		if (m_Expanded)
		{
			rolloverFromID = "ScriptPaletteCollapseButton";
		}
		m_ExpandButton.SetRolloverFromID(rolloverFromID);
		if (m_Expanded)
		{
			m_ExpandButton.SetBackingSprite("Script/ScriptPaletteCollapseButtonBacking");
		}
		else
		{
			m_ExpandButton.SetBackingSprite("Script/ScriptPaletteExpandButtonBacking");
		}
	}

	private void UpdateExpanded()
	{
		UpdateExpandPosition();
		UpdateExpandButton();
	}

	public void ToggleExpanded()
	{
		m_Expanded = !m_Expanded;
		UpdateExpanded();
	}
}
