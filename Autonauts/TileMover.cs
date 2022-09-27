using SimpleJSON;
using UnityEngine;

public class TileMover : Holdable
{
	private static int m_NumDirections = 8;

	[HideInInspector]
	public int m_Rotation;

	[HideInInspector]
	public bool m_Move;

	[HideInInspector]
	public float m_MoveNormalDelay;

	private float m_MoveTimer;

	private float m_MoveDelay;

	private Vector3 m_MoveStartPosition;

	private Vector3 m_MoveEndPosition;

	protected bool m_Turn;

	private float m_TurnTimer;

	private float m_TurnDelay;

	public static TileCoord[] m_DirectionTable = new TileCoord[8]
	{
		new TileCoord(1, 0),
		new TileCoord(1, 1),
		new TileCoord(0, 1),
		new TileCoord(-1, 1),
		new TileCoord(-1, 0),
		new TileCoord(-1, -1),
		new TileCoord(0, -1),
		new TileCoord(1, -1)
	};

	protected new void Awake()
	{
		base.Awake();
		m_Rotation = 0;
		m_Move = false;
		m_MoveNormalDelay = 0.2f;
		m_Turn = false;
		m_TurnDelay = 0.2f;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Rotation", m_Rotation);
	}

	public override void Load(JSONNode Node)
	{
		m_Rotation = JSONUtils.GetAsInt(Node, "Rotation", 0);
		SetRotation(m_Rotation);
		base.Load(Node);
	}

	public static float ConvertRotationToDegrees(int Rotation)
	{
		return (float)Rotation * 360f / (float)m_NumDirections - 90f;
	}

	public static int ConvertDegreesToRotation(float Rotation)
	{
		return (int)(Rotation * (float)m_NumDirections / 360f);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Dropped)
		{
			if ((bool)Info.m_Object && (bool)Info.m_Object.GetComponent<TileMover>())
			{
				SetRotation(Info.m_Object.GetComponent<TileMover>().m_Rotation);
			}
			else
			{
				SetRotation(0);
			}
		}
		base.SendAction(Info);
	}

	public void SetRotation(int Rotation)
	{
		m_Rotation = Rotation;
		base.transform.rotation = Quaternion.Euler(0f, ConvertRotationToDegrees(m_Rotation), 0f);
	}

	public void LookAt(TileCoord Position)
	{
		TileCoord tileCoord = Position - m_TileCoord;
		int rotation = ConvertDegreesToRotation(Mathf.Atan2(tileCoord.y, tileCoord.x) * 57.29578f);
		SetRotation(rotation);
	}

	public void Turn(int Rotation)
	{
		int num = m_Rotation + Rotation;
		if (num < 0)
		{
			num += m_NumDirections;
		}
		if (num >= m_NumDirections)
		{
			num -= m_NumDirections;
		}
		SetRotation(num);
		m_Turn = true;
		m_TurnTimer = 0f;
	}

	public virtual void MoveDirection(TileCoord Direction)
	{
		int num = -1;
		if (Direction.x == 1)
		{
			if (Direction.y == 0)
			{
				num = 0;
			}
			else if (Direction.y > 0)
			{
				num = 1;
			}
			else if (Direction.y < 0)
			{
				num = 7;
			}
		}
		else if (Direction.x == 0)
		{
			if (Direction.y > 0)
			{
				num = 2;
			}
			else if (Direction.y < 0)
			{
				num = 6;
			}
		}
		else if (Direction.x < 1)
		{
			if (Direction.y == 0)
			{
				num = 4;
			}
			else if (Direction.y > 0)
			{
				num = 3;
			}
			else if (Direction.y < 0)
			{
				num = 5;
			}
		}
		if (num != -1)
		{
			SetRotation(num);
			TileCoord tilePosition = m_TileCoord + Direction;
			m_Move = true;
			m_MoveTimer = 0f;
			m_MoveDelay = m_MoveNormalDelay;
			if ((m_Rotation & 1) == 1)
			{
				m_MoveDelay *= 1.41f;
			}
			m_MoveStartPosition = m_TileCoord.ToWorldPositionTileCentered(m_FloatsInWater);
			m_MoveEndPosition = tilePosition.ToWorldPositionTileCentered(m_FloatsInWater);
			SetTilePosition(tilePosition);
		}
	}

	public void MoveForward()
	{
		TileCoord tileCoord = m_DirectionTable[m_Rotation];
		TileCoord tilePosition = m_TileCoord + tileCoord;
		m_Move = true;
		m_MoveTimer = 0f;
		m_MoveDelay = m_MoveNormalDelay;
		if (m_Rotation % 1 == 1)
		{
			m_MoveDelay *= 1.41f;
		}
		m_MoveStartPosition = m_TileCoord.ToWorldPositionTileCentered(m_FloatsInWater);
		m_MoveEndPosition = tilePosition.ToWorldPositionTileCentered(m_FloatsInWater);
		SetTilePosition(tilePosition);
	}

	public TileCoord GetNearestAdjacentTileFromTarget(TileCoord Target)
	{
		TileCoord tileCoord = m_TileCoord;
		Vector2 vector = default(Vector2);
		vector.x = Target.x - tileCoord.x;
		vector.y = Target.y - tileCoord.y;
		vector.Normalize();
		vector.x = Mathf.Round(vector.x);
		vector.y = Mathf.Round(vector.y);
		if (vector.x == 0f && vector.y == 0f)
		{
			vector.x = 1f;
		}
		tileCoord.x += (int)vector.x;
		tileCoord.y += (int)vector.y;
		if (tileCoord.x < 0)
		{
			tileCoord.x = 0;
		}
		if (tileCoord.x >= TileManager.Instance.m_TilesWide)
		{
			tileCoord.x = TileManager.Instance.m_TilesWide - 1;
		}
		if (tileCoord.y < 0)
		{
			tileCoord.y = 0;
		}
		if (tileCoord.y >= TileManager.Instance.m_TilesHigh)
		{
			tileCoord.y = TileManager.Instance.m_TilesHigh - 1;
		}
		return tileCoord;
	}

	protected virtual void MoveEnded()
	{
	}

	protected virtual void TurnEnded()
	{
	}

	public void UpdateTurning()
	{
		if (m_Turn)
		{
			m_TurnTimer += TimeManager.Instance.m_NormalDelta;
			if (m_TurnTimer >= m_TurnDelay)
			{
				m_TurnTimer = m_TurnDelay;
				m_Turn = false;
			}
			if (!m_Turn)
			{
				TurnEnded();
			}
		}
	}

	public void UpdateMovement()
	{
		if (m_Move)
		{
			float moveTimer = m_MoveTimer;
			m_MoveTimer += TimeManager.Instance.m_NormalDelta;
			if (m_MoveTimer >= m_MoveDelay)
			{
				m_MoveTimer = m_MoveDelay;
				m_Move = false;
			}
			float num = m_MoveTimer / m_MoveDelay;
			Vector3 position = (m_MoveEndPosition - m_MoveStartPosition) * num + m_MoveStartPosition;
			base.transform.position = position;
			if (moveTimer < m_MoveDelay * 0.5f && m_MoveTimer >= m_MoveDelay * 0.5f)
			{
				Tile tile = TileManager.Instance.GetTile(m_TileCoord);
				if ((bool)tile.m_AssociatedObject)
				{
					tile.m_AssociatedObject.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Bump, default(TileCoord)));
				}
			}
			if (!m_Move)
			{
				MoveEnded();
			}
		}
		else
		{
			MoveEnded();
		}
	}
}
