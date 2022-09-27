using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EraRequirement : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Quest m_Quest;

	private Image m_Image;

	private Text m_Text;

	public void SetQuest(Quest NewQuest)
	{
		m_Quest = NewQuest;
		m_Image = base.transform.Find("Image").GetComponent<Image>();
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + m_Quest.m_IconName, typeof(Sprite));
		m_Image.sprite = sprite;
		m_Text = base.transform.Find("Text").GetComponent<Text>();
		m_Text.text = TextManager.Instance.Get(m_Quest.m_Title);
	}

	private void UpdateValues()
	{
		if (m_Quest != null && m_Quest.GetIsComplete())
		{
			m_Image.color = new Color(1f, 1f, 1f, 0.5f);
			m_Text.color = new Color(0f, 0f, 0f, 0.5f);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		HudManager.Instance.ActivateQuestRollover(Activate: true, m_Quest);
		float num = 1.1f;
		base.transform.localScale = new Vector3(num, num, num);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
		float num = 1f;
		base.transform.localScale = new Vector3(num, num, num);
	}

	private void OnDisable()
	{
		float num = 1f;
		base.transform.localScale = new Vector3(num, num, num);
	}

	private void Update()
	{
		UpdateValues();
	}
}
