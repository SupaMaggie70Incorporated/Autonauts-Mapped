using System.Collections.Generic;
using UnityEngine;

public class TutorialPointerManager : MonoBehaviour
{
	public enum Type
	{
		Explore,
		PickupRock,
		Use,
		Stow,
		Recall,
		OpenEdit,
		SelectWorkbench,
		DropWorkbench,
		CloseEdit,
		BuildWorkbench,
		EngageWorkbench,
		EngageChoppingBlock,
		SelectAxe,
		AddIngredients,
		PickupAxe,
		PickupPick,
		ChopLog,
		ChopPlank,
		SelectAssemblyUnit,
		BuildWorkerWorkbench,
		BuildBot,
		RechargeBot,
		UseWhistle,
		SelectBot,
		ClickRecord,
		ChopTree,
		GiveAxe,
		GiveShovel,
		GivePick,
		EditSearchArea,
		MaxSearchArea,
		EndEditSearchArea,
		Repeat,
		Play,
		Stop,
		SelectShovel,
		PickupShovel,
		DigSoil,
		MineStone,
		PickupTreeSeed,
		PlantTreeSeed,
		AutopediaObjects,
		AutopediaTips,
		AutopediaResearch,
		RepeatDropdown,
		RepeatTargetButton,
		Autopedia,
		AutopediaFood,
		AutopediaBerries,
		AutopediaMushrooms,
		StoreLogs,
		TakeLogs,
		Total
	}

	public static TutorialPointerManager Instance;

	private Type m_TutorialType;

	private static Vector2 m_Middle = new Vector2(0.5f, 0.5f);

	private static Vector2 m_TopMiddle = new Vector2(0.5f, 1f);

	private static Vector2 m_BottomMiddle = new Vector2(0.5f, 0f);

	private static Vector2 m_BottomLeft = new Vector2(0f, 0f);

	private static Vector2 m_NoOffset = new Vector2(0f, 0f);

	private TutorialPointerInfo[] m_Info = new TutorialPointerInfo[52]
	{
		new TutorialPointerInfo(Active: true, m_BottomLeft, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomLeft, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomLeft, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_TopMiddle, m_TopMiddle, new Vector2(0f, -40f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 40f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_TopMiddle, m_TopMiddle, new Vector2(0f, -40f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 10f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 40f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, -20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, -20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, -20f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, new Vector2(0f, 0f), TutorialPointer.TargetType.Screen),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World),
		new TutorialPointerInfo(Active: true, m_BottomMiddle, m_BottomMiddle, m_NoOffset, TutorialPointer.TargetType.World)
	};

	private TutorialPointer m_TutorialPointer;

	private List<GameStateManager.State> m_NormalObjectStates;

	private List<GameStateManager.State> m_NormalStates;

	private List<GameStateManager.State> m_TeachingStates;

	private List<GameStateManager.State> m_BotSelectStates;

	private List<GameStateManager.State> m_BuildingSelectStates;

	private bool m_Visible;

	private bool m_TypeActive;

	private bool m_CeremonyActive;

	private void Awake()
	{
		Instance = this;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Tutorial/TutorialPointer", typeof(GameObject));
		m_TutorialPointer = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_RolloversRootTransform).GetComponent<TutorialPointer>();
		m_NormalObjectStates = new List<GameStateManager.State>();
		m_NormalObjectStates.Add(GameStateManager.State.Normal);
		m_NormalObjectStates.Add(GameStateManager.State.Edit);
		m_NormalStates = new List<GameStateManager.State>();
		m_NormalStates.Add(GameStateManager.State.Normal);
		m_TeachingStates = new List<GameStateManager.State>();
		m_TeachingStates.Add(GameStateManager.State.TeachWorker);
		m_BotSelectStates = new List<GameStateManager.State>();
		m_BotSelectStates.Add(GameStateManager.State.SelectWorker);
		m_BuildingSelectStates = new List<GameStateManager.State>();
		m_BuildingSelectStates.Add(GameStateManager.State.SelectObject);
		m_Visible = true;
	}

