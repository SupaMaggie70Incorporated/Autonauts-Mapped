using UnityEngine;

public class CeremonySpeech : MonoBehaviour
{
	public TutorialTutor m_Tutor;

	public TutorialDialog m_Dialog;

	private void Awake()
	{
		CheckGadgets();
	}

	public void CheckGadgets()
	{
		if (!m_Tutor)
		{
			m_Tutor = base.transform.Find("Tutor").GetComponent<TutorialTutor>();
			m_Dialog = base.transform.Find("Dialog").GetComponent<TutorialDialog>();
			m_Dialog.SetText("");
		}
	}

	public BaseButtonImage GetButton()
	{
		return base.transform.Find("StandardAcceptButton").GetComponent<BaseButtonImage>();
	}

	public void SetActive(bool Active)
	{
		base.gameObject.SetActive(Active);
	}

	public void SetSpeechFromID(string TextID)
	{
		CheckGadgets();
		m_Tutor.StartTalking();
		m_Dialog.StartSpeechFromID(TextID, Ceremony: true);
	}

	public void SetSpeech(string Text)
	{
		CheckGadgets();
		m_Tutor.StartTalking();
		m_Dialog.StartSpeech(Text, Ceremony: true);
	}

	private void Update()
	{
		if (m_Tutor.GetIsTalking() && !m_Dialog.GetIsTalking())
		{
			m_Tutor.StopTalking();
		}
	}
}
