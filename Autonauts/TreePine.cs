using UnityEngine;

public class TreePine : MyTree
{
	private GameObject[] m_Leaves;

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_Leaves != null)
		{
			for (int i = 0; i < 3; i++)
			{
				if ((bool)m_Leaves[i])
				{
					m_Leaves[i].GetComponent<MeshRenderer>().enabled = true;
				}
			}
		}
		SetHighlight(Highlighted: false);
		base.StopUsing(AndDestroy);
	}

	protected override void UpdateModel()
	{
		base.UpdateModel();
		m_Leaves = new GameObject[3];
		if ((bool)m_ModelRoot.transform.Find("Leaves1"))
		{
			m_Leaves[0] = m_ModelRoot.transform.Find("Leaves1").gameObject;
		}
		if ((bool)m_ModelRoot.transform.Find("Leaves2"))
		{
			m_Leaves[1] = m_ModelRoot.transform.Find("Leaves2").gameObject;
		}
		if ((bool)m_ModelRoot.transform.Find("Leaves3"))
		{
			m_Leaves[2] = m_ModelRoot.transform.Find("Leaves3").gameObject;
		}
	}

	public override void UpdateChops(float Percent)
	{
		base.UpdateChops(Percent);
		if (Percent > 0.25f && (bool)m_Leaves[2])
		{
			m_Leaves[2].GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			m_Leaves[2].GetComponent<MeshRenderer>().enabled = true;
		}
		if (Percent > 0.5f && (bool)m_Leaves[1])
		{
			m_Leaves[1].GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			m_Leaves[1].GetComponent<MeshRenderer>().enabled = true;
		}
		if (Percent > 0.75f && (bool)m_Leaves[0])
		{
			m_Leaves[0].GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			m_Leaves[0].GetComponent<MeshRenderer>().enabled = true;
		}
	}

	protected override void CreateChoppedGoodies(TileCoord StartPosition)
	{
		base.CreateChoppedGoodies(StartPosition);
		if (Random.Range(0, 2) == 0)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Stick, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, StartPosition, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
		}
		int num = Random.Range(0, 4);
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile2 = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeSeed, randomEmptyTile2.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass2, StartPosition, randomEmptyTile2, 0f, baseClass2.transform.position.y, 4f);
			QuestManager.Instance.AddEvent(QuestEvent.Type.CreateTreeSeed, Bot: false, 0, this);
		}
		if (num > 0)
		{
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
	}

	protected override void CreateHammeredGoodies(TileCoord StartPosition)
	{
		base.CreateHammeredGoodies(StartPosition);
		if (Random.Range(0, 2) == 0)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Stick, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, StartPosition, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
		int num = Random.Range(0, 2);
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile2 = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeSeed, randomEmptyTile2.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass2, StartPosition, randomEmptyTile2, 0f, baseClass2.transform.position.y, 4f);
			QuestManager.Instance.AddEvent(QuestEvent.Type.CreateTreeSeed, Bot: false, 0, this);
		}
	}
}
