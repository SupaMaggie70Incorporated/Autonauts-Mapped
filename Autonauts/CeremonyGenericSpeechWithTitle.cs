using UnityEngine;

public class CeremonyGenericSpeechWithTitle : CeremonyBase
{
	protected GameObject m_TitleStrip;

	protected BaseText m_Title;

	protected CeremonySpeech m_Speech;

	protected void Awake()
	{
		m_TitleStrip = base.transform.Find("TitleStrip").gameObject;
		m_Title = base.transform.Find("TitleStrip/Title").GetComponent<BaseText>();
		m_Speech = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, null);
	}

	protected void SetTitle(string TextID)
	{
		m_Title.SetTextFromID(TextID);
	}

	public void SetSpeechImages(string Frame1, string Frame2)
	{
		m_Speech.CheckGadgets();
		m_Speech.m_Tutor.SetImages(Frame1, Frame2);
	}

	protected void SetSpeech(string TextID)
	{
		if (TextID == null)
		{
			m_Speech.SetActive(Active: false);
			return;
		}
		m_Speech.SetActive(Active: true);
		m_Speech.SetSpeechFromID(TextID);
	}

	protected void ShowSpeech(bool Show)
	{
		m_Speech.SetActive(Show);
	}

	public virtual void OnAcceptClicked(BaseGadget NewGadget)
	{
		End();
	}

	protected virtual void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
	}
}
