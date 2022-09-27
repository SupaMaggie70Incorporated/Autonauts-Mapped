using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TileCoordObject : Savable
{
	public TileCoord m_TileCoord;

	[HideInInspector]
	public Plot m_Plot;

	[HideInInspector]
	public bool m_HiddenWithPlot;

	[HideInInspector]
	public Actionable m_BaggedBy;

	[HideInInspector]
	public List<PlaySound> m_AmbientSounds;

	protected bool m_FloatsInWater;

	[HideInInspector]
	public bool m_Sleeping;

	[HideInInspector]
	public bool m_SleepRequested;

	[HideInInspector]
	public Vector3 m_ModPosition;

	[HideInInspector]
	public Vector3 m_ModMoveToPosition;

	[HideInInspector]
	public float m_ModMoveDistance;

	[HideInInspector]
	public float m_ModMoveTimer;

	[HideInInspector]
	public Vector3 m_ModMoveDelta;

	[HideInInspector]
	public float m_ModOldDiff;

	[HideInInspector]
	public float m_ModSpeed;

	[HideInInspector]
	public static float m_ModMoveToFinishDistance = 0.25f;

	[HideInInspector]
	public static float m_ModWobbleHeight = 0.5f;

	[HideInInspector]
	public float m_ModHeight;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("TileCoordObject", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_HiddenWithPlot = true;
		m_BaggedBy = null;
		m_AmbientSounds = null;
		m_TileCoord = new TileCoord(0, 0);
		UpdateTileCoord();
		m_FloatsInWater = false;
		m_Sleeping = false;
		m_SleepRequested = false;
		Sleep();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (ObjectTypeList.Instance.GetIsSleepy(m_TypeIdentifier) && (bool)m_Plot)
		{
			m_Plot.UpdateObjectMerger(this, Immediate: false);
		}
		PlotManager.Instance.RemoveObject(this);
		base.StopUsing(AndDestroy);
	}

	protected void OnDestroy()
	{
		if ((bool)PlotManager.Instance)
		{
			PlotManager.Instance.RemoveObject(this);
		}
	}

	public override void PostLoad()
	{
		base.PostLoad();
		if (ObjectTypeList.Instance.GetStackableFromIdentifier(m_TypeIdentifier))
		{
			PlotManager.Instance.GetPlotAtTile(m_TileCoord).StackObjectsAtTile(m_TileCoord);
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Hide:
			base.gameObject.SetActive(value: false);
			break;
		case ActionType.Show:
			base.gameObject.SetActive(value: true);
			break;
		case ActionType.SetBagged:
			if (m_BaggedBy == null)
			{
				m_BaggedBy = Info.m_Object;
			}
			break;
		case ActionType.SetUnbagged:
			if ((bool)m_BaggedBy)
			{
				m_BaggedBy = null;
			}
			break;
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if ((uint)(action - 38) <= 1u)
		{
			return true;
		}
		return false;
	}

	protected virtual void TileCoordChanged(TileCoord Position)
	{
		bool flag = false;
		if (m_Sleeping || m_SleepRequested)
		{
			flag = true;
		}
		PlotManager.Instance.UpdateObject(this, Position);
		if (flag)
		{
			Sleep();
		}
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		m_TileCoord.Save(Node, "T");
	}

	public override void Load(JSONNode Node)
	{
		TileCoord position = default(TileCoord);
		position.Load(Node, "T");
		base.transform.position = position.ToWorldPositionTileCentered(m_FloatsInWater);
		TileCoordChanged(position);
		base.Load(Node);
	}

	public void UpdateTileCoord()
	{
		TileCoord position = default(TileCoord);
		position.FromWorldPosition(base.transform.position);
		TileCoordChanged(position);
	}

	public void SetPosition(Vector3 Position, bool UpdateCoord = true)
	{
		base.transform.position = Position;
		if (UpdateCoord)
		{
			UpdateTileCoord();
		}
	}

	public void SetPosition(float x, float y, float z, bool UpdateCoord = true)
	{
		SetPosition(new Vector3(x, y, z), UpdateCoord);
	}

	public void SetTilePosition(TileCoord Position)
	{
		TileCoordChanged(Position);
	}

	public void UpdatePositionToTilePosition(TileCoord Position)
	{
		TileCoordChanged(Position);
		base.transform.position = m_TileCoord.ToWorldPositionTileCentered(m_FloatsInWater);
	}

	public virtual List<TileCoord> GetAdjacentTiles()
	{
		List<TileCoord> list = new List<TileCoord>();
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				TileCoord tileCoord = new TileCoord(j + m_TileCoord.x, i + m_TileCoord.y);
				if (tileCoord.x >= 0 && tileCoord.y >= 0 && tileCoord.x < TileManager.Instance.m_TilesWide && tileCoord.y < TileManager.Instance.m_TilesHigh && !list.Contains(tileCoord) && !TileManager.Instance.GetTileSolidToPlayer(tileCoord))
				{
					list.Add(tileCoord);
				}
			}
		}
		return list;
	}

	public virtual TileCoord GetNearestAdjacentTile(TileCoord Position)
	{
		TileCoord result = default(TileCoord);
		result.x = m_TileCoord.x;
		result.y = m_TileCoord.y;
		List<TileCoord> adjacentTiles = GetAdjacentTiles();
		float num = 1E+07f;
		foreach (TileCoord item in adjacentTiles)
		{
			float num2 = (Position - item).Magnitude();
			if (num2 < num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	public virtual void UpdatePlotVisibility()
	{
		if (!m_Sleeping)
		{
			bool flag = true;
			if (m_HiddenWithPlot && (bool)m_Plot)
			{
				flag = m_Plot.m_Visible;
			}
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = flag;
			}
		}
	}

	public void Sleep()
	{
		if (ObjectTypeList.Instance.GetIsSleepy(m_TypeIdentifier) && (bool)m_Plot && !m_SleepRequested)
		{
			m_SleepRequested = true;
			m_Plot.UpdateObjectMerger(this, Immediate: false);
			base.enabled = false;
		}
	}

	public void Wake()
	{
		if (ObjectTypeList.Instance.GetIsSleepy(m_TypeIdentifier) && (bool)m_Plot && m_SleepRequested)
		{
			m_SleepRequested = false;
			m_Plot.UpdateObjectMerger(this, Immediate: true);
			base.enabled = true;
		}
	}

	public virtual void MaterialsChanged()
	{
	}

	public static void SpawnObjects(TileCoord BaseCoord, ObjectType NewType, int NewMinCount, int NewMaxCount, bool AroundTile)
	{
		int num = Random.Range(NewMinCount, NewMaxCount + 1);
		for (int i = 0; i < num; i++)
		{
			TileCoord tileCoord = BaseCoord;
			if (AroundTile)
			{
				tileCoord = TileHelpers.GetRandomEmptyTile(BaseCoord);
			}
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewType, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, tileCoord, tileCoord, 0f, baseClass.transform.position.y, 4f);
		}
	}
}
