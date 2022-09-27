using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectCancel : MonoBehaviour
{
	private void Awake()
	{
	}

	public void SetText(string ID)
	{
		base.transform.Find("Panel").Find("Text").GetComponent<Text>()
			.text = TextManager.Instance.Get(ID);
	}

	public void OnClick()
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBuildingSelect>().CancelClicked();
	}

	public void OnMouseEnter()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
	}

	public void OnMouseExit()
	{
	}
}
