using UnityEngine;

public class TrackRollover : GeneralRollover
{
	private TrainTrack m_Target;

	private BaseImage m_Image;

	private BaseText m_Info;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Info = m_Panel.transform.Find("Info").GetComponent<BaseText>();
		m_Image = m_Panel.transform.Find("ObjectImage").GetComponent<BaseImage>();
		Hide();
	}

	private void UpdateInfo()
	{
		if (m_Target.m_LinkedSystem != null && m_Target.m_LinkedSystem.m_Type == LinkedSystem.SystemType.Track)
		{
			LinkedSystemTrack linkedSystemTrack = (LinkedSystemTrack)m_Target.m_LinkedSystem;
			string newText = "TrackAllGood";
			if (!linkedSystemTrack.GetIsEnoughTrack())
			{
				newText = "TrackTooShort";
			}
			else if (!linkedSystemTrack.GetHasAnyStops())
			{
				newText = "TrackNoStops";
			}
			else if (linkedSystemTrack.GetNumMinecarts() == 0)
			{
				newText = "TrackNeedsTrain";
			}
			else if (linkedSystemTrack.GetNumMinecarts() > 1)
			{
				newText = "TrackTooManyTrains";
			}
			m_Info.SetTextFromID(newText);
		}
	}

	public void SetTarget(TrainTrack Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)Target)
			{
				m_Title.SetText(Target.GetHumanReadableName());
				Sprite icon = IconManager.Instance.GetIcon(Target.m_TypeIdentifier);
				m_Image.SetSprite(icon);
				UpdateInfo();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		if ((bool)m_Target)
		{
			return true;
		}
		return false;
	}
}
