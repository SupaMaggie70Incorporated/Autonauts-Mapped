public class StopActive : Effect2D
{
	private bool m_Points;

	public override void Restart()
	{
		base.Restart();
		UpdateTransform();
	}

	protected new void Awake()
	{
		base.Awake();
		SetSprite("MouseCross");
	}

	public void SetPoints(bool Points)
	{
		m_Points = Points;
	}

	private void UpdateImage()
	{
		if (!(m_Target == null))
		{
			string sprite = "";
			if ((bool)m_Target.GetComponent<TrainTrackStop>())
			{
				sprite = (m_Target.GetComponent<TrainTrackStop>().m_PlayerStop ? "TrackStopStop" : "TrackStopGo");
			}
			if ((bool)m_Target.GetComponent<TrainTrackPoints>())
			{
				sprite = (m_Target.GetComponent<TrainTrackPoints>().m_PlayerSwitch ? "TrackPointsTurn" : "TrackPointsAhead");
			}
			SetSprite(sprite);
		}
	}

	private void Update()
	{
		UpdateImage();
		UpdateTransform();
	}
}
