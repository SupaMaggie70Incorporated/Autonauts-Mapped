using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrackCurve : TrainTrack
{
	public override void Restart()
	{
		base.Restart();
		RemoveTile(new TileCoord(1, 0));
		RemoveTile(new TileCoord(1, 1));
		RemoveTile(new TileCoord(0, 1));
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 1), new TileCoord(0, 0));
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
				CreateBuffer(0, new Vector3(0f - Tile.m_Size, 0f, 0f - Tile.m_Size - 1.5f), Quaternion.Euler(0f, 90f, 0f));
			}
		}
		if (m_ConnectedRightChanged)
		{
			m_ConnectedRightChanged = false;
			DestroyBuffer(1);
			if (!m_ConnectedRight)
			{
				CreateBuffer(1, new Vector3(Tile.m_Size + 1.5f, 0f, Tile.m_Size), Quaternion.Euler(0f, 0f, 0f));
			}
		}
	}

	protected override void Refresh()
	{
		if ((bool)m_Plot)
		{
			m_Plot.UpdateObjectMerger(this, Immediate: true);
		}
		TrainTrack trainTrack = null;
		TrainTrack trainTrack2 = null;
		if (!m_PlayerDeleted)
		{
			bool flag = true;
			if (m_Rotation == 1 || m_Rotation == 3)
			{
				flag = false;
			}
			trainTrack = TrainTrack.GetConnectedTrackInDirection(GetStartPosition(), flag, !flag, -GetStartDirection());
			trainTrack2 = TrainTrack.GetConnectedTrackInDirection(GetEndPosition(), !flag, flag, -GetEndDirection());
		}
		if (m_ConnectedDown != trainTrack)
		{
			m_ConnectedDown = trainTrack;
			m_ConnectedDownChanged = true;
			DestroyBuffer(0);
		}
		if (m_ConnectedRight != trainTrack2)
		{
			m_ConnectedRight = trainTrack2;
			m_ConnectedRightChanged = true;
			DestroyBuffer(1);
		}
	}

	public float GetCurveRadius()
	{
		return Tile.m_Size * 2f;
	}

	public Vector3 GetCurvePosition(float Percent, int VehicleRotation, out Quaternion NewRotation)
	{
		Vector3 vector = new TileCoord(1, 1).ToWorldPosition();
		float curveRadius = GetCurveRadius();
		float num = Percent * (float)Math.PI * 0.5f - (float)Math.PI;
		vector.x += Mathf.Cos(num) * curveRadius;
		vector.z -= Mathf.Sin(num) * curveRadius;
		if (m_Rotation == 3)
		{
			float z = vector.z;
			vector.z = vector.x;
			vector.x = 0f - z;
		}
		else if (m_Rotation == 2)
		{
			vector.z = 0f - vector.z;
			vector.x = 0f - vector.x;
		}
		else if (m_Rotation == 1)
		{
			float z = vector.z;
			vector.z = 0f - vector.x;
			vector.x = z;
		}
		num += (float)Math.PI / 2f * (float)m_Rotation + (float)Math.PI;
		int num2 = m_Rotation * 2;
		int num3 = (num2 + 6) % 8;
		if (VehicleRotation == num2 || VehicleRotation == num3)
		{
			num += (float)Math.PI;
		}
		NewRotation = Quaternion.Euler(0f, num * 57.29578f, 0f);
		return vector + m_TileCoord.ToWorldPositionTileCentered();
	}

	public TileCoord GetStartPosition()
	{
		TileCoord tileCoord = new TileCoord(-1, 1);
		tileCoord.Rotate(m_Rotation);
		return tileCoord + m_TileCoord;
	}

	public TileCoord GetStartDirection()
	{
		TileCoord result = new TileCoord(0, -1);
		result.Rotate(m_Rotation);
		return result;
	}

	public TileCoord GetEndPosition()
	{
		TileCoord tileCoord = new TileCoord(1, -1);
		tileCoord.Rotate(m_Rotation);
		return tileCoord + m_TileCoord;
	}

	public TileCoord GetEndDirection()
	{
		TileCoord result = new TileCoord(-1, 0);
		result.Rotate(m_Rotation);
		return result;
	}

	public override void GetCarriagePosition(float Distance, TileCoord AttachDirection, bool MoveForwards, Vector3 OldPosition, Transform OldTransform, out Vector3 NewPosition, out Quaternion NewRotation, List<TrainTrackPoints> SwitchList, Minecart NewMinecart)
	{
		NewPosition = default(Vector3);
		NewRotation = Quaternion.identity;
		float num = GetCurveRadius() * (float)Math.PI * 0.5f;
		bool flag = false;
		if (AttachDirection == -GetStartDirection() || AttachDirection == GetEndDirection())
		{
			flag = true;
		}
		Vector3 vector = base.transform.InverseTransformPoint(OldPosition);
		vector.z = 0f - vector.z;
		float num2;
		if (vector.z >= Tile.m_Size)
		{
			num2 = Tile.m_Size * 0.5f + num + (vector.z - Tile.m_Size);
		}
		else if (vector.x < Tile.m_Size)
		{
			Vector3 vector2 = vector + new Vector3(0f - Tile.m_Size, 0f, 0f - Tile.m_Size);
			float num3 = ((0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f - 90f) / 90f;
			num2 = Tile.m_Size * 0.5f + num * num3;
		}
		else
		{
			num2 = Tile.m_Size * 1.5f - vector.x;
		}
		TrainTrack trainTrack = null;
		if (flag)
		{
			num2 = num + Tile.m_Size - num2;
			if (Distance > num2)
			{
				trainTrack = m_ConnectedDown;
				Vector3 vector3 = -GetStartDirection().ToWorldPosition() * 0.5f;
				OldPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() + vector3;
				AttachDirection = -GetStartDirection();
			}
		}
		else if (Distance > num2)
		{
			trainTrack = m_ConnectedRight;
			Vector3 vector4 = -GetEndDirection().ToWorldPosition() * 0.5f;
			OldPosition = GetEndPosition().ToWorldPositionTileCenteredWithoutHeight() + vector4;
			AttachDirection = -GetEndDirection();
		}
		if ((bool)trainTrack)
		{
			Distance -= num2;
			trainTrack.GetCarriagePosition(Distance, AttachDirection, MoveForwards, OldPosition, OldTransform, out NewPosition, out NewRotation, SwitchList, NewMinecart);
			return;
		}
		Distance += Tile.m_Size + num - num2;
		if (!flag)
		{
			Distance = Tile.m_Size + num - Distance;
		}
		float y;
		if (Distance >= Tile.m_Size * 0.5f + num)
		{
			Distance -= Tile.m_Size * 0.5f + num;
			NewPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() - GetStartDirection().ToWorldPosition() * (Distance / Tile.m_Size);
			y = m_Rotation * 90 + 90;
		}
		else if (Distance >= Tile.m_Size * 0.5f)
		{
			Distance -= Tile.m_Size * 0.5f;
			float num4 = 0f - (Distance / num * 90f + 90f);
			NewPosition.x = Mathf.Cos(num4 * ((float)Math.PI / 180f)) * GetCurveRadius();
			NewPosition.z = (0f - Mathf.Sin(num4 * ((float)Math.PI / 180f))) * GetCurveRadius();
			NewPosition += new Vector3(Tile.m_Size, 0f, 0f - Tile.m_Size);
			NewPosition = base.transform.TransformPoint(NewPosition);
			y = (float)(m_Rotation * 90) + num4 - 90f;
		}
		else
		{
			Distance -= Tile.m_Size * 0.5f;
			NewPosition = GetEndPosition().ToWorldPositionTileCenteredWithoutHeight() + GetEndDirection().ToWorldPosition() * (Distance / Tile.m_Size);
			y = m_Rotation * 90 + 180;
		}
		NewRotation = Quaternion.Euler(0f, y, 0f);
	}

	public override bool CheckTrackAhead(float Distance, TileCoord MoveDirection, Vector3 OldPosition, List<TrainTrackPoints> SwitchList)
	{
		float num = GetCurveRadius() * (float)Math.PI * 0.5f;
		bool flag = false;
		if (MoveDirection == -GetStartDirection() || MoveDirection == GetEndDirection())
		{
			flag = true;
		}
		Vector3 vector = base.transform.InverseTransformPoint(OldPosition);
		vector.z = 0f - vector.z;
		float num2;
		if (vector.z >= Tile.m_Size)
		{
			num2 = Tile.m_Size * 0.5f + num + (vector.z - Tile.m_Size);
		}
		else if (vector.x < Tile.m_Size)
		{
			Vector3 vector2 = vector + new Vector3(0f - Tile.m_Size, 0f, 0f - Tile.m_Size);
			float num3 = ((0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f - 90f) / 90f;
			num2 = Tile.m_Size * 0.5f + num * num3;
		}
		else
		{
			num2 = Tile.m_Size * 1.5f - vector.x;
		}
		TrainTrack trainTrack = null;
		if (flag)
		{
			num2 = num + Tile.m_Size - num2;
			if (Distance > num2)
			{
				trainTrack = m_ConnectedDown;
				Vector3 vector3 = -GetStartDirection().ToWorldPosition() * 0.5f;
				OldPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() + vector3;
				MoveDirection = -GetStartDirection();
			}
		}
		else if (Distance > num2)
		{
			trainTrack = m_ConnectedRight;
			Vector3 vector4 = -GetEndDirection().ToWorldPosition() * 0.5f;
			OldPosition = GetEndPosition().ToWorldPositionTileCenteredWithoutHeight() + vector4;
			MoveDirection = -GetEndDirection();
		}
		if ((bool)trainTrack)
		{
			Distance -= num2;
			return trainTrack.CheckTrackAhead(Distance, MoveDirection, OldPosition, SwitchList);
		}
		if (Distance > num2)
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
		float num = GetCurveRadius() * (float)Math.PI * 0.5f;
		bool flag = false;
		if (MoveDirection == -GetStartDirection() || MoveDirection == GetEndDirection())
		{
			flag = true;
		}
		Vector3 vector = base.transform.InverseTransformPoint(OldPosition);
		vector.z = 0f - vector.z;
		float num2;
		if (vector.z >= Tile.m_Size)
		{
			num2 = Tile.m_Size * 0.5f + num + (vector.z - Tile.m_Size);
		}
		else if (vector.x < Tile.m_Size)
		{
			Vector3 vector2 = vector + new Vector3(0f - Tile.m_Size, 0f, 0f - Tile.m_Size);
			float num3 = ((0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f - 90f) / 90f;
			num2 = Tile.m_Size * 0.5f + num * num3;
		}
		else
		{
			num2 = Tile.m_Size * 1.5f - vector.x;
		}
		TrainTrack trainTrack = null;
		if (flag)
		{
			num2 = num + Tile.m_Size - num2;
			if (Distance > num2)
			{
				trainTrack = m_ConnectedDown;
				Vector3 vector3 = -GetStartDirection().ToWorldPosition() * 0.5f;
				OldPosition = GetStartPosition().ToWorldPositionTileCenteredWithoutHeight() + vector3;
				MoveDirection = -GetStartDirection();
			}
		}
		else if (Distance > num2)
		{
			trainTrack = m_ConnectedRight;
			Vector3 vector4 = -GetEndDirection().ToWorldPosition() * 0.5f;
			OldPosition = GetEndPosition().ToWorldPositionTileCenteredWithoutHeight() + vector4;
			MoveDirection = -GetEndDirection();
		}
		if ((bool)trainTrack)
		{
			Distance -= num2;
			return trainTrack.CheckForMinecart(Distance, MoveDirection, OldPosition, TestTrack, SwitchList);
		}
		return false;
	}
}
