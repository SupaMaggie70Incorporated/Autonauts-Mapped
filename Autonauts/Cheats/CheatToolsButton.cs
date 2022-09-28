using UnityEngine;
using UnityEngine.UI;

public class CheatToolsButton : Button
{
	private int m_Index;

	protected new void Awake()
	{
		base.onClick.AddListener(delegate
		{
			OnClick();
		});
	}

	public void SetObjectType(ObjectType NewObject, int Index)
	{
		m_Index = Index;
		Sprite icon = IconManager.Instance.GetIcon(NewObject);
		base.transform.Find("Image").GetComponent<Image>().sprite = icon;
		GetComponent<ButtonRollover>().m_RolloverTag = ObjectTypeList.Instance.GetSaveNameFromIdentifier(NewObject);
		GetComponent<ButtonRollover>().m_Offset = new Vector3(0f, 100f, 0f);
	}

	public void SetContainerType(ObjectType NewObject, ObjectType NewContents, int Index)
	{
		m_Index = Index;
		base.transform.Find("Image").GetComponent<Image>().sprite = IconManager.Instance.GetIcon(NewObject);
		string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(NewObject);
		string text = TextManager.Instance.Get(saveNameFromIdentifier);
		saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(NewContents);
		text = text + " (" + TextManager.Instance.Get(saveNameFromIdentifier) + ")";
		GetComponent<ButtonRollover>().m_RolloverText = text;
		GetComponent<ButtonRollover>().m_Offset = new Vector3(0f, 100f, 0f);
	}

	public void OnMouseEnter()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
		GetComponent<Image>().color = base.colors.highlightedColor;
	}

	public void OnMouseExit()
	{
		GetComponent<Image>().color = base.colors.normalColor;
	}

	private void OnClick()
	{
	}
}
