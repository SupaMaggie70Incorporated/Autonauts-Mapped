using System.Collections.Generic;
using UnityEngine;

public class FastButton : MonoBehaviour
{
	private bool m_Fast;

	private StandardButtonImage m_Button;

	private float m_FastFlashTimer;

	private void Awake()
	{
		m_Button = base.transform.Find("FastButton").GetComponent<StandardButtonImage>();
		m_Button.SetActive(Active: false);
		m_Button.SetAction(OnFastClicked, m_Button);
	}

	private void UpdateImage()
	{
		string text = "SlowButton";
		if (TimeManager.Instance.GetIsFastTime())
		{
			text = "FastButton";
		}
		m_Button.SetSprite("Buttons/" + text);
	}

	public void OnFastClicked(BaseGadget NewGadget)
	{
		TimeManager.Instance.ToggleFastTime();
		UpdateImage();
		bool isFastTime = TimeManager.Instance.GetIsFastTime();
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("StoneHenge");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			item.Key.GetComponent<StoneHenge>().SetActive(isFastTime);
		}
	}

	public bool GetActive()
	{
		return m_Button.GetActive();
	}

	public void SetActive(bool Active)
	{
		if (m_Button.GetActive() != Active)
		{
			m_Button.SetActive(Active);
			UpdateImage();
		}
	}

	private void UpdateActive()
	{
		if (TimeManager.Instance.GetIsFastTime())
		{
			m_FastFlashTimer += TimeManager.Instance.m_NormalDelta;
			Color backingColour = new Color(1f, 1f, 1f, 1f);
			if ((int)(m_FastFlashTimer * 60f) % 120 < 60)
			{
				backingColour = new Color(1f, 0f, 0f, 1f);
			}
			m_Button.SetBackingColour(backingColour);
		}
		else
		{
			m_Button.SetBackingColour(new Color(1f, 1f, 1f, 1f));
		}
	}

	private void Update()
	{
		UpdateActive();
	}
}
