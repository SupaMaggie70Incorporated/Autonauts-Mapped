using System.Collections.Generic;
using UnityEngine;

public class FolkRollover : Rollover
{
	private static float m_BarSpacing = 35f;

	protected Folk m_Target;

	protected BaseText m_Title;

	private BaseText m_Level;

	private StandardProgressBar m_HeartProgress;

	private LevelHeart m_LevelHeart;

	private FolkProgressBar m_DefaultProgress;

	private List<FolkProgressBar> m_ProgressBars;

	private Vector2 m_BarPosition;

	private float m_Timer;

	private bool m_ProgressDisabled;

	private bool m_PreviousTier;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		Hide();
		HideAll();
	}

	protected override void CheckGadgets()
	{
		base.CheckGadgets();
		if (m_Title == null)
		{
			m_Title = base.transform.Find("BasePanel").Find("Title").GetComponent<BaseText>();
			m_Level = base.transform.Find("BasePanel").Find("Level").GetComponent<BaseText>();
			GetHeartProgressBar();
			CreateRequirementProgress();
		}
	}

	private void HideAll()
	{
		m_Level.SetActive(Active: false);
		foreach (FolkProgressBar progressBar in m_ProgressBars)
		{
			progressBar.SetActive(Active: false);
		}
		m_HeartProgress.SetActive(Active: false);
	}

	private void CreateRequirementProgress()
	{
		m_DefaultProgress = m_Panel.transform.Find("FolkProgressBar").GetComponent<FolkProgressBar>();
		m_DefaultProgress.SetActive(Active: false);
		m_BarPosition = m_DefaultProgress.GetComponent<RectTransform>().anchoredPosition;
		m_ProgressBars = new List<FolkProgressBar>();
		for (int i = 0; i < 7; i++)
		{
			FolkProgressBar item = CreateProgressBar((FolkProgressBar.Type)i);
			m_ProgressBars.Add(item);
		}
	}

	private FolkProgressBar CreateProgressBar(FolkProgressBar.Type NewType)
	{
		FolkProgressBar component = Object.Instantiate(m_DefaultProgress, m_Panel.transform).GetComponent<FolkProgressBar>();
		component.GetComponent<RectTransform>().anchoredPosition = m_BarPosition;
		component.SetType(NewType);
		m_BarPosition.y -= m_BarSpacing;
		return component;
	}

	private void GetHeartProgressBar()
	{
		m_HeartProgress = m_Panel.transform.Find("HeartProgressBar").GetComponent<StandardProgressBar>();
		m_LevelHeart = m_HeartProgress.transform.Find("LevelHeart").GetComponent<LevelHeart>();
	}

	private bool GetAnyRequirementsLow()
	{
		if (!m_ProgressDisabled)
		{
			foreach (FolkProgressBar progressBar in m_ProgressBars)
			{
				if (progressBar.GetRequirementLow())
				{
					return true;
				}
			}
		}
		return false;
	}

	private int GetTargetTier()
	{
		int num = m_Target.GetTier();
		if (m_PreviousTier)
		{
			num--;
		}
		return num;
	}

	private void UpdateHeartProgressBar()
	{
		float value = m_Target.m_HappyinessTimer / VariableManager.Instance.GetVariableAsFloat(ObjectType.Folk, "HappinessDelay");
		m_HeartProgress.SetValue(value);
		_ = (Color)new Color32(122, 2, 0, byte.MaxValue);
		if (m_Target.GetIsHappy())
		{
			_ = (Color)new Color32(byte.MaxValue, 21, 0, byte.MaxValue);
		}
		int value2 = m_Target.GetHeartTier();
		if (m_ProgressDisabled)
		{
			value2 = GetTargetTier();
		}
		m_LevelHeart.SetValue(value2);
		if (GetAnyRequirementsLow())
		{
			if ((int)(m_Timer * 60f) % 16 < 8)
			{
				m_LevelHeart.SetColour(new Color(1f, 0f, 0f, 1f));
			}
			else
			{
				m_LevelHeart.SetColour(new Color(1f, 1f, 1f, 1f));
			}
		}
		else
		{
			m_LevelHeart.SetColour(new Color(1f, 1f, 1f, 1f));
		}
		if (m_ProgressDisabled)
		{
			m_HeartProgress.HideSlider();
		}
	}

	private FolkProgressBar.Type GetLowestProgressType()
	{
		FolkProgressBar.Type result = FolkProgressBar.Type.Total;
		int num = 100;
		foreach (FolkProgressBar progressBar in m_ProgressBars)
		{
			int tier = progressBar.GetTier();
			if (tier < num || progressBar.GetProgress() == 0f)
			{
				num = tier;
				result = progressBar.m_Type;
			}
		}
		return result;
	}

	protected override void UpdateTarget()
	{
		if (m_Target == null)
		{
			return;
		}
		m_Timer += TimeManager.Instance.m_NormalDelta;
		if (!m_ProgressDisabled)
		{
			FolkProgressBar.Type lowestProgressType = GetLowestProgressType();
			foreach (FolkProgressBar progressBar in m_ProgressBars)
			{
				progressBar.UpdateBar(m_Timer);
				progressBar.SetLowest(progressBar.m_Type == lowestProgressType);
			}
		}
		UpdateHeartProgressBar();
	}

	public virtual void SetTarget(Folk Target, bool DisableBars = false, bool PreviousTier = false)
	{
		if (!(Target != m_Target))
		{
			return;
		}
		m_Target = Target;
		if ((bool)Target)
		{
			CheckGadgets();
			m_ProgressDisabled = DisableBars;
			m_PreviousTier = PreviousTier;
			m_Title.SetText(Target.GetHumanReadableName());
			string text = TextManager.Instance.Get("FolkRolloverTier", (GetTargetTier() + 1).ToString());
			m_Level.SetText(text);
			m_Level.SetActive(Active: true);
			int num = 0;
			FolkProgressBar folkProgressBar = null;
			foreach (FolkProgressBar progressBar in m_ProgressBars)
			{
				progressBar.SetTarget(m_Target, m_ProgressDisabled, m_PreviousTier);
				if (progressBar.GetActive())
				{
					num++;
					folkProgressBar = progressBar;
				}
			}
			if (m_PreviousTier)
			{
				folkProgressBar.gameObject.SetActive(value: false);
			}
			float num2 = (float)num * m_BarSpacing + 90f;
			if (num2 < 200f)
			{
				num2 = 200f;
			}
			m_Panel.SetHeight(num2);
			UpdateTarget();
			m_HeartProgress.SetActive(Active: true);
		}
		else
		{
			HideAll();
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
