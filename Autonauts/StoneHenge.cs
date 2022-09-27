using SimpleJSON;
using UnityEngine;

public class StoneHenge : Wonder
{
	private MyParticles m_Particles;

	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		CollectionManager.Instance.AddCollectable("StoneHenge", this);
		SetDimensions(new TileCoord(-3, -6), new TileCoord(3, 0), new TileCoord(0, 1));
		HideAccessModel();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		SetActive(Active: false);
		base.StopUsing(AndDestroy);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if (Info.m_Action == ActionType.RefreshFirst)
		{
			bool isFastTime = TimeManager.Instance.GetIsFastTime();
			SetActive(isFastTime);
		}
	}

	public override void Moved()
	{
		base.Moved();
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: false, StopSpawning: true);
			m_Particles = null;
			SetActive(Active: true);
		}
	}

	public void SetActive(bool Active)
	{
		if (!GetIsSavable() || m_Blueprint)
		{
			return;
		}
		if (Active)
		{
			if (m_Particles == null)
			{
				m_Particles = ParticlesManager.Instance.CreateParticles("StoneHengeActive", base.transform.TransformPoint(new Vector3(0f, 4f, 8.8f)), base.transform.rotation * Quaternion.Euler(0f, 0f, 90f));
				m_Particles.transform.SetParent(base.transform);
			}
			m_PlaySound = AudioManager.Instance.StartEvent("StoneHengeActive", this, Remember: true);
			return;
		}
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_PlaySound = null;
		if ((bool)m_Particles)
		{
			m_Particles.Stop();
			ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true);
			m_Particles = null;
		}
	}
}
