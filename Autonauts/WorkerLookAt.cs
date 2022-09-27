using UnityEngine;

public class WorkerLookAt : BaseClass
{
	private static float m_StartDistanceFromOwner = 1f;

	private Farmer m_Owner;

	private Farmer m_Target;

	private MeshRenderer m_MeshRenderer;

	private float m_TextureOffset;

	public void SetOwnerAndTarget(Farmer Owner, Farmer Target)
	{
		m_Owner = Owner;
		m_Target = Target;
		Material material = (Material)Resources.Load("Materials/WorkerLookAt", typeof(Material));
		m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
		m_MeshRenderer.material = material;
	}

	public void SetActive(bool Active)
	{
		base.gameObject.SetActive(Active);
	}

	public void UpdateShape()
	{
		if (!(m_Target == null) && !(m_Owner == null))
		{
			base.transform.position = m_Owner.transform.TransformPoint(new Vector3(0f, 2f, 0f - m_StartDistanceFromOwner));
			base.transform.LookAt(m_Target.transform.position + new Vector3(0f, 2f, 0f));
			float magnitude = (m_Target.transform.position - m_Owner.transform.position).magnitude;
			magnitude -= 2f * m_StartDistanceFromOwner;
			if (magnitude < 0f)
			{
				magnitude = 0f;
			}
			base.transform.localScale = new Vector3(0.5f, 0.01f, magnitude);
			m_MeshRenderer.material.mainTextureScale = new Vector2(magnitude / 3f, 1f);
			float textureOffset = m_TextureOffset;
			m_MeshRenderer.material.mainTextureOffset = new Vector2(textureOffset, 0f);
			if ((bool)TimeManager.Instance)
			{
				m_TextureOffset += TimeManager.Instance.m_NormalDelta * -5f;
			}
		}
	}

	private void Update()
	{
		UpdateShape();
	}
}
