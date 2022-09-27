using UnityEngine;

public class CeremonyGenericSpeech : CeremonyBase
{
	public CeremonySpeech m_Speech;

	protected void Awake()
	{
		m_Speech = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, null);
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

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		End();
	}

	protected virtual void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
	}
}
