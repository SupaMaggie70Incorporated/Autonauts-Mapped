using System.Collections.Generic;
using UnityEngine;

public class CameraStatePlaySequence : CameraState
{
	private Vector3 m_PlayCameraPosition;

	private Quaternion m_PlayCameraRotation;

	public CameraStatePlaySequence(Camera NewCamera)
		: base(NewCamera)
	{
	}

	public override void StartUse()
	{
		base.StartUse();
		List<Transform> list = new List<Transform>();
		List<float> list2 = new List<float>();
		foreach (CameraSequenceWaypoint waypoint in CameraSequence.Instance.m_Waypoints)
		{
			list.Add(waypoint.m_Transform.transform);
			list2.Add(waypoint.m_Time);
		}
		CameraManager.Instance.StartSpline(list, list2);
	}
}
