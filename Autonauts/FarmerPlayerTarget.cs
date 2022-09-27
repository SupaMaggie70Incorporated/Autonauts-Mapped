using UnityEngine;

public class FarmerPlayerTarget : MonoBehaviour
{
	private bool m_Active;

	private float m_Delay;

	private bool m_Flash;

	private float m_FlashTimer;

	private MeshRenderer m_Mesh;

	private void Awake()
	{
		m_Active = true;
		m_Delay = 0f;
	}

	public void SetPosition(TileCoord Position)
	{
		base.transform.position = Position.ToWorldPositionTileCentered();
	}

	private void UpdateVisible()
	{
		if (SaveLoadManager.m_Video)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (m_Active)
		{
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetActive(bool Active)
	{
		m_Active = Active;
		UpdateVisible();
		m_Flash = false;
		if (m_Mesh == null)
		{
			m_Mesh = GetComponentInChildren<MeshRenderer>();
		}
		m_Mesh.enabled = true;
		m_Mesh.material.color = GeneralUtils.GetIndicatorColour();
	}

	public void Delay()
	{
		m_Delay = 0.25f;
		m_Flash = false;
	}

	public void Flash()
	{
		m_Delay = 1f;
		m_Flash = true;
		m_FlashTimer = 0f;
	}

	public void SetActionType(AFO.AT NewActionType)
	{
		string text = "PlayerGoTo";
		switch (NewActionType)
		{
		case AFO.AT.Secondary:
		case AFO.AT.AltSecondary:
			text = "PlayerGoTo2";
			break;
		case AFO.AT.AltPrimary:
			text = "PlayerGoTo3";
			break;
		}
		Texture value = (Texture)Resources.Load("Textures/" + text, typeof(Texture));
		MeshRenderer componentInChildren = GetComponentInChildren<MeshRenderer>();
		componentInChildren.material.SetTexture("_MainTex", value);
		componentInChildren.material.SetTexture("_EmissionMap", value);
		componentInChildren.material.SetColor("_EmissionColor", GeneralUtils.GetIndicatorColour());
	}

	private void Update()
	{
		if (m_Delay > 0f)
		{
			m_Delay -= TimeManager.Instance.m_NormalDelta;
			if (m_Delay <= 0f)
			{
				m_Delay = 0f;
				SetActive(Active: false);
			}
		}
		if (m_Flash)
		{
			m_FlashTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_FlashTimer * 60f) % 10 < 5)
			{
				m_Mesh.enabled = true;
			}
			else
			{
				m_Mesh.enabled = false;
			}
		}
	}
}
