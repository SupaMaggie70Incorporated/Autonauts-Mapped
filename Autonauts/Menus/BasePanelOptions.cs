using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class BasePanelOptions : MonoBehaviour
{
	[SerializeField]
	public bool m_Title = true;

	private BaseText m_Text;

	private BasePanel m_Panel;

	private void OnValidate()
	{
		CheckGadgets();
		if ((bool)m_Text)
		{
			m_Text.gameObject.SetActive(m_Title);
		}
	}

	public virtual void OnButtonClicked()
	{
	}

	private void CheckGadgets()
	{
		if (m_Panel == null)
		{
			m_Panel = base.transform.Find("Panel").GetComponent<BasePanel>();
			m_Text = m_Panel.transform.Find("TitleStrip/Title").GetComponent<BaseText>();
		}
	}

	public void SetActive(bool Active)
	{
		CheckGadgets();
		m_Panel.SetActive(Active);
	}

	public BasePanel GetPanel()
	{
		CheckGadgets();
		return m_Panel;
	}

	public BaseText GetTitle()
	{
		CheckGadgets();
		return m_Text;
	}

	public BaseButtonBack GetBackButton()
	{
		CheckGadgets();
		return m_Panel.transform.Find("BackButton").GetComponent<BaseButtonBack>();
	}

	public void SetTitleText(string NewString)
	{
		GetTitle().SetText(NewString);
	}

	public void SetTitleTextFromID(string NewString)
	{
		GetTitle().SetTextFromID(NewString);
	}

	public void SetTitleColour(Color NewColour)
	{
		GetTitle().SetColour(NewColour);
	}

	public void SetTitleStripColour(Color NewColour)
	{
		CheckGadgets();
		m_Panel.transform.Find("TitleStrip").GetComponent<Image>().color = NewColour;
	}

	public float GetWidth()
	{
		return GetComponent<RectTransform>().sizeDelta.x;
	}

	public float GetHeight()
	{
		return GetComponent<RectTransform>().sizeDelta.y;
	}
}
