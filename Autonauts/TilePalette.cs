using System.Collections.Generic;
using UnityEngine;

public class TilePalette : MonoBehaviour
{
	public static Tile.TileType m_CurrentType;

	public static List<Tile.TileType> m_NewUnlocked;

	private static List<Tile.TileType> m_Available;

	private List<TileButton> m_Buttons;

	public static void AddNewUnlocked(Tile.TileType NewType)
	{
		m_NewUnlocked.Add(NewType);
	}

	public static void Init()
	{
		m_CurrentType = Tile.TileType.Empty;
		m_NewUnlocked = new List<Tile.TileType>();
		m_Available = new List<Tile.TileType>();
		m_Available.Add(Tile.TileType.Empty);
		m_Available.Add(Tile.TileType.ClaySoil);
		m_Available.Add(Tile.TileType.Clay);
		m_Available.Add(Tile.TileType.IronSoil);
		m_Available.Add(Tile.TileType.IronSoil2);
		m_Available.Add(Tile.TileType.Iron);
		m_Available.Add(Tile.TileType.Sand);
		m_Available.Add(Tile.TileType.Soil);
		m_Available.Add(Tile.TileType.SoilTilled);
		m_Available.Add(Tile.TileType.SoilHole);
		m_Available.Add(Tile.TileType.CoalSoil);
		m_Available.Add(Tile.TileType.CoalSoil2);
		m_Available.Add(Tile.TileType.CoalSoil3);
		m_Available.Add(Tile.TileType.Coal);
		m_Available.Add(Tile.TileType.StoneSoil);
		m_Available.Add(Tile.TileType.Stone);
		m_Available.Add(Tile.TileType.WaterShallow);
		m_Available.Add(Tile.TileType.WaterDeep);
		m_Available.Add(Tile.TileType.SeaWaterShallow);
		m_Available.Add(Tile.TileType.SeaWaterDeep);
		m_Available.Add(Tile.TileType.Dredged);
		m_Available.Add(Tile.TileType.Swamp);
	}

	private void Awake()
	{
		RectTransform component = GetComponent<RectTransform>();
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/BackButton", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).transform.localPosition = new Vector3(component.rect.width - 30f, component.rect.height / 2f - 30f, 0f);
		m_Buttons = new List<TileButton>();
		CreateTileButtons();
	}

	private void OnDestroy()
	{
	}

	private void DeleteBuildingButtons()
	{
		foreach (TileButton button in m_Buttons)
		{
			Object.Destroy(button.gameObject);
		}
		m_Buttons.Clear();
	}

	private void CreateTileButtons()
	{
		Transform transform = base.transform.Find("Panel").Find("Scroll View").Find("Viewport")
			.Find("Content");
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Terraform/TileButton", typeof(GameObject));
		int count = m_Available.Count;
		float num = 80f;
		float y = (float)((count - 1) / 2 + 1) * num + 7f;
		transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, y);
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			Tile.TileType tileType = m_Available[i];
			bool @new = false;
			if (m_NewUnlocked.Contains(tileType))
			{
				m_NewUnlocked.Remove(tileType);
				@new = true;
			}
			TileButton component = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, transform).GetComponent<TileButton>();
			component.SetInfo(this, tileType, @new);
			float x = (float)(num2 % 2) * num + 7f;
			float y2 = (float)(num2 / 2) * (0f - num) - 7f;
			component.transform.localPosition = new Vector3(x, y2, 0f);
			m_Buttons.Add(component);
			num2++;
		}
		UpdateCurrentType();
	}

	public void UpdateCurrentType()
	{
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			if (m_Buttons[i].m_TileType == m_CurrentType)
			{
				m_Buttons[i].SetSelected(Selected: true);
			}
			else
			{
				m_Buttons[i].SetSelected(Selected: false);
			}
		}
	}

	public void SetCurrentType(Tile.TileType NewType)
	{
		m_CurrentType = NewType;
		UpdateCurrentType();
	}

	public void SetTile(Tile.TileType NewType)
	{
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTerraform>().SetCurrentTileType(NewType);
		SetCurrentType(NewType);
	}
}
