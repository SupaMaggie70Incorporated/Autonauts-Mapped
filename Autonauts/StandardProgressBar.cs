using UnityEngine;
using UnityEngine.UI;

public class StandardProgressBar : BaseProgressBar
{
	public override void SetFillColour(Color NewColour)
	{
		base.transform.Find("Fill Area").Find("Fill").GetComponent<Image>()
			.color = NewColour;
	}

	public void SetBorderColour(Color NewColour)
	{
		base.transform.Find("Border").GetComponent<Image>().color = NewColour;
	}

	public void SetBackgroundColour(Color NewColour)
	{
		base.transform.Find("Background").GetComponent<Image>().color = NewColour;
	}
}
