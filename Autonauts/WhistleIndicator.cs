using UnityEngine;

public class WhistleIndicator : MonoBehaviour
{
	public static WhistleIndicator Instance;

	public TileCoord m_TopLeft;

	public TileCoord m_BottomRight;

	private Wobbler m_Wobbler;

	private float m_Timer;

	private Material m_Material;

	private float m_StartAlpha;

	private void Awake()
	{
		GetComponent<MeshRenderer>().material.renderQueue = 2999;
		m_Wobbler = new Wobbler();
		m_Wobbler.Go(0.2f, 1f, 1f);
		Instance = this;
		m_Material = GetComponent<MeshRenderer>().material;
		m_StartAlpha = m_Material.color.a;
		m_Timer = 0f;
	}

	public void UpdateArea()
	{
		Vector3 position = (m_TopLeft.ToWorldPositionTileCentered() + m_BottomRight.ToWorldPositionTileCentered()) / 2f;
		position.y = 0.1f;
		base.transform.position = position;
		int num = m_BottomRight.x - m_TopLeft.x + 1;
		int num2 = m_BottomRight.y - m_TopLeft.y + 1;
		float num3 = 10f;
		Vector3 localScale = new Vector3((float)num * Tile.m_Size / num3, 1f, (float)num2 * Tile.m_Size / num3);
		if (m_Wobbler.m_Wobbling)
		{
			localScale *= 1f - m_Wobbler.m_Height;
		}
		base.transform.localScale = localScale;
	}

	public void SetCoords(TileCoord TopLeft, TileCoord BottomRight)
	{
		m_TopLeft = TopLeft;
		m_BottomRight = BottomRight;
		UpdateArea();
	}

	private void Update()
	{
		if (m_Wobbler.m_Wobbling)
		{
			m_Wobbler.Update();
			UpdateArea();
		}
		m_Timer += TimeManager.Instance.m_NormalDelta;
		if (m_Timer > 1f)
		{
			Object.Destroy(base.gameObject);
		}
		else if (m_Timer > 0.5f)
		{
			float num = 1f - (m_Timer - 0.5f) / 0.5f;
			Color color = m_Material.color;
			color.a = num * m_StartAlpha;
			m_Material.color = color;
		}
	}
}
