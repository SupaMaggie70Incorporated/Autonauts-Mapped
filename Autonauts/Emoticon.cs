using UnityEngine;
using UnityEngine.UI;

public class Emoticon : Effect2D
{
	private Vector3 m_Velocity;

	private float m_Life;

	private Image m_Background;

	private BaseClass m_FollowObject;

	public override void Restart()
	{
		base.Restart();
		m_Velocity = new Vector3(0f, 2f, 0f);
		m_Life = 1f;
		UpdateTransform();
		m_FollowObject = null;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Image = base.transform.Find("Image").GetComponent<Image>();
		m_Background = base.transform.GetComponent<Image>();
	}

	public void Follow(BaseClass FollowObject)
	{
		m_FollowObject = FollowObject;
	}

	public void SetEmoticon(string Name, float Life, string BackgroundName)
	{
		if (Name == "")
		{
			m_Image.gameObject.SetActive(value: false);
		}
		else
		{
			m_Image.gameObject.SetActive(value: true);
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Effects/Emoticons/" + Name, typeof(Sprite));
			m_Image.sprite = sprite;
		}
		m_Life = Life;
		if (BackgroundName == "")
		{
			m_Background.gameObject.SetActive(value: false);
			return;
		}
		m_Background.gameObject.SetActive(value: true);
		Sprite sprite2 = (Sprite)Resources.Load("Textures/Hud/Effects/Emoticons/" + BackgroundName, typeof(Sprite));
		m_Background.sprite = sprite2;
	}

	private void Update()
	{
		m_Velocity *= 0.5f;
		m_WorldPosition += m_Velocity;
		if ((bool)m_FollowObject)
		{
			m_WorldPosition.x = m_FollowObject.transform.position.x;
			m_WorldPosition.z = m_FollowObject.transform.position.z;
		}
		UpdateTransform();
		if ((bool)TimeManager.Instance)
		{
			m_Life -= TimeManager.Instance.m_NormalDelta;
			if (m_Life <= 0f)
			{
				StopUsing();
			}
		}
	}
}
