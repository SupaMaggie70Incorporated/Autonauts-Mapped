using UnityEngine;

public class RoofTest : MonoBehaviour
{
	private MeshRenderer m_Mesh;

	private void Start()
	{
		m_Mesh = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		m_Mesh.material.color = new Color(1f, 1f, 1f, 1f);
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(CameraManager.Instance.m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hitInfo, 1000f, 16) && hitInfo.collider.gameObject == base.gameObject)
		{
			m_Mesh.material.color = new Color(1f, 1f, 1f, 0.25f);
		}
	}
}
