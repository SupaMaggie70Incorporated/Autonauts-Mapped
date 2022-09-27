using UnityEngine;

public class ToolPitchfork : ToolFillable
{
	private GameObject m_Water;

	private GameObject m_Hay;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolPitchfork", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)m_ModelRoot.transform.Find("Hay"))
		{
			m_Hay = m_ModelRoot.transform.Find("Hay").gameObject;
		}
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
	}

	public void StartUsing()
	{
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public void StopUsing()
	{
		base.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
	}

	protected override void UpdateContentsModel()
	{
		base.UpdateContentsModel();
		if (!m_Hay)
		{
			return;
		}
		if (m_Stored > 0)
		{
			m_Hay.SetActive(value: true);
			MeshRenderer componentInChildren = m_Hay.GetComponentInChildren<MeshRenderer>();
			if ((bool)componentInChildren)
			{
				componentInChildren.enabled = true;
			}
		}
		else
		{
			m_Hay.SetActive(value: false);
		}
	}

	public override void Empty(int Amount)
	{
		base.Empty(Amount);
	}

	public override bool CanAcceptObjectType(ObjectType NewType)
	{
		if (NewType != ObjectType.GrassCut)
		{
			return false;
		}
		return base.CanAcceptObjectType(NewType);
	}

	public void CheckFull(TileCoord LastGrassCoord, Farmer NewFarmer)
	{
		if (GetIsFull())
		{
			Empty(m_Capacity);
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(LastGrassCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.HayBale, randomEmptyTile.ToWorldPositionTileCentered(), base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass, LastGrassCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 3f);
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, NewFarmer.m_TypeIdentifier == ObjectType.Worker, ObjectType.HayBale, this);
		}
	}
}
