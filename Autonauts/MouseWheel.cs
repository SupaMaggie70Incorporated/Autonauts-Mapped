using UnityEngine;

public class MouseWheel : MonoBehaviour
{
	private Transform m_ContentParent;

	private GameObject m_Viewport;

	private void Awake()
	{
		m_Viewport = base.transform.Find("Viewport").gameObject;
		m_ContentParent = base.transform.Find("Viewport").Find("Content");
	}

	private void UpdateMouseWheel()
	{
		Debug.Log("Ah");
		Rect rect = GetComponent<RectTransform>().rect;
		Vector3 vector = m_Viewport.transform.InverseTransformPoint(HudManager.Instance.GetMouseInWorldSpace());
		if (vector.x < 0f || vector.x > rect.width || vector.y > 0f || vector.y < 0f - rect.height)
		{
			return;
		}
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.MouseWheelFocusLost();
		}
		float height = m_Viewport.GetComponent<RectTransform>().rect.height;
		float height2 = m_ContentParent.GetComponent<RectTransform>().rect.height;
		float num = TimeManager.Instance.m_NormalDelta;
		if (num == 0f)
		{
			num = 1f / 60f;
		}
		if (Input.GetMouseButton(0))
		{
			return;
		}
		float axis = MyInputManager.m_Rewired.GetAxis("MouseScrollWheel");
		if (axis > 0f)
		{
			Vector3 localPosition = m_ContentParent.localPosition;
			localPosition.y -= 2000f * num;
			if (localPosition.y < 0f)
			{
				localPosition.y = 0f;
			}
			m_ContentParent.localPosition = localPosition;
		}
		if (axis < 0f)
		{
			Vector3 localPosition2 = m_ContentParent.localPosition;
			localPosition2.y += 2000f * num;
			float num2 = height2 - height;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (localPosition2.y > num2)
			{
				localPosition2.y = num2;
			}
			m_ContentParent.localPosition = localPosition2;
		}
	}

	private void Update()
	{
	}
}
