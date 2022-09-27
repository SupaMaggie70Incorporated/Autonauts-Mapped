using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaybackSlider : StandardProgressBar
{
	public bool m_UserMoving;

	private GameObject m_SliderTime;

	private BaseText m_SliderText;

	private Image m_Fill;

	protected new void Awake()
	{
		base.Awake();
		m_SliderTime = base.transform.Find("SliderTime").gameObject;
		m_SliderTime.SetActive(value: false);
		m_SliderText = m_SliderTime.transform.Find("Text").GetComponent<BaseText>();
		m_Fill = base.transform.Find("Fill Area/Fill").GetComponent<Image>();
		m_Fill.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		m_SliderTime.SetActive(value: true);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		m_SliderTime.SetActive(value: false);
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		m_UserMoving = true;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		m_UserMoving = false;
	}

	public void SetProgress(float Percent)
	{
		GetWidth();
		m_Fill.GetComponent<RectTransform>().anchorMax = new Vector2(Percent, 1f);
	}

	public void SetProgressToTarget()
	{
		_ = GetComponent<RectTransform>().rect.width;
		_ = GetComponent<RectTransform>().rect.height;
		GetValue();
		m_Fill.GetComponent<RectTransform>().anchorMax = new Vector2(GetValue(), 1f);
	}

	private void UpdateSliderTime()
	{
		Vector3 vector = base.transform.InverseTransformPoint(Input.mousePosition);
		if (m_SliderTime.activeSelf)
		{
			float width = GetComponent<RectTransform>().rect.width;
			float num = (vector.x + width * 0.5f) / width;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			string text = GeneralUtils.ConvertTimeToString(num * RecordingManager.Instance.m_TotalTime);
			m_SliderText.SetText(text);
			Vector3 localPosition = m_SliderTime.transform.localPosition;
			localPosition.x = vector.x;
			m_SliderTime.transform.localPosition = localPosition;
		}
	}

	private void Update()
	{
		UpdateSliderTime();
	}
}
