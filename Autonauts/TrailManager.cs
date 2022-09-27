using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
	public static TrailManager Instance;

	private List<TrailMaker> m_AvailableTrails;

	private List<TrailMaker> m_ActiveTrails;

	private List<TrailMaker2> m_AvailableTrails2;

	private List<TrailMaker2> m_ActiveTrails2;

	private void Awake()
	{
		Instance = this;
		m_AvailableTrails = new List<TrailMaker>();
		m_ActiveTrails = new List<TrailMaker>();
		m_AvailableTrails2 = new List<TrailMaker2>();
		m_ActiveTrails2 = new List<TrailMaker2>();
	}

	private void OnDestroy()
	{
		foreach (TrailMaker activeTrail in m_ActiveTrails)
		{
			if ((bool)activeTrail)
			{
				Object.DestroyImmediate(activeTrail.gameObject);
			}
		}
		foreach (TrailMaker2 item in m_ActiveTrails2)
		{
			if ((bool)item)
			{
				Object.DestroyImmediate(item.gameObject);
			}
		}
	}

	public TrailMaker2 StartTrail(Transform FollowObject)
	{
		TrailMaker2 trailMaker;
		if (m_AvailableTrails2.Count == 0)
		{
			trailMaker = Object.Instantiate((GameObject)Resources.Load("Prefabs/TrailMaker2", typeof(GameObject)), default(Vector3), Quaternion.identity, MapManager.Instance.m_MiscRootTransform).GetComponent<TrailMaker2>();
		}
		else
		{
			trailMaker = m_AvailableTrails2[0];
			m_AvailableTrails2.RemoveAt(0);
		}
		trailMaker.StartTrail(FollowObject);
		trailMaker.gameObject.SetActive(value: true);
		m_ActiveTrails2.Add(trailMaker);
		return trailMaker;
	}

	public void StopTrail(TrailMaker2 NewTrailMaker)
	{
		if (!(NewTrailMaker == null))
		{
			NewTrailMaker.Stop();
		}
	}

	public void TrailFinished(TrailMaker2 NewTrailMaker)
	{
		NewTrailMaker.gameObject.SetActive(value: false);
		m_ActiveTrails2.Remove(NewTrailMaker);
		m_AvailableTrails2.Add(NewTrailMaker);
	}

	public TrailMaker StartTrail(Transform FollowObject, Vector3 StartPoint, Vector3 EndPoint)
	{
		TrailMaker trailMaker;
		if (m_AvailableTrails.Count == 0)
		{
			trailMaker = Object.Instantiate((GameObject)Resources.Load("Prefabs/TrailMaker", typeof(GameObject)), default(Vector3), Quaternion.identity, MapManager.Instance.m_MiscRootTransform).GetComponent<TrailMaker>();
		}
		else
		{
			trailMaker = m_AvailableTrails[0];
			trailMaker.gameObject.SetActive(value: true);
			m_AvailableTrails.RemoveAt(0);
		}
		m_ActiveTrails.Add(trailMaker);
		trailMaker.StartPreTrail(FollowObject, StartPoint, EndPoint);
		return trailMaker;
	}

	public void StopTrail(TrailMaker NewTrailMaker)
	{
		if (!(NewTrailMaker == null))
		{
			NewTrailMaker.EndPreTrail();
		}
	}

	public void TrailFinished(TrailMaker NewTrailMaker)
	{
		NewTrailMaker.gameObject.SetActive(value: false);
		m_ActiveTrails.Remove(NewTrailMaker);
		m_AvailableTrails.Add(NewTrailMaker);
	}
}
