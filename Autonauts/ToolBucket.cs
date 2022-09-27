using System.Collections.Generic;
using UnityEngine;

public class ToolBucket : ToolFillable
{
	public GameObject m_Liquid;

	private Dictionary<ObjectType, Color> m_PossibleContents;

	public static bool GetIsTypeBucket(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolBucketCrude || NewType == ObjectType.ToolBucket || NewType == ObjectType.ToolBucketMetal)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolBucket", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Liquid = m_ModelRoot.transform.Find("Liquid").gameObject;
		Material sharedMaterial = m_Liquid.GetComponent<MeshRenderer>().sharedMaterial;
		m_Liquid.GetComponent<MeshRenderer>().material = new Material(sharedMaterial);
		m_PossibleContents = new Dictionary<ObjectType, Color>();
		m_PossibleContents.Add(ObjectType.Water, new Color(0.7607843f, 44f / 51f, 44f / 51f));
		m_PossibleContents.Add(ObjectType.SeaWater, new Color(0.7607843f, 44f / 51f, 44f / 51f));
		m_PossibleContents.Add(ObjectType.Milk, new Color(1f, 1f, 1f));
		m_PossibleContents.Add(ObjectType.Sand, new Color(1f, 14f / 15f, 0f));
		m_PossibleContents.Add(ObjectType.Soil, new Color(0.498039216f, 0.2f, 0f));
		m_PossibleContents.Add(ObjectType.Honey, new Color(1f, 0.7647059f, 0f));
		m_PossibleContents.Add(ObjectType.Mortar, new Color(0.5019608f, 0.5019608f, 0f));
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}

	protected override void UpdateContentsModel()
	{
		base.UpdateContentsModel();
		if (m_HeldType == ObjectTypeList.m_Total || !m_PossibleContents.ContainsKey(m_HeldType))
		{
			m_Liquid.SetActive(value: false);
			m_Liquid.GetComponent<MeshRenderer>().enabled = false;
			return;
		}
		Color color = m_PossibleContents[m_HeldType];
		m_Liquid.GetComponent<MeshRenderer>().material.color = color;
		m_Liquid.SetActive(value: true);
		m_Liquid.GetComponent<MeshRenderer>().enabled = true;
	}

	public override void Fill(ObjectType NewType, int Amount)
	{
		base.Fill(NewType, Amount);
		AudioManager.Instance.StartEvent("ToolBucketFill", this);
	}

	public override void Empty(int Amount)
	{
		base.Empty(Amount);
		AudioManager.Instance.StartEvent("ToolBucketEmpty", this);
	}

	public override bool CanAcceptObjectType(ObjectType NewType)
	{
		if (!m_PossibleContents.ContainsKey(NewType))
		{
			return false;
		}
		return base.CanAcceptObjectType(NewType);
	}
}
