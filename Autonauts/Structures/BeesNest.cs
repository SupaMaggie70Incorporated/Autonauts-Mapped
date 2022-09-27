using System.Collections.Generic;
using UnityEngine;

public class BeesNest : Holdable
{
	private float m_NestRadius = 1.25f;

	private float m_NestHeight = 1f;

	private List<BeesNestBee> m_Bees;

	private MyTree m_TreeParent;

	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		if (m_PlaySound == null)
		{
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalBeeIdle", this, Remember: true, Follow: true);
		}
	}

	protected new void Awake()
	{
		base.Awake();
		m_TreeParent = null;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_PlaySound != null)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
			m_PlaySound = null;
		}
		if ((bool)m_TreeParent)
		{
			m_TreeParent.m_BeesNest = null;
		}
		foreach (BeesNestBee bee in m_Bees)
		{
			Object.Destroy(bee.m_Bee);
		}
		m_Bees.Clear();
		base.StopUsing(AndDestroy);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Bees = new List<BeesNestBee>();
	}

	public void CreatedInTree(MyTree NewTree)
	{
		m_TreeParent = NewTree;
		foreach (BeesNestBee bee in m_Bees)
		{
			bee.UpdateNest();
		}
		UpdatePlotVisibility();
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Stowed:
			foreach (BeesNestBee bee in m_Bees)
			{
				bee.m_Bee.SetActive(value: false);
			}
			break;
		case ActionType.Recalled:
			foreach (BeesNestBee bee2 in m_Bees)
			{
				bee2.m_Bee.SetActive(value: true);
				bee2.UpdateNest();
			}
			break;
		}
		base.SendAction(Info);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		m_TreeParent = null;
		base.ActionDropped(PreviousHolder, DropLocation);
	}

	public override void UpdatePlotVisibility()
	{
		if ((bool)m_TreeParent && (bool)m_TreeParent.m_Plot)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = m_TreeParent.m_Plot.m_Visible;
			}
			{
				foreach (BeesNestBee bee in m_Bees)
				{
					if ((bool)bee.m_Bee)
					{
						componentsInChildren = bee.m_Bee.GetComponentsInChildren<MeshRenderer>();
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].enabled = m_TreeParent.m_Plot.m_Visible;
						}
					}
				}
				return;
			}
		}
		base.UpdatePlotVisibility();
		bool flag = true;
		if (m_HiddenWithPlot && (bool)m_Plot)
		{
			flag = m_Plot.m_Visible;
		}
		foreach (BeesNestBee bee2 in m_Bees)
		{
			if ((bool)bee2.m_Bee)
			{
				MeshRenderer[] componentsInChildren = bee2.m_Bee.GetComponentsInChildren<MeshRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = flag;
				}
			}
		}
	}

	private void Update()
	{
		if (m_Bees.Count == 0)
		{
			GameObject original = ModelManager.Instance.Load(ObjectType.AnimalBee, "Models/Animals/AnimalBee", RandomVariants: false);
			m_Bees = new List<BeesNestBee>();
			for (int i = 0; i < 2; i++)
			{
				GameObject obj = Object.Instantiate(original, default(Vector3), Quaternion.identity, MapManager.Instance.m_AnimalsRootTransform);
				obj.SetActive(value: true);
				Vector3 position = default(Vector3);
				float f = Random.Range(0, 360);
				position.x = Mathf.Cos(f) * m_NestRadius;
				position.z = Mathf.Sin(f) * m_NestRadius;
				position.y = Random.Range(0f, m_NestHeight);
				BeesNestBee item = new BeesNestBee(obj, position, this);
				m_Bees.Add(item);
			}
			UpdatePlotVisibility();
		}
		foreach (BeesNestBee bee in m_Bees)
		{
			bee.Update();
		}
	}
}
