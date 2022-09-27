using UnityEngine;

public class GameOptionsManager : MonoBehaviour
{
	public static GameOptionsManager Instance;

	public GameOptions m_Options;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		m_Options = new GameOptions();
	}
}
