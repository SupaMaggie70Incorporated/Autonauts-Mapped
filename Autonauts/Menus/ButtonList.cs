using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class ButtonList : MonoBehaviour
{
	[SerializeField]
	public int m_ObjectCount;

	[SerializeField]
	public GameObject m_Object;

	[SerializeField]
	public float m_ButtonWidth = 100f;

	[SerializeField]
	public float m_ButtonHeight = 30f;

	[SerializeField]
	public int m_ButtonsPerRow = 1;

	[SerializeField]
	public float m_HorizontalSpacing = 40f;

	[SerializeField]
	public float m_VerticalSpacing = 40f;

	[HideInInspector]
	public List<BaseGadget> m_Buttons;

	[SerializeField]
	private float m_AnchorX = 0.5f;

	[SerializeField]
	private float m_AnchorY = 0.5f;

	[HideInInspector]
	public Action<GameObject, int> m_CreateObjectCallback;

	private void DestroyButtons()
	{
		if (m_Buttons == null)
		{
			return;
		}
		foreach (BaseGadget button in m_Buttons)
		{
			if ((bool)button)
			{
				UnityEngine.Object.Destroy(button.gameObject);
			}
		}
	}

	public void CreateButtons()
	{
		if (m_Buttons == null)
		{
			m_Buttons = new List<BaseGadget>();
		}
		DestroyButtons();
		m_Object.gameObject.SetActive(value: false);
		m_Buttons = new List<BaseGadget>();
		float num = (m_AnchorX - 1f) * (GetWidth() - m_ButtonWidth);
		float num2 = (m_AnchorY - 1f) * (0f - (GetHeight() - m_ButtonHeight));
		for (int i = 0; i < m_ObjectCount; i++)
		{
			BaseGadget component = UnityEngine.Object.Instantiate(m_Object, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).GetComponent<BaseGadget>();
			component.gameObject.SetActive(value: true);
			if ((bool)component.GetComponent<BaseGadget>())
			{
				component.GetComponent<BaseGadget>().SetSize(m_ButtonWidth, m_ButtonHeight);
			}
			float x = (float)(i % m_ButtonsPerRow) * m_HorizontalSpacing + num;
			float y = (float)(i / m_ButtonsPerRow) * (0f - m_VerticalSpacing) + num2;
			component.transform.localPosition = new Vector3(x, y, 0f);
			m_CreateObjectCallback(component.gameObject, i);
			m_Buttons.Add(component);
		}
	}

	private void Awake()
	{
		CreateButtons();
	}

	private void OnDestroy()
	{
		DestroyButtons();
	}

	public float GetWidth()
	{
		return (float)(m_ButtonsPerRow - 1) * m_HorizontalSpacing + m_ButtonWidth;
	}

	public float GetHeight()
	{
		int num = 1;
		if (m_ObjectCount != 0)
		{
			num = m_ObjectCount / m_ButtonsPerRow;
			if (m_ObjectCount % m_ButtonsPerRow == 0)
			{
				num--;
			}
		}
		return (float)num * m_VerticalSpacing + m_ButtonHeight;
	}

	public void AutoSetButtonsPerRow(float Width)
	{
		m_ButtonsPerRow = (int)(Width / m_HorizontalSpacing);
	}

	public void SetAllInteractable(bool Interactable)
	{
		foreach (BaseGadget button in m_Buttons)
		{
			button.SetInteractable(Interactable);
		}
	}
}
