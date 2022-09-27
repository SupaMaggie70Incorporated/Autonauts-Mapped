using System.Collections.Generic;
using UnityEngine;

public class PlotObjectMergerManager : MonoBehaviour
{
	public static PlotObjectMergerManager Instance;

	private List<PlotObjectMerger> m_DirtyMergers;

	private void Awake()
	{
		Instance = this;
		m_DirtyMergers = new List<PlotObjectMerger>();
	}

	public void NewDirty(PlotObjectMerger NewMerger)
	{
		if (NewMerger.m_Dirty)
		{
			if (m_DirtyMergers[m_DirtyMergers.Count - 1] == NewMerger)
			{
				return;
			}
			m_DirtyMergers.Remove(NewMerger);
		}
		else
		{
			NewMerger.m_Dirty = true;
		}
		m_DirtyMergers.Add(NewMerger);
	}

	public void RemoveDirty(PlotObjectMerger NewMerger)
	{
		m_DirtyMergers.Remove(NewMerger);
	}

	public void FinishAllDirty()
	{
		foreach (PlotObjectMerger dirtyMerger in m_DirtyMergers)
		{
			dirtyMerger.Merge();
			dirtyMerger.m_Dirty = false;
		}
		m_DirtyMergers.Clear();
	}

	private void Update()
	{
		if (m_DirtyMergers.Count > 0)
		{
			PlotObjectMerger plotObjectMerger = m_DirtyMergers[0];
			m_DirtyMergers.RemoveAt(0);
			plotObjectMerger.Merge();
			plotObjectMerger.m_Dirty = false;
		}
	}
}
