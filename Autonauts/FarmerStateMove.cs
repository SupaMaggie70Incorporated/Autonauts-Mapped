using UnityEngine;

public class FarmerStateMove : FarmerStateBase
{
	protected bool m_StartAnimation;

	private bool m_VibrateAnimation;

	private float m_OldPercent;

	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		if (!m_Farmer.m_Nudge)
		{
			m_StartAnimation = true;
		}
		m_VibrateAnimation = false;
		if (m_Farmer.m_TypeIdentifier == ObjectType.Worker && WorkerDrive.GetIsTypeVibraty(m_Farmer.GetComponent<Worker>().m_Drive))
		{
			m_VibrateAnimation = true;
		}
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		Tile tile = TileManager.Instance.GetTile(m_Farmer.m_TileCoord);
		string text = "";
		if (TileHelpers.GetTileWater(tile.m_TileType) && tile.m_Floor == null)
		{
			text = ((tile.m_TileType != Tile.TileType.Swamp) ? "FarmerStepWater" : "FarmerStepSwamp");
			MyParticles newParticles = ParticlesManager.Instance.CreateParticles("SplashSmall", m_Farmer.transform.position, Quaternion.Euler(-90f, 0f, 0f));
			ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
		}
		else if ((bool)m_Farmer.GetComponent<FarmerPlayer>())
		{
			if (!m_Farmer.GetComponent<FarmerPlayer>().m_Scooter)
			{
				text = (((bool)tile.m_Floor && tile.m_Floor.m_TypeIdentifier == ObjectType.StonePath) ? "PlayerMovePath" : ((tile.m_TileType != Tile.TileType.Iron && tile.m_TileType != Tile.TileType.Stone) ? ((tile.m_TileType != Tile.TileType.Clay) ? "PlayerMove" : "PlayerClayMove") : "PlayerStoneMove"));
			}
		}
		else
		{
			text = ((tile.m_TileType == Tile.TileType.Iron || tile.m_TileType == Tile.TileType.Stone) ? m_Farmer.GetComponent<Worker>().m_DriveInfo.m_MoveStoneSoundName : ((tile.m_TileType != Tile.TileType.Clay) ? m_Farmer.GetComponent<Worker>().m_DriveInfo.m_MoveSoundName : m_Farmer.GetComponent<Worker>().m_DriveInfo.m_MoveClaySoundName));
			AudioManager.Instance.StartEvent(text, m_Farmer);
		}
		if (text != "")
		{
			AudioManager.Instance.StartEvent(text, m_Farmer);
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		m_Farmer.UpdateMovement();
		if (m_Farmer.m_State == Farmer.State.Moving)
		{
			float num;
			if (m_VibrateAnimation)
			{
				num = 0f;
				if ((int)(m_Farmer.m_StateTimer * 60f) % 4 < 2)
				{
					num = 1f;
				}
				m_Farmer.transform.position += new Vector3(0f, num * 0.1f, 0f);
			}
			else
			{
				num = 0f;
				if ((int)(m_Farmer.m_StateTimer * 60f) % 8 < 5)
				{
					num = 1f;
				}
				m_Farmer.transform.position += new Vector3(0f, num * 0.5f, 0f);
			}
			if (num == 0f && m_OldPercent == 1f)
			{
				DoAnimationAction();
			}
			m_OldPercent = num;
		}
		RecordingManager.Instance.UpdateObject(m_Farmer);
	}
}
