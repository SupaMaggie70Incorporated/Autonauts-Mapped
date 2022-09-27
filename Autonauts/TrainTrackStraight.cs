using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TrainTrackStraight : TrainTrack
{
	public enum Type
	{
		Straight,
		Cross,
		End,
		Total
	}

	private static bool m_Log;

	protected static string m_CrossModelName;

	protected static string m_StraightModelName;

	[HideInInspector]
	public bool m_Cross;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Track", m_TypeIdentifier);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/TrainTrackCross", ObjectType.TrainTrack);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/TrainTrack", ObjectType.TrainTrack);
	}

	public override void Restart()
	{
		m_CrossModelName = "TrainTrackCross";
		m_StraightModelName = "TrainTrack";
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 0));
		HideAccessModel();
	}

	public override void Delete()
	{
		m_ParentBuilding = null;
		LoadNewModel("Models/Buildings/Floors/" + m_StraightModelName);
		m_Cross = false;
		base.Delete();
	}

	public override void CopyFrom(Building OriginalBuilding)
	{
		TrainTrackStraight trainTrackStraight = null;
		trainTrackStraight = ((OriginalBuilding.m_TypeIdentifier != ObjectType.ConverterFoundation) ? OriginalBuilding.GetComponent<TrainTrackStraight>() : OriginalBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<TrainTrackStraight>());
		m_Cross = trainTrackStraight.m_Cross;
	}

	public void SetCross()
	{
		m_Cross = true;
		string text = m_StraightModelName;
		if (m_Cross)
		{
			text = m_CrossModelName;
		}
		LoadNewModel("Models/Buildings/Floors/" + text);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "C", m_Cross);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		if (JSONUtils.GetAsBool(Node, "C", DefaultValue: false))
		{
			SetCross();
		}
	}

	public override void RefreshBuffers(bool Force = false)
	{
		if ((Force || GameStateManager.Instance.GetActualState() != GameStateManager.State.Edit) && m_ConnectedDownChanged)
		{
			m_ConnectedDownChanged = false;
			DestroyBuffer(0);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = true;
			bool flag6 = false;
			if (m_Cross)
			{
				flag6 = true;
			}
			else if (m_Rotation % 2 == 1)
			{
				flag5 = false;
				flag6 = true;
			}
			if ((bool)m_ConnectedUp)
			{
				flag = true;
			}
			if ((bool)m_ConnectedDown)
			{
				flag2 = true;
			}
			if ((bool)m_ConnectedLeft)
			{
				flag3 = true;
			}
			if ((bool)m_ConnectedRight)
			{
				flag4 = true;
			}
			if ((flag6 && flag != flag2) || (flag5 && flag3 != flag4))
			{
				CreateBuffer(0, new Vector3(1.5f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
			}
		}
	}

	public bool GetHorizontal()
	{
		if (m_Rotation % 2 == 0 || m_Cross)
		{
			return true;
		}
		return false;
	}

	public bool GetVertical()
	{
		if (m_Rotation % 2 == 1 || m_Cross)
		{
			return true;
		}
		return false;
	}

	private bool GetHasStop(TileCoord Direction, int Rotation)
	{
		Tile tile = TileManager.Instance.GetTile(m_TileCoord + Direction);
		if (tile != null && (bool)tile.m_Building && TrainTrackStop.GetIsTypeTrainTrackStop(tile.m_Building.m_TypeIdentifier) && tile.m_Building.m_Rotation == Rotation)
		{
			return true;
		}
		return false;
	}

	public bool GetHasStop()
	{
		if (GetVertical())
		{
			if (GetHasStop(new TileCoord(-1, 0), 3) || GetHasStop(new TileCoord(1, 0), 1))
			{
				return true;
			}
		}
		else if (GetHasStop(new TileCoord(0, -1), 0) || GetHasStop(new TileCoord(0, 1), 2))
		{
			return true;
		}
		return false;
	}

	private Building GetStop(TileCoord Direction, int Rotation)
	{
		Tile tile = TileManager.Instance.GetTile(m_TileCoord + Direction);
		if (tile != null && (bool)tile.m_Building && TrainTrackStop.GetIsTypeTrainTrackStop(tile.m_Building.m_TypeIdentifier) && tile.m_Building.m_Rotation == Rotation)
		{
			return tile.m_Building;
		}
		return null;
	}

	public Building GetStop()
	{
		if (GetVertical())
		{
			Building stop = GetStop(new TileCoord(-1, 0), 3);
			if ((bool)stop)
			{
				return stop;
			}
			stop = GetStop(new TileCoord(1, 0), 1);
			if ((bool)stop)
			{
				return stop;
			}
		}
		else
		{
			Building stop2 = GetStop(new TileCoord(0, -1), 0);
			if ((bool)stop2)
			{
				return stop2;
			}
			stop2 = GetStop(new TileCoord(0, 1), 2);
			if ((bool)stop2)
			{
				return stop2;
			}
		}
		return null;
	}

	protected override void Refresh()
	{
		if (m_Plot == null)
		{
			return;
		}
		m_Plot.UpdateObjectMerger(this, Immediate: true);
		bool flag = true;
		bool flag2 = false;
		if (m_Cross)
		{
			flag2 = true;
		}
		else if (m_Rotation % 2 == 1)
		{
			flag = false;
			flag2 = true;
		}
		TrainTrack trainTrack = null;
		TrainTrack trainTrack2 = null;
		TrainTrack trainTrack3 = null;
		TrainTrack trainTrack4 = null;
		if (!m_PlayerDeleted)
		{
			trainTrack = TrainTrack.GetConnectedTrackInDirection(m_TileCoord, flag2, flag, new TileCoord(0, -1));
			trainTrack2 = TrainTrack.GetConnectedTrackInDirection(m_TileCoord, flag2, flag, new TileCoord(0, 1));
			trainTrack3 = TrainTrack.GetConnectedTrackInDirection(m_TileCoord, flag2, flag, new TileCoord(-1, 0));
			trainTrack4 = TrainTrack.GetConnectedTrackInDirection(m_TileCoord, flag2, flag, new TileCoord(1, 0));
		}
		if (trainTrack2 != m_ConnectedDown || trainTrack3 != m_ConnectedLeft || trainTrack4 != m_ConnectedRight || trainTrack != m_ConnectedUp)
		{
			m_ConnectedDown = trainTrack2;
			m_ConnectedLeft = trainTrack3;
			m_ConnectedRight = trainTrack4;
			m_ConnectedUp = trainTrack;
			m_ConnectedDownChanged = true;
			DestroyBuffer(0);
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			if ((bool)m_ConnectedUp)
			{
				flag3 = true;
			}
			if ((bool)m_ConnectedDown)
			{
				flag4 = true;
			}
			if ((bool)m_ConnectedLeft)
			{
				flag5 = true;
			}
			if ((bool)m_ConnectedRight)
			{
				flag6 = true;
			}
			int num = 0;
			if (flag3 || flag4 || flag5 || flag6)
			{
				num = ((flag2 && flag3 != flag4) ? (flag3 ? 1 : 3) : ((flag && flag5 != flag6) ? ((!flag5) ? 2 : 0) : (flag2 ? 1 : 0)));
				base.transform.localRotation = Quaternion.Euler(0f, (float)num * 90f, 0f);
			}
		}
	}

	public override void GetCarriagePosition(float Distance, TileCoord AttachDirection, bool MoveForwards, Vector3 OldPosition, Transform OldTransform, out Vector3 NewPosition, out Quaternion NewRotation, List<TrainTrackPoints> SwitchList, Minecart NewMinecart)
	{
		NewPosition = default(Vector3);
		NewRotation = Quaternion.identity;
		TrainTrack trainTrack = null;
		Vector3 vector = OldPosition - m_TileCoord.ToWorldPosition();
		vector.z = 0f - vector.z;
		float num;
		if (AttachDirection.x > 0)
		{
			num = Tile.m_Size - vector.x;
			trainTrack = m_ConnectedRight;
		}
		else if (AttachDirection.x < 0)
		{
			num = vector.x;
			trainTrack = m_ConnectedLeft;
		}
		else if (AttachDirection.y > 0)
		{
			num = Tile.m_Size - vector.z;
			trainTrack = m_ConnectedDown;
		}
		else
		{
			num = vector.z;
			trainTrack = m_ConnectedUp;
		}
		if (Distance > num && (bool)trainTrack)
		{
			Distance -= num;
			OldPosition.x += (float)AttachDirection.x * num;
			OldPosition.z -= (float)AttachDirection.y * num;
			trainTrack.GetCarriagePosition(Distance, AttachDirection, MoveForwards, OldPosition, OldTransform, out NewPosition, out NewRotation, SwitchList, NewMinecart);
			return;
		}
		if (trainTrack == null)
		{
			num -= Tile.m_Size * 0.5f;
		}
		if (Distance > num)
		{
			NewPosition = base.transform.position;
		}
		else
		{
			NewPosition.x = OldPosition.x + (float)AttachDirection.x * Distance;
			NewPosition.z = OldPosition.z - (float)AttachDirection.y * Distance;
		}
		if (m_Cross)
		{
			NewRotation = OldTransform.rotation;
			return;
		}
		float y = 0f;
		if (GetVertical())
		{
			y = 90f;
		}
		NewRotation = Quaternion.Euler(0f, y, 0f);
	}

	public override bool CheckTrackAhead(float Distance, TileCoord MoveDirection, Vector3 OldPosition, List<TrainTrackPoints> SwitchList)
	{
		TrainTrack trainTrack = null;
		Vector3 vector = OldPosition - m_TileCoord.ToWorldPosition();
		vector.z = 0f - vector.z;
		float num;
		if (MoveDirection.x > 0)
		{
			num = Tile.m_Size - vector.x;
			trainTrack = m_ConnectedRight;
		}
		else if (MoveDirection.x < 0)
		{
			num = vector.x;
			trainTrack = m_ConnectedLeft;
		}
		else if (MoveDirection.y > 0)
		{
			num = Tile.m_Size - vector.z;
			trainTrack = m_ConnectedDown;
		}
		else
		{
			num = vector.z;
			trainTrack = m_ConnectedUp;
		}
		if (Distance > num)
		{
			if ((bool)trainTrack)
			{
				Distance -= num;
				OldPosition.x += (float)MoveDirection.x * num;
				OldPosition.z -= (float)MoveDirection.y * num;
				return trainTrack.CheckTrackAhead(Distance, MoveDirection, OldPosition, SwitchList);
			}
			return false;
		}
		return true;
	}

	public override bool CheckForMinecart(float Distance, TileCoord MoveDirection, Vector3 OldPosition, TrainTrack TestTrack, List<TrainTrackPoints> SwitchList)
	{
		if (TestTrack == GetComponent<TrainTrack>())
		{
			return true;
		}
		TrainTrack trainTrack = null;
		Vector3 vector = OldPosition - m_TileCoord.ToWorldPosition();
		vector.z = 0f - vector.z;
		float num;
		if (MoveDirection.x > 0)
		{
			num = Tile.m_Size - vector.x;
			trainTrack = m_ConnectedRight;
		}
		else if (MoveDirection.x < 0)
		{
			num = vector.x;
			trainTrack = m_ConnectedLeft;
		}
		else if (MoveDirection.y > 0)
		{
			num = Tile.m_Size - vector.z;
			trainTrack = m_ConnectedDown;
		}
		else
		{
			num = vector.z;
			trainTrack = m_ConnectedUp;
		}
		if (Distance > num)
		{
			if ((bool)trainTrack)
			{
				Distance -= num;
				OldPosition.x += (float)MoveDirection.x * num;
				OldPosition.z -= (float)MoveDirection.y * num;
				return trainTrack.CheckForMinecart(Distance, MoveDirection, OldPosition, TestTrack, SwitchList);
			}
			return false;
		}
		return false;
	}
}
