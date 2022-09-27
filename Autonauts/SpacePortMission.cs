using UnityEngine;

public class SpacePortMission : MonoBehaviour
{
	public OffworldMission m_Mission;

	private SpacePortSelect m_Parent;

	private BaseImage m_Panel;

	private BaseImage m_Image;

	private BaseText m_Name;

	private StandardButtonImage m_Accept;

	private StandardButtonImage m_Decline;

	private void CheckGadgets()
	{
		if (!(m_Image != null))
		{
			m_Panel = base.transform.Find("Panel").GetComponent<BaseImage>();
			m_Panel.m_OnEnterEvent = OnPointerEnter;
			m_Panel.m_OnExitEvent = OnPointerExit;
			m_Image = base.transform.Find("Image").GetComponent<BaseImage>();
			m_Name = base.transform.Find("Name").GetComponent<BaseText>();
			m_Accept = base.transform.Find("AcceptButton").GetComponent<StandardButtonImage>();
			m_Accept.SetAction(OnAcceptClicked, m_Accept);
			m_Decline = base.transform.Find("DeclineButton").GetComponent<StandardButtonImage>();
			m_Decline.SetAction(OnDeclineClicked, m_Decline);
		}
	}

	private void SetInteractable(bool Interactable)
	{
		m_Accept.SetInteractable(Interactable);
		float a = 1f;
		if (!Interactable)
		{
			a = 0.5f;
		}
		m_Panel.SetColour(new Color(1f, 1f, 1f, a));
		m_Image.SetColour(new Color(1f, 1f, 1f, a));
		m_Name.SetColour(new Color(0f, 0f, 0f, a));
	}

	public void UpdateMission()
	{
		if (m_Parent == null)
		{
			return;
		}
		bool interactable = true;
		if (m_Mission.m_Selected)
		{
			m_Accept.SetBackingSprite("Buttons/ButtonCancel");
			m_Accept.SetSprite("SpacePort/MissionStop");
			m_Accept.SetRolloverFromID("SpacePortSelectStop");
			m_Decline.SetInteractable(Interactable: false);
		}
		else
		{
			m_Accept.SetBackingSprite("Buttons/ButtonAccept");
			m_Accept.SetSprite("SpacePort/MissionAccept");
			m_Accept.SetRolloverFromID("SpacePortSelectAccept");
			if (OffworldMissionsManager.Instance.m_SelectedMission == null)
			{
				m_Decline.SetInteractable(Interactable: true);
			}
			else
			{
				interactable = false;
				m_Decline.SetInteractable(Interactable: false);
			}
		}
		m_Decline.SetActive(!m_Mission.m_Daily);
		SetInteractable(interactable);
	}

	public void SetMission(OffworldMission NewMission, SpacePortSelect Parent)
	{
		m_Mission = NewMission;
		m_Parent = Parent;
		CheckGadgets();
		string text = "SpacePort/MissionPanel";
		if (NewMission.m_Daily)
		{
			text += "Daily";
		}
		m_Panel.SetSprite(text);
		m_Image.SetSprite(m_Mission.GetImage());
		m_Name.SetText(m_Mission.GetName(Short: false));
		UpdateMission();
	}

	public void OnAcceptClicked(BaseGadget NewButton)
	{
		if (m_Mission.m_Selected)
		{
			m_Parent.MissionStopped(this);
		}
		else
		{
			m_Parent.MissionAccepted(this);
		}
	}

	public void OnDeclineClicked(BaseGadget NewButton)
	{
		m_Parent.MissionDeclined(this);
	}

	public void OnPointerEnter(BaseGadget NewButton)
	{
		HudManager.Instance.ActivateSpacePortRollover(Activate: true, m_Mission);
	}

	public void OnPointerExit(BaseGadget NewButton)
	{
		HudManager.Instance.ActivateSpacePortRollover(Activate: false, (OffworldMission)null);
	}
}
