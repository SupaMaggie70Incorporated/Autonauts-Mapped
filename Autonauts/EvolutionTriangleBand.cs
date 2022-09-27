using UnityEngine;
using UnityEngine.UI;

public class EvolutionTriangleBand : MaskableGraphic
{
	[SerializeField]
	public Color m_Colour2;

	[SerializeField]
	public Texture m_Texture;

	public static float m_Slope = 0.5f;

	private Wobbler m_Wobbler;

	private float m_AppearTimer;

	private static float m_AppearDelay = 0.125f;

	private float m_AppearX;

	public Texture texture
	{
		get
		{
			return m_Texture;
		}
		set
		{
			if (!(m_Texture == value))
			{
				m_Texture = value;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (!(m_Texture == null))
			{
				return m_Texture;
			}
			return Graphic.s_WhiteTexture;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		SetVerticesDirty();
		SetMaterialDirty();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		Vector2 vector = new Vector2(0f, 0f) - base.rectTransform.pivot;
		vector.x *= base.rectTransform.rect.width;
		vector.y *= base.rectTransform.rect.height;
		Vector2 vector2 = new Vector2(base.rectTransform.rect.xMin, base.rectTransform.rect.yMin);
		Vector2 vector3 = new Vector2(base.rectTransform.rect.xMax, base.rectTransform.rect.yMax);
		float num = base.rectTransform.rect.yMax - base.rectTransform.rect.yMin;
		float num2 = base.rectTransform.rect.xMax - base.rectTransform.rect.xMin;
		float num3 = num2 - num * m_Slope * 2f;
		Vector2 vector4 = new Vector2(vector3.x - num2 / 2f, vector3.y);
		int currentVertCount = vh.currentVertCount;
		UIVertex v = default(UIVertex);
		v.color = color;
		v.position = vector2;
		v.uv0 = default(Vector2);
		vh.AddVert(v);
		v.position = vector2 + new Vector2(num2, 0f);
		vh.AddVert(v);
		v.position = vector4 + new Vector2(num3 / 2f, 0f);
		vh.AddVert(v);
		v.position = vector4 + new Vector2((0f - num3) / 2f, 0f);
		vh.AddVert(v);
		vh.AddTriangle(currentVertCount, currentVertCount + 2, currentVertCount + 1);
		vh.AddTriangle(currentVertCount + 3, currentVertCount + 2, currentVertCount);
		currentVertCount = vh.currentVertCount;
		v = default(UIVertex);
		v.color = m_Colour2;
		v.position = vector2 + new Vector2(num2, 0f);
		v.uv0 = default(Vector2);
		vh.AddVert(v);
		v.position = vector2 + new Vector2(2000f, 0f);
		vh.AddVert(v);
		v.position = vector4 + new Vector2(2000f, 0f);
		vh.AddVert(v);
		v.position = vector4 + new Vector2(num3 / 2f, 0f);
		vh.AddVert(v);
		vh.AddTriangle(currentVertCount, currentVertCount + 2, currentVertCount + 1);
		vh.AddTriangle(currentVertCount + 3, currentVertCount + 2, currentVertCount);
	}

	public void SetColour(Color NewColour)
	{
		color = NewColour;
	}

	public Color GetColour()
	{
		return color;
	}

	public void SetTitleVisible(bool Visible)
	{
		base.transform.Find("Title").GetComponent<BaseText>().SetActive(Visible);
		base.transform.Find("Image").GetComponent<BaseImage>().SetActive(Visible);
	}

	public void SetTitle(string Title)
	{
		base.transform.Find("Title").GetComponent<BaseText>().SetText(Title);
	}

	public void SetImage(string Image)
	{
		base.transform.Find("Image").GetComponent<BaseImage>().SetSprite(Image);
	}

	public void StartAppear()
	{
		m_AppearTimer = m_AppearDelay;
		m_AppearX = base.transform.localPosition.x;
		m_Wobbler.Restart();
		m_Wobbler.m_WobbleWhilePaused = true;
		UpdateAppearPosition();
	}

	private void UpdateAppearPosition()
	{
		float num = m_AppearTimer / m_AppearDelay * HudManager.Instance.m_HalfScaledWidth;
		float num2 = m_AppearX + num;
		m_Wobbler.Update();
		num2 += m_Wobbler.m_Height * 100f;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = num2;
		base.transform.localPosition = localPosition;
	}

	private void Update()
	{
		if (m_AppearTimer > 0f)
		{
			m_AppearTimer -= TimeManager.Instance.m_PauseDelta;
			if (m_AppearTimer <= 0f)
			{
				m_AppearTimer = 0f;
				m_Wobbler.Go(0.5f, 2f, 1f);
			}
		}
		if (m_Wobbler.m_Wobbling)
		{
			m_Wobbler.Update();
		}
		UpdateAppearPosition();
	}
}
