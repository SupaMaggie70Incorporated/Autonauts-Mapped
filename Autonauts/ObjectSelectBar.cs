using UnityEngine;

public class ObjectSelectBar : MonoBehaviour
{
	public void SetTitle(string TitleID)
	{
		base.transform.Find("Title").GetComponent<BaseText>().SetTextFromID(TitleID);
	}
}
