using UnityEngine;

public class TeachWorkerScriptController : MonoBehaviour
{
	public static TeachWorkerScriptController Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void SetShort(bool Short)
	{
		string n = "NormalTemplate";
		if (Short)
		{
			n = "ShortTemplate";
		}
		GameObject oldObject = base.transform.Find(n).gameObject;
		ObjectUtils.TransferRectTransform(TeachWorkerScriptEdit.Instance.gameObject, oldObject);
	}
}
