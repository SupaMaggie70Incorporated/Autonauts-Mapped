using System;
using UnityEngine;

public class BenchSaw : Converter
{
	private GameObject m_Blade;

	private GameObject m_Handle;

	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, 0), new TileCoord(1, 0), new TileCoord(-2, 0));
		SetSpawnPoint(new TileCoord(2, 0));
		m_Blade = m_ModelRoot.transform.Find("Blade").gameObject;
		m_Handle = m_ModelRoot.transform.Find("Handle").gameObject;
	}

	protected override void UpdateIngredients()
	{
		if (m_Ingredients.Count > 0)
		{
			m_Ingredients[0].gameObject.SetActive(value: true);
			m_Ingredients[0].transform.SetParent(m_IngredientsRoot);
			m_Ingredients[0].transform.localPosition = new Vector3(0f, 0f, 0f);
			m_Ingredients[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingBenchSawMaking", this, Remember: true);
		m_Ingredients[0].transform.localRotation = Quaternion.identity;
		m_Particles = ParticlesManager.Instance.CreateParticles("Chips", base.transform.localPosition + new Vector3(-1.26f, -1.08f, 0.03f), base.transform.rotation * Quaternion.Euler(-60f, 90f, 0f));
	}

	protected override void UpdateConverting()
	{
		float num = m_StateTimer / m_ConversionDelay;
		m_Blade.transform.localPosition = new Vector3(0f, 2.13f + Mathf.Cos(num * (float)Math.PI * 20f) * 0.67f, 0f);
		m_Handle.transform.localRotation = Quaternion.Euler(0f, 0f, m_StateTimer * 360f * 5f) * ObjectUtils.m_ModelRotator;
		m_Ingredients[0].transform.localPosition = new Vector3(num * 6f, 0f, 0f);
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		m_Blade.transform.localPosition = new Vector3(0f, 2.36f, 0f);
		m_Handle.transform.localRotation = Quaternion.Euler(0f, 0f, -45f) * ObjectUtils.m_ModelRotator;
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
		QuestManager.Instance.AddEvent(QuestEvent.Type.ProcessWood, m_LastEngagerType == ObjectType.Worker, 0, this);
	}
}
