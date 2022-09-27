using UnityEngine;

public class BuildingReferenceArc
{
	public Building m_Building;

	public Worker m_Bot;

	public Arc m_Arc;

	private MeshRenderer m_ArcMaterial;

	public BuildingReferenceArc(Building NewBuilding, Worker NewBot, Arc NewArc)
	{
		m_Building = NewBuilding;
		m_Bot = NewBot;
		m_Arc = NewArc;
		m_ArcMaterial = m_Arc.GetComponent<MeshRenderer>();
	}

	private void UpdateArcShape()
	{
		m_Arc.transform.position = m_Building.m_TileCoord.ToWorldPositionTileCentered();
		Vector3 position = m_Bot.transform.position;
		float num = (m_Building.transform.position - position).magnitude / 8f;
		if (num < 2f)
		{
			num = 2f;
		}
		float width = 0.75f;
		if (m_Bot.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			width = 0.375f;
		}
		m_Arc.SetTarget(position, num, width);
	}

	private void UpdateArcMaterial()
	{
		m_Arc.m_Animate = false;
		Material material;
		if (m_Bot.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			material = MaterialManager.Instance.m_MaterialArcIdle;
		}
		else if (m_Bot.m_WorkerInterpreter.GetCurrentInstructionReference() == m_Building.GetComponent<BaseClass>())
		{
			material = MaterialManager.Instance.m_MaterialArcActive;
			m_Arc.m_Animate = true;
		}
		else
		{
			material = MaterialManager.Instance.m_MaterialArcInactive;
		}
		m_ArcMaterial.material = material;
	}

	public void Update()
	{
		UpdateArcShape();
		UpdateArcMaterial();
	}
}
