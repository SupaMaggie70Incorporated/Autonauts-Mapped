using UnityEngine;

public class BuildingMod : Building
{
	private Vector2Int TopLeft = new Vector2Int(-1, -1);

	private Vector2Int BottomRight = new Vector2Int(1, 1);

	private Vector2Int AccessPoint = new Vector2Int(0, 0);

	public override void Restart()
	{
		base.Restart();
		Vector3 Scale = new Vector3(-1f, 1f, 1f);
		ModManager.Instance.ModBuildingClass.GetModelScale(m_TypeIdentifier, out Scale);
		m_ModelRoot.transform.localScale = new Vector3(Scale.x, Scale.y, Scale.z);
		if (ModManager.Instance.ModBuildingClass.GetModelRotation(m_TypeIdentifier, out var Rot))
		{
			m_ModelRoot.transform.localRotation = Quaternion.Euler(Rot.x, Rot.y, Rot.z);
		}
		if (ModManager.Instance.ModBuildingClass.GetModelTranslation(m_TypeIdentifier, out var Trans))
		{
			m_ModelRoot.transform.localPosition = new Vector3(Trans.x, Trans.y, Trans.z);
		}
		ModManager.Instance.ModBuildingClass.ModCoordsTL.TryGetValue(m_TypeIdentifier, out TopLeft);
		ModManager.Instance.ModBuildingClass.ModCoordsBR.TryGetValue(m_TypeIdentifier, out BottomRight);
		ModManager.Instance.ModBuildingClass.ModCoordsAccess.TryGetValue(m_TypeIdentifier, out AccessPoint);
		SetDimensions(new TileCoord(TopLeft.x, TopLeft.y), new TileCoord(BottomRight.x, BottomRight.y), new TileCoord(AccessPoint.x, AccessPoint.y));
		if (ModManager.Instance.ModBuildingClass.ModAccessNotHidden.ContainsKey(m_TypeIdentifier))
		{
			HideAccessModel(Hide: false);
		}
		else
		{
			HideAccessModel();
		}
	}
}