	private void OnDestroy()
	{
		if ((bool)m_TutorialPointer)
		{
			Object.Destroy(m_TutorialPointer.gameObject);
		}
	}

	public void SetVisible(bool Visible)
	{
		m_Visible = Visible;
		UpdateActive();
	}

	public void CeremonyActive(bool Active)
	{
		m_CeremonyActive = Active;
		UpdateActive();
	}

	private void UpdateActive()
	{
		m_TutorialPointer.SetActive(m_Visible && m_TypeActive && !m_CeremonyActive);
	}

	public void SetType(Type NewType)
	{
		m_TutorialType = NewType;
		UpdateTutorialType();
		m_TutorialPointer.UpdatePositionAndScale();
		if (NewType != Type.Total)
		{
			m_TutorialPointer.SetOffset(m_Info[(int)NewType].m_Offset);
			m_TutorialPointer.SetAnchorAndPivot(m_Info[(int)NewType].m_Anchor, m_Info[(int)NewType].m_Pivot);
		}
	}

	private void TargetNearestPlotEdge()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players.Count != 0)
			{
				FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
				int num = component.m_TileCoord.x / Plot.m_PlotTilesWide;
				int num2 = component.m_TileCoord.y / Plot.m_PlotTilesHigh;
				int num3 = num2 * Plot.m_PlotTilesHigh - 1;
				int num4 = num3 + Plot.m_PlotTilesHigh + 1;
				int num5 = num * Plot.m_PlotTilesWide - 1;
				int num6 = num5 + Plot.m_PlotTilesWide + 1;
				int num7 = 1000000;
				Plot plotAtPlot = PlotManager.Instance.GetPlotAtPlot(num, num2 - 1);
				if ((bool)plotAtPlot && !plotAtPlot.m_Visible)
				{
					num7 = component.m_TileCoord.y - num3;
				}
				int num8 = 1000000;
				plotAtPlot = PlotManager.Instance.GetPlotAtPlot(num, num2 + 1);
				if ((bool)plotAtPlot && !plotAtPlot.m_Visible)
				{
					num8 = num4 - component.m_TileCoord.y;
				}
				int num9 = 1000000;
				plotAtPlot = PlotManager.Instance.GetPlotAtPlot(num - 1, num2);
				if ((bool)plotAtPlot && !plotAtPlot.m_Visible)
				{
					num9 = component.m_TileCoord.x - num5;
				}
				int num10 = 1000000;
				plotAtPlot = PlotManager.Instance.GetPlotAtPlot(num + 1, num2);
				if ((bool)plotAtPlot && !plotAtPlot.m_Visible)
				{
					num10 = num6 - component.m_TileCoord.x;
				}
				TileCoord tileTarget = default(TileCoord);
				if (num7 <= num8 && num7 <= num9 && num7 <= num10)
				{
					tileTarget = new TileCoord(component.m_TileCoord.x, num3);
				}
				else if (num8 <= num9 && num8 <= num10 && num8 <= num7)
				{
					tileTarget = new TileCoord(component.m_TileCoord.x, num4);
				}
				else if (num10 <= num9 && num10 <= num8 && num10 <= num7)
				{
					tileTarget = new TileCoord(num6, component.m_TileCoord.y);
				}
				else if (num9 <= num10 && num9 <= num8 && num9 <= num7)
				{
					tileTarget = new TileCoord(num5, component.m_TileCoord.y);
				}
				m_TutorialPointer.SetTileTarget(tileTarget);
				m_TypeActive = true;
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetNearestObject(ObjectType NewType, List<GameStateManager.State> PossibleStates)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players.Count != 0)
			{
				FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
				_ = m_Info[(int)m_TutorialType];
				AFO.AT newActionType = AFO.AT.Primary;
				List<TileCoordObject> nearestObjectOfType = PlotManager.Instance.GetNearestObjectOfType(NewType, component.m_TileCoord, 10, component, ObjectTypeList.m_Total, newActionType, "", IgnorePlotVisibility: false, CheckBagged: false, IgnoreAction: true);
				if (nearestObjectOfType.Count > 0)
				{
					m_TutorialPointer.SetWorldTarget(nearestObjectOfType[0]);
				}
				else
				{
					m_TutorialPointer.SetWorldTarget(null);
				}
				m_TypeActive = true;
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetLogStorage(List<GameStateManager.State> PossibleStates, bool IncludeEmpty)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players == null || players.Count == 0)
			{
				return;
			}
			FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
			_ = m_Info[(int)m_TutorialType];
			AFO.AT newActionType = AFO.AT.Primary;
			if (IncludeEmpty)
			{
				newActionType = AFO.AT.Secondary;
			}
			List<TileCoordObject> objectsOfType = PlotManager.Instance.GetObjectsOfType(ObjectType.StoragePalette, component.m_TileCoord, 10, null, ObjectType.Log, newActionType, "", IgnorePlotVisibility: false, CheckBagged: false);
			float num = 1E+07f;
			TileCoordObject worldTarget = null;
			foreach (TileCoordObject item in objectsOfType)
			{
				if (item.GetComponent<Storage>().m_ObjectType == ObjectTypeList.m_Total || item.GetComponent<Storage>().m_ObjectType == ObjectType.Log)
				{
					float num2 = (item.m_TileCoord - component.m_TileCoord).Magnitude();
					if (num2 < num)
					{
						num = num2;
						worldTarget = item;
					}
				}
			}
			m_TutorialPointer.SetWorldTarget(worldTarget);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetBot(List<GameStateManager.State> PossibleStates, bool Recharge)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			m_TutorialPointer.SetWorldTarget(null);
			m_TypeActive = false;
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
			if (collection == null)
			{
				return;
			}
			{
				foreach (KeyValuePair<BaseClass, int> item in collection)
				{
					if (item.Key.GetComponent<Worker>().m_WorkerInterpreter.GetCurrentScript() == null && (!Recharge || item.Key.GetComponent<Worker>().m_Energy == 0f))
					{
						m_TutorialPointer.SetWorldTarget(item.Key);
						m_TypeActive = true;
						break;
					}
				}
				return;
			}
		}
		m_TypeActive = false;
	}

	private void TargetNearestTile(Tile.TileType NewType, List<GameStateManager.State> PossibleStates, bool RMB)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players.Count != 0)
			{
				FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
				_ = m_Info[(int)m_TutorialType];
				AFO.AT newActionType = AFO.AT.Primary;
				if (RMB)
				{
					newActionType = AFO.AT.Secondary;
				}
				TileCoord topLeftIn = component.m_TileCoord + new TileCoord(-10, -10);
				TileCoord bottomRightIn = component.m_TileCoord + new TileCoord(10, 10);
				List<TileCoord> nearestTileOfType = TileHelpers.GetNearestTileOfType(NewType, topLeftIn, bottomRightIn, HighInstruction.FindType.Full, component, component.m_FarmerCarry.GetTopObjectType(), newActionType, "");
				if (nearestTileOfType.Count > 0)
				{
					m_TutorialPointer.SetTileTarget(nearestTileOfType[0]);
					m_TypeActive = true;
				}
				else
				{
					m_TypeActive = false;
				}
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetObjectUnlessPlayerState(ObjectType NewType, Farmer.State NewState, List<GameStateManager.State> PossibleStates)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			if (CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State != NewState)
			{
				TargetNearestObject(NewType, PossibleStates);
				m_TypeActive = true;
			}
			else
			{
				m_TypeActive = false;
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetGiveBot(ObjectType NewType, List<GameStateManager.State> PossibleStates)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			m_TutorialPointer.SetWorldTarget(null);
			m_TypeActive = false;
			{
				foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Worker"))
				{
					if (item.Key.GetComponent<Worker>().m_WorkerInterpreter.GetCurrentScript() == null && item.Key.GetComponent<Worker>().m_FarmerCarry.GetTopObjectType() == ObjectTypeList.m_Total)
					{
						m_TutorialPointer.SetWorldTarget(item.Key);
						m_TypeActive = true;
						break;
					}
				}
				return;
			}
		}
		m_TypeActive = false;
	}

	private void TargetStow()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
			GameObject gameObject = null;
			if (component.m_FarmerCarry.GetTopObject() != null)
			{
				m_TypeActive = true;
				gameObject = HudManager.Instance.m_InventoryBar.m_CarrySlots.m_Buttons[0].gameObject;
				m_TutorialPointer.SetScreenTarget(gameObject);
			}
			else
			{
				m_TypeActive = false;
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetRecall()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
			GameObject screenTarget = null;
			if (component.m_FarmerInventory.GetAnyInventory())
			{
				screenTarget = HudManager.Instance.m_InventoryBar.m_InventorySlots.m_Buttons[0].gameObject;
			}
			m_TutorialPointer.SetScreenTarget(screenTarget);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetEditButton()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			GameObject screenTarget = ModeButton.Get(ModeButton.Type.BuildingPalette).gameObject;
			m_TutorialPointer.SetScreenTarget(screenTarget);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetSelectBuildingPalette(ObjectType NewType)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameObject buttonFromObjectType = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().m_BuildingPalette.GetButtonFromObjectType(NewType);
			m_TutorialPointer.SetScreenTarget(buttonFromObjectType);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetDropWorkbench()
	{
	}

	private void TargetCloseEditButton()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			BaseButtonBack componentInChildren = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().m_BuildingPalette.GetComponentInChildren<BaseButtonBack>();
			m_TutorialPointer.SetScreenTarget(componentInChildren.gameObject);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetBuildWorkbench()
	{
		if (CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State != Farmer.State.Engaged)
		{
			TargetNearestObject(ObjectType.Workbench, m_NormalObjectStates);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetSelectWorkbenchObject(ObjectType NewType)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.BuildingSelect)
		{
			GameObject menu = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBuildingSelect>().m_Menu;
			if ((bool)menu && (bool)menu.GetComponent<ConverterSelect>())
			{
				ConverterSelect component = menu.GetComponent<ConverterSelect>();
				m_TutorialPointer.SetScreenTarget(component.GetButtonFromObjectType(NewType));
			}
			else
			{
				m_TutorialPointer.SetScreenTarget(null);
			}
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetAddIngredients()
	{
		if (CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State != Farmer.State.Engaged)
		{
			TargetNearestObject(ObjectType.Workbench, m_NormalObjectStates);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private bool AnyConvertersWorking()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Converter");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				if (item.Key.GetComponent<Converter>().m_State != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void TargetBuild(ObjectType NewType, List<GameStateManager.State> PossibleStates)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()) && !AnyConvertersWorking())
		{
			if (CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State != Farmer.State.Engaged)
			{
				TargetNearestObject(NewType, m_NormalObjectStates);
				m_TypeActive = true;
			}
			else
			{
				m_TypeActive = false;
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetTeachingButton(string ButtonName)
	{
		if (TeachWorkerScriptEdit.Instance != null && TeachWorkerScriptEdit.Instance.m_CurrentTarget != null && GameStateManager.Instance.GetActualState() != GameStateManager.State.SelectWorker && GameStateManager.Instance.GetActualState() != GameStateManager.State.EditArea)
		{
			GameObject screenTarget;
			switch (ButtonName)
			{
			case "RecordButton":
			case "PlayButton":
			case "StopButton":
				screenTarget = TeachWorkerScriptEdit.Instance.m_ButtonsRoot.transform.Find("RecordBorder/" + ButtonName).gameObject;
				break;
			default:
				screenTarget = TeachWorkerScriptEdit.Instance.m_ButtonsRoot.transform.Find(ButtonName).gameObject;
				break;
			}
			m_TutorialPointer.SetScreenTarget(screenTarget);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private GameObject GetSearchInstruction(HighInstruction NewInstruction)
	{
		if (NewInstruction.m_Type == HighInstruction.Type.FindNearestObject || NewInstruction.m_Type == HighInstruction.Type.FindNearestTile)
		{
			return NewInstruction.m_HudParent.m_EditArea;
		}
		foreach (HighInstruction child in NewInstruction.m_Children)
		{
			GameObject searchInstruction = GetSearchInstruction(child);
			if ((bool)searchInstruction)
			{
				return searchInstruction;
			}
		}
		return null;
	}

	private void TargetEditSearchArea()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
		{
			GameObject gameObject = null;
			foreach (HighInstruction item in TeachWorkerScriptEdit.Instance.m_Instructions.m_List)
			{
				gameObject = GetSearchInstruction(item);
				if ((bool)gameObject)
				{
					break;
				}
			}
			m_TutorialPointer.SetScreenTarget(gameObject);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetMaxSearchArea()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.EditArea)
		{
			m_TutorialPointer.SetScreenTarget(GameStateEditArea.Instance.m_EditArea.m_MaxButton.gameObject);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetEndEditSearchArea()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.EditArea)
		{
			m_TutorialPointer.SetScreenTarget(GameStateEditArea.Instance.m_EditArea.m_ApplyButton.gameObject);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetTileUnlessPlayerState(Tile.TileType NewType, Farmer.State NewState, List<GameStateManager.State> PossibleStates)
	{
		if (PossibleStates.Contains(GameStateManager.Instance.GetActualState()))
		{
			if (CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State != NewState)
			{
				TargetNearestTile(NewType, PossibleStates, RMB: true);
			}
			else
			{
				m_TypeActive = false;
			}
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void UpdateDefault()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			m_TutorialPointer.SetWorldTarget(null);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetAutopediaButton(Autopedia.Page ButtonPage)
	{
		if (Autopedia.Instance != null)
		{
			BaseGadget baseGadget = Autopedia.Instance.m_ButtonList.m_Buttons[(int)ButtonPage];
			m_TutorialPointer.SetScreenTarget(baseGadget.gameObject);
			m_TutorialPointer.SetRotation(180f);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private GameObject GetRepeatDropdownInstruction(HighInstruction NewInstruction)
	{
		if (NewInstruction.m_Type == HighInstruction.Type.Repeat)
		{
			return NewInstruction.m_HudParent.GetComponent<HudInstructionChildren>().m_Dropdown.gameObject;
		}
		foreach (HighInstruction child in NewInstruction.m_Children)
		{
			GameObject repeatBuildingInstruction = GetRepeatBuildingInstruction(child);
			if ((bool)repeatBuildingInstruction)
			{
				return repeatBuildingInstruction;
			}
		}
		return null;
	}

	private void TargetRepeatDropdown()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
		{
			GameObject gameObject = null;
			foreach (HighInstruction item in TeachWorkerScriptEdit.Instance.m_Instructions.m_List)
			{
				gameObject = GetRepeatDropdownInstruction(item);
				if ((bool)gameObject)
				{
					break;
				}
			}
			m_TutorialPointer.SetScreenTarget(gameObject);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private GameObject GetRepeatBuildingInstruction(HighInstruction NewInstruction)
	{
		if (NewInstruction.m_Type == HighInstruction.Type.Repeat && HighInstruction.GetConditionRequireObject(HighInstruction.GetConditionTypeFromRepeatName(NewInstruction.m_Argument)))
		{
			return NewInstruction.m_HudParent.m_ObjectSelect;
		}
		foreach (HighInstruction child in NewInstruction.m_Children)
		{
			GameObject repeatBuildingInstruction = GetRepeatBuildingInstruction(child);
			if ((bool)repeatBuildingInstruction)
			{
				return repeatBuildingInstruction;
			}
		}
		return null;
	}

	private void TargetRepeatTargetButton()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
		{
			GameObject gameObject = null;
			foreach (HighInstruction item in TeachWorkerScriptEdit.Instance.m_Instructions.m_List)
			{
				gameObject = GetRepeatBuildingInstruction(item);
				if ((bool)gameObject)
				{
					break;
				}
			}
			m_TutorialPointer.SetScreenTarget(gameObject);
			m_TypeActive = true;
		}
		else
		{
			m_TypeActive = false;
		}
	}

	private void TargetAutopedia()
	{
		m_TypeActive = false;
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			ModeButton modeButton = HudManager.Instance.m_ModeButtons[8];
			m_TutorialPointer.SetScreenTarget(modeButton.gameObject);
			m_TypeActive = true;
		}
	}

	private void TargetAutopediaFood()
	{
		m_TypeActive = false;
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Autopedia && Autopedia.m_Page == Autopedia.Page.Objects)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<Autopedia>();
			if ((bool)Objects.Instance.m_Select)
			{
				BaseGadget baseGadget = Objects.Instance.m_Select.m_CategoryButtonList.m_Buttons[3];
				m_TutorialPointer.SetScreenTarget(baseGadget.gameObject);
				m_TypeActive = true;
			}
		}
	}

	private void TargetAutopediaBerries()
	{
		m_TypeActive = false;
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Autopedia && Autopedia.m_Page == Autopedia.Page.Objects && (bool)Objects.Instance.m_Select && Objects.Instance.m_Select.m_CurrentCategory == ObjectCategory.Food)
		{
			BaseButtonImage buttonFromObjectType = Objects.Instance.m_Select.GetButtonFromObjectType(ObjectType.Berries);
			if ((bool)buttonFromObjectType)
			{
				m_TutorialPointer.SetScreenTarget(buttonFromObjectType.gameObject);
			}
			m_TypeActive = true;
		}
	}

	private void TargetAutopediaMushrooms()
	{
		m_TypeActive = false;
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Autopedia && Autopedia.m_Page == Autopedia.Page.Objects && (bool)Objects.Instance.m_Select && Objects.Instance.m_Select.m_CurrentCategory == ObjectCategory.Food)
		{
			BaseButtonImage buttonFromObjectType = Objects.Instance.m_Select.GetButtonFromObjectType(ObjectType.MushroomDug);
			if ((bool)buttonFromObjectType)
			{
				m_TutorialPointer.SetScreenTarget(buttonFromObjectType.gameObject);
			}
			m_TypeActive = true;
		}
	}

	private void UpdateTutorialType()
	{
		m_TutorialPointer.SetRotation(0f);
		switch (m_TutorialType)
		{
		case Type.Explore:
			TargetNearestPlotEdge();
			break;
		case Type.PickupRock:
			TargetNearestObject(ObjectType.Rock, m_NormalStates);
			break;
		case Type.Use:
			TargetObjectUnlessPlayerState(ObjectType.TreePine, Farmer.State.Chopping, m_NormalStates);
			break;
		case Type.Stow:
			TargetStow();
			break;
		case Type.Recall:
			TargetRecall();
			break;
		case Type.OpenEdit:
			TargetEditButton();
			break;
		case Type.SelectWorkbench:
			TargetSelectBuildingPalette(ObjectType.Workbench);
			break;
		case Type.DropWorkbench:
			TargetDropWorkbench();
			break;
		case Type.CloseEdit:
			TargetCloseEditButton();
			break;
		case Type.BuildWorkbench:
			TargetBuild(ObjectType.ConverterFoundation, m_NormalStates);
			break;
		case Type.EngageWorkbench:
			TargetNearestObject(ObjectType.Workbench, m_NormalStates);
			break;
		case Type.EngageChoppingBlock:
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.SelectObject)
			{
				TargetNearestObject(ObjectType.ChoppingBlock, m_BuildingSelectStates);
			}
			else if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
			{
				TargetNearestObject(ObjectType.ChoppingBlock, m_TeachingStates);
			}
			else
			{
				TargetNearestObject(ObjectType.ChoppingBlock, m_NormalStates);
			}
			break;
		case Type.SelectAxe:
			TargetSelectWorkbenchObject(ObjectType.ToolAxeStone);
			break;
		case Type.AddIngredients:
			TargetBuild(ObjectType.Workbench, m_NormalStates);
			break;
		case Type.PickupAxe:
			TargetNearestObject(ObjectType.ToolAxeStone, m_NormalStates);
			break;
		case Type.PickupPick:
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
			{
				TargetNearestObject(ObjectType.ToolPickStone, m_TeachingStates);
			}
			else
			{
				TargetNearestObject(ObjectType.ToolPickStone, m_NormalStates);
			}
			break;
		case Type.ChopLog:
			TargetObjectUnlessPlayerState(ObjectType.Log, Farmer.State.Chopping, m_NormalStates);
			break;
		case Type.ChopPlank:
			TargetObjectUnlessPlayerState(ObjectType.Plank, Farmer.State.Chopping, m_NormalStates);
			break;
		case Type.SelectAssemblyUnit:
			TargetSelectBuildingPalette(ObjectType.WorkerAssembler);
			break;
		case Type.BuildWorkerWorkbench:
			TargetBuild(ObjectType.ConverterFoundation, m_NormalObjectStates);
			break;
		case Type.BuildBot:
			TargetBuild(ObjectType.WorkerAssembler, m_NormalStates);
			break;
		case Type.RechargeBot:
			TargetBot(m_NormalStates, Recharge: true);
			break;
		case Type.UseWhistle:
			m_TypeActive = false;
			break;
		case Type.SelectBot:
			TargetBot(m_BotSelectStates, Recharge: false);
			break;
		case Type.ClickRecord:
			TargetTeachingButton("RecordButton");
			break;
		case Type.ChopTree:
			TargetObjectUnlessPlayerState(ObjectType.TreePine, Farmer.State.Chopping, m_TeachingStates);
			break;
		case Type.GiveAxe:
			TargetGiveBot(ObjectType.ToolAxeStone, m_NormalStates);
			break;
		case Type.GiveShovel:
			TargetGiveBot(ObjectType.ToolShovelStone, m_TeachingStates);
			break;
		case Type.GivePick:
			TargetGiveBot(ObjectType.ToolPickStone, m_TeachingStates);
			break;
		case Type.EditSearchArea:
			TargetEditSearchArea();
			break;
		case Type.MaxSearchArea:
			TargetMaxSearchArea();
			break;
		case Type.EndEditSearchArea:
			TargetEndEditSearchArea();
			break;
		case Type.Repeat:
			TargetTeachingButton("RepeatButton");
			break;
		case Type.Play:
			TargetTeachingButton("PlayButton");
			break;
		case Type.Stop:
			TargetTeachingButton("StopButton");
			break;
		case Type.SelectShovel:
			TargetSelectWorkbenchObject(ObjectType.ToolShovelStone);
			break;
		case Type.PickupShovel:
			TargetNearestObject(ObjectType.ToolShovelStone, m_NormalStates);
			break;
		case Type.Total:
			m_TypeActive = false;
			break;
		case Type.DigSoil:
			TargetTileUnlessPlayerState(Tile.TileType.Soil, Farmer.State.Shovel, m_TeachingStates);
			break;
		case Type.MineStone:
			TargetTileUnlessPlayerState(Tile.TileType.StoneSoil, Farmer.State.Mining, m_TeachingStates);
			break;
		case Type.PickupTreeSeed:
			TargetNearestObject(ObjectType.TreeSeed, m_TeachingStates);
			break;
		case Type.PlantTreeSeed:
			TargetTileUnlessPlayerState(Tile.TileType.SoilHole, Farmer.State.Seed, m_TeachingStates);
			break;
		case Type.AutopediaObjects:
			TargetAutopediaButton(Autopedia.Page.Objects);
			break;
		case Type.AutopediaTips:
			TargetAutopediaButton(Autopedia.Page.Tips);
			break;
		case Type.AutopediaResearch:
			TargetAutopediaButton(Autopedia.Page.Research);
			break;
		case Type.RepeatDropdown:
			TargetRepeatDropdown();
			break;
		case Type.RepeatTargetButton:
			TargetRepeatTargetButton();
			break;
		case Type.Autopedia:
			TargetAutopedia();
			break;
		case Type.AutopediaFood:
			TargetAutopediaFood();
			break;
		case Type.AutopediaBerries:
			TargetAutopediaBerries();
			break;
		case Type.AutopediaMushrooms:
			TargetAutopediaMushrooms();
			break;
		case Type.StoreLogs:
			TargetLogStorage(m_NormalStates, IncludeEmpty: true);
			break;
		case Type.TakeLogs:
			TargetLogStorage(m_TeachingStates, IncludeEmpty: false);
			break;
		default:
			UpdateDefault();
			break;
		}
		UpdateActive();
	}

	private void Update()
	{
		UpdateTutorialType();
	}
}
