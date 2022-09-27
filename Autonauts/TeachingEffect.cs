using UnityEngine;

public class TeachingEffect : MonoBehaviour
{
	private GameObject m_RecordSprite;

	private float m_RecordFlashTimer;

	private void Awake()
	{
		m_RecordSprite = base.transform.Find("Icon").gameObject;
		m_RecordFlashTimer = 0f;
	}

	private void Update()
	{
		m_RecordFlashTimer += TimeManager.Instance.m_NormalDelta;
		if ((int)(m_RecordFlashTimer * 60f) % 30 < 15)
		{
			m_RecordSprite.SetActive(value: true);
		}
		else
		{
			m_RecordSprite.SetActive(value: false);
		}
	}
}
