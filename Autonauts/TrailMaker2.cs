using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class TrailMaker2 : MonoBehaviour
{
	private float m_Time;

	private SmoothTrail m_Trail;

	private Transform m_FollowObject;

	private int m_Starting;

	private bool m_Stopping;

	private void Awake()
	{
		m_Trail = GetComponent<SmoothTrail>();
	}

	public void StartTrail(Transform FollowObject)
	{
		m_FollowObject = FollowObject;
		Bounds bounds = ObjectUtils.ObjectBounds(FollowObject.gameObject);
		m_Trail.TrailData.SizeOverLife.RemoveKey(0);
		m_Trail.TrailData.SizeOverLife.RemoveKey(0);
		m_Trail.TrailData.SizeOverLife.AddKey(0f, bounds.size.y / 2f);
		m_Trail.TrailData.SizeOverLife.AddKey(1f, bounds.size.y / 2f);
		m_Starting = 1;
		m_Trail.ClearSystem(emitState: false);
		base.transform.SetParent(m_FollowObject);
		base.transform.localPosition = default(Vector3);
		base.transform.localRotation = Quaternion.identity;
		m_Trail.ClearSystem(emitState: false);
		m_Stopping = false;
	}

	public void Stop()
	{
		m_Stopping = true;
		base.transform.SetParent(MapManager.Instance.m_UnusedRootTransform);
	}

	private void Update()
	{
		if (m_FollowObject == null || m_Stopping)
		{
			m_Stopping = false;
			m_Trail.Emit = false;
			m_FollowObject = null;
		}
		if (m_Starting > 0)
		{
			m_Starting--;
			if (m_Starting == 0)
			{
				m_Trail.ClearSystem(emitState: true);
			}
		}
		else if (!m_Trail.GetActive())
		{
			m_Trail.ClearSystem(emitState: false);
			TrailManager.Instance.TrailFinished(this);
		}
	}
}
