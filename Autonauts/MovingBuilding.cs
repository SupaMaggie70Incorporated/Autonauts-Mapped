using System.Collections.Generic;

public class MovingBuilding
{
	public int m_Index;

	public Building m_Building;

	public TileCoord m_NewPosition;

	public int m_NewRotation;

	public List<TileCoordObject> m_OutputObjects;

	public int m_SortValue;

	public MovingBuilding(int Index, Building NewBuilding, TileCoord NewPosition, int NewRotation)
	{
		m_Index = Index;
		m_Building = NewBuilding;
		m_NewPosition = NewPosition;
		m_NewRotation = NewRotation;
		m_OutputObjects = new List<TileCoordObject>();
	}

	public static void GetOutputObjects(List<TileCoordObject> OutputObjects, Building NewBuilding)
	{
		OutputObjects.Clear();
		if (!Converter.GetIsTypeConverter(NewBuilding.m_TypeIdentifier))
		{
			return;
		}
		Converter component = NewBuilding.GetComponent<Converter>();
		if (!component.m_SpawnModel.activeSelf)
		{
			return;
		}
		ObjectType resultType = component.GetResultType();
		if (resultType == ObjectTypeList.m_Total)
		{
			return;
		}
		foreach (IngredientRequirement item in component.m_Results[component.m_ResultsToCreate])
		{
			resultType = item.m_Type;
			TileCoord spawnPoint = component.GetSpawnPoint();
			foreach (TileCoordObject item2 in PlotManager.Instance.GetObjectsAtTile(spawnPoint, resultType))
			{
				if (item2.GetIsSavable() && !BaggedManager.Instance.IsObjectBagged(item2))
				{
					OutputObjects.Add(item2);
				}
			}
		}
	}

	public void GetOutputObjects()
	{
		GetOutputObjects(m_OutputObjects, m_Building);
	}

	public void RemoveOutputObjects()
	{
		foreach (TileCoordObject outputObject in m_OutputObjects)
		{
			PlotManager.Instance.RemoveObject(outputObject);
		}
	}

	public void MoveOutputObjects()
	{
		if (!Converter.GetIsTypeConverter(m_Building.m_TypeIdentifier))
		{
			return;
		}
		Converter component = m_Building.GetComponent<Converter>();
		if (!component.m_SpawnModel.activeSelf)
		{
			return;
		}
		TileCoord spawnPoint = component.GetSpawnPoint();
		foreach (TileCoordObject outputObject in m_OutputObjects)
		{
			outputObject.UpdatePositionToTilePosition(spawnPoint);
		}
		PlotManager.Instance.GetPlotAtTile(spawnPoint).StackObjectsAtTile(spawnPoint);
	}
}
