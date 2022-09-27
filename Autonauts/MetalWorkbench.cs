using UnityEngine;

public class MetalWorkbench : LinkedSystemConverter
{
	private PlaySound m_PlaySound;

	private GameObject m_Hammer;

	private Vector3 m_HammerStartPosition;

	private GameObject m_HotMetal;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_PulleySide = 2;
		m_DisplayIngredients = false;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Hammer = m_ModelRoot.transform.Find("Hammer").gameObject;
		m_HammerStartPosition = m_Hammer.transform.localPosition;
		m_HotMetal = m_ModelRoot.transform.Find("HotMetal").gameObject;
		m_HotMetal.gameObject.SetActive(value: false);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		SquashIngredients();
	}

	public override void DoConvertAnimAction()
	{
		base.DoConvertAnimAction();
		UpdateIngredients();
		SquashIngredients();
		DoConvertAnimActionParticles("BigSparks").transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		AudioManager.Instance.StartEvent("BuildingMetalWorkbenchMaking", this);
	}

	protected override void UpdateIngredients()
	{
		base.UpdateIngredients();
		m_HotMetal.gameObject.SetActive(m_Ingredients.Count > 0);
	}

	protected override void UpdateConverting()
	{
		UpdateSquashIngredients();
		float animConvertTimer = m_AnimConvertTimer;
		m_AnimConvertTimer += TimeManager.Instance.m_NormalDelta;
		float num = 0.25f;
		float num2 = 0.5f;
		if (m_AnimConvertTimer >= num && animConvertTimer < num)
		{
			DoConvertAnimAction();
		}
		if (m_AnimConvertTimer >= num)
		{
			float num3 = 1f - (m_AnimConvertTimer - num) / (num2 - num);
			Vector3 localPosition = m_Hammer.transform.localPosition;
			localPosition.y = m_HammerStartPosition.y + num3 * (m_IngredientsRoot.position.y - m_HammerStartPosition.y);
			m_Hammer.transform.localPosition = localPosition;
		}
		if (m_AnimConvertTimer > num2)
		{
			m_AnimConvertTimer -= num2;
			m_Hammer.transform.localPosition = m_HammerStartPosition;
		}
	}

	protected override void EndConverting()
	{
		EndSquashIngredients();
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_Hammer.transform.localPosition = m_HammerStartPosition;
	}
}
