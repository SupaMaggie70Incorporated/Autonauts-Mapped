using System;
using UnityEngine;

public class CeremonyBlueprint : MonoBehaviour
{
	public BaseButtonImage m_Button;

	public void Init(ObjectType NewType, Action<BaseGadget> NewAction)
	{
		Transform transform = base.transform.Find("BasePanel");
		Sprite icon = IconManager.Instance.GetIcon(NewType);
		transform.Find("ObjectBox").Find("ObjectImage").GetComponent<BaseImage>()
			.SetSprite(icon);
		BaseText component = transform.Find("Title").GetComponent<BaseText>();
		string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(NewType);
		component.SetText(humanReadableNameFromIdentifier);
		BaseText component2 = transform.Find("Description").GetComponent<BaseText>();
		string descriptionFromIdentifier = ObjectTypeList.Instance.GetDescriptionFromIdentifier(NewType);
		component2.SetText(descriptionFromIdentifier);
		m_Button = transform.Find("StandardAcceptButton").GetComponent<BaseButtonImage>();
		m_Button.SetAction(NewAction, m_Button);
	}
}
