using UnityEngine;

public class About : BaseMenu
{
	protected new void Start()
	{
		base.Start();
		BaseScrollView component = base.transform.Find("BasePanelOptions").Find("BaseScrollView").GetComponent<BaseScrollView>();
		GameObject content = component.GetContent();
		TextAsset textAsset = (TextAsset)Resources.Load("Data/Credits", typeof(TextAsset));
		BaseText component2 = content.transform.Find("Text").GetComponent<BaseText>();
		component2.SetText(textAsset.text);
		float preferredHeight = component2.GetPreferredHeight();
		component.SetScrollSize(preferredHeight);
	}
}
