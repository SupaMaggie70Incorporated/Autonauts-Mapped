using SimpleJSON;
using UnityEngine;

public class TrainTrackStop : Building
{
	private Transform m_Sign;

	private bool m_SignFlash;

	private float m_SignFlashTimer;

	private MeshRenderer m_SignMaterial;

	private bool m_EngagedWithTrack;

	public bool m_PlayerStop;

	private TrackIndicator m_TrackIndicator;

	private Material m_TrackIndicatorMaterial;

	public static bool GetIsTypeTrainTrackStop(ObjectType NewType)
	{
		if (NewType == ObjectType.TrainTrackStop || NewType == ObjectType.TrainRefuellingStation)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		HideAccessModel();
		m_PlayerStop = false;
		UpdateTrackIndicator();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Sign = m_ModelRoot.transform.Find("Sign");
		if ((bool)m_Sign)
		{
			m_SignMaterial = m_Sign.GetComponent<MeshRenderer>();
		}
		GameObject original = (GameObject)Resources.Load("Prefabs/TrackIndicator", typeof(GameObject));
		m_TrackIndicator = Object.Instantiate(original, null).GetComponent<TrackIndicator>();
		float num = -0.1f * Tile.m_Size * 1f;
		m_TrackIndicator.transform.localScale = new Vector3(num, 1f, num);
		m_TrackIndicator.SetParent(base.transform);
		m_TrackIndicator.SetPosition(new Vector3(0f, 0.75f, 0f - Tile.m_Size));
		m_TrackIndicatorMaterial = m_TrackIndicator.GetComponent<MeshRenderer>().material;
		m_TrackIndicator.gameObject.SetActive(value: false);
	}

	protected new void OnDestroy()
	{
		if ((bool)m_TrackIndicator)
		{
			Object.Destroy(m_TrackIndicator.gameObject);
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
		JSONUtils.Set(Node, "Stop", m_PlayerStop);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_PlayerStop = JSONUtils.GetAsBool(Node, "Stop", DefaultValue: false);
		UpdateTrackIndicator();
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if (Info.m_Action == ActionType.Refresh || Info.m_Action == ActionType.RefreshFirst)
		{
			UpdateAdjacentTrainTrack();
		}
	}

	private TrainTrack GetAdjacentTrack()
	{
		TileCoord tileCoord = new TileCoord(0, 1);
		tileCoord.Rotate(m_Rotation);
		TileCoord position = m_TileCoord + tileCoord;
		Tile tile = TileManager.Instance.GetTile(position);
		if (tile != null && tile.m_Floor != null && (tile.m_Floor.m_TypeIdentifier == ObjectType.TrainTrack || tile.m_Floor.m_TypeIdentifier == ObjectType.TrainTrackBridge))
		{
			return tile.m_Floor.GetComponent<TrainTrack>();
		}
		return null;
	}

	private void UpdateAdjacentTrainTrack()
	{
		TrainTrack adjacentTrack = GetAdjacentTrack();
		if ((bool)adjacentTrack)
		{
			adjacentTrack.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
			if ((bool)m_Sign)
			{
				m_Sign.localRotation = Quaternion.Euler(-90f, 0f, 180f);
				m_SignFlash = false;
				m_SignMaterial.sharedMaterial = MaterialManager.Instance.m_Material;
			}
		}
		else if ((bool)m_Sign)
		{
			m_Sign.localRotation = Quaternion.Euler(0f, 0f, -55f) * Quaternion.Euler(-90f, 0f, 180f);
			m_SignFlash = true;
		}
	}

	private void UpdateSign()
	{
		if ((bool)m_Sign && m_SignFlash)
		{
			m_SignFlashTimer += TimeManager.Instance.m_NormalDelta;
			Material sharedMaterial = MaterialManager.Instance.m_Material;
			if ((int)(m_SignFlashTimer * 60f) % 12 < 6)
			{
				sharedMaterial = MaterialManager.Instance.m_MaterialPulleyOn;
			}
			m_SignMaterial.sharedMaterial = sharedMaterial;
		}
	}

	private void UpdateTrackIndicator()
	{
		string text = "TrackStopGo";
		if (m_PlayerStop)
		{
			text = "TrackStopStop";
		}
		Texture2D value = (Texture2D)Resources.Load("Textures/" + text, typeof(Texture2D));
		m_TrackIndicatorMaterial.SetTexture("_MainTex", value);
	}

	public void TogglePlayerStop()
	{
		m_PlayerStop = !m_PlayerStop;
		UpdateTrackIndicator();
	}

	public void ShowPlayerStop(bool Show)
	{
		m_TrackIndicator.gameObject.SetActive(Show);
	}

	protected void Update()
	{
		UpdateSign();
	}
}
