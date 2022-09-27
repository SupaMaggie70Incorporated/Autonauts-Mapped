using System.Collections.Generic;
using UnityEngine;

public class AreaIndicatorManager : MonoBehaviour
{
	public static AreaIndicatorManager Instance;

	private List<AreaIndicator> m_Areas;

	private List<AreaIndicator> m_UsedAreas;

	private void Awake()
	{
		Instance = this;
		m_Areas = new List<AreaIndicator>();
		m_UsedAreas = new List<AreaIndicator>();
	}

	public AreaIndicator Add()
	{
		AreaIndicator areaIndicator;
		if (m_Areas.Count == 0)
		{
			areaIndicator = Object.Instantiate((GameObject)Resources.Load("Prefabs/AreaIndicator", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, MapManager.Instance.m_MiscRootTransform).GetComponent<AreaIndicator>();
		}
		else
		{
			areaIndicator = m_Areas[0];
			areaIndicator.Restart();
			m_Areas.RemoveAt(0);
		}
		areaIndicator.SetUsed(Used: true);
		m_UsedAreas.Add(areaIndicator);
		return areaIndicator;
	}

	public void Remove(AreaIndicator NewArea)
	{
		m_UsedAreas.Remove(NewArea);
		m_Areas.Add(NewArea);
		NewArea.SetUsed(Used: false);
	}

	public AreaIndicator GetAreaIndicatorFromInstruction(HighInstruction NewInstruction)
	{
		if (NewInstruction.m_ActionInfo.m_ObjectUID != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(NewInstruction.m_ActionInfo.m_ObjectUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				if (Sign.GetIsTypeSign(objectFromUniqueID.m_TypeIdentifier))
				{
					return objectFromUniqueID.GetComponent<Sign>().m_Indicator;
				}
				if ((bool)objectFromUniqueID.GetComponent<Converter>())
				{
					return objectFromUniqueID.GetComponent<Converter>().m_Indicator;
				}
			}
		}
		else
		{
			foreach (AreaIndicator usedArea in m_UsedAreas)
			{
				if (usedArea.m_Instruction == NewInstruction)
				{
					return usedArea;
				}
			}
		}
		return null;
	}

	public void ActivateInstructionArea(HighInstruction NewInstruction, bool Active)
	{
		AreaIndicator areaIndicatorFromInstruction = GetAreaIndicatorFromInstruction(NewInstruction);
		if ((bool)areaIndicatorFromInstruction)
		{
			areaIndicatorFromInstruction.SetActive(Active);
		}
	}
}
