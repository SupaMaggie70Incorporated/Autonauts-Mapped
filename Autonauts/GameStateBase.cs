using System.Collections.Generic;
using UnityEngine;

public class GameStateBase : MonoBehaviour
{
	private Actionable m_PreviousHighlightObject;

	protected CustomStandaloneInputModule m_EventSystem;

	public GameStateManager.State m_BaseState;

	public TileCoord m_CursorTilePosition;

	private bool m_BufferedClick;

	private bool m_BufferedClickKey;

	private Actionable m_BufferedClickObject;

	private TileCoord m_BufferedClickPosition;

	protected void Awake()
	{
		m_EventSystem = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
		m_PreviousHighlightObject = null;
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.StopMousePan();
		}
		if ((bool)Cursor.Instance)
		{
			Cursor.Instance.NoTarget();
		}
	}

	public virtual void ShutDown()
	{
	}

	public GameObject TestCollisionOffset(out Vector3 CollisionPoint, bool TestTiles, bool TestBuildings, bool TestWorkers, bool TestMisc, bool TestWorkersDead, Vector3 Offset)
	{
		CollisionPoint = default(Vector3);
		RaycastHit hitInfo = default(RaycastHit);
		int num = 0;
		if (TestTiles)
		{
			num += 512;
		}
		if (TestBuildings)
		{
			num += 1024;
		}
		if (TestWorkers)
		{
			num += 2048;
		}
		if (TestMisc)
		{
			num += 4096;
		}
		if (TestWorkersDead)
		{
			num += 16384;
		}
		if (Physics.Raycast(CameraManager.Instance.m_Camera.ScreenPointToRay(Offset), out hitInfo, 1000f, num))
		{
			GameObject result = hitInfo.collider.gameObject;
			CollisionPoint = hitInfo.point;
			if (CollisionPoint.x < 0f)
			{
				CollisionPoint.x = 0f;
			}
			if (CollisionPoint.z > 0f)
			{
				CollisionPoint.z = 0f;
			}
			float num2 = (float)(TileManager.Instance.m_TilesWide - 1) * Tile.m_Size;
			if (CollisionPoint.x > num2)
			{
				CollisionPoint.x = num2;
			}
			num2 = (float)(-(TileManager.Instance.m_TilesHigh - 1)) * Tile.m_Size;
			if (CollisionPoint.z < num2)
			{
				CollisionPoint.z = num2;
			}
			return result;
		}
		return null;
	}

	public GameObject TestMouseCollision(out Vector3 CollisionPoint, bool TestTiles, bool TestBuildings, bool TestWorkers, bool TestMisc, bool TestWorkersDead)
	{
		return TestCollisionOffset(out CollisionPoint, TestTiles, TestBuildings, TestWorkers, TestMisc, TestWorkersDead, Input.mousePosition);
	}

	protected bool TestMouseButtonDown(int Button)
	{
		return Input.GetMouseButtonDown(Button);
	}

	protected GameObject GetRootObject(GameObject Object)
	{
		Transform parent = Object.transform;
		while ((bool)parent && (!parent.GetComponent<Selectable>() || (parent.GetComponent<Building>() == null && !parent.GetComponent<Savable>().GetIsSavable() && parent.GetComponent<Savable>().m_TypeIdentifier != ObjectType.CertificateReward) || ((bool)parent.GetComponent<Building>() && parent.GetComponent<Building>().m_Blueprint)))
		{
			parent = parent.parent;
		}
		if ((bool)parent)
		{
			return parent.gameObject;
		}
		return null;
	}

	protected void HighlightObject(Actionable TargetObject)
	{
		if (!(TargetObject != m_PreviousHighlightObject))
		{
			return;
		}
		if ((bool)m_PreviousHighlightObject)
		{
			m_PreviousHighlightObject.GetComponent<Selectable>().SetHighlight(Highlighted: false);
		}
		if ((bool)TargetObject && (bool)TargetObject.GetComponent<Selectable>())
		{
			m_PreviousHighlightObject = TargetObject;
			if ((bool)m_PreviousHighlightObject)
			{
				m_PreviousHighlightObject.GetComponent<Selectable>().SetHighlight(Highlighted: true);
			}
		}
		else
		{
			m_PreviousHighlightObject = null;
		}
	}

	protected void OnDestroy()
	{
		HighlightObject(null);
	}

	public virtual void Pushed(GameStateManager.State NewState)
	{
		HighlightObject(null);
	}

	public virtual void Popped(GameStateManager.State NewState)
	{
		HighlightObject(null);
	}

	public virtual void UpdateState()
	{
	}

	public bool GetObjectUnderMouse(bool TestTiles, bool TestBuildings, bool TestMisc, bool TestBots, out int UID, out TileCoord NewCoord, out Vector3 HitPosition)
	{
		UID = 0;
		NewCoord = default(TileCoord);
		GameObject gameObject = TestMouseCollision(out HitPosition, TestTiles, TestBuildings, TestWorkers: false, TestMisc, TestBots);
		if ((bool)gameObject)
		{
			NewCoord = new TileCoord(HitPosition);
			if (!gameObject.GetComponent<PlotRoot>())
			{
				gameObject = GetRootObject(gameObject);
				if ((bool)gameObject && (bool)gameObject.GetComponent<BaseClass>())
				{
					UID = gameObject.GetComponent<BaseClass>().m_UniqueID;
				}
			}
			return true;
		}
		return false;
	}

	private bool IsMouseInsideWindow()
	{
		if (!new Rect(0f, 0f, Screen.width, Screen.height).Contains(Input.mousePosition) && (!SaveLoadManager.m_Video || !SaveLoadManager.m_TestBuild))
		{
			return false;
		}
		return true;
	}

	protected void UpdateNormalGameControl()
	{
		if (!IsMouseInsideWindow())
		{
			return;
		}
		bool flag = false;
		bool key = Input.GetKey(KeyCode.LeftControl);
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (players == null || players.Count == 0)
		{
			return;
		}
		bool flag2 = false;
		if ((bool)players[0].GetComponent<Farmer>().m_EngagedObject && players[0].GetComponent<Farmer>().m_EngagedObject.GetComponent<Vehicle>() == null)
		{
			flag2 = true;
		}
		Actionable actionable = null;
		TileCoord tileCoord = default(TileCoord);
		Vector3 CollisionPoint;
		GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: false, TestMisc: true, TestWorkersDead: true);
		if ((bool)gameObject)
		{
			if (!flag2 && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				Selectable selectableObjectAtTile = gameObject.transform.parent.GetComponent<Plot>().GetSelectableObjectAtTile(tileCoord);
				if ((bool)selectableObjectAtTile)
				{
					gameObject = selectableObjectAtTile.gameObject;
				}
				else
				{
					Tile tile = TileManager.Instance.GetTile(tileCoord);
					if ((bool)tile.m_BuildingFootprint)
					{
						bool flag3 = true;
						if (tile.m_BuildingFootprint.m_TypeIdentifier != ObjectType.ConverterFoundation && Converter.GetIsTypeConverter(tile.m_BuildingFootprint.m_TypeIdentifier))
						{
							Converter component = tile.m_BuildingFootprint.GetComponent<Converter>();
							if (component == null)
							{
								ErrorMessage.LogError("Strange null converter " + tile.m_BuildingFootprint.m_TypeIdentifier);
							}
							if ((bool)component && (tileCoord == component.GetSpawnPoint() || tileCoord == component.GetAccessPosition()))
							{
								flag3 = false;
							}
						}
						if (flag3)
						{
							gameObject = tile.m_BuildingFootprint.gameObject;
						}
					}
				}
			}
			if (gameObject.GetComponent<PlotRoot>() == null)
			{
				gameObject = GetRootObject(gameObject);
				if ((bool)gameObject && (bool)gameObject.GetComponent<Selectable>() && (bool)gameObject.GetComponent<Selectable>().m_Plot && !gameObject.GetComponent<Selectable>().m_Plot.m_Visible)
				{
					gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: true);
				}
			}
			if ((bool)gameObject)
			{
				if ((bool)gameObject.GetComponent<PlotRoot>())
				{
					tileCoord = new TileCoord(CollisionPoint);
					Cursor.Instance.TargetTile(tileCoord);
					Tile.TileType tileType = TileManager.Instance.GetTileType(tileCoord);
					HudManager.Instance.ActivateTileRollover(Activate: true, tileType, tileCoord);
					actionable = gameObject.transform.parent.GetComponent<Actionable>();
					flag = true;
				}
				else
				{
					tileCoord = new TileCoord(CollisionPoint);
					Cursor.Instance.TargetTile(tileCoord);
					Cursor.Instance.NoTarget();
					if ((bool)gameObject && (bool)gameObject.GetComponent<Actionable>() && (bool)gameObject.GetComponent<Actionable>().GetActionInfo(new GetActionInfo(GetAction.IsPickable)))
					{
						if (!flag2)
						{
							if ((bool)gameObject.GetComponent<Floor>())
							{
								tileCoord = new TileCoord(CollisionPoint);
								Selectable selectableObjectAtTile2 = PlotManager.Instance.GetPlotAtTile(tileCoord).GetSelectableObjectAtTile(tileCoord);
								if ((bool)selectableObjectAtTile2)
								{
									gameObject = selectableObjectAtTile2.gameObject;
								}
							}
							if ((bool)gameObject.GetComponent<Building>())
							{
								Building building;
								if ((bool)gameObject.GetComponent<Floor>() || ((bool)gameObject.GetComponent<ConverterFoundation>() && (bool)gameObject.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<Floor>()))
								{
									building = gameObject.GetComponent<Building>();
								}
								else
								{
									building = TileManager.Instance.GetTile(gameObject.GetComponent<Building>().m_TileCoord).m_Building;
									if (building == null)
									{
										return;
									}
									building = building.GetTopFoundation();
									gameObject = building.gameObject;
								}
								if (Building.GetIsTypeWalkable(building.m_TypeIdentifier))
								{
									tileCoord = new TileCoord(CollisionPoint);
									actionable = gameObject.GetComponent<Actionable>();
								}
								else
								{
									tileCoord = building.GetAccessPosition();
									if (building.m_TypeIdentifier == ObjectType.ConverterFoundation)
									{
										ObjectType topObjectType = players[0].GetComponent<FarmerPlayer>().m_FarmerCarry.GetTopObjectType();
										if (!building.GetComponent<Converter>().CanAcceptIngredient(topObjectType))
										{
											tileCoord = new TileCoord(CollisionPoint);
										}
									}
									actionable = gameObject.GetComponent<Actionable>();
								}
								flag = true;
							}
							else if ((bool)gameObject.GetComponent<Selectable>())
							{
								if (gameObject.GetComponent<Selectable>().IsSelectable())
								{
									tileCoord.Copy(gameObject.GetComponent<TileCoordObject>().m_TileCoord);
									actionable = gameObject.GetComponent<Actionable>();
									flag = true;
								}
								else
								{
									tileCoord = new TileCoord(CollisionPoint);
									Cursor.Instance.TargetTile(tileCoord);
									actionable = PlotManager.Instance.GetPlotAtTile(tileCoord);
									flag = true;
								}
							}
						}
						else if ((bool)gameObject.GetComponent<ResearchStation>())
						{
							actionable = gameObject.GetComponent<Actionable>();
						}
					}
				}
			}
		}
		m_CursorTilePosition = tileCoord;
		HighlightObject(actionable);
		if (!flag)
		{
			HudManager.Instance.ActivateTileRollover(Activate: false, Tile.TileType.Total, default(TileCoord));
			Cursor.Instance.NoTarget();
			return;
		}
		FarmerPlayer component2 = players[0].GetComponent<FarmerPlayer>();
		if ((bool)actionable)
		{
			if (key)
			{
				ActionType actionType = component2.AutoAction(new ActionInfo(ActionType.Total, tileCoord, actionable, "", "", AFO.AT.AltPrimary), AndAction: false);
				ActionType actionType2 = component2.AutoAction(new ActionInfo(ActionType.Total, tileCoord, actionable, "", "", AFO.AT.AltSecondary), AndAction: false);
				if ((actionType != ActionType.Total && actionType != ActionType.Fail) || (actionType2 != ActionType.Total && actionType2 != ActionType.Fail))
				{
					Cursor.Instance.SetUsable(Usable: true);
				}
				else
				{
					Cursor.Instance.SetUsable(Usable: false);
				}
			}
			else
			{
				ActionInfo actionInfo = new ActionInfo(ActionType.Total, tileCoord, actionable, "", "", AFO.AT.Primary);
				ActionType actionType3 = component2.AutoAction(actionInfo, AndAction: false);
				ActionInfo actionInfo2 = new ActionInfo(ActionType.Total, tileCoord, actionable, "", "", AFO.AT.Secondary);
				ActionType actionType4 = component2.AutoAction(actionInfo2, AndAction: false);
				if (actionType3 == ActionType.Pickup && !component2.m_FarmerCarry.CanAddCarry(actionable.GetComponent<Holdable>()))
				{
					actionType3 = ActionType.Fail;
				}
				if ((actionType3 != ActionType.Total && actionType3 != ActionType.Fail) || (actionType4 != ActionType.Total && actionType4 != ActionType.Fail && actionType4 != ActionType.DropAll) || (actionType4 == ActionType.DropAll && (bool)component2.m_EngagedObject && Crane.GetIsTypeCrane(component2.m_EngagedObject.m_TypeIdentifier) && (bool)component2.m_EngagedObject.GetComponent<Crane>().m_HeldObject && Minecart.GetIsTypeMinecart(component2.m_EngagedObject.GetComponent<Crane>().m_HeldObject.m_TypeIdentifier)))
				{
					if (actionType3 == ActionType.UseInHands)
					{
						Cursor.Instance.SetUsable(Usable: true, Animate: true);
					}
					else
					{
						Cursor.Instance.SetUsable(Usable: true);
					}
				}
				else if (actionable.m_TypeIdentifier != ObjectType.Plot)
				{
					Cursor.Instance.SetUsable(Usable: false);
				}
				else if (!TileManager.Instance.GetTileSolidToPlayer(tileCoord) || ((bool)component2.m_EngagedObject && component2.m_EngagedObject.m_TypeIdentifier == ObjectType.Canoe))
				{
					Cursor.Instance.SetUsable(Usable: true);
				}
				else
				{
					Cursor.Instance.SetUsable(Usable: false);
				}
				if (Input.GetKey(KeyCode.LeftAlt))
				{
					ActionInfo actionInfo3 = new ActionInfo(ActionType.Total, tileCoord, actionable, "", "", AFO.AT.AltPrimary);
					ActionType newAltAction = component2.AutoAction(actionInfo3, AndAction: false);
					ActionInfo actionInfo4 = new ActionInfo(ActionType.Total, tileCoord, actionable, "", "", AFO.AT.AltSecondary);
					ActionType newAltAction2 = component2.AutoAction(actionInfo4, AndAction: false);
					HudManager.Instance.SetMouseActions(actionInfo, actionType3, actionInfo2, actionType4, actionInfo3, newAltAction, actionInfo4, newAltAction2);
				}
			}
		}
		else
		{
			Cursor.Instance.SetUsable(Usable: true);
		}
		if (TestMouseButtonDown(0))
		{
			if (SaveLoadManager.m_Video && Input.GetKey(KeyCode.Backslash))
			{
				m_BufferedClick = true;
				m_BufferedClickKey = key;
				m_BufferedClickObject = actionable;
				m_BufferedClickPosition = tileCoord;
			}
			else
			{
				DoMouseClick(key, actionable, tileCoord);
			}
		}
		else if (TestMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift))
		{
			if (key)
			{
				component2.GoToAndAction(tileCoord, actionable, AFO.AT.AltSecondary);
			}
			else
			{
				component2.GoToAndAction(tileCoord, actionable, AFO.AT.Secondary);
			}
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TileSelectEffect, tileCoord.ToWorldPositionTileCentered() + new Vector3(0f, 0.1f, 0f), Quaternion.identity).GetComponent<TileSelectEffect>().SetScale(Cursor.Instance.m_Scale / 0.3f);
			Cursor.Instance.Pinch();
		}
	}

	private void CheckBufferedClick()
	{
		if (SaveLoadManager.m_Video && !Input.GetKey(KeyCode.Backslash) && m_BufferedClick)
		{
			m_BufferedClick = false;
			DoMouseClick(m_BufferedClickKey, m_BufferedClickObject, m_BufferedClickPosition);
		}
	}

	private void DoMouseClick(bool Key, Actionable TargetObject, TileCoord TilePosition)
	{
		FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
		if (Key)
		{
			component.GoToAndAction(TilePosition, TargetObject, AFO.AT.AltPrimary);
		}
		else
		{
			component.GoToAndAction(TilePosition, TargetObject);
		}
		if (!Tile.GetIsSolid(TileManager.Instance.GetTile(TilePosition).m_TileType))
		{
			if ((bool)TargetObject.GetComponent<Plot>())
			{
				AudioManager.Instance.StartEvent("UITileSelected");
			}
			else
			{
				AudioManager.Instance.StartEvent("UIObjectSelected");
			}
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TileSelectEffect, TilePosition.ToWorldPositionTileCentered() + new Vector3(0f, 0.1f, 0f), Quaternion.identity).GetComponent<TileSelectEffect>().SetScale(Cursor.Instance.m_Scale / 0.3f);
			Cursor.Instance.Pinch();
		}
		else
		{
			AudioManager.Instance.StartEvent("UIInvalidSelection");
		}
	}

	public void CheckPlanningToggle()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("ShowPlanningAreas"))
		{
			PlanningManager.Instance.ToggleShowAreas();
			if ((bool)GetComponent<GameStatePlanning>())
			{
				GetComponent<GameStatePlanning>().UpdateShowToggle();
			}
		}
	}

	public void UpdateInventoryKeys()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (players == null || players.Count == 0)
		{
			return;
		}
		FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
		if (MyInputManager.m_Rewired.GetButtonDown("InventoryUp"))
		{
			component.SendAction(new ActionInfo(ActionType.CycleObject, component.m_TileCoord, null, "Up"));
		}
		if (MyInputManager.m_Rewired.GetButtonDown("InventoryDown"))
		{
			component.SendAction(new ActionInfo(ActionType.CycleObject, component.m_TileCoord, null, "Down"));
		}
		if (MyInputManager.m_Rewired.GetButtonDown("InventoryStow"))
		{
			if (component.m_FarmerCarry.GetTopObject() != null)
			{
				component.SendAction(new ActionInfo(ActionType.StowObject, component.m_TileCoord));
			}
			else
			{
				component.SendAction(new ActionInfo(ActionType.RecallObject, component.m_TileCoord));
			}
		}
		if (MyInputManager.m_Rewired.GetButtonDown("InventorySwap"))
		{
			component.SendAction(new ActionInfo(ActionType.SwapObject, component.m_TileCoord));
		}
		if (MyInputManager.m_Rewired.GetButtonDown("UseInHand"))
		{
			Holdable topObject = component.m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if ((bool)topObject.GetComponent<ToolBasket>() || (bool)topObject.GetComponent<ToolFillable>() || (bool)topObject.GetComponent<Instrument>())
				{
					component.SendAction(new ActionInfo(ActionType.UseInHands, component.m_TileCoord, ObjectTypeList.m_NothingObject, "", "", AFO.AT.Total, "", component.m_FarmerCarry.GetTopObjectType()));
				}
				else if (Sign.GetIsTypeSign(topObject.m_TypeIdentifier))
				{
					if (GameStateManager.Instance.GetActualState() != GameStateManager.State.TeachWorker)
					{
						component.SendAction(new ActionInfo(ActionType.EngageObject, component.m_TileCoord, topObject));
					}
				}
				else
				{
					component.SendAction(new ActionInfo(ActionType.Shout, component.m_TileCoord, null, "Go!"));
				}
			}
			else
			{
				component.SendAction(new ActionInfo(ActionType.Shout, component.m_TileCoord, null, "Go!"));
			}
		}
		CheckPlanningToggle();
	}

	private void Update()
	{
		CheckBufferedClick();
	}
}
