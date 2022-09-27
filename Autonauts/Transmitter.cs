using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Transmitter : Building
{
	public static Transmitter Instance;

	public static TileCoord m_StartingOffsetFromPlayer = new TileCoord(1, -2);

	private MyParticles m_Particles;

	private MyParticles m_Particles2;

	private PlaySound m_Sound;

	private Dictionary<Quest.ID, CertificateReward> m_Rewards;

	public override void CheckAddCollectable(bool FromLoad)
	{
		base.CheckAddCollectable(FromLoad);
		if (!ObjectTypeList.m_Loading || FromLoad)
		{
			CollectionManager.Instance.AddCollectable("Transmitter", this);
		}
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		HideAccessModel();
		if (m_Particles == null && (bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("Transmitter", default(Vector3), Quaternion.Euler(-90f, 0f, 0f));
			m_Particles2 = ParticlesManager.Instance.CreateParticles("Transmitter", new Vector3(0f, 26f, 0f), Quaternion.Euler(90f, 0f, 0f));
		}
		m_Particles.Clear();
		m_Particles.Stop();
		m_Particles2.Clear();
		m_Particles2.Stop();
	}

	public override void PostCreate()
	{
		Instance = this;
		base.PostCreate();
		m_Rewards = new Dictionary<Quest.ID, CertificateReward>();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		foreach (KeyValuePair<Quest.ID, CertificateReward> reward in m_Rewards)
		{
			CertificateReward component = reward.Value.GetComponent<CertificateReward>();
			component.transform.SetParent(null);
			component.StopUsing(AndDestroy);
		}
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles);
			m_Particles = null;
			ParticlesManager.Instance.DestroyParticles(m_Particles2);
			m_Particles2 = null;
		}
		base.StopUsing(AndDestroy);
	}

	protected override void Start()
	{
		base.Start();
		UpdateRewards(ShowCeremony: false);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Transmitter", this);
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		_ = action - 41;
		_ = 1;
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		return Info.m_Action switch
		{
			GetAction.IsDeletable => false, 
			GetAction.IsDuplicatable => false, 
			_ => base.GetActionInfo(Info), 
		};
	}

	public void SetReceiving(bool Receiving)
	{
		m_Particles2.transform.position = base.transform.position + new Vector3(0f, 16f, 0f);
		if (Receiving)
		{
			m_Particles2.Play();
			m_Sound = AudioManager.Instance.StartEvent("BuildingTransmitter", this, Remember: true, Follow: true);
		}
		else
		{
			m_Particles2.Stop();
			AudioManager.Instance.StopEvent(m_Sound);
		}
	}

	public void SetTransmitting(bool Transmit)
	{
		m_Particles.transform.position = base.transform.position;
		if (Transmit)
		{
			m_Particles.Play();
			m_Sound = AudioManager.Instance.StartEvent("BuildingTransmitter", this, Remember: true, Follow: true);
			QuestManager.Instance.AddEvent(QuestEvent.Type.Communicate, Bot: false, 0, this);
		}
		else
		{
			m_Particles.Stop();
			AudioManager.Instance.StopEvent(m_Sound);
		}
	}

	public static void SetTransmittingGlobal(bool Transmit)
	{
		using Dictionary<BaseClass, int>.Enumerator enumerator = CollectionManager.Instance.GetCollection("Transmitter").GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.Key.GetComponent<Transmitter>().SetTransmitting(Transmit);
		}
	}

	public void UpdateRewards(bool ShowCeremony)
	{
		if (QuestManager.Instance == null)
		{
			return;
		}
		bool flag = false;
		foreach (CertificateInfo certificateInfo in QuestData.Instance.m_AcademyData.m_CertificateInfos)
		{
			Quest.ID iD = certificateInfo.m_ID;
			if (QuestManager.Instance.GetQuest(iD) != null && QuestManager.Instance.GetQuest(iD).GetIsComplete() && !m_Rewards.ContainsKey(iD))
			{
				CertificateReward component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CertificateReward, base.transform.position, Quaternion.identity).GetComponent<CertificateReward>();
				component.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
				component.SetQuest(iD);
				component.transform.SetParent(base.transform);
				component.transform.localRotation = Quaternion.identity;
				m_Rewards.Add(iD, component);
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		float height = ObjectTypeList.Instance.GetHeight(ObjectType.CertificateReward);
		int num = m_Rewards.Count - 1;
		foreach (KeyValuePair<Quest.ID, CertificateReward> reward in m_Rewards)
		{
			reward.Value.transform.localPosition = new Vector3(0f, height * (float)num, 0f);
			num--;
		}
		m_ModelRoot.transform.localPosition = new Vector3(0f, height * (float)m_Rewards.Count, 0f);
		if (ShowCeremony)
		{
			Vector3 localPosition = base.transform.position + new Vector3(0f, 3f, 0f);
			MyParticles myParticles = ParticlesManager.Instance.CreateParticles("TransmitterReward", localPosition, Quaternion.Euler(90f, 0f, 0f));
			myParticles.transform.localScale = new Vector3(3f, 3f, 3f);
			ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
			AudioManager.Instance.StartEvent("TransmitterReward", this);
		}
	}
}
