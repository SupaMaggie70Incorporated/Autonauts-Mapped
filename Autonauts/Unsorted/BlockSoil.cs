using UnityEngine;

public class BlockSoil : BaseClass
{
	private Vector2[] m_OriginalUVs;

	public override void Restart()
	{
		base.Restart();
		if (m_OriginalUVs == null)
		{
			MeshFilter componentInChildren = GetComponentInChildren<MeshFilter>();
			m_OriginalUVs = new Vector2[componentInChildren.mesh.uv.Length];
			int num = 0;
			Vector2[] uv = componentInChildren.mesh.uv;
			foreach (Vector2 vector in uv)
			{
				m_OriginalUVs[num] = vector;
				num++;
			}
		}
	}

	public void SetTile(TileCoord Position)
	{
		Texture value = (Texture)Resources.Load("Textures/Writable/TileMap", typeof(Texture));
		m_ModelRoot.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", value);
		int num = 16;
		float num2 = 1f / (float)num;
		int tileTexture = TileManager.Instance.GetTileTexture(Position);
		float x = (float)(tileTexture % num) * num2;
		float y = (float)(tileTexture / num) * num2;
		MeshFilter componentInChildren = GetComponentInChildren<MeshFilter>();
		Vector2[] array = new Vector2[componentInChildren.mesh.uv.Length];
		int num3 = 0;
		Vector2[] originalUVs = m_OriginalUVs;
		foreach (Vector2 vector in originalUVs)
		{
			array[num3] = vector * new Vector2(num2, num2) + new Vector2(x, y);
			num3++;
		}
		componentInChildren.mesh.uv = array;
	}
}
