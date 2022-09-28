using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonRollover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public string m_RolloverTag = "";

	public string m_RolloverText = "";

	public Vector3 m_Offset;

	private void Awake()
	{
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		string text = "";
		if (m_RolloverTag != "" && m_RolloverTag != "Blah")
		{
			string humanReadableNameFromString = ObjectTypeList.Instance.GetHumanReadableNameFromString(m_RolloverTag);
			text = TextManager.Instance.Get(humanReadableNameFromString);
		}
		else
		{
			text = m_RolloverText;
		}
		Vector3 position = HudManager.Instance.ConvertUIToScreenSpace(m_Offset, base.transform);
		HudManager.Instance.ActivateUIRollover(Activate: true, text, position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HudManager.Instance.ActivateUIRollover(Activate: false, "", base.transform.position);
	}
}
