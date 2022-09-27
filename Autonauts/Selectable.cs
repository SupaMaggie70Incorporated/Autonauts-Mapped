using cakeslice;
using UnityEngine;

public class Selectable : TileCoordObject
{
	public bool m_Highlighted;

	private bool m_ShowHighlight;

	public static bool GetIsSelectable(TileCoordObject TempObject)
	{
		if (TempObject == null)
		{
			Debug.Log("***");
		}
		if (!Floor.GetIsTypeFloor(TempObject.m_TypeIdentifier) && TempObject.m_TypeIdentifier != ObjectType.FarmerPlayer && TempObject.m_TypeIdentifier != ObjectType.TutorBot && TempObject.m_TypeIdentifier != ObjectType.AnimalBee && TempObject.m_TypeIdentifier != ObjectType.AnimalSilkmoth && (TempObject.m_TypeIdentifier != ObjectType.Worker || TempObject.gameObject.layer == 14) && TempObject.GetIsSavable() && (bool)TempObject.GetComponent<Selectable>())
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Selectable", m_TypeIdentifier);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Highlighted = false;
		m_ShowHighlight = true;
	}

	public void UpdateHighlight()
	{
		if (!m_ShowHighlight)
		{
			return;
		}
		if (m_Highlighted)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].gameObject.GetComponent<Outline>() == null && (((bool)m_Plot && m_Plot.m_Visible) || m_TypeIdentifier == ObjectType.CertificateReward))
				{
					componentsInChildren[i].gameObject.AddComponent<Outline>();
				}
			}
		}
		else
		{
			CleanHighlight();
		}
	}

	public virtual void SetHighlight(bool Highlighted)
	{
		if (m_Highlighted == Highlighted)
		{
			return;
		}
		m_Highlighted = Highlighted;
		if (!SaveLoadManager.m_Video || Input.GetKey(KeyCode.Backslash))
		{
			UpdateHighlight();
			if (Highlighted)
			{
				HudManager.Instance.ActivateHoldableRollover(Activate: true, this);
			}
			else
			{
				HudManager.Instance.ActivateHoldableRollover(Activate: false, this);
			}
		}
	}

	public void ForceHighlight(bool Highlighted)
	{
		m_Highlighted = Highlighted;
		UpdateHighlight();
	}

	public void ShowHighlight(bool Show)
	{
		m_ShowHighlight = Show;
	}

	public virtual bool IsSelectable()
	{
		return true;
	}

	public virtual void LoadNewModel(string ModelName, bool RandomVariants = false)
	{
		InstantiationManager.Instance.LoadModel(this, ModelName, RandomVariants);
		UpdateHighlight();
	}
}
