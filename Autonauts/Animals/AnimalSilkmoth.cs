using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class AnimalSilkmoth : Animal
{
	private float m_NewTargetTimer;

	private BaseClass m_TargetObject;

	private Vector3 m_TargetPosition;

	private Vector3 m_TargetDelta;

	private float m_TargetHeading;

	private float m_TargetTimer;

	private static float m_Speed = 3f;

	private float m_HeightTimer;

	private float m_Height;

	private float m_HeightDelta;

	private static float m_MinHeight = 8f;

	private static float m_MaxHeight = 10f;

	private float m_FlapTimer;

	private Transform m_Wing1;

	private Transform m_Wing2;

	private Quaternion m_Wing1Rotation;

	private Quaternion m_Wing2Rotation;

	public override void Restart()
	{
		base.Restart();
		m_Height = base.transform.position.y;
		m_TargetPosition = base.transform.position;
		m_TargetPosition.y = 0f;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Wing1 = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Wing1");
		m_Wing1Rotation = m_Wing1.localRotation;
		m_Wing2 = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Wing2");
		m_Wing2Rotation = m_Wing2.localRotation;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		ClearTarget();
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		m_TileCoord = new TileCoord(base.transform.position);
		base.Save(Node);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_TargetPosition = base.transform.position;
		m_TargetPosition.y = 0f;
	}

	public void ClearTarget(bool Deregister = true)
	{
		if (!m_TargetObject)
		{
			return;
		}
		if (Deregister)
		{
			if (m_TargetObject.m_TypeIdentifier == ObjectType.TreeMulberry)
			{
				m_TargetObject.GetComponent<TreeMulberry>().SetSilkmoth(null);
			}
			else if (m_TargetObject.m_TypeIdentifier == ObjectType.SilkwormStation)
			{
				m_TargetObject.GetComponent<SilkwormStation>().RemoveSilkmoth(this);
			}
		}
		m_TargetObject = null;
	}

	private void SetTarget(BaseClass NewTarget)
	{
		m_TargetObject = NewTarget;
		if ((bool)m_TargetObject)
		{
			if (m_TargetObject.m_TypeIdentifier == ObjectType.TreeMulberry)
			{
				m_TargetObject.GetComponent<TreeMulberry>().SetSilkmoth(this);
			}
			else if (m_TargetObject.m_TypeIdentifier == ObjectType.SilkwormStation)
			{
				m_TargetObject.GetComponent<SilkwormStation>().AddSilkmoth(this);
			}
		}
	}

	public void FindTarget()
	{
		ClearTarget();
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("MulberryTree");
		if (collection != null && collection.Count > 0)
		{
			BaseClass baseClass = null;
			float num = 1E+08f;
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				BaseClass key = item.Key;
				float magnitude = (key.transform.position - base.transform.position).magnitude;
				if (magnitude < num)
				{
					TreeMulberry component = key.GetComponent<TreeMulberry>();
					if ((bool)component && component.m_State == MyTree.State.WaitingEmpty && component.GetSilkmoth() == null && component.GetIsSavable())
					{
						num = magnitude;
						baseClass = key;
					}
				}
			}
			if ((bool)baseClass)
			{
				SetTarget(baseClass);
				return;
			}
		}
		Dictionary<BaseClass, int> collection2 = CollectionManager.Instance.GetCollection("SilkwormStation");
		if (collection2 != null && collection2.Count > 0)
		{
			BaseClass baseClass2 = null;
			float num2 = 1E+08f;
			foreach (KeyValuePair<BaseClass, int> item2 in collection2)
			{
				BaseClass key2 = item2.Key;
				float magnitude2 = (key2.transform.position - base.transform.position).magnitude;
				if (magnitude2 < num2)
				{
					SilkwormStation component2 = key2.GetComponent<SilkwormStation>();
					if ((bool)component2 && component2.CanAddSilkmoth() && component2.GetIsSavable())
					{
						num2 = magnitude2;
						baseClass2 = key2;
					}
				}
			}
			if ((bool)baseClass2)
			{
				SetTarget(baseClass2);
				return;
			}
		}
		StopUsing();
	}

	private void UpdateRotation()
	{
		float num = ((0f - Mathf.Atan2(m_TargetDelta.x, m_TargetDelta.z)) * 57.29578f - m_TargetHeading) % 360f;
		if (num > 180f)
		{
			num = 360f - num;
		}
		if (num < -180f)
		{
			num = 360f + num;
		}
		m_TargetHeading += num * TimeManager.Instance.m_NormalDelta * 5f;
		base.transform.localRotation = Quaternion.Euler(0f, 0f - m_TargetHeading + 180f, 0f);
	}

	private void UpdateMoveToTarget()
	{
		if (m_TargetTimer <= 0f)
		{
			Vector3 position = m_TargetObject.transform.position;
			position.x += Random.Range(-3f, 3f);
			position.z += Random.Range(-3f, 3f);
			m_TargetDelta = position - base.transform.position;
			m_TargetDelta.y = 0f;
			m_TargetDelta.Normalize();
			m_TargetDelta *= m_Speed;
			m_TargetTimer = Random.Range(0.5f, 1f);
		}
		m_TargetTimer -= TimeManager.Instance.m_NormalDelta;
		m_TargetPosition += m_TargetDelta * TimeManager.Instance.m_NormalDelta;
	}

	private void UpdateHeightMovement()
	{
		if (m_HeightTimer <= 0f)
		{
			m_HeightTimer = Random.Range(0.5f, 1f);
			float num = Random.Range(m_MinHeight, m_MaxHeight);
			m_HeightDelta = (num - m_Height) / m_HeightTimer;
		}
		m_HeightTimer -= TimeManager.Instance.m_NormalDelta;
		m_Height += m_HeightDelta * TimeManager.Instance.m_NormalDelta;
	}

	private void UpdatePosition()
	{
		Vector3 targetPosition = m_TargetPosition;
		targetPosition.y = m_Height;
		base.transform.position = targetPosition;
	}

	private void UpdateTarget()
	{
		if ((bool)m_TargetObject && m_TargetObject.m_TypeIdentifier == ObjectType.SilkwormStation)
		{
			m_NewTargetTimer += TimeManager.Instance.m_NormalDelta;
			if (m_NewTargetTimer > 1f)
			{
				m_NewTargetTimer = 0f;
				FindTarget();
			}
		}
	}

	private void UpdateFlapAnim()
	{
		m_FlapTimer += TimeManager.Instance.m_NormalDelta;
		bool num = (int)(m_FlapTimer * 60f) % 10 < 5;
		float num2 = 0f;
		if (!num)
		{
			num2 = 70f;
		}
		m_Wing1.localRotation = Quaternion.Euler(0f, 0f, 0f - num2) * m_Wing1Rotation;
		m_Wing2.localRotation = Quaternion.Euler(0f, 0f, num2) * m_Wing2Rotation;
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			if (m_TargetObject == null)
			{
				FindTarget();
			}
			if (m_TargetObject != null)
			{
				UpdateMoveToTarget();
				UpdateHeightMovement();
				UpdatePosition();
				UpdateRotation();
				UpdateTarget();
			}
			UpdateFlapAnim();
		}
	}
}
