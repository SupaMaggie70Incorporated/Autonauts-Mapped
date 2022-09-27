using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TrainTrackPoints : TrainTrack
{
	public bool m_Left;

	public bool m_PlayerSwitch;

	private TrackIndicator m_TrackIndicator;

	private Material m_TrackIndicatorMaterial;

	public static bool GetIsTypeTrainTrackPoints(ObjectType NewType)
	{
		if (NewType == ObjectType.TrainTrackPointsLeft || NewType == ObjectType.TrainTrackPointsRight)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -2), new TileCoord(1, 0), new TileCoord(0, 0));
		m_PlayerSwitch = false;
		UpdateTrackIndicator();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		GameObject original = (GameObject)Resources.Load("Prefabs/TrackIndicator", typeof(GameObject));
		m_TrackIndicator = UnityEngine.Object.Instantiate(original, null).GetComponent<TrackIndicator>();
		float num = -0.1f * Tile.m_Size * 3f;
		float num2 = num;
		if (m_TypeIdentifier == ObjectType.TrainTrackPointsLeft)
		{
			num2 = 0f - num2;
		}
		m_TrackIndicator.transform.localScale = new Vector3(num2, 1f, num);
		m_TrackIndicator.SetParent(base.transform);
		m_TrackIndicator.SetPosition(new Vector3(Tile.m_Size * 0.5f, 0.75f, Tile.m_Size));
		m_TrackIndicatorMaterial = m_TrackIndicator.GetComponent<MeshRenderer>().material;
		m_TrackIndicator.gameObject.SetActive(value: false);
	}

	protected new void OnDestroy()
	{
		if ((bool)m_TrackIndicator)
		{
			UnityEngine.Object.Destroy(m_TrackIndicator.gameObject);
		}
		base.OnDestroy();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_TrackIndicator)
		{
			m_TrackIndicator.gameObject.SetActive(value: false);
		}
		base.StopUsing(AndDestroy);
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_CountIndex != 0)
		{
			text = text + " " + m_CountIndex;
		}
		return text;
	}

	public override void SetHighlight(bool Highlighted)
	{
		base.SetHighlight(Highlighted);
		m_TrackIndicator.SetFlash(Highlighted);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Switch", m_PlayerSwitch);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_PlayerSwitch = JSONUtils.GetAsBool(Node, "Switch", DefaultValue: false);
		UpdateTrackIndicator();
	}

	public override void RefreshBuffers(bool Force = false)
	{
		if (!Force && GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			return;
		}
		if (m_ConnectedDownChanged)
		{
			m_ConnectedDownChanged = false;
			DestroyBuffer(0);
			if (!m_ConnectedDown)
			{
				CreateBuffer(0, GetStartOffset().ToWorldPosition() + new Vector3(0f, 0f, Tile.m_Size * -0.5f), Quaternion.Euler(0f, 90f, 0f));
			}
		}
		if (m_ConnectedUpChanged)
		{
			m_ConnectedUpChanged = false;
			DestroyBuffer(1);
			if (!m_ConnectedUp)
			{
				CreateBuffer(1, GetEndOffset1().ToWorldPosition() + new Vector3(0f, 0f, Tile.m_Size * 0.5f), Quaternion.Euler(0f, -90f, 0f));
			}
		}
		if (m_ConnectedLeftChanged)
		{
			m_ConnectedLeftChanged = false;
			DestroyBuffer(2);
			if (!m_ConnectedLeft)
			{
				CreateBuffer(2, GetEndOffset2().ToWorldPosition() + new Vector3(0f, 0f, Tile.m_Size * 0.5f), Quaternion.Euler(0f, -90f, 0f));
			}
		}
	}

	private void UpdateTrackIndicator()
	{
		string text = "TrackPointsAhead";
		if (m_PlayerSwitch)
		{
			text = "TrackPointsTurn";
		}
		Texture2D value = (Texture2D)Resources.Load("Textures/" + text, typeof(Texture2D));
		m_TrackIndicatorMaterial.SetTexture("_MainTex", value);
	}

	public void TogglePlayerSwitch()
	{
		m_PlayerSwitch = !m_PlayerSwitch;
		UpdateTrackIndicator();
	}

	public void ShowPlayerSwitch(bool Show)
	{
		m_TrackIndicator.gameObject.SetActive(Show);
	}

	protected override void Refresh()
	{
		if ((bool)m_Plot)
		{
			m_Plot.UpdateObjectMerger(this, Immediate: true);
		}
		TrainTrack trainTrack = null;
		TrainTrack trainTrack2 = null;
		TrainTrack trainTrack3 = null;
		if (!m_PlayerDeleted)
		{
			bool flag = true;
			if (m_Rotation == 1 || m_Rotation == 3)
			{
				flag = false;
			}
			trainTrack = TrainTrack.GetConnectedTrackInDirection(GetStartPosition(), flag, !flag, -GetStartDirection());
			trainTrack2 = TrainTrack.GetConnectedTrackInDirection(GetEndPosition1(), flag, !flag, -GetEndDirection());
			trainTrack3 = TrainTrack.GetConnectedTrackInDirection(GetEndPosition2(), flag, !flag, -GetEndDirection());
		}
		if (m_ConnectedDown != trainTrack)
		{
			m_ConnectedDown = trainTrack;
			m_ConnectedDownChanged = true;
			DestroyBuffer(0);
		}
		if (m_ConnectedUp != trainTrack2)
		{
			m_ConnectedUp = trainTrack2;
			m_ConnectedUpChanged = true;
			DestroyBuffer(1);
		}
		if (m_ConnectedLeft != trainTrack3)
		{
			m_ConnectedLeft = trainTrack3;
			m_ConnectedLeftChanged = true;
			DestroyBuffer(2);
		}
	}

	public int GetStartTile(TileCoord NewCoord)
	{
		if (NewCoord == GetStartPosition())
		{
			return 0;
		}
		if (NewCoord == GetEndPosition1())
		{
			return 1;
		}
		if (NewCoord == GetEndPosition2())
		{
			return 2;
		}
		Debug.Log("Bad tile start position");
		return 0;
	}

	private TileCoord GetTileOffsetFromStartTile(int StartTile)
	{
		return StartTile switch
		{
			0 => GetStartOffset(), 
			1 => GetEndOffset1(), 
			_ => GetEndOffset2(), 
		};
	}

	private float GetCurveOffset(float Percent, out float CurveRotation)
	{
		if (Percent > 1f)
		{
			Percent = 1f;
		}
		if (Percent < 0f)
		{
			Percent = 0f;
		}
		float num = (0f - Mathf.Cos(Percent * (float)Math.PI)) * 0.5f + 0.5f;
		num *= 0f - Tile.m_Size;
		CurveRotation = (0f - Mathf.Cos(Percent * (float)Math.PI * 2f)) * 0.5f + 0.5f;
		CurveRotation *= -30f;
		if (!m_Left)
		{
			num = 0f - num;
			CurveRotation = 0f - CurveRotation;
		}
		return num;
	}

	public Vector3 GetPointsPosition(float Percent, int StartTile, List<TrainTrackPoints> SwitchList, int VehicleRotation, out Quaternion NewRotation, Minecart NewMinecart)
	{
		Vector3 position = GetTileOffsetFromStartTile(StartTile).ToWorldPosition();
		float num = TileMover.ConvertRotationToDegrees(VehicleRotation);
		float CurveRotation = 0f;
		bool flag = false;
		if (StartTile == 0)
		{
			flag = GetSwitchAtStart(NewMinecart);
			position.z += Tile.m_Size * 2f * Percent;
			if (flag)
			{
				position.x += GetCurveOffset(Percent, out CurveRotation);
			}
		}
		else
		{
			position.z -= Tile.m_Size * 2f * Percent;
			if (StartTile == 2)
			{
				position.x -= GetCurveOffset(Percent, out CurveRotation);
				flag = true;
			}
		}
		num += CurveRotation;
		NewRotation = Quaternion.Euler(0f, num, 0f);
		position = base.transform.TransformPoint(position);
		if (flag && !SwitchList.Contains(this))
		{
			SwitchList.Add(this);
			Debug.Log("Add " + m_UniqueID);
		}
		return position;
	}

	public TileCoord GetStartDirection()
	{
		TileCoord result = new TileCoord(0, -1);
		result.Rotate(m_Rotation);
		return result;
	}

	public TileCoord GetEndDirection()
	{
		TileCoord result = new TileCoord(0, 1);
		result.Rotate(m_Rotation);
		return result;
	}

	private TileCoord GetStartOffset()
	{
		if (m_Left)
		{
			return new TileCoord(1, 0);
		}
		return new TileCoord(0, 0);
	}

	public TileCoord GetStartPosition()
	{
		TileCoord startOffset = GetStartOffset();
		startOffset.Rotate(m_Rotation);
		return startOffset + m_TileCoord;
	}

	private TileCoord GetEndOffset1()
	{
		if (m_Left)
		{
			return new TileCoord(1, -2);
		}
		return new TileCoord(0, -2);
	}

	public TileCoord GetEndPosition1()
	{
		TileCoord endOffset = GetEndOffset1();
		endOffset.Rotate(m_Rotation);
		return endOffset + m_TileCoord;
	}

	private TileCoord GetEndOffset2()
	{
		if (m_Left)
		{
			return new TileCoord(0, -2);
		}
		return new TileCoord(1, -2);
	}

	public TileCoord GetEndPosition2()
	{
		TileCoord endOffset = GetEndOffset2();
		endOffset.Rotate(m_Rotation);
		return endOffset + m_TileCoord;
	}

	private bool GetSwitchAtStart(Minecart NewMinecart)
	{
		if ((bool)NewMinecart.m_Engager)
		{
			if (NewMinecart.m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				return m_PlayerSwitch;
			}
			if (NewMinecart.m_Engager.m_TypeIdentifier == ObjectType.Worker)
			{
				return NewMinecart.m_Engager.GetComponent<Worker>().m_WorkerInterpreter.GetTurnAtSoon(this);
			}
		}
		return false;
	}

	private bool GetSwitchAtEnd(Vector3 Offset)
	{
		if (m_Left && Offset.x < Tile.m_Size - 0.01f)
		{
			return true;
		}
		if (!m_Left && Offset.x > 0.01f)
		{
			return true;
		}
		return false;
	}

	public override void GetCarriagePosition(float Distance, TileCoord AttachDirection, bool MoveForwards, Vector3 OldPosition, Transform OldTransform, out Vector3 NewPosition, out Quaternion NewRotation, List<TrainTrackPoints> SwitchList, Minecart NewMinecart)
	{
		NewPosition = default(Vector3);
		NewRotation = Quaternion.identity;
		Vector3 offset = base.transform.InverseTransformPoint(OldPosition);
		offset.z = 0f - offset.z;
		bool flag = false;
		TileCoord tileCoord = AttachDirection;
		if (MoveForwards)
		{
			tileCoord = -tileCoord;
		}
		flag = ((!(tileCoord == GetStartDirection())) ? GetSwitchAtEnd(offset) : GetSwitchAtStart(NewMinecart));
		TrainTrack trainTrack = null;
		float num;
		if (AttachDirection == GetStartDirection())
		{
			num = Tile.m_Size * 2.5f + offset.z;
			if (Distance > num)
			{
				Vector3 vector = -GetEndDirection().ToWorldPosition() * 0.5f;
				if (!flag)
				{
					trainTrack = m_ConnectedUp;
					OldPosition = GetEndPosition1().ToWorldPositionTileCenteredWithoutHeight() + vector;
				}
				else
				{
					trainTrack = m_ConnectedLeft;
					OldPosition = GetEndPosition2().ToWorldPositionTileCenteredWithoutHeight() + vector;
				}
			}
		}
		else
		{
			num = Tile.m_Size * 0.5f - offset.z;
			if (Distance > num)
			{
				trainTrack = m_ConnectedDown;
				Vector3 vector2 = -GetStartDirection().ToWorldPosition() * 0.5f;
				OldPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() + vector2;
			}
		}
		if (flag && !SwitchList.Contains(this))
		{
			SwitchList.Add(this);
			Debug.Log("Add " + m_UniqueID);
		}
		if ((bool)trainTrack)
		{
			Distance -= num;
			trainTrack.GetCarriagePosition(Distance, AttachDirection, MoveForwards, OldPosition, OldTransform, out NewPosition, out NewRotation, SwitchList, NewMinecart);
			return;
		}
		Vector3 position = GetTileOffsetFromStartTile(0).ToWorldPosition();
		float num2 = Tile.m_Size * 2f;
		float num3 = ((!(AttachDirection == GetStartDirection())) ? ((0f - (offset.z + Distance)) / num2) : ((0f - offset.z + Distance) / num2));
		position.z += Tile.m_Size * 2f * num3;
		float CurveRotation = 0f;
		if (SwitchList.Contains(this))
		{
			position.x += GetCurveOffset(num3, out CurveRotation);
		}
		float num4 = m_Rotation * 90 + 90;
		num4 += CurveRotation;
		NewRotation = Quaternion.Euler(0f, num4, 0f);
		NewPosition = base.transform.TransformPoint(position);
	}

	public override bool CheckTrackAhead(float Distance, TileCoord MoveDirection, Vector3 OldPosition, List<TrainTrackPoints> SwitchList)
	{
		Vector3 vector = base.transform.InverseTransformPoint(OldPosition);
		vector.z = 0f - vector.z;
		TrainTrack trainTrack = null;
		float num;
		if (MoveDirection == GetStartDirection())
		{
			num = Tile.m_Size * 2.5f + vector.z;
			if (Distance > num)
			{
				Vector3 vector2 = -GetEndDirection().ToWorldPosition() * 0.5f;
				if (!SwitchList.Contains(this))
				{
					trainTrack = m_ConnectedUp;
					OldPosition = GetEndPosition1().ToWorldPositionTileCenteredWithoutHeight() + vector2;
				}
				else
				{
					trainTrack = m_ConnectedLeft;
					OldPosition = GetEndPosition2().ToWorldPositionTileCenteredWithoutHeight() + vector2;
				}
			}
		}
		else
		{
			num = Tile.m_Size * 0.5f - vector.z;
			if (Distance > num)
			{
				trainTrack = m_ConnectedDown;
				Vector3 vector3 = -GetStartDirection().ToWorldPosition() * 0.5f;
				OldPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() + vector3;
			}
		}
		if ((bool)trainTrack)
		{
			Distance -= num;
			return trainTrack.CheckTrackAhead(Distance, MoveDirection, OldPosition, SwitchList);
		}
		if (Distance > num)
		{
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
		Vector3 vector = base.transform.InverseTransformPoint(OldPosition);
		vector.z = 0f - vector.z;
		TrainTrack trainTrack = null;
		float num;
		if (MoveDirection == GetStartDirection())
		{
			num = Tile.m_Size * 2.5f + vector.z;
			if (Distance > num)
			{
				Vector3 vector2 = -GetEndDirection().ToWorldPosition() * 0.5f;
				if (!SwitchList.Contains(this))
				{
					trainTrack = m_ConnectedUp;
					OldPosition = GetEndPosition1().ToWorldPositionTileCenteredWithoutHeight() + vector2;
				}
				else
				{
					trainTrack = m_ConnectedLeft;
					OldPosition = GetEndPosition2().ToWorldPositionTileCenteredWithoutHeight() + vector2;
				}
			}
		}
		else
		{
			num = Tile.m_Size * 0.5f - vector.z;
			if (Distance > num)
			{
				trainTrack = m_ConnectedDown;
				Vector3 vector3 = -GetStartDirection().ToWorldPosition() * 0.5f;
				OldPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() + vector3;
			}
		}
		if ((bool)trainTrack)
		{
			Distance -= num;
			return trainTrack.CheckForMinecart(Distance, MoveDirection, OldPosition, TestTrack, SwitchList);
		}
		return false;
	}
}
