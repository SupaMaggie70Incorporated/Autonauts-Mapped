using System.Collections.Generic;
using cakeslice;
using TMPro;
using UnityEngine;

public class Cursor : MonoBehaviour
{
	private static bool m_AlwaysShowTileCursor;

	public static Cursor Instance;

	public TileCoord m_TileCoord;

	public Vector3 m_Scale;

	private Vector3 m_StartScale;

	private AreaIndicator m_AreaIndicator;

	[HideInInspector]
	public ObjectType m_ModelIdentifier;

	public List<CursorModel> m_Model;

	[HideInInspector]
	public bool m_ModelBad;

	[HideInInspector]
	public bool m_ModelOutOfBounds;

	private List<BaseClass> m_ModelBadException;

	private List<BaseClass> m_ExtraBadException;

	private Color m_ModelColour;

	public bool m_Usable;

	private bool m_UsableAnimate;

	private float m_UsableAnimateTimer;

	private float m_PinchTimer;

	private TextMeshPro m_CoordsText;

	private int m_Rotation;

	public List<Selectable> m_BadObjects;

	public bool m_Snapping;

	private int m_SnapRotation;

	private void Awake()
	{
		Instance = this;
		m_Rotation = 0;
		m_ModelBad = false;
		m_ModelOutOfBounds = false;
		m_ModelBadException = new List<BaseClass>();
		m_ExtraBadException = new List<BaseClass>();
		m_Model = new List<CursorModel>();
		m_ModelIdentifier = ObjectTypeList.m_Total;
		m_AreaIndicator = base.transform.Find("AreaIndicator").GetComponent<AreaIndicator>();
		m_StartScale.x = Tile.m_Size;
		m_StartScale.z = Tile.m_Size;
		m_Scale = m_StartScale;
		m_CoordsText = base.transform.Find("Coords").GetComponent<TextMeshPro>();
		m_CoordsText.gameObject.SetActive(value: false);
		m_Usable = false;
		m_PinchTimer = 0f;
		m_BadObjects = new List<Selectable>();
	}

	private void DestroyOldModels()
	{
		foreach (CursorModel item in m_Model)
		{
			if ((bool)item.m_Model.GetComponent<Building>())
			{
				item.m_Model.GetComponent<Building>().SetBlueprint(Blueprint: false);
				if (item.m_Model.m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					ModelManager.Instance.RestoreStandardMaterials(item.m_Model.GetComponent<ConverterFoundation>().m_NewBuilding);
				}
				else
				{
					ModelManager.Instance.RestoreStandardMaterials(item.m_Model);
				}
				MeshCollider[] componentsInChildren = item.m_Model.GetComponentsInChildren<MeshCollider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
			}
			item.m_Model.StopUsing();
		}
		m_Model.Clear();
	}

