using UnityEngine;

public class LoomCrude : Converter
{
	private PlaySound m_PlaySound;

	private GameObject m_Shuttle;

	private GameObject m_Top;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_Shuttle = m_ModelRoot.transform.Find("Shuttle").gameObject;
		m_Top = m_ModelRoot.transform.Find("Top").gameObject;
		m_Top.GetComponent<MeshCollider>().enabled = false;
		SetTopScale(0f);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingLoomMaking", this, Remember: true);
	}

	private void UpdateShuttleAndTop()
	{
		int num = 20;
		int num2 = num / 2;
		int num3 = (int)(m_StateTimer * 60f) % num;
		float num4 = ((num3 >= num2) ? (1f - (float)(num3 - num2) / (float)num2) : ((float)num3 / (float)num2));
		float num5 = 0.2f;
		float num6 = 3.8f;
		float x = num5 + num4 * (num6 - num5);
		int num7 = (int)(m_StateTimer * 60f) / num;
		int num8 = (int)(m_ConversionDelay * 60f) / num;
		num4 = (float)num7 / (float)num8;
		float num9 = 2.46f;
		float num10 = 0.72f;
		float y = num10 + num4 * (num9 - num10);
		m_Shuttle.transform.localPosition = new Vector3(x, y, -0.64f);
		SetTopScale(num4);
		StartIngredientsDown();
	}

	public override void DoConvertAnimAction()
	{
		base.DoConvertAnimAction();
		UpdateIngredients();
		MoveIngredientsDown();
	}

	protected override void UpdateConverting()
	{
		UpdateShuttleAndTop();
		UpdateConvertAnimTimer(0.15f);
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		SetTopScale(0f);
	}

	private void SetTopScale(float Scale)
	{
		if (Scale == 0f)
		{
			Scale = 0.001f;
		}
		m_Top.transform.localScale = new Vector3(1f, 1f, Scale);
	}
}
