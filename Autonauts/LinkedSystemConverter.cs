using UnityEngine;

public class LinkedSystemConverter : Converter
{
	private GameObject m_Pulley;

	protected int m_PulleySide;

	private float m_PulleyFlashTimer;

	private MeshRenderer[] m_PulleyMaterials;

	public static bool GetIsTypeLinkedSystemConverter(ObjectType NewType)
	{
		if (NewType == ObjectType.BenchSaw2 || NewType == ObjectType.Gristmill || NewType == ObjectType.BasicMetalWorkbench || NewType == ObjectType.SewingStation || NewType == ObjectType.ClayStation || NewType == ObjectType.MetalWorkbench || NewType == ObjectType.MedicineStation || NewType == ObjectType.PrintingPress || NewType == ObjectType.PaperMill || NewType == ObjectType.Ziggurat || NewType == ObjectType.VehicleAssemblerGood || NewType == ObjectType.LoomGood || NewType == ObjectType.WorkerWorkbenchMk3 || NewType == ObjectType.MortarMixerGood || NewType == ObjectType.SpinningJenny || NewType == ObjectType.TranscendBuilding)
		{
			return true;
		}
		return false;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		CreatePulley();
	}

	public override void SetResultToCreate(int Result)
	{
		base.SetResultToCreate(Result);
		CheckWallsFloors();
	}

	private void CreatePulley()
	{
		Transform newParent = base.transform;
		if ((bool)FindNode("BeltPoint"))
		{
			newParent = FindNode("BeltPoint").transform;
		}
		m_Pulley = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Special/MechanicalPulley", newParent, RandomVariants: false);
		m_Pulley.transform.localPosition = default(Vector3);
		m_Pulley.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
		m_PulleyMaterials = m_Pulley.GetComponentsInChildren<MeshRenderer>();
	}

	public int GetPulleySide()
	{
		return (m_PulleySide + m_Rotation) % 4;
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			RefreshConnection();
		}
	}

	public int GetResultEnergyRequired()
	{
		return VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "LinkedEnergy", CheckValid: false);
	}

	protected override void StartAddAnything(AFO Info)
	{
		base.StartAddAnything(Info);
		if (!SaveLoadManager.Instance.m_Loading && AreRequrementsMet() && m_AutoConvert)
		{
			int resultEnergyRequired = GetResultEnergyRequired();
			((LinkedSystemMechanical)m_LinkedSystem).UseEnergy(resultEnergyRequired);
		}
	}

	protected override ActionType GetActionFromAnything(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_LinkedSystem == null)
		{
			return ActionType.Fail;
		}
		int resultEnergyRequired = GetResultEnergyRequired();
		if (!((LinkedSystemMechanical)m_LinkedSystem).GetIsEnergyAvailable(resultEnergyRequired))
		{
			return ActionType.Fail;
		}
		return base.GetActionFromAnything(Info);
	}

	public override void SetLinkedSystem(LinkedSystem NewSystem)
	{
		base.SetLinkedSystem(NewSystem);
		UpdatePulleyState();
	}

	private void UpdatePulleyState()
	{
		if (!(m_Pulley == null))
		{
			if (m_LinkedSystem == null || m_LinkedSystem.m_Buildings.Count == 1)
			{
				m_Pulley.SetActive(value: true);
			}
			else
			{
				m_Pulley.SetActive(value: false);
			}
		}
	}

	protected void UpdatePulley()
	{
		UpdatePulleyState();
		if (m_Pulley.gameObject.activeSelf)
		{
			m_PulleyFlashTimer += TimeManager.Instance.m_NormalDelta;
			Material sharedMaterial = MaterialManager.Instance.m_MaterialPulleyOff;
			if ((int)(m_PulleyFlashTimer * 60f) % 12 < 6)
			{
				sharedMaterial = MaterialManager.Instance.m_MaterialPulleyOn;
			}
			MeshRenderer[] pulleyMaterials = m_PulleyMaterials;
			for (int i = 0; i < pulleyMaterials.Length; i++)
			{
				pulleyMaterials[i].sharedMaterial = sharedMaterial;
			}
		}
	}

	protected new void Update()
	{
		base.Update();
		UpdatePulley();
	}
}
