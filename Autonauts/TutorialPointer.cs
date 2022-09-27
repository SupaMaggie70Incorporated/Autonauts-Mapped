using UnityEngine;

public class TutorialPointer : BasePanel
{
	public enum TargetType
	{
		World,
		Tile,
		Screen,
		Total
	}

	private TargetType m_TargetType;

	public BaseClass m_WorldTarget;

	private TileCoord m_TileTarget;

	private GameObject m_ScreenTarget;

	private BaseClass m_ObjectLocation;

	private Vector2 m_ScreenLocation;

	private Vector2 m_Offset;

	private float m_Timer;

	protected new void Awake()
	{
		base.Awake();
		ClearTarget();
	}

	public void SetWorldTarget(BaseClass NewTarget)
	{
		if (m_WorldTarget != NewTarget || m_TargetType != 0)
		{
			m_TargetType = TargetType.World;
			m_WorldTarget = NewTarget;
			m_ScreenTarget = null;
			base.gameObject.SetActive(value: true);
			base.transform.SetParent(HudManager.Instance.m_WorldRolloversRootTransform);
		}
	}

	public void SetTileTarget(TileCoord NewTarget)
	{
		if (m_TileTarget != NewTarget || m_TargetType != TargetType.Tile)
		{
			m_TargetType = TargetType.Tile;
			m_TileTarget = NewTarget;
			m_WorldTarget = null;
			m_ScreenTarget = null;
			base.gameObject.SetActive(value: true);
			base.transform.SetParent(HudManager.Instance.m_WorldRolloversRootTransform);
		}
	}

	public void SetScreenTarget(GameObject NewTarget)
	{
		if (m_ScreenTarget != NewTarget || m_TargetType != TargetType.Screen)
		{
			m_TargetType = TargetType.Screen;
			m_ScreenTarget = NewTarget;
			m_WorldTarget = null;
			base.gameObject.SetActive(value: true);
			base.transform.SetParent(HudManager.Instance.m_RolloversRootTransform);
		}
	}

	public void ClearTarget()
	{
		m_TargetType = TargetType.Total;
		base.gameObject.SetActive(value: false);
	}

	public void SetRotation(float Rotation)
	{
		base.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, Rotation);
	}

	public void SetAnchorAndPivot(Vector2 Anchor, Vector2 Pivot)
	{
		GetComponent<RectTransform>().anchorMin = Anchor;
		GetComponent<RectTransform>().anchorMax = Anchor;
		GetComponent<RectTransform>().pivot = Pivot;
	}

	private Vector3 WorldToScreen(Vector3 WorldPosition)
	{
		Vector3 vector = CameraManager.Instance.m_Camera.WorldToScreenPoint(WorldPosition);
		if ((bool)HudManager.Instance)
		{
			vector = HudManager.Instance.ScreenToCanvas(vector);
		}
		return vector;
	}

	private Vector3 GetAboveObjectLocation(GameObject NewObject, Vector3 Offset)
	{
		Bounds bounds = ObjectUtils.ObjectBounds(NewObject);
		Vector3 center = bounds.center;
		center.y = bounds.max.y;
		return center + Offset;
	}

	private Vector3 GetTileLocation(Vector3 Offset)
	{
		return m_TileTarget.ToWorldPositionTileCentered() + Offset;
	}

	private Vector3 GetScreenLocation(GameObject NewTarget, Vector3 Offset)
	{
		Vector3 vector = (new Vector2(0.5f, 0.5f) - NewTarget.GetComponent<RectTransform>().pivot) * NewTarget.GetComponent<RectTransform>().sizeDelta;
		Vector3 position = NewTarget.transform.position + vector;
		return base.transform.parent.InverseTransformPoint(position) + Offset;
	}

	private bool GetIsAnimationDown()
	{
		if ((int)(m_Timer * 60f) % 20 < 10)
		{
			return true;
		}
		return false;
	}

	private float GetWorldAnimationOffset()
	{
		if (GetIsAnimationDown())
		{
			return 0f;
		}
		return 2f;
	}

	private float GetScreenAnimationOffset()
	{
		if (GetIsAnimationDown())
		{
			return 0f;
		}
		return 20f;
	}

	private Vector3 GetTargetLocation(Vector3 Offset, out Vector3 WorldPosition)
	{
		WorldPosition = default(Vector3);
		Vector3 result = default(Vector3);
		switch (m_TargetType)
		{
		case TargetType.World:
		{
			if (!m_WorldTarget)
			{
				break;
			}
			GameObject newObject = m_WorldTarget.gameObject;
			if (Converter.GetIsTypeConverter(m_WorldTarget.m_TypeIdentifier))
			{
				Transform ingredientsRoot = m_WorldTarget.GetComponent<Converter>().m_IngredientsRoot;
				if ((bool)ingredientsRoot)
				{
					newObject = ingredientsRoot.gameObject;
				}
			}
			WorldPosition = GetAboveObjectLocation(newObject, Offset);
			WorldPosition.y += GetWorldAnimationOffset();
			result = WorldToScreen(WorldPosition);
			break;
		}
		case TargetType.Tile:
			WorldPosition = GetTileLocation(Offset);
			WorldPosition.y += GetWorldAnimationOffset();
			result = WorldToScreen(WorldPosition);
			break;
		case TargetType.Screen:
			if ((bool)m_ScreenTarget)
			{
				result = GetScreenLocation(m_ScreenTarget, Offset);
				result.y += GetScreenAnimationOffset();
			}
			break;
		}
		return result;
	}

	public void UpdatePositionAndScale()
	{
		Vector3 WorldPosition;
		Vector3 targetLocation = GetTargetLocation(m_Offset, out WorldPosition);
		base.transform.localPosition = targetLocation;
		float num = 0.5f;
		if (m_TargetType == TargetType.World || m_TargetType == TargetType.Tile)
		{
			num = 1f / (CameraManager.Instance.m_Camera.transform.position - WorldPosition).magnitude * 20f;
		}
		base.transform.localScale = new Vector3(num, num, num);
	}

	public void SetOffset(Vector2 Offset)
	{
		m_Offset = Offset;
	}

	private void Update()
	{
		UpdatePositionAndScale();
		if (TimeManager.Instance.m_PauseTimeEnabled)
		{
			m_Timer += TimeManager.Instance.m_PauseDelta;
		}
		else
		{
			m_Timer += TimeManager.Instance.m_NormalDelta;
		}
	}
}
