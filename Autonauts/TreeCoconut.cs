using UnityEngine;

public class TreeCoconut : MyTree
{
	private GameObject m_Coconuts;

	private bool m_CoconutsActive;

	public override string GetHumanReadableName()
	{
		return base.GetHumanReadableName();
	}

	protected override void GrowingFinished()
	{
		SetState(State.GrowingFruit);
	}

	protected override void UpdateModel()
	{
		base.UpdateModel();
		Transform transform = m_ModelRoot.transform.Find("Coconuts");
		if ((bool)transform)
		{
			transform.gameObject.SetActive(value: false);
			if (m_State == State.Waiting)
			{
				transform.gameObject.SetActive(value: true);
			}
			m_CoconutsActive = transform.gameObject.activeSelf;
		}
	}

	protected override void CreateChoppedGoodies(TileCoord StartPosition)
	{
		base.CreateChoppedGoodies(StartPosition);
		if (m_CoconutsActive)
		{
			int num = Random.Range(1, 3);
			for (int i = 0; i < num; i++)
			{
				TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Coconut, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
				SpawnAnimationManager.Instance.AddJump(baseClass, StartPosition, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
			}
			if (num > 0)
			{
				AudioManager.Instance.StartEvent("ObjectCreated", this);
			}
		}
	}

	protected override void CreateHammeredGoodies(TileCoord StartPosition)
	{
		base.CreateHammeredGoodies(StartPosition);
		int num = Random.Range(1, 3);
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Coconut, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, StartPosition, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
		}
		if (num > 0)
		{
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
		SetState(State.GrowingFruit);
	}
}
