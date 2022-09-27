using UnityEngine;

public class CameraSequenceWaypoint : BaseGadget
{
	public GameObject m_Transform;

	private bool m_DOF;

	private float m_DOFFocalDistance;

	private float m_DOFFocalLength;

	private float m_DOFApeture;

	public float m_Time;

	private BaseInputField m_TimeInputField;

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Transform.gameObject);
	}

	private void CheckGadgets()
	{
		if (!m_TimeInputField)
		{
			m_Transform = new GameObject("Waypoint");
			m_TimeInputField = base.transform.Find("Time").GetComponent<BaseInputField>();
			m_TimeInputField.SetAction(OnTimeChanged, m_TimeInputField);
			BaseButton component = base.transform.Find("Remove").GetComponent<BaseButton>();
			component.SetAction(OnDeleteClicked, component);
		}
	}

	private void UpdateData()
	{
		CheckGadgets();
		string text = (int)m_Transform.transform.position.x + "," + (int)m_Transform.transform.position.y + "," + (int)m_Transform.transform.position.z;
		base.transform.Find("Coords").GetComponent<BaseText>().SetText(text);
		m_TimeInputField.SetStartText(m_Time.ToString());
	}

	public void SetData(Vector3 Position, Quaternion Rotation, bool DOF, float DOFFocalDistance, float DOFFocalLength, float DOFApeture, float Time)
	{
		CheckGadgets();
		m_Transform.transform.position = Position;
		m_Transform.transform.rotation = Rotation;
		m_DOF = DOF;
		m_DOFFocalDistance = DOFFocalDistance;
		m_DOFFocalLength = DOFFocalLength;
		m_DOFApeture = DOFApeture;
		m_Time = Time;
		UpdateData();
	}

	public void OnDeleteClicked(BaseGadget NewGadget)
	{
		CameraSequence.Instance.DeleteWaypoint(this);
	}

	public void OnTimeChanged(BaseGadget NewGadget)
	{
		float result = 0f;
		if (float.TryParse(m_TimeInputField.GetText(), out result))
		{
			m_Time = result;
			CameraSequence.Instance.Sort();
		}
	}
}
