using UnityEngine;

public class FlowerPotRollover : Rollover
{
	[HideInInspector]
	public FlowerPot m_Target;

	private BaseText m_Text;

	private BaseText m_Seeds;

	private BaseText m_Fertiliser;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Text = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		m_Seeds = m_Panel.transform.Find("Seeds").GetComponent<BaseText>();
		m_Fertiliser = m_Panel.transform.Find("Fertiliser").GetComponent<BaseText>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		string text = "0";
		if (m_Target.m_State == FlowerPot.State.Empty || m_Target.m_State == FlowerPot.State.Fertiliser)
		{
			text = "1";
		}
		m_Seeds.SetText(text);
		text = "0";
		if (m_Target.m_State == FlowerPot.State.Empty || m_Target.m_State == FlowerPot.State.Seeds || m_Target.m_State == FlowerPot.State.Dead)
		{
			text = "1";
		}
		m_Fertiliser.SetText(text);
		string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(ObjectType.FlowerPot);
		string text2 = TextManager.Instance.Get(saveNameFromIdentifier);
		int type = (int)m_Target.GetComponent<FlowerPot>().m_Type;
		if (type != 7)
		{
			string text3 = TextManager.Instance.Get(FlowerWild.m_TypeNames[type]);
			text2 = text2 + " (" + text3 + ")";
		}
		m_Text.SetText(text2);
	}

	public void SetTarget(FlowerPot Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)m_Target)
			{
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
