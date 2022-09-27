using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureAtlasManager : MonoBehaviour
{
	public static TextureAtlasManager Instance;

	public Texture2D m_Texture;

	public float m_HalfTexelOffset;

	private List<Texture2D> m_OldTextures;

	public Rect[] m_RectangleResults;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		m_Texture = new Texture2D(4096, 4096);
		m_Texture.name = "Big Texture";
		m_Texture.filterMode = FilterMode.Point;
		m_OldTextures = new List<Texture2D>();
	}

	public void AddTexture(Texture2D NewTexture)
	{
		if (!m_OldTextures.Contains(NewTexture))
		{
			m_OldTextures.Add(NewTexture);
		}
	}

	public void Finish()
	{
		foreach (Texture2D oldTexture in m_OldTextures)
		{
			if (!oldTexture.isReadable)
			{
				ErrorMessage.LogError("Texture not readable " + oldTexture.name);
			}
		}
		m_RectangleResults = m_Texture.PackTextures(m_OldTextures.ToArray(), 1, 4096, makeNoLongerReadable: true);
		if (m_RectangleResults == null)
		{
			ErrorMessage.LogError("Texture packing failed");
		}
		if (Application.isEditor)
		{
			Texture2D texture2D = new Texture2D(4096, 4096);
			texture2D.PackTextures(m_OldTextures.ToArray(), 1, 4096);
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(Application.persistentDataPath + "/TextureAtlas.png", bytes);
		}
		m_OldTextures = null;
		m_HalfTexelOffset = 1f / (float)m_Texture.width * 0.5f;
	}
}
