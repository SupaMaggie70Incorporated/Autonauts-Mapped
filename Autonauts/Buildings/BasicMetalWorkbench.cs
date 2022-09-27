using System;
using UnityEngine;

public class BasicMetalWorkbench : LinkedSystemConverter
{
	private PlaySound m_PlaySound;

	private Transform m_BellowsTop;

	private Transform m_Bellows;

	private float m_BellowsTimer;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(2, 0));
		m_PulleySide = 2;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Bellows = m_ModelRoot.transform.Find("Bellows/BellowsCentre");
		m_BellowsTop = m_ModelRoot.transform.Find("Bellows/BellowsTop");
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
		DoConvertAnimActionParticles("Sparks");
		AudioManager.Instance.StartEvent("BuildingBasicMetalWorkbenchMaking", this);
	}

	private void UpdateBellows()
	{
		m_BellowsTimer += TimeManager.Instance.m_NormalDelta;
		float num = Mathf.Cos(m_BellowsTimer * (float)Math.PI * 8f) * 0.5f + 0.5f;
		float z = 0.4f + num * 1f;
		Vector3 localScale = m_Bellows.localScale;
		localScale.z = z;
		m_Bellows.localScale = localScale;
		float num2 = 1.6f;
		float num3 = 3.6f;
		float z2 = num2 + (num3 - num2) * num;
		Vector3 localPosition = m_BellowsTop.localPosition;
		localPosition.z = z2;
		m_BellowsTop.localPosition = localPosition;
		num2 = 1.117f;
		num3 = 2.12f;
		z2 = num2 + (num3 - num2) * num;
		localPosition = m_Bellows.localPosition;
		localPosition.z = z2;
		m_Bellows.localPosition = localPosition;
	}

	protected override void UpdateConverting()
	{
		UpdateSquashIngredients();
		UpdateConvertAnimTimer(0.25f);
		UpdateBellows();
	}

	protected override void EndConverting()
	{
		EndSquashIngredients();
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
	}
}
