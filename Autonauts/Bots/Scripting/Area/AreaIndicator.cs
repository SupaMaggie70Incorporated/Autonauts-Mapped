using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class AreaIndicator : MonoBehaviour
{
	private static float m_DetectEdge = 0.15f;

	public HighInstruction m_Instruction;

	public BaseClass m_Sign;

	public TileCoord m_TopLeft;

	public TileCoord m_BottomRight;

	public HighInstruction.FindType m_FindType;

	private GameObject m_Outline;

	private Material m_OutlineMaterial;

	private float m_OutlineTimer;

	private GameObject m_Anchor;

	private Material m_AnchorMaterial;

	private TextMeshPro m_Name;

	private TextMeshPro m_Dimensions;

	private Wobbler m_Wobbler;

	private bool m_ScaleUp;

	private bool m_BeingEdited;

	private bool m_Active;

	public bool m_AnchorActive;

	public float m_AnchorX;

	public float m_AnchorY;

	public Vector3 m_Scale;

	private Material m_Material;

	private Color m_FillColour;

	private void Awake()
	{
		m_Outline = base.transform.Find("Outline").gameObject;
		m_OutlineMaterial = m_Outline.GetComponent<MeshRenderer>().material;
		m_Anchor = base.transform.Find("Anchor").gameObject;
		m_AnchorMaterial = m_Anchor.GetComponent<MeshRenderer>().material;
		m_Anchor.gameObject.SetActive(value: false);
		m_Name = base.transform.Find("Name").GetComponent<TextMeshPro>();
		m_Name.gameObject.SetActive(value: false);
		m_Dimensions = base.transform.Find("Dimensions").GetComponent<TextMeshPro>();
		m_Dimensions.gameObject.SetActive(value: false);
		m_FillColour = new Color(1f, 1f, 1f, 1f);
		MeshRenderer component = GetComponent<MeshRenderer>();
		m_Material = component.material;
		m_Material.renderQueue = 2999;
		m_Wobbler = new Wobbler();
		Restart();
	}

	public void Restart()
	{
		m_Wobbler.Restart();
		m_Instruction = null;
		m_Sign = null;
		m_ScaleUp = true;
		m_BeingEdited = false;
		m_Name.gameObject.SetActive(value: false);
		m_Dimensions.gameObject.SetActive(value: false);
		SetFindType(HighInstruction.FindType.Full);
	}

	public void EnableOutline(bool Enabled)
	{
		m_Outline.SetActive(Enabled);
	}

	public void SetInstruction(HighInstruction NewInstruction)
	{
		m_Instruction = NewInstruction;
		UpdateArea();
		SetActive(Active: false);
	}

	public void SetNameEnabled(bool Enabled)
	{
		m_Name.gameObject.SetActive(Enabled);
	}

	public void SetNameString(string Name)
	{
		m_Name.text = Name;
	}

	public void SetNameColour(Color NewColour)
	{
	}

	private void UpdateNamePosition()
	{
		float num = m_Scale.x - 0.5f;
		Vector3 localPosition = new Vector3(num * -0.5f, 0f, m_Scale.z * -0.5f + 0.25f);
		m_Name.transform.localPosition = localPosition;
		Vector2 sizeDelta = m_Name.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = num;
		m_Name.GetComponent<RectTransform>().sizeDelta = sizeDelta;
	}

	public void SetDimensionsEnabled(bool Enabled)
	{
		m_Dimensions.gameObject.SetActive(Enabled);
	}

	private void UpdateDimensionsPosition()
	{
		float num = m_Scale.x - 0.5f;
		Vector3 localPosition = new Vector3(num * 0.5f, 0f, m_Scale.z * 0.5f - 0.25f);
		m_Dimensions.transform.localPosition = localPosition;
		Vector2 sizeDelta = m_Name.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.x = num;
		m_Dimensions.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		int num2 = m_BottomRight.x - m_TopLeft.x + 1;
		int num3 = m_BottomRight.y - m_TopLeft.y + 1;
		m_Dimensions.text = num2 + "x" + num3;
	}

	public void SetSign(BaseClass NewSign)
	{
		m_Sign = NewSign;
		UpdateArea();
		SetActive(Active: false);
	}

	public void Scale(bool Up)
	{
		m_ScaleUp = Up;
		float time = 0.3f;
		if (!Up)
		{
			time = 0.2f;
		}
		m_Wobbler.Go(time, 1f, 1f);
	}

	public void CancelScale()
	{
		if (m_Wobbler.m_Wobbling)
		{
			m_ScaleUp = !m_ScaleUp;
			m_Wobbler.m_Wobbling = false;
		}
	}

	public void SetVisible(bool Visible)
	{
		base.gameObject.SetActive(Visible);
		m_ScaleUp = Visible;
		m_Wobbler.m_Wobbling = false;
		UpdateArea();
	}

	public void SetUsed(bool Used)
	{
		SetColour(new Color(1f, 1f, 1f, 1f));
		SetFillColour(new Color(1f, 1f, 1f, 1f));
		base.gameObject.SetActive(Used);
	}

	public void SetBeingEdited(bool Edited)
	{
		m_BeingEdited = Edited;
		if (m_BeingEdited)
		{
			GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
		}
		else
		{
			GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
		}
	}

	public void SetActive(bool Active)
	{
		m_Active = Active;
		if (!Active)
		{
			SetAnchor(Active: false, 0f, 0f);
		}
		MeshRenderer component = m_Outline.GetComponent<MeshRenderer>();
		string text = "AreaIndicatorOutline";
		if (m_Active)
		{
			text = "AreaIndicatorOutlineSelected";
		}
		Material material2 = (component.material = (Material)Resources.Load("Materials/" + text, typeof(Material)));
		m_OutlineMaterial = component.material;
		m_OutlineMaterial.color = GeneralUtils.GetIndicatorColour();
	}

	public void SetAnchorFromPosition(Vector3 Position)
	{
		Vector3 position = base.transform.position;
		position.x -= m_Scale.x * 0.5f;
		position.z += m_Scale.z * 0.5f;
		Vector3 vector = position;
		vector.x += m_Scale.x;
		vector.z -= m_Scale.z;
		float num = (vector.x - position.x) * m_DetectEdge;
		float num2 = (vector.z - position.z) * m_DetectEdge;
		if (Position.x > position.x - num && Position.x < vector.x + num && Position.z < position.z - num2 && Position.z > vector.z + num2)
		{
			float x = 0.5f;
			float y = 0.5f;
			if (Position.z > position.z + num2)
			{
				y = 0f;
			}
			else if (Position.z < vector.z - num2)
			{
				y = 1f;
			}
			if (Position.x < position.x + num)
			{
				x = 0f;
			}
			else if (Position.x > vector.x - num)
			{
				x = 1f;
			}
			SetAnchor(Active: true, x, y);
		}
		else
		{
			SetAnchor(Active: false, 0f, 0f);
		}
	}

	public void SetAnchor(bool Active, float x, float y)
	{
		m_AnchorActive = Active;
		m_AnchorX = x;
		m_AnchorY = y;
		m_Anchor.gameObject.SetActive(Active);
		UpdateAnchorPosition();
		string text = "AreaIndicatorAnchor";
		if (x == 0.5f && y == 0.5f)
		{
			text = "AreaIndicatorMove";
		}
		Texture value = (Texture)Resources.Load("Textures/" + text, typeof(Texture));
		m_AnchorMaterial.SetTexture("_MainTex", value);
		m_AnchorMaterial.SetTexture("_EmissionMap", value);
		m_AnchorMaterial.color = GeneralUtils.GetIndicatorColour();
		m_AnchorMaterial.SetColor("_EmissionColor", GeneralUtils.GetIndicatorColour());
		float num = 1f;
		if (x == 0.5f && y == 0.5f)
		{
			num = 2f;
		}
		int num2 = m_BottomRight.x - m_TopLeft.x;
		int num3 = m_BottomRight.y - m_TopLeft.y;
		if (num2 > 1 && num3 > 1)
		{
			num *= 2f;
		}
		m_Anchor.transform.localScale = new Vector3(num, num, num);
	}

	public void SetColour(Color NewColour)
	{
		m_OutlineMaterial.color = NewColour * GeneralUtils.GetIndicatorColour();
		m_AnchorMaterial.color = NewColour * GeneralUtils.GetIndicatorColour();
	}

	public void SetFillColour(Color NewColour)
	{
		m_FillColour = NewColour;
		UpdateFillColour();
	}

	private void UpdateFillColour()
	{
		float a = m_Material.color.a;
		Color color = m_FillColour * GeneralUtils.GetIndicatorColour();
		color.a = a;
		m_Material.color = color;
	}

	private void UpdateAnchorPosition()
	{
		m_Anchor.transform.localPosition = new Vector3(m_Scale.x * (m_AnchorX - 0.5f), 0.01f, (0f - m_Scale.z) * (m_AnchorY - 0.5f));
	}

	public void UpdateArea()
	{
		if ((bool)m_Sign)
		{
			if (Sign.GetIsTypeSign(m_Sign.m_TypeIdentifier))
			{
				m_TopLeft = m_Sign.GetComponent<Sign>().m_TopLeft;
				m_BottomRight = m_Sign.GetComponent<Sign>().m_BottomRight;
			}
			if ((bool)m_Sign.GetComponent<Converter>())
			{
				m_BottomRight = (m_TopLeft = m_Sign.GetComponent<Converter>().GetSpawnPoint());
			}
		}
		if (m_Instruction != null)
		{
			char[] separator = new char[1] { ' ' };
			string[] array = m_Instruction.m_Argument.Split(separator);
			m_FindType = HighInstruction.FindType.Full;
			if (array.Length > 4)
			{
				m_FindType = HighInstruction.GetFindTypeFromName(array[4]);
			}
			if (m_Sign == null)
			{
				int nx = int.Parse(array[0]);
				int ny = int.Parse(array[1]);
				int nx2 = int.Parse(array[2]);
				int ny2 = int.Parse(array[3]);
				m_TopLeft = new TileCoord(nx, ny);
				m_BottomRight = new TileCoord(nx2, ny2);
			}
		}
		Vector3 position = (m_TopLeft.ToWorldPositionTileCentered() + m_BottomRight.ToWorldPositionTileCentered()) / 2f;
		if (m_BeingEdited)
		{
			position.y = 1f;
		}
		else
		{
			position.y = 0.25f;
		}
		base.transform.position = position;
		int num = m_BottomRight.x - m_TopLeft.x + 1;
		int num2 = m_BottomRight.y - m_TopLeft.y + 1;
		m_Scale = new Vector3((float)num * Tile.m_Size, 1f, (float)num2 * Tile.m_Size);
		if (m_Wobbler.m_Wobbling)
		{
			if (m_ScaleUp)
			{
				m_Scale *= 1f - m_Wobbler.m_Height;
			}
			else
			{
				m_Scale *= 1f - m_Wobbler.m_Timer / m_Wobbler.m_WobbleTime;
			}
		}
		else if (!m_ScaleUp)
		{
			m_Scale *= 0f;
		}
		UpdateBaseMesh(m_Scale);
		UpdateOutlineMesh(m_Scale);
		UpdateFindType();
		UpdateNamePosition();
		UpdateDimensionsPosition();
	}

	private void UpdateFindType()
	{
		TileCoord tileCoord = m_BottomRight - m_TopLeft + new TileCoord(1, 1);
		m_Material.SetTextureScale("_MainTex", new Vector2((float)tileCoord.x / 2f, 0f - (float)tileCoord.y / 2f));
		m_Material.SetTextureOffset("_MainTex", new Vector2(m_TopLeft.x, -m_TopLeft.y));
		string text = "AreaIndicatorIdle" + m_FindType;
		Texture2D mainTexture = (Texture2D)Resources.Load("Textures/" + text, typeof(Texture2D));
		m_Material.mainTexture = mainTexture;
		UpdateFillColour();
	}

	private void AddUV(Vector2[] uv, int i, float Width)
	{
		uv[i] = new Vector2(0f, 0f);
		uv[i + 1] = new Vector2(Width, 0f);
		uv[i + 2] = new Vector2(0f, 1f);
		uv[i + 3] = new Vector2(Width, 1f);
	}

	private void UpdateBaseMesh(Vector3 Scale)
	{
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] uv = new Vector2[num];
		float x = Scale.x;
		float z = Scale.z;
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(x, 0f, 0f);
		array[2] = new Vector3(0f, 0f, 0f - z);
		array[3] = new Vector3(x, 0f, 0f - z);
		for (int i = 0; i < num; i++)
		{
			array[i].x -= Scale.x / 2f;
			array[i].z += Scale.z / 2f;
		}
		AddUV(uv, 0, 1f);
		Vector3[] array2 = new Vector3[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = Vector3.up;
		}
		int num2 = 6;
		int[] array3 = new int[num2];
		for (int k = 0; k < num2 / 6; k++)
		{
			array3[k * 6] = k * 4;
			array3[k * 6 + 1] = k * 4 + 1;
			array3[k * 6 + 2] = k * 4 + 2;
			array3[k * 6 + 3] = k * 4 + 1;
			array3[k * 6 + 4] = k * 4 + 3;
			array3[k * 6 + 5] = k * 4 + 2;
		}
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.triangles = null;
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = uv;
		mesh.triangles = array3;
		mesh.RecalculateBounds();
	}

	private void UpdateOutlineMesh(Vector3 Scale)
	{
		int num = 16;
		Vector3[] array = new Vector3[num];
		Vector2[] uv = new Vector2[num];
		float num2 = 0.2f;
		float x = Scale.x;
		float z = Scale.z;
		float num3 = z - num2 * 2f;
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(x, 0f, 0f);
		array[2] = new Vector3(0f, 0f, 0f - num2);
		array[3] = new Vector3(x, 0f, 0f - num2);
		array[4] = new Vector3(x, 0f, 0f - num2);
		array[5] = new Vector3(x, 0f, 0f - (num3 + num2));
		array[6] = new Vector3(x - num2, 0f, 0f - num2);
		array[7] = new Vector3(x - num2, 0f, 0f - (num3 + num2));
		array[8] = new Vector3(x, 0f, 0f - z);
		array[9] = new Vector3(0f, 0f, 0f - z);
		array[10] = new Vector3(x, 0f, 0f - (z - num2));
		array[11] = new Vector3(0f, 0f, 0f - (z - num2));
		array[12] = new Vector3(0f, 0f, 0f - (z - num2));
		array[13] = new Vector3(0f, 0f, 0f - num2);
		array[14] = new Vector3(num2, 0f, 0f - (z - num2));
		array[15] = new Vector3(num2, 0f, 0f - num2);
		for (int i = 0; i < num; i++)
		{
			array[i].x -= Scale.x / 2f;
			array[i].z += Scale.z / 2f;
		}
		AddUV(uv, 0, x);
		AddUV(uv, 4, num3);
		AddUV(uv, 8, x);
		AddUV(uv, 12, num3);
		Vector3[] array2 = new Vector3[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = Vector3.up;
		}
		int num4 = 24;
		int[] array3 = new int[num4];
		for (int k = 0; k < num4 / 6; k++)
		{
			array3[k * 6] = k * 4;
			array3[k * 6 + 1] = k * 4 + 1;
			array3[k * 6 + 2] = k * 4 + 2;
			array3[k * 6 + 3] = k * 4 + 1;
			array3[k * 6 + 4] = k * 4 + 3;
			array3[k * 6 + 5] = k * 4 + 2;
		}
		Mesh mesh = m_Outline.GetComponent<MeshFilter>().mesh;
		mesh.triangles = null;
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = uv;
		mesh.triangles = array3;
		mesh.RecalculateBounds();
	}

	public void SetFindType(HighInstruction.FindType NewFindType)
	{
		m_FindType = NewFindType;
		if (m_Instruction != null)
		{
			m_Instruction.SetFindNearestArea(m_TopLeft, m_BottomRight, m_FindType);
		}
		UpdateFindType();
	}

	public void SetCoords(TileCoord TopLeft, TileCoord BottomRight)
	{
		m_TopLeft = TopLeft;
		m_BottomRight = BottomRight;
		if (m_Sign != null)
		{
			if (Sign.GetIsTypeSign(m_Sign.m_TypeIdentifier))
			{
				m_Sign.GetComponent<Sign>().SetArea(TopLeft, BottomRight);
			}
		}
		else if (m_Instruction != null)
		{
			m_Instruction.SetFindNearestArea(TopLeft, BottomRight, m_FindType);
		}
		UpdateArea();
		UpdateAnchorPosition();
	}

	public void SetScale(Vector3 Scale)
	{
		m_Scale = Scale;
		UpdateBaseMesh(m_Scale);
		UpdateOutlineMesh(m_Scale);
	}

	private void UpdateOutline()
	{
		m_OutlineTimer += TimeManager.Instance.m_NormalDelta * 4f;
		Vector2 mainTextureOffset = new Vector2(m_OutlineTimer, 0f);
		m_OutlineMaterial.mainTextureOffset = mainTextureOffset;
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		if (m_Wobbler.m_Wobbling)
		{
			m_Wobbler.Update();
			UpdateArea();
			if (!m_Wobbler.m_Wobbling && !m_ScaleUp)
			{
				AreaIndicatorManager.Instance.Remove(this);
			}
		}
		if (m_Active)
		{
			UpdateOutline();
		}
	}
}
