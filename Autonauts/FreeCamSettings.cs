using UnityEngine;
using UnityEngine.UI;

public class FreeCamSettings : BaseGadget
{
	private GameObject m_Icons;

	private BasePanelOptions m_Panel;

	private BaseButtonImage m_Enlarge;

	private BaseToggle m_DOFGadget;

	private GameObject m_Controls;

	private BaseToggle m_AutoDOFGadget;

	private StandardSlider m_DOFFocalDistanceGadget;

	private StandardSlider m_DOFFocalLengthGadget;

	private StandardSlider m_DOFApertureGadget;

	protected new void Awake()
	{
		base.Awake();
		SetAction(OnClicked, this);
	}

	private void CheckGadgets()
	{
		if (!m_Panel)
		{
			m_Icons = base.transform.Find("Icons").gameObject;
			m_Panel = base.transform.Find("Panel").GetComponent<BasePanelOptions>();
			BasePanel panel = m_Panel.GetPanel();
			m_DOFGadget = panel.transform.Find("DOFToggle/BaseToggle").GetComponent<BaseToggle>();
			m_DOFGadget.SetStartOn(SettingsManager.Instance.m_DOFEnabled);
			m_DOFGadget.SetAction(OnDOFClicked, m_DOFGadget);
			m_Controls = panel.transform.Find("Controls").gameObject;
			m_AutoDOFGadget = m_Controls.transform.Find("AutoToggle/BaseToggle").GetComponent<BaseToggle>();
			m_AutoDOFGadget.SetStartOn(SettingsManager.Instance.m_AutoDOFEnabled);
			m_AutoDOFGadget.SetAction(OnAutoClicked, m_AutoDOFGadget);
			m_DOFFocalDistanceGadget = m_Controls.transform.Find("Distance").GetComponent<StandardSlider>();
			m_DOFFocalDistanceGadget.SetStartValue(SettingsManager.Instance.m_DOFFocalDistance);
			m_DOFFocalDistanceGadget.SetAction(OnDistanceChanged, null);
			m_DOFFocalLengthGadget = m_Controls.transform.Find("Length").GetComponent<StandardSlider>();
			m_DOFFocalLengthGadget.SetStartValue(SettingsManager.Instance.m_DOFFocalLength);
			m_DOFFocalLengthGadget.SetAction(OnLengthChanged, null);
			m_DOFApertureGadget = m_Controls.transform.Find("Aperture").GetComponent<StandardSlider>();
			m_DOFApertureGadget.SetStartValue(SettingsManager.Instance.m_DOFAperture);
			m_DOFApertureGadget.SetAction(OnApertureChanged, null);
			BaseButton component = panel.transform.Find("BackButton").GetComponent<BaseButton>();
			component.SetAction(OnBackButtonClicked, component);
			UpdateEnabled();
			UpdateAuto();
		}
	}

	public void SetLocked(bool Locked)
	{
		CheckGadgets();
		GetComponent<Image>().enabled = !Locked;
		m_Panel.SetActive(!Locked);
		m_Icons.SetActive(Locked);
	}

	public void OnDOFClicked(BaseGadget NewGadget)
	{
		SettingsManager.Instance.SetDOF(m_DOFGadget.GetOn());
		UpdateEnabled();
	}

	private void UpdateEnabled()
	{
		bool on = m_DOFGadget.GetOn();
		m_AutoDOFGadget.SetInteractable(on);
		m_DOFFocalLengthGadget.SetInteractable(on);
		m_DOFApertureGadget.SetInteractable(on);
		UpdateAuto();
	}

	private void UpdateAuto()
	{
		m_DOFFocalDistanceGadget.SetInteractable(m_DOFGadget.GetOn() && !SettingsManager.Instance.m_AutoDOFEnabled);
	}

	public void OnAutoClicked(BaseGadget NewGadget)
	{
		SettingsManager.Instance.SetAutoDOF(m_AutoDOFGadget.GetOn());
		UpdateAuto();
	}

	public void OnDistanceChanged(BaseGadget NewGadget)
	{
		SettingsManager.Instance.SetDOFFocalDistance(m_DOFFocalDistanceGadget.GetValue());
		SettingsManager.Instance.UpdateDOF();
	}

	public void OnLengthChanged(BaseGadget NewGadget)
	{
		SettingsManager.Instance.SetDOFFocalLength(m_DOFFocalLengthGadget.GetValue());
		SettingsManager.Instance.UpdateDOF();
	}

	public void OnApertureChanged(BaseGadget NewGadget)
	{
		SettingsManager.Instance.SetDOFAperture(m_DOFApertureGadget.GetValue());
		SettingsManager.Instance.UpdateDOF();
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		GameStateFreeCam.Instance.SetLocked(Locked: true);
	}

	public void OnBackButtonClicked(BaseGadget NewGadget)
	{
		GameStateFreeCam.Instance.SetLocked(Locked: true);
	}

	private void Update()
	{
		if (SettingsManager.Instance.m_AutoDOFEnabled)
		{
			m_DOFFocalDistanceGadget.SetValue(SettingsManager.Instance.m_DOFFocalDistance);
		}
	}
}
