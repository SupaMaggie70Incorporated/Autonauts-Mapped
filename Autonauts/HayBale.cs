using SimpleJSON;
using UnityEngine;

public class HayBale : Holdable
{
	private int m_EatsLeft;

	private int m_MaxEats;

	private Wobbler m_Wobbler;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Other/HayBaleEaten", ObjectType.HayBale);
	}

	public override void Restart()
	{
		base.Restart();
		m_MaxEats = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "MaxEats");
		m_EatsLeft = m_MaxEats;
		m_Wobbler.Restart();
		base.enabled = true;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Eaten", m_EatsLeft);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_EatsLeft = JSONUtils.GetAsInt(Node, "Eaten", 0);
		UpdateModel();
	}

	public override string GetHumanReadableName()
	{
		if (m_EatsLeft != m_MaxEats)
		{
			return TextManager.Instance.Get("HayBaleEaten");
		}
		return base.GetHumanReadableName();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker && m_EatsLeft != m_MaxEats)
		{
			return ActionType.Total;
		}
		return base.GetActionFromObject(Info);
	}

	public void Nibble()
	{
		if (!m_BeingHeld)
		{
			base.enabled = true;
			m_Wobbler.Go(0.5f, 5f, 0.2f);
		}
	}

	private void UpdateModel()
	{
		if (m_EatsLeft != m_MaxEats)
		{
			LoadNewModel("Models/Other/HayBaleEaten");
		}
	}

	public void Eat()
	{
		if (!m_BeingHeld)
		{
			bool flag = false;
			if (m_EatsLeft == m_MaxEats)
			{
				flag = true;
			}
			m_EatsLeft--;
			if (m_EatsLeft == 0)
			{
				StopUsing();
			}
			else if (flag)
			{
				UpdateModel();
			}
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if (action == ActionType.Bump)
		{
			Nibble();
		}
	}

	private void Update()
	{
		m_Wobbler.Update();
		float y = 1f + m_Wobbler.m_Height;
		base.transform.localScale = new Vector3(1f, y, 1f);
		if (!m_Wobbler.m_Wobbling)
		{
			if (m_EatsLeft == m_MaxEats)
			{
				LoadNewModel("Models/Other/HayBale");
			}
			base.enabled = false;
		}
	}
}
