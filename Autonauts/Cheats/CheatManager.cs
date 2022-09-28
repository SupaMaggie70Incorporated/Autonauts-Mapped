using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
	public static CheatManager Instance;

	public bool m_InstantBuild;

	public bool m_FillStorage;

	public bool m_DrainBot;

	public bool m_CheckAll;

	public bool m_CheatMissions;

	public bool m_FastEat;

	public bool m_CursorClicks;

	public bool m_CheapResearch;

	public bool m_ShowMaterials;

	public bool m_CheatsEnabled;

	private string m_LastKeys;

	private void Awake()
	{
		Instance = this;
		m_InstantBuild = false;
		m_FillStorage = false;
		m_DrainBot = false;
		m_CheckAll = true;
		m_CheatMissions = false;
		m_FastEat = false;
		m_CursorClicks = false;
		m_CheapResearch = false;
		m_ShowMaterials = false;
		m_LastKeys = "";
		if (Application.isEditor)
		{
			m_CheatsEnabled = true;
		}
	}

	private void CheckEnable()
	{
		for (int i = 0; i <= 296; i++)
		{
			KeyCode key = (KeyCode)i;
			if (Input.GetKeyDown(key))
			{
				m_LastKeys += key;
				char[] array = m_LastKeys.ToCharArray();
				char[] array2 = "DENKI".ToCharArray();
				int num = array.Length - 1;
				if (num >= array2.Length || array[num] != array2[num])
				{
					m_LastKeys = "";
				}
				else if (num == array2.Length - 1)
				{
					m_CheatsEnabled = true;
				}
			}
		}
	}

	public void UpdateNormal()
	{
		if (m_CheatsEnabled && !CustomStandaloneInputModule.Instance.IsUIInUse() && MyInputManager.m_Rewired.GetButtonDown("Cheats"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				TimeManager.Instance.ToggleFastTime();
			}
			else
			{
				GameStateManager.Instance.SetState(GameStateManager.State.CheatTools);
			}
		}
	}

	private void CheckValidObjects()
	{
		if (!m_CheckAll || ((bool)SaveLoadManager.Instance && SaveLoadManager.Instance.m_Loading))
		{
			return;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Savable");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			BaseClass key = item.Key;
			if (key == null)
			{
				ErrorMessage.LogError("A null object was detected");
			}
			else if (key.m_UniqueID == -1)
			{
				TileCoordObject component = key.GetComponent<TileCoordObject>();
				if ((bool)component)
				{
					ErrorMessage.LogError(string.Concat("A UID -1 object was detected : ", key.m_TypeIdentifier, " ", component.m_TileCoord.x, ",", component.m_TileCoord.y));
				}
				else
				{
					ErrorMessage.LogError("A UID -1 object was detected : " + key.m_TypeIdentifier);
				}
			}
		}
	}

	private void CheckCursorClicks()
	{
		if (!m_CursorClicks || (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2)))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/CursorClick", typeof(GameObject)), Vector3.zero, Quaternion.identity, HudManager.Instance.m_SaveImageRootTransform);
		gameObject.transform.localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
		if (Input.GetMouseButtonDown(0))
		{
			if (MyInputManager.Instance.GetCTRLHeld())
			{
				gameObject.GetComponent<CursorClick>().SetColor(new Color(0f, 0f, 1f));
			}
			else
			{
				gameObject.GetComponent<CursorClick>().SetColor(new Color(1f, 1f, 1f));
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if (MyInputManager.Instance.GetCTRLHeld())
			{
				gameObject.GetComponent<CursorClick>().SetColor(new Color(1f, 0.5f, 0f));
			}
			else
			{
				gameObject.GetComponent<CursorClick>().SetColor(new Color(1f, 0f, 0f));
			}
		}
		else if (Input.GetMouseButtonDown(2))
		{
			if (MyInputManager.Instance.GetCTRLHeld())
			{
				gameObject.GetComponent<CursorClick>().SetColor(new Color(1f, 0f, 1f));
			}
			else
			{
				gameObject.GetComponent<CursorClick>().SetColor(new Color(0f, 1f, 0f));
			}
		}
	}

	private void Update()
	{
		if (!m_CheatsEnabled)
		{
			CheckEnable();
			return;
		}
		CheckValidObjects();
		CheckCursorClicks();
		if (MyInputManager.m_Rewired.GetButtonDown("ClearError"))
		{
			ErrorMessage.Instance.Clear();
		}
	}
}
