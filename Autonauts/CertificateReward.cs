using UnityEngine;

public class CertificateReward : Holdable
{
	public Quest.ID m_QuestID;

	protected override void ActionBeingHeld(Actionable Holder)
	{
		if (!m_BeingHeld)
		{
			SetIsSavable(IsSavable: false);
			m_BeingHeld = true;
			PlotManager.Instance.RemoveObject(this);
			DespawnManager.Instance.Remove(this);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsPickable)
		{
			return true;
		}
		return base.GetActionInfo(Info);
	}

	public override bool IsSelectable()
	{
		return true;
	}

	public void SetQuest(Quest.ID NewQuestID)
	{
		m_QuestID = NewQuestID;
		Quest quest = QuestManager.Instance.GetQuest(m_QuestID);
		string iconName = quest.GetIconName();
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + iconName, typeof(Sprite));
		Transform transform = m_ModelRoot.transform.Find("Reward");
		if ((bool)transform)
		{
			transform.GetComponent<MeshRenderer>().material.color = quest.m_Colour;
		}
		transform = m_ModelRoot.transform.Find("Sign");
		if ((bool)transform)
		{
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			if ((bool)sprite)
			{
				component.material.SetTexture("_MainTex", sprite.texture);
			}
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.rotation = Quaternion.identity;
	}
}
