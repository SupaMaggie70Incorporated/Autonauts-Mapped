using SimpleJSON;
using UnityEngine;
using UnityEngine.Rendering;

public class Sign : Holdable
{
	public enum State
	{
		Empty,
		Full,
		Total
	}

	[HideInInspector]
	public State m_State;

	[HideInInspector]
	public string m_SignName;

	[HideInInspector]
	public int m_MaxSize;

	private RenderTexture m_RenderTexture;

	private Texture2D m_FinalTexture;

	public AreaIndicator m_Indicator;

	public TileCoord m_TopLeft;

	public TileCoord m_BottomRight;

	private bool m_CoordsSet;

	private bool m_BeingHeldByPlayer;

	public bool m_AreaLinkedToPosition;

	private bool m_KeepIndicator;

	public static bool GetIsTypeSign(ObjectType NewType)
	{
		if (NewType != ObjectType.Sign && NewType != ObjectType.Sign2 && NewType != ObjectType.Billboard)
		{
			return NewType == ObjectType.Sign3;
		}
		return true;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Sign", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Sign", this);
		}
		m_State = State.Empty;
		m_SignName = "";
		GameObject gameObject = m_ModelRoot.transform.Find("Text").gameObject;
		gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
		if (m_RenderTexture == null)
		{
			Bounds bounds = gameObject.GetComponent<MeshRenderer>().bounds;
			int width = (int)(64f * bounds.size.x);
			int height = (int)(64f * bounds.size.y);
			m_RenderTexture = new RenderTexture(width, height, 32);
			m_FinalTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
			m_FinalTexture.wrapMode = TextureWrapMode.Clamp;
			gameObject.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", m_FinalTexture);
		}
		if ((bool)m_ModelRoot.transform.Find("Text2"))
		{
			gameObject = m_ModelRoot.transform.Find("Text2").gameObject;
			gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
			gameObject.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", m_FinalTexture);
		}
		UpdateTexture();
		m_CoordsSet = false;
		m_BeingHeldByPlayer = false;
		m_AreaLinkedToPosition = true;
		m_MaxSize = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Range");
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		ShowIndicator(Show: false);
		if (m_RenderTexture != null)
		{
			Object.Destroy(m_RenderTexture);
		}
		if (m_FinalTexture != null)
		{
			Object.Destroy(m_FinalTexture);
		}
	}

	public override string GetHumanReadableName()
	{
		return base.GetHumanReadableName();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Name", m_SignName);
		JSONUtils.Set(Node, "R", (int)base.transform.localRotation.eulerAngles.y);
		m_TopLeft.Save(Node, "TL");
		m_BottomRight.Save(Node, "BR");
		JSONUtils.Set(Node, "Linked", m_AreaLinkedToPosition);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Sign", this);
		SetName(JSONUtils.GetAsString(Node, "Name", ""));
		base.transform.localRotation = Quaternion.Euler(0f, JSONUtils.GetAsInt(Node, "R", 0), 0f);
		m_TopLeft.Load(Node, "TL");
		m_BottomRight.Load(Node, "BR");
		JSONUtils.GetAsString(Node, "FT", HighInstruction.FindType.Full.ToString());
		m_CoordsSet = true;
		m_AreaLinkedToPosition = JSONUtils.GetAsBool(Node, "Linked", DefaultValue: false);
	}

	public override void SetHighlight(bool Highlighted)
	{
		base.SetHighlight(Highlighted);
		if (GameStateManager.Instance.GetCurrentState().m_BaseState != GameStateManager.State.Edit)
		{
			UpdateIndicator();
		}
	}

	public void KeepIndicator(bool Keep)
	{
		m_KeepIndicator = Keep;
		UpdateIndicator();
	}

	private void UpdateIndicator()
	{
		if (m_BeingHeldByPlayer || m_Highlighted || m_KeepIndicator)
		{
			ShowIndicator(Show: true);
		}
		else
		{
			ShowIndicator(Show: false);
		}
	}

	public void SetBeingHeldByPlayer(bool Held)
	{
		m_BeingHeldByPlayer = Held;
		UpdateIndicator();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		CheckCoordChanged(Holder.GetComponent<TileCoordObject>().m_TileCoord);
	}

	public override void SendAction(ActionInfo Info)
	{
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged)
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			if ((bool)m_Engager.GetComponent<FarmerPlayer>())
			{
				GameStateManager.Instance.StartRenameSign(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		}
		base.SendAction(Info);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		return GetActionFromCurrentState(Info, "Sign", m_State.ToString());
	}

	private void UpdateTexture()
	{
		Camera renderCamera = HudManager.Instance.m_RenderCamera;
		renderCamera.gameObject.SetActive(value: true);
		renderCamera.targetTexture = m_RenderTexture;
		RenderTexture.active = m_RenderTexture;
		BaseText component = Object.Instantiate((GameObject)Resources.Load("Prefabs/WorldObjects/Other/SignText", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_RenderCanvasRootTransform).GetComponent<BaseText>();
		component.transform.localPosition = new Vector3(0f, 0f, 0f);
		if (m_SignName == "")
		{
			component.SetText("(" + TextManager.Instance.Get("SignBlank") + ")");
		}
		else
		{
			component.SetText(m_SignName);
		}
		Vector2 sizeDelta = HudManager.Instance.m_RenderCanvasRootTransform.GetComponent<RectTransform>().sizeDelta;
		float num2 = (renderCamera.aspect = (float)m_RenderTexture.width / (float)m_RenderTexture.height);
		component.GetComponent<RectTransform>().sizeDelta = new Vector2(num2 * sizeDelta.y, sizeDelta.y);
		renderCamera.clearFlags = CameraClearFlags.Color;
		renderCamera.backgroundColor = new Color(1f, 1f, 1f, 0f);
		renderCamera.Render();
		m_FinalTexture.ReadPixels(new Rect(0f, 0f, m_RenderTexture.width, m_RenderTexture.height), 0, 0);
		m_FinalTexture.Apply();
		Object.Destroy(component.gameObject);
		renderCamera.targetTexture = null;
		RenderTexture.active = null;
		renderCamera.gameObject.SetActive(value: false);
	}

	public void SetName(string NewName)
	{
		m_SignName = NewName;
		m_State = State.Full;
		UpdateTexture();
	}

	public void ShowIndicator(bool Show)
	{
		if (Show)
		{
			if (!m_Indicator)
			{
				m_Indicator = AreaIndicatorManager.Instance.Add();
				m_Indicator.SetSign(this);
				m_Indicator.SetActive(Active: true);
				m_Indicator.Scale(Up: true);
			}
			else
			{
				m_Indicator.SetActive(Active: true);
				m_Indicator.Scale(Up: true);
			}
		}
		else if ((bool)m_Indicator)
		{
			m_Indicator.Scale(Up: false);
			m_Indicator = null;
			m_KeepIndicator = false;
		}
	}

	public void SetArea(TileCoord TopLeft, TileCoord BottomRight)
	{
		if (TopLeft.x < 0)
		{
			TopLeft.x = 0;
		}
		if (TopLeft.y < 0)
		{
			TopLeft.y = 0;
		}
		if (BottomRight.x >= TileManager.Instance.m_TilesWide)
		{
			BottomRight.x = TileManager.Instance.m_TilesWide - 1;
		}
		if (BottomRight.y >= TileManager.Instance.m_TilesHigh)
		{
			BottomRight.y = TileManager.Instance.m_TilesHigh - 1;
		}
		m_TopLeft = TopLeft;
		m_BottomRight = BottomRight;
		m_CoordsSet = true;
	}

	public void UpdateCoordChanged(TileCoord NewPosition, TileCoord OldPosition)
	{
		if (!m_AreaLinkedToPosition)
		{
			return;
		}
		TileCoord tileCoord = NewPosition - OldPosition;
		TileCoord topLeft = m_TopLeft + tileCoord;
		TileCoord bottomRight = m_BottomRight + tileCoord;
		if (topLeft.x < 0 || bottomRight.x >= TileManager.Instance.m_TilesWide || topLeft.y < 0 || bottomRight.y >= TileManager.Instance.m_TilesHigh)
		{
			return;
		}
		for (int i = topLeft.y; i <= bottomRight.y; i++)
		{
			for (int j = topLeft.x; j <= bottomRight.x; j++)
			{
				if (!PlotManager.Instance.GetPlotAtTile(new TileCoord(j, i)).m_Visible)
				{
					return;
				}
			}
		}
		m_TopLeft = topLeft;
		m_BottomRight = bottomRight;
		if ((bool)m_Indicator)
		{
			m_Indicator.UpdateArea();
		}
	}

	public void CheckCoordChanged(TileCoord NewPosition)
	{
		PositionChanged(NewPosition, m_TileCoord);
	}

	private void PositionChanged(TileCoord Position, TileCoord OldCoord)
	{
		m_TileCoord = Position;
		if (!m_CoordsSet)
		{
			m_CoordsSet = true;
			m_TopLeft = m_TileCoord;
			m_BottomRight = m_TileCoord;
		}
		else
		{
			UpdateCoordChanged(Position, OldCoord);
		}
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		_ = m_TileCoord;
		base.TileCoordChanged(Position);
		PositionChanged(Position, m_TileCoord);
	}
}
