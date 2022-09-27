public class MouseAction : BaseGadget
{
	private BaseImage m_Image;

	private BaseImage m_Result;

	protected new void Awake()
	{
		base.Awake();
		CheckGadgets();
	}

	private void CheckGadgets()
	{
		if (!m_Image)
		{
			m_Image = base.transform.Find("MouseImage").GetComponent<BaseImage>();
			m_Result = base.transform.Find("Result").GetComponent<BaseImage>();
		}
	}

	public void SetRMB()
	{
		CheckGadgets();
		m_Image.SetSprite("MouseRMB");
	}

	public void SetAction(ActionInfo NewInfo, ActionType NewAction)
	{
		CheckGadgets();
		if (NewAction != ActionType.Fail)
		{
			m_Result.SetSprite("MouseTick");
		}
		else
		{
			m_Result.SetSprite("MouseCross");
		}
	}
}