	public void SetModel(ObjectType ModelIdentifier, bool ForceFoundation = false)
	{
		m_Snapping = false;
		if (ModelIdentifier == m_ModelIdentifier)
		{
			return;
		}
		m_ModelIdentifier = ModelIdentifier;
		DestroyOldModels();
		ClearBadModels();
		m_ModelBad = false;
		if (ModelIdentifier != ObjectTypeList.m_Total)
		{
			BaseClass baseClass;
			if (ResourceManager.Instance.FoundationRequired(ModelIdentifier) || ForceFoundation)
			{
				baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ConverterFoundation, new Vector3(Tile.m_Size * 0.5f, 0f, (0f - Tile.m_Size) * 0.5f), Quaternion.identity);
				baseClass.GetComponent<ConverterFoundation>().SetNewBuilding(ModelIdentifier);
				baseClass.GetComponent<ConverterFoundation>().DestroyBlueprint();
			}
			else
			{
				baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ModelIdentifier, new Vector3(-1f, -1f), Quaternion.identity);
			}
			baseClass.transform.parent = base.transform;
			baseClass.transform.localPosition = default(Vector3);
			baseClass.GetComponent<Savable>().SetIsSavable(IsSavable: false);
			baseClass.GetComponent<TileCoordObject>().SetTilePosition(new TileCoord(0, 0));
			if ((bool)baseClass.GetComponent<Building>())
			{
				baseClass.GetComponent<Building>().SetBlueprint(Blueprint: true);
			}
			MeshCollider[] componentsInChildren = GetComponentsInChildren<MeshCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			baseClass.transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
			if (ResourceManager.Instance.GetResource(ModelIdentifier) == 0)
			{
				StandardShaderUtils.MakeObjectTransparent(baseClass.GetComponent<Building>());
			}
			m_ModelColour = new Color(0.75f, 0.75f, 1f, 0.65f);
			SetModelBad(Bad: false);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<TileCoordObject>().SetTilePosition(m_TileCoord);
			}
			SetSize(baseClass, m_TileCoord);
			CursorModel item = new CursorModel(baseClass, default(TileCoord), 0);
			m_Model.Add(item);
			baseClass.GetComponent<Building>().SetRotation(0);
			baseClass.GetComponent<Building>().UpdateTileCoord();
			m_Rotation = 0;
			PlotManager.Instance.RemoveObject(baseClass.GetComponent<TileCoordObject>());
			if (baseClass.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				PlotManager.Instance.RemoveObject(baseClass.GetComponent<ConverterFoundation>().m_NewBuilding);
			}
		}
		else
		{
			m_AreaIndicator.SetVisible(Visible: true);
		}
	}

	public void SetModels(List<BaseClass> Models, TileCoord Origin, bool New)
	{
		m_Snapping = false;
		m_ModelIdentifier = Models[0].m_TypeIdentifier;
		DestroyOldModels();
		ClearBadModels();
		m_ModelColour = new Color(0.75f, 0.75f, 1f, 0.65f);
		foreach (BaseClass Model in Models)
		{
			Building component = Model.GetComponent<Building>();
			ObjectType typeIdentifier = Model.m_TypeIdentifier;
			if (typeIdentifier == ObjectType.ConverterFoundation)
			{
				typeIdentifier = Model.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
			}
			BaseClass baseClass;
			if ((New && ResourceManager.Instance.FoundationRequired(typeIdentifier)) || (!New && Model.m_TypeIdentifier == ObjectType.ConverterFoundation))
			{
				baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ConverterFoundation, new Vector3(Tile.m_Size / 2f, 0f, (0f - Tile.m_Size) / 2f), Quaternion.identity);
				ConverterFoundation component2 = baseClass.GetComponent<ConverterFoundation>();
				component2.SetNewBuilding(typeIdentifier);
				if (Model.m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					ConverterFoundation component3 = Model.GetComponent<ConverterFoundation>();
					component2.m_TempIngredientsFlag = component3.m_Ingredients.Count > 0;
				}
			}
			else
			{
				baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(typeIdentifier, base.transform.localPosition, Quaternion.identity);
			}
			baseClass.transform.parent = base.transform;
			Vector3 vector = Origin.ToWorldPositionTileCentered();
			vector.y = 0f;
			Vector3 localPosition = Model.transform.localPosition - vector;
			if (Models.Count == 1)
			{
				localPosition.y = 0f;
			}
			baseClass.transform.localPosition = localPosition;
			if (typeIdentifier != ObjectType.TrainTrack && typeIdentifier != ObjectType.TrainTrackBridge)
			{
				baseClass.transform.localRotation = Model.transform.localRotation;
			}
			else
			{
				TrainTrackStraight trainTrackStraight = ((Model.m_TypeIdentifier != ObjectType.ConverterFoundation) ? Model.GetComponent<TrainTrackStraight>() : Model.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<TrainTrackStraight>());
				if (trainTrackStraight.m_Cross)
				{
					if (baseClass.m_TypeIdentifier == ObjectType.ConverterFoundation)
					{
						baseClass.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<TrainTrackStraight>().SetCross();
					}
					else
					{
						baseClass.GetComponent<TrainTrackStraight>().SetCross();
					}
				}
			}
			baseClass.GetComponent<Savable>().SetIsSavable(IsSavable: false);
			baseClass.GetComponent<TileCoordObject>().SetTilePosition(component.m_TileCoord);
			baseClass.GetComponent<TileCoordObject>().m_HiddenWithPlot = false;
			if ((bool)baseClass.GetComponent<Building>())
			{
				baseClass.GetComponent<Building>().SetBlueprint(Blueprint: true);
			}
			MeshCollider[] componentsInChildren = GetComponentsInChildren<MeshCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			MeshRenderer[] componentsInChildren2 = baseClass.GetComponentsInChildren<MeshRenderer>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].enabled = true;
			}
			baseClass.transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
			if (ResourceManager.Instance.GetResource(Model.m_TypeIdentifier) == 0)
			{
				StandardShaderUtils.MakeObjectTransparent(baseClass.GetComponent<Building>());
			}
			if ((bool)baseClass)
			{
				baseClass.GetComponent<TileCoordObject>().SetTilePosition(m_TileCoord);
			}
			TileCoord tileCoord = component.m_TileCoord - Origin;
			CursorModel cursorModel = new CursorModel(baseClass, tileCoord, component.m_Rotation);
			m_Model.Insert(m_Model.Count, cursorModel);
			foreach (CursorModel item in m_Model)
			{
				if (item.m_Position == tileCoord && item != cursorModel)
				{
					item.m_Model.GetComponent<Building>().m_TotalLevels += baseClass.GetComponent<Building>().m_NumLevels;
					break;
				}
			}
			if (!baseClass.GetComponent<Storage>())
			{
				continue;
			}
			ObjectType objectType = ObjectTypeList.m_Total;
			int amount = 0;
			if (Model.m_TypeIdentifier != ObjectType.ConverterFoundation)
			{
				objectType = Model.GetComponent<Storage>().m_ObjectType;
				if (Model.GetComponent<Storage>().m_ParentBuilding == null)
				{
					amount = Model.GetComponent<Storage>().GetStored();
				}
			}
			baseClass.GetComponent<Storage>().SetObjectType(objectType);
			baseClass.GetComponent<Storage>().AddToStored(objectType, amount, null);
		}
		m_Rotation = 0;
		if (Models.Count == 1)
		{
			m_Rotation = Models[0].GetComponent<Building>().m_Rotation;
		}
		SetRotation(m_Rotation);
	}

	public void SetModelColour(Color NewColour)
	{
		m_ModelColour = NewColour;
	}

	public void SetModelBad(bool Bad)
	{
		if (m_Model.Count == 0)
		{
			return;
		}
		foreach (CursorModel item in m_Model)
		{
			MeshRenderer[] componentsInChildren = item.m_Model.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] materials = componentsInChildren[i].materials;
				foreach (Material material in materials)
				{
					if (Bad)
					{
						material.color = new Color(1f, 0f, 0f, 0.5f);
					}
					else
					{
						material.color = m_ModelColour;
					}
					material.renderQueue = 3100;
				}
			}
		}
	}

	public void SetModelBadException(List<BaseClass> ModelBadException)
	{
		if (ModelBadException != null)
		{
			m_ModelBadException = ModelBadException;
		}
		else
		{
			m_ModelBadException.Clear();
		}
		m_ExtraBadException.Clear();
	}

	public void SetExtraBadException(List<BaseClass> ExtraBadException)
	{
		if (ExtraBadException != null)
		{
			m_ExtraBadException = ExtraBadException;
		}
		else
		{
			m_ExtraBadException.Clear();
		}
	}

	private void ClearBadModels()
	{
		foreach (Selectable badObject in m_BadObjects)
		{
			if ((bool)badObject)
			{
				badObject.SetHighlight(Highlighted: false);
			}
		}
		m_BadObjects.Clear();
	}

	public void CheckModelBadPosition()
	{
		m_ModelBad = false;
		ClearBadModels();
		foreach (CursorModel item in m_Model)
		{
			Selectable Object = null;
			Building component = item.m_Model.GetComponent<Building>();
			if (MapManager.Instance.CheckBuildingIntersection(component, m_ModelBadException, out Object, m_ExtraBadException))
			{
				m_ModelBad = true;
				if ((bool)Object)
				{
					m_BadObjects.Add(Object);
					foreach (Selectable badObject in m_BadObjects)
					{
						badObject.SetHighlight(Highlighted: true);
					}
				}
			}
			component.CheckWallsFloors();
		}
		SetModelBad(m_ModelBad);
	}

	public void SetVisible(bool Visible)
	{
		if (SaveLoadManager.m_Video && !Input.GetKey(KeyCode.Backslash))
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(Visible);
		if (Visible)
		{
			return;
		}
		foreach (CursorModel item in m_Model)
		{
			if ((bool)item.m_Model)
			{
				Building component = item.m_Model.GetComponent<Building>();
				if ((bool)component)
				{
					component.HideWallFloorIcon();
				}
			}
		}
	}

	public void NoTarget(bool Force = false)
	{
		ClearBadModels();
		if (!m_AlwaysShowTileCursor && (!m_CoordsText.gameObject.activeSelf || Force))
		{
			SetVisible(Visible: false);
		}
	}

	public void Pinch()
	{
		m_PinchTimer = 0.1f;
	}

	private void UpdateColour()
	{
		float num = 1f;
		if ((bool)PlotManager.Instance.GetPlotAtTile(m_TileCoord) && !PlotManager.Instance.GetPlotAtTile(m_TileCoord).m_Visible)
		{
			num = 0.5f;
		}
		Color color = ((!m_Usable || m_ModelBad) ? new Color(num, 0f, 0f, 1f) : new Color(num, num, num, 1f));
		m_AreaIndicator.SetColour(color);
		OutlineEffect component = CameraManager.Instance.m_Camera.GetComponent<OutlineEffect>();
		component.lineColor0 = color;
		component.lineIntensity = 1f;
	}

	public void SetUsable(bool Usable, bool Animate = false)
	{
		m_Usable = Usable;
		m_UsableAnimate = Animate;
		UpdateColour();
	}

	private void SetSize(BaseClass NewObject, TileCoord TilePosition)
	{
		Vector3 vector = TilePosition.ToWorldPositionTileCentered();
		if (TileHelpers.GetTileWater(TileManager.Instance.GetTileType(TilePosition)))
		{
			vector.y = 0f;
		}
		if ((bool)NewObject && (Bridge.GetIsTypeBridge(NewObject.m_TypeIdentifier) || NewObject.m_TypeIdentifier == ObjectType.TrainTrackBridge))
		{
			vector.y = 0f;
		}
		Vector3 vector2 = vector;
		float num = 1f;
		float num2 = 1f;
		Vector3 vector3 = default(Vector3);
		if ((bool)NewObject)
		{
			Building component = NewObject.GetComponent<Building>();
			if ((bool)component)
			{
				component.GetBoundingRectangle(out var TopLeft, out var BottomRight);
				num = BottomRight.x - TopLeft.x + 1;
				num2 = BottomRight.y - TopLeft.y + 1;
				vector2.x = (BottomRight.ToWorldPositionTileCentered().x + TopLeft.ToWorldPositionTileCentered().x) / 2f;
				vector2.z = (BottomRight.ToWorldPositionTileCentered().z + TopLeft.ToWorldPositionTileCentered().z) / 2f;
				Tile tile = TileManager.Instance.GetTile(TilePosition);
				ObjectType typeIdentifier = NewObject.m_TypeIdentifier;
				if (typeIdentifier == ObjectType.ConverterFoundation)
				{
					typeIdentifier = NewObject.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
				}
				if ((bool)tile.m_Building && !Floor.GetIsTypeFloor(typeIdentifier))
				{
					vector.y += tile.m_Building.GetBuildingHeightOffset();
					Building topBuilding = tile.m_Building.GetTopBuilding();
					if (m_ModelBadException.Contains(topBuilding))
					{
						vector.y -= topBuilding.m_LevelHeight;
					}
				}
			}
		}
		else
		{
			float num3 = 0.3f;
			float num4 = Tile.m_Size * num3 * 0.5f;
			float num5 = 0.7f;
			float tileHeight = TileManager.Instance.GetTileHeight(TilePosition);
			float num6 = (0f - (tileHeight - TileManager.Instance.GetTileHeight(TilePosition + new TileCoord(-1, 0)))) * num5;
			if (num6 > 0f)
			{
				num -= num3 * num6;
				vector3 += new Vector3(num4 * num6, 0f, 0f);
			}
			num6 = (0f - (tileHeight - TileManager.Instance.GetTileHeight(TilePosition + new TileCoord(1, 0)))) * num5;
			if (num6 > 0f)
			{
				num -= num3 * num6;
				vector3 -= new Vector3(num4 * num6, 0f, 0f);
			}
			num6 = (0f - (tileHeight - TileManager.Instance.GetTileHeight(TilePosition + new TileCoord(0, -1)))) * num5;
			if (num6 > 0f)
			{
				num2 -= num3 * num6;
				vector3 -= new Vector3(0f, 0f, num4 * num6);
			}
			num6 = (0f - (tileHeight - TileManager.Instance.GetTileHeight(TilePosition + new TileCoord(0, 1)))) * num5;
			if (num6 > 0f)
			{
				num2 -= num3 * num6;
				vector3 += new Vector3(0f, 0f, num4 * num6);
			}
		}
		m_Scale = new Vector3(m_StartScale.x * num, m_StartScale.y, num2 * m_StartScale.z);
		float num7 = 1f;
		if (m_PinchTimer > 0f)
		{
			num7 = 0.85f;
			m_PinchTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_PinchTimer <= 0f)
			{
				m_PinchTimer = 0f;
			}
		}
		Vector3 scale = m_Scale * num7;
		m_AreaIndicator.SetScale(scale);
		m_AreaIndicator.transform.localPosition = vector2 - vector + new Vector3(0f, 0.2f, 0f) + vector3;
		base.transform.position = vector;
	}

	public void TargetTile(TileCoord Position)
	{
		m_TileCoord = Position;
		ClearBadModels();
		SetVisible(Visible: true);
		if (m_Model.Count > 0 && (bool)m_Model[0].m_Model.GetComponent<Building>())
		{
			foreach (CursorModel item in m_Model)
			{
				item.m_Model.GetComponent<TileCoordObject>().SetTilePosition(item.m_Position + Position);
				item.m_Model.GetComponent<Building>().UpdateTiles();
				item.m_Model.GetComponent<Building>().UpdateAccessModelPosition();
			}
			CheckModelBadPosition();
			if (m_Model.Count == 1)
			{
				SetSize(m_Model[0].m_Model.GetComponent<BaseClass>(), m_Model[0].m_Model.GetComponent<TileCoordObject>().m_TileCoord);
			}
			else
			{
				Vector3 position = Position.ToWorldPositionTileCentered();
				base.transform.position = position;
			}
		}
		else
		{
			SetSize(null, Position);
		}
		UpdateColour();
	}

	public void Target(GameObject NewTarget)
	{
		ClearBadModels();
		if ((bool)NewTarget)
		{
			SetVisible(Visible: true);
			SetSize(NewTarget.GetComponent<BaseClass>(), NewTarget.GetComponent<TileCoordObject>().m_TileCoord);
		}
		else if (!m_AlwaysShowTileCursor && !m_CoordsText.gameObject.activeSelf)
		{
			SetVisible(Visible: false);
		}
	}

	public int GetRotation()
	{
		if (m_Snapping)
		{
			return m_SnapRotation;
		}
		return m_Rotation;
	}

	public int SetRotation(int Rotation)
	{
		ClearBadModels();
		int num = Rotation - m_Rotation;
		if (num < 0)
		{
			num += 4;
		}
		num = ((m_Rotation != Rotation) ? 1 : 0);
		m_Rotation = Rotation;
		foreach (CursorModel item in m_Model)
		{
			if (m_Model.Count > 1)
			{
				TileCoord position = item.m_Position;
				position.Rotate(num);
				item.m_Position = position;
				Vector3 localPosition = position.ToWorldPosition();
				localPosition.y = item.m_Model.transform.localPosition.y;
				item.m_Model.transform.localPosition = localPosition;
			}
			int num2 = ((m_Model.Count <= 1) ? Rotation : ((item.m_Rotation + Rotation) % 4));
			item.m_Model.GetComponent<Building>().m_Rotation = num2;
			item.m_Model.transform.localRotation = Quaternion.Euler(0f, (float)num2 * 90f, 0f);
			item.m_Model.GetComponent<Building>().UpdateAccessModelPosition();
			item.m_Model.GetComponent<Building>().UpdateTiles();
			item.m_Model.enabled = true;
		}
		return 0;
	}

	public void SnapTo(TileCoord NewPosition, int NewRotation)
	{
		m_Snapping = true;
		TargetTile(NewPosition);
		m_SnapRotation = NewRotation;
		CursorModel cursorModel = m_Model[0];
		cursorModel.m_Model.GetComponent<Building>().m_Rotation = NewRotation;
		cursorModel.m_Model.transform.localRotation = Quaternion.Euler(0f, (float)NewRotation * 90f, 0f);
		cursorModel.m_Model.GetComponent<Building>().UpdateAccessModelPosition();
		cursorModel.m_Model.GetComponent<Building>().UpdateTiles();
	}

	public void StopSnap()
	{
		m_Snapping = false;
		SetRotation(m_Rotation);
	}

	public void GetAreaRectangle(out int Left, out int Right, out int Top, out int Bottom)
	{
		Left = 10000;
		Right = -10000;
		Top = 10000;
		Bottom = -10000;
		foreach (CursorModel item in m_Model)
		{
			Building component = item.m_Model.GetComponent<Building>();
			foreach (TileCoord tile in component.m_Tiles)
			{
				TileCoord tileCoord = tile - component.m_TileCoord + item.m_Position;
				if (Left > tileCoord.x)
				{
					Left = tileCoord.x;
				}
				if (Right < tileCoord.x)
				{
					Right = tileCoord.x;
				}
				if (Top > tileCoord.y)
				{
					Top = tileCoord.y;
				}
				if (Bottom < tileCoord.y)
				{
					Bottom = tileCoord.y;
				}
			}
		}
	}

	private void UpdateUsableAnimate()
	{
		if (m_UsableAnimate)
		{
			m_UsableAnimateTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_UsableAnimateTimer * 60f) % 12 < 6)
			{
				m_AreaIndicator.SetScale(m_Scale * 0.8f);
			}
			else
			{
				m_AreaIndicator.SetScale(m_Scale);
			}
		}
	}

	public void SetMaterial(string MaterialName)
	{
		MeshRenderer component = m_AreaIndicator.GetComponent<MeshRenderer>();
		Material material2 = (component.material = (Material)Resources.Load("Materials/" + MaterialName, typeof(Material)));
		float a = component.material.color.a;
		Color indicatorColour = GeneralUtils.GetIndicatorColour();
		indicatorColour.a = a;
		component.material.color = indicatorColour;
	}

	public void EnableOutline(bool Enable)
	{
		m_AreaIndicator.EnableOutline(Enable);
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		if (SaveLoadManager.m_TestBuild && TimeManager.Instance.m_NormalDelta > 0f)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				SetVisible(Visible: true);
				m_CoordsText.gameObject.SetActive(value: true);
				string text = m_TileCoord.x + "," + m_TileCoord.y;
				m_CoordsText.text = text;
				float y = CameraManager.Instance.m_CameraRotation.eulerAngles.y;
				m_CoordsText.transform.localRotation = Quaternion.Euler(0f, y, 0f);
			}
			else
			{
				m_CoordsText.gameObject.SetActive(value: false);
			}
		}
		UpdateUsableAnimate();
	}
}
