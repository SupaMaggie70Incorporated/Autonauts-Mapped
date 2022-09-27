using SimpleJSON;
using UnityEngine;

public class AnimalSilkworm : Holdable
{
	public enum State
	{
		Wait,
		Wriggle,
		StationIdle,
		StationConverting,
		StationConverted,
		StationTaking,
		Total
	}

	public State m_State;

	private float m_StateTimer;

	private float m_WriggleTimer;

	private float m_Scale;

	private float m_ConvertTime;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Parts/Clothes/SilkRaw", ObjectType.AnimalSilkworm);
	}

	public override void Restart()
	{
		base.Restart();
		m_Scale = Random.Range(0.75f, 1.25f);
		base.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
		m_ConvertTime = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "ConversionDelay");
		SetState(State.Wait);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		LoadNewModel("Models/Animals/AnimalSilkworm");
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		State asInt = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		float asFloat = JSONUtils.GetAsFloat(Node, "STT", 0f);
		SetState(asInt);
		m_StateTimer = asFloat;
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.StationTaking)
		{
			m_ModelRoot.SetActive(value: true);
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Wait:
			m_StateTimer = 0f - Random.Range(1f, 2f);
			break;
		case State.StationTaking:
			m_ModelRoot.SetActive(value: false);
			break;
		case State.StationConverted:
			LoadNewModel("Models/Parts/Clothes/SilkRaw");
			break;
		}
	}

	private void UpdateStationConverting()
	{
		if (m_StateTimer >= m_ConvertTime)
		{
			base.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
			SetState(State.StationConverted);
			return;
		}
		float num = m_Scale;
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			num *= 1.2f;
		}
		m_ModelRoot.transform.localScale = new Vector3(num, num, num);
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Wait:
			if (m_StateTimer >= 0f)
			{
				SetState(State.Wriggle);
			}
			break;
		case State.Wriggle:
			if (m_StateTimer >= 0.25f)
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				SetState(State.Wait);
			}
			else if ((int)(m_StateTimer * 60f) % 10 < 5)
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 10f, 0f);
			}
			else
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, -10f, 0f);
			}
			break;
		case State.StationConverting:
			UpdateStationConverting();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
