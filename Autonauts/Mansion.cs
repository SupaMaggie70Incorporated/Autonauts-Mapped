using UnityEngine;

public class Mansion : Housing
{
	private MyParticles m_Smoke;

	private Transform m_SmokeTransform;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -3), new TileCoord(2, 0), new TileCoord(0, 1));
		if (m_Smoke == null)
		{
			m_SmokeTransform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Smokepoint");
			if ((bool)ParticlesManager.Instance)
			{
				m_Smoke = ParticlesManager.Instance.CreateParticles("HouseSmoke", m_SmokeTransform.position, Quaternion.Euler(-90f, 0f, 0f));
				m_Smoke.Stop();
			}
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		ParticlesManager.Instance.DestroyParticles(m_Smoke);
		m_Smoke = null;
		base.StopUsing(AndDestroy);
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			m_Smoke.transform.position = m_SmokeTransform.position;
		}
	}

	protected override void UpdateUsage()
	{
		base.UpdateUsage();
		UpdateSmoke();
	}

	private void UpdateSmoke()
	{
		if (m_Smoke == null)
		{
			return;
		}
		if (m_Folks.Count > 0 && !m_Dead.gameObject.activeSelf)
		{
			if (!m_Smoke.m_Particles.isPlaying)
			{
				m_Smoke.Play();
			}
		}
		else if (m_Smoke.m_Particles.isPlaying)
		{
			m_Smoke.Stop();
		}
	}

	public override void AddFolk(Folk NewFolk)
	{
		base.AddFolk(NewFolk);
		UpdateSmoke();
	}

	public override void ReleaseFolk(Folk NewFolk)
	{
		base.ReleaseFolk(NewFolk);
		UpdateSmoke();
	}
}
