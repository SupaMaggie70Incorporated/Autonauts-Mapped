using UnityEngine.UI;

public class UnlockedObject : CeremonyBase
{
	public void SetObjectType(ObjectType NewObject)
	{
		base.transform.Find("Image").GetComponent<Image>().sprite = IconManager.Instance.GetIcon(NewObject);
	}
}
