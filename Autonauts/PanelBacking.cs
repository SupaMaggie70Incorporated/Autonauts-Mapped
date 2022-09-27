using UnityEngine;
using UnityEngine.UI;

public class PanelBacking : MaskableGraphic
{
	[SerializeField]
	public Texture m_Texture;

	[SerializeField]
	public Texture m_Texture2;

	[SerializeField]
	private float m_BorderSize = 1f;

	[SerializeField]
	private float m_BorderScale = 1f;

	public float m_UVScale = 1f;

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

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		SetVerticesDirty();
		SetMaterialDirty();
	}

	private void AddQuad(VertexHelper vh, Vector2 corner1, Vector2 corner2, Vector2 uvCorner1, Vector2 uvCorner2, float ColourPercent, float ColourPercent2, float ColourPercent3, float ColourPercent4)
	{
		int currentVertCount = vh.currentVertCount;
		UIVertex v = default(UIVertex);
		v.color = color;
		v.position = corner1;
		v.uv0 = uvCorner1;
		v.uv1 = new Vector2(ColourPercent3, ColourPercent);
		vh.AddVert(v);
		v.position = new Vector2(corner2.x, corner1.y);
		v.uv0 = new Vector2(uvCorner2.x, uvCorner1.y);
		v.uv1 = new Vector2(ColourPercent4, ColourPercent);
		vh.AddVert(v);
		v.position = corner2;
		v.uv0 = uvCorner2;
		v.uv1 = new Vector2(ColourPercent4, ColourPercent2);
		vh.AddVert(v);
		v.position = new Vector2(corner1.x, corner2.y);
		v.uv0 = new Vector2(uvCorner1.x, uvCorner2.y);
		v.uv1 = new Vector2(ColourPercent3, ColourPercent2);
		vh.AddVert(v);
		vh.AddTriangle(currentVertCount, currentVertCount + 2, currentVertCount + 1);
		vh.AddTriangle(currentVertCount + 3, currentVertCount + 2, currentVertCount);
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		Vector2 vector = new Vector2(0f, 0f) - base.rectTransform.pivot;
		vector.x *= base.rectTransform.rect.width;
		vector.y *= base.rectTransform.rect.height;
		Vector2 vector2 = new Vector2(base.rectTransform.rect.xMin, base.rectTransform.rect.yMin);
		new Vector2(base.rectTransform.rect.xMax, base.rectTransform.rect.yMax);
		float num = m_BorderSize / (float)m_Texture.width;
		float num2 = m_BorderSize * m_BorderScale;
		if (num2 > base.rectTransform.rect.width * 0.5f)
		{
			num2 = base.rectTransform.rect.width * 0.5f;
		}
		float num3 = base.rectTransform.rect.width - num2 * 2f;
		float num4 = base.rectTransform.rect.height - num2 * 2f;
		float num5 = 1f - num * 2f;
		float num6 = num2 / base.rectTransform.rect.width * m_UVScale;
		float num7 = (1f - num6) * m_UVScale;
		AddQuad(vh, vector2, vector2 + new Vector2(num2, num2), new Vector2(0f, 0f), new Vector2(num, num), 0f, num6, 0f, num6);
		AddQuad(vh, vector2 + new Vector2(num2, 0f), vector2 + new Vector2(num2 + num3, num2), new Vector2(num, 0f), new Vector2(num + num5, num), 0f, num6, num6, num7);
		AddQuad(vh, vector2 + new Vector2(num2 + num3, 0f), vector2 + new Vector2(base.rectTransform.rect.width, num2), new Vector2(num + num5, 0f), new Vector2(1f, num), 0f, num6, num7, 1f);
		AddQuad(vh, vector2 + new Vector2(0f, num2), vector2 + new Vector2(num2, num2 + num4), new Vector2(0f, num), new Vector2(num, num + num5), num6, num7, 0f, num6);
		AddQuad(vh, vector2 + new Vector2(num2, num2), vector2 + new Vector2(num2 + num3, num2 + num4), new Vector2(num, num), new Vector2(num + num5, num + num5), num6, num7, num6, num7);
		AddQuad(vh, vector2 + new Vector2(num2 + num3, num2), vector2 + new Vector2(base.rectTransform.rect.width, num2 + num4), new Vector2(num + num5, num), new Vector2(1f, num + num5), num6, num7, num7, 1f);
		AddQuad(vh, vector2 + new Vector2(0f, num2 + num4), vector2 + new Vector2(num2, base.rectTransform.rect.height), new Vector2(0f, num + num5), new Vector2(num, 1f), num7, 1f, 0f, num6);
		AddQuad(vh, vector2 + new Vector2(num2, num2 + num4), vector2 + new Vector2(num2 + num3, base.rectTransform.rect.height), new Vector2(num, num + num5), new Vector2(num + num5, 1f), num7, 1f, num6, num7);
		AddQuad(vh, vector2 + new Vector2(num2 + num3, num2 + num4), vector2 + new Vector2(base.rectTransform.rect.width, base.rectTransform.rect.height), new Vector2(num + num5, num + num5), new Vector2(1f, 1f), num7, 1f, num7, 1f);
		if (m_Material == null)
		{
			Material original = (Material)Resources.Load("Materials/HUD/ColourGradient", typeof(Material));
			m_Material = Object.Instantiate(original);
			m_Material.SetTexture("_MainTex", m_Texture);
			m_Material.SetTexture("_MainTex2", m_Texture2);
		}
	}

	public void SetColour(Color NewColour)
	{
		color = NewColour;
	}

	public Color GetColour()
	{
		return color;
	}

	public void SetBackingGradient(string Name)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		m_Texture2 = sprite.texture;
		SetVerticesDirty();
		Object.Destroy(m_Material);
		m_Material = null;
		SetMaterialDirty();
	}

	public float GetWidth()
	{
		return base.rectTransform.rect.width;
	}

	public float GetHeight()
	{
		return base.rectTransform.rect.height;
	}

	public void SetSize(Vector2 Position)
	{
		base.rectTransform.sizeDelta = Position;
	}

	public void SetSize(float Width, float Height)
	{
		base.rectTransform.sizeDelta = new Vector2(Width, Height);
	}

	public void SetWidth(float Width)
	{
		base.rectTransform.sizeDelta = new Vector2(Width, GetHeight());
	}

	public void SetHeight(float Height)
	{
		base.rectTransform.sizeDelta = new Vector2(GetWidth(), Height);
	}
}
