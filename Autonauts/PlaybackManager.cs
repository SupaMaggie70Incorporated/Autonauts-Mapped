using System.Collections.Generic;
using UnityEngine;

public class PlaybackManager : MonoBehaviour
{
	private static bool m_Log;

	private static bool m_LogVerbose;

	public static PlaybackManager Instance;

	public float m_Time;

	private float m_PlaybackSpeed;

	public int m_Frame;

	public int m_OldFrame;

	private List<List<RecordingAction>> m_Timeline;

	private Dictionary<int, BaseClass> m_Objects;

	private PlaybackSlider m_Slider;

	public bool m_Loading;

	private bool m_Playing;

	private List<MovingBuilding> m_MovingBuildingList;

	private CustomStandaloneInputModule m_EventSystem;

	private Dictionary<int, RecordingAction> m_PreProcessActionObjects;

	private List<RecordingAction> m_PreProcessActionList;

	private int m_ActionsDropped;

	private int m_ObjectsCreated;

	private int m_TilesChanged;

	private void Awake()
	{
		Instance = this;
		m_Playing = false;
		m_PlaybackSpeed = 250f;
		m_Timeline = new List<List<RecordingAction>>();
		m_Objects = new Dictionary<int, BaseClass>();
		m_MovingBuildingList = new List<MovingBuilding>();
		m_EventSystem = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
	}

	public void SetSlider(PlaybackSlider NewSlider)
	{
		m_Slider = NewSlider;
	}

	private void CheckTimelineLength(int Frame)
	{
		while (m_Timeline.Count <= Frame)
		{
			m_Timeline.Add(new List<RecordingAction>());
		}
	}

	private void ConvertTiles()
	{
		int tilesWidth = RecordingManager.Instance.m_TilesWidth;
		int tilesHeight = RecordingManager.Instance.m_TilesHeight;
		int[] array = new int[tilesWidth * tilesHeight];
		for (int i = 0; i < tilesWidth * tilesHeight; i++)
		{
			array[i] = RecordingManager.Instance.m_StartingTiles[i];
		}
		foreach (TileRecording tile in RecordingManager.Instance.m_Tiles)
		{
			int f = tile.f;
			CheckTimelineLength(f);
			RecordingAction recordingAction = new RecordingAction();
			recordingAction.m_Action = RecordingAction.Action.ChangeTile;
			int num = tile.y * tilesWidth + tile.x;
			recordingAction.m_OldRotation = array[num];
			recordingAction.m_X = tile.x;
			recordingAction.m_Y = tile.y;
			recordingAction.m_Rotation = (int)tile.m_Type;
			array[num] = recordingAction.m_Rotation;
			m_Timeline[f].Add(recordingAction);
		}
	}

	private void ConvertObjects()
	{
		foreach (KeyValuePair<int, ObjectRecording> @object in RecordingManager.Instance.m_Objects)
		{
			ObjectRecording value = @object.Value;
			int oldX = 0;
			int oldY = 0;
			int oldRotation = 0;
			for (int i = 0; i < value.m_Stamps.Count; i++)
			{
				RecordingStamp recordingStamp = value.m_Stamps[i];
				int f = recordingStamp.f;
				CheckTimelineLength(f);
				RecordingAction recordingAction = new RecordingAction();
				if (i == 0)
				{
					recordingAction.m_Action = RecordingAction.Action.Create;
				}
				else if (i == value.m_Stamps.Count - 1 && recordingStamp.x == 0 && recordingStamp.y == 0)
				{
					recordingAction.m_Action = RecordingAction.Action.Destroy;
				}
				else if (RecordingStamp.GetIsSpecial(recordingStamp.x))
				{
					recordingAction.m_Action = RecordingAction.Action.Special;
				}
				else
				{
					recordingAction.m_Action = RecordingAction.Action.Move;
				}
				recordingAction.m_Stamp = recordingStamp;
				recordingAction.m_UID = @object.Key;
				recordingAction.m_Type = ObjectTypeList.Instance.GetIdentifierFromSaveName(value.m_ObjectType);
				recordingAction.m_Index = recordingStamp.i;
				recordingAction.m_X = recordingStamp.x;
				recordingAction.m_Y = recordingStamp.y;
				recordingAction.m_Rotation = recordingStamp.r;
				recordingAction.m_OldX = oldX;
				recordingAction.m_OldY = oldY;
				recordingAction.m_OldRotation = oldRotation;
				if (RecordingStamp.GetIsSpecial(recordingStamp.x))
				{
					recordingAction.m_SpecialData = recordingStamp.SpecialData1;
					recordingAction.m_OldSpecialData = recordingStamp.SpecialData2;
				}
				if (recordingAction.m_Action != RecordingAction.Action.Special)
				{
					oldX = recordingAction.m_X;
					oldY = recordingAction.m_Y;
					oldRotation = recordingAction.m_Rotation;
				}
				m_Timeline[f].Add(recordingAction);
				if (recordingAction.m_Type == ObjectType.FarmerPlayer && recordingAction.m_Action == RecordingAction.Action.Create)
				{
					TileCoord tileCoord = new TileCoord(recordingStamp.x, recordingStamp.y);
					CameraManager.Instance.Focus(tileCoord.ToWorldPositionTileCenteredWithoutHeight());
				}
			}
		}
	}

	private void ConvertPlots()
	{
		foreach (KeyValuePair<int, PlotRecording> plot in RecordingManager.Instance.m_Plots)
		{
			PlotRecording value = plot.Value;
			int f = value.f;
			CheckTimelineLength(f);
			RecordingAction recordingAction = new RecordingAction();
			recordingAction.m_Action = RecordingAction.Action.ShowPlot;
			recordingAction.m_X = value.x;
			recordingAction.m_Y = value.y;
			m_Timeline[f].Add(recordingAction);
		}
	}

	private void ConvertRecording()
	{
		ConvertTiles();
		ConvertObjects();
		ConvertPlots();
	}

	private void CreateTiles()
	{
		int tilesWidth = RecordingManager.Instance.m_TilesWidth;
		int tilesHeight = RecordingManager.Instance.m_TilesHeight;
		Tile[] array = new Tile[tilesWidth * tilesHeight];
		for (int i = 0; i < tilesHeight * tilesWidth; i++)
		{
			array[i] = new Tile();
			array[i].m_TileType = (Tile.TileType)RecordingManager.Instance.m_StartingTiles[i];
		}
		MapManager.Instance.Load(array, tilesWidth, tilesHeight);
	}

	public void Init()
	{
		string loadFileName = SessionManager.Instance.m_LoadFileName;
		RecordingManager.Instance.Load(loadFileName);
		CreateTiles();
		ConvertRecording();
		ProcessFrameForward(m_Timeline[0], Forwards: true);
		CameraManager.Instance.SetDistance(20f);
		CameraManager.Instance.SetState(CameraManager.State.Normal);
		ObjectTypeList.Instance.SetUniqueIDCounter(100000000);
	}

	public float GetLoadPercent()
	{
		return 0f;
	}

	private static int SortPanelsByName(MovingBuilding p1, MovingBuilding p2)
	{
		return p1.m_Index - p2.m_Index;
	}

	private void CheckMoveBuildings()
	{
		if (m_MovingBuildingList.Count <= 0)
		{
			return;
		}
		m_MovingBuildingList.Sort(SortPanelsByName);
		foreach (MovingBuilding movingBuilding in m_MovingBuildingList)
		{
			movingBuilding.m_Building.Wake();
		}
		BuildingManager.Instance.MoveBuildings(m_MovingBuildingList);
		m_MovingBuildingList.Clear();
	}

	private void CreateObject(RecordingAction NewAction, int x, int y, int r)
	{
		TileCoord tilePosition = new TileCoord(x, y);
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewAction.m_Type, tilePosition.ToWorldPositionTileCentered(), Quaternion.identity);
		if (NewAction.m_Type == ObjectType.Worker)
		{
			Worker component = baseClass.GetComponent<Worker>();
			component.SetFrame(ObjectType.WorkerFrameMk1);
			component.SetDrive(ObjectType.WorkerDriveMk1);
			component.SetHead(ObjectType.WorkerHeadMk1);
			component.UpdateModel();
		}
		else if (baseClass.m_TypeIdentifier == ObjectType.FlowerWild)
		{
			baseClass.GetComponent<TileCoordObject>().SetTilePosition(new TileCoord(x, y));
			baseClass.GetComponent<FlowerWild>().SetType((FlowerWild.Type)r);
		}
		else if (ObjectTypeList.Instance.GetIsBuilding(NewAction.m_Type))
		{
			Building component2 = baseClass.GetComponent<Building>();
			component2.SetTilePosition(tilePosition);
			component2.SetRotation(r);
			MapManager.Instance.AddBuilding(component2);
		}
		else
		{
			baseClass.GetComponent<TileCoordObject>().SetTilePosition(new TileCoord(x, y));
		}
		SetObjectPosition(NewAction.m_Index, baseClass, x, y, r);
		m_Objects.Add(NewAction.m_UID, baseClass);
	}

	private void SetObjectPosition(int Index, BaseClass NewObject, int x, int y, int r)
	{
		TileCoord tileCoord = new TileCoord(x, y);
		if (NewObject.m_TypeIdentifier == ObjectType.Worker || NewObject.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			Farmer component = NewObject.GetComponent<Farmer>();
			component.UpdatePositionToTilePosition(tileCoord);
			component.SetRotation(r);
		}
		else if (ObjectTypeList.Instance.GetIsBuilding(NewObject.m_TypeIdentifier))
		{
			Building component2 = NewObject.GetComponent<Building>();
			m_MovingBuildingList.Add(new MovingBuilding(Index, component2, tileCoord, r));
		}
		else
		{
			NewObject.GetComponent<TileCoordObject>().SetTilePosition(tileCoord);
		}
	}

	private void MoveObject(RecordingAction NewAction, bool Forwards)
	{
		BaseClass newObject = m_Objects[NewAction.m_UID];
		if (Forwards)
		{
			SetObjectPosition(NewAction.m_Index, newObject, NewAction.m_X, NewAction.m_Y, NewAction.m_Rotation);
		}
		else
		{
			SetObjectPosition(NewAction.m_Index, newObject, NewAction.m_OldX, NewAction.m_OldY, NewAction.m_OldRotation);
		}
	}

	private void DestroyObject(RecordingAction NewAction)
	{
		if (m_Log && !m_Objects.ContainsKey(NewAction.m_UID))
		{
			ErrorMessage.LogError("Missing UID " + NewAction.m_UID);
		}
		else if (m_Objects.ContainsKey(NewAction.m_UID))
		{
			BaseClass baseClass = m_Objects[NewAction.m_UID];
			m_Objects.Remove(NewAction.m_UID);
			if (ObjectTypeList.Instance.GetIsBuilding(NewAction.m_Type))
			{
				Building component = baseClass.GetComponent<Building>();
				MapManager.Instance.RemoveBuilding(component);
				BuildingManager.Instance.RefreshBuilding(component);
				PlotManager.Instance.RemoveObject(component);
				component.gameObject.SetActive(value: false);
				component.StopUsing();
			}
			else
			{
				baseClass.StopUsing();
			}
		}
	}

	private void ProcessSpecial(RecordingAction NewAction, bool Forwards)
	{
		BaseClass baseClass = m_Objects[NewAction.m_UID];
		RecordingStamp stamp = NewAction.m_Stamp;
		switch (stamp.GetSpecialMessage())
		{
		case RecordingStamp.SpecialMessage.SetBotHead:
			if (Forwards)
			{
				baseClass.GetComponent<Worker>().SetHead((ObjectType)stamp.SpecialData1);
			}
			else
			{
				baseClass.GetComponent<Worker>().SetHead((ObjectType)NewAction.m_OldSpecialData);
			}
			break;
		case RecordingStamp.SpecialMessage.SetBotFrame:
			if (Forwards)
			{
				baseClass.GetComponent<Worker>().SetFrame((ObjectType)stamp.SpecialData1);
			}
			else
			{
				baseClass.GetComponent<Worker>().SetFrame((ObjectType)NewAction.m_OldSpecialData);
			}
			break;
		case RecordingStamp.SpecialMessage.SetBotDrive:
			if (Forwards)
			{
				baseClass.GetComponent<Worker>().SetDrive((ObjectType)stamp.SpecialData1);
			}
			else
			{
				baseClass.GetComponent<Worker>().SetDrive((ObjectType)NewAction.m_OldSpecialData);
			}
			break;
		}
	}

	private void ProcessAction(RecordingAction NewAction, bool Forwards)
	{
		BaseClass baseClass = null;
		if (m_Objects.ContainsKey(NewAction.m_UID))
		{
			baseClass = m_Objects[NewAction.m_UID];
			if (baseClass == null)
			{
				if (m_Log)
				{
					Debug.Log("Null Object " + NewAction.m_UID);
				}
				return;
			}
		}
		else if (NewAction.m_Action == RecordingAction.Action.Move)
		{
			if (m_Log)
			{
				Debug.Log("Missing Object " + NewAction.m_UID);
			}
			return;
		}
		if (NewAction.m_Action != RecordingAction.Action.Move || !ObjectTypeList.Instance.GetIsBuilding(baseClass.m_TypeIdentifier))
		{
			CheckMoveBuildings();
		}
		if (m_LogVerbose)
		{
			Debug.Log(string.Concat(NewAction.m_Action, " ", NewAction.m_Type));
		}
		switch (NewAction.m_Action)
		{
		case RecordingAction.Action.Create:
			if (Forwards)
			{
				CreateObject(NewAction, NewAction.m_X, NewAction.m_Y, NewAction.m_Rotation);
			}
			else
			{
				DestroyObject(NewAction);
			}
			break;
		case RecordingAction.Action.Move:
			MoveObject(NewAction, Forwards);
			break;
		case RecordingAction.Action.Special:
			ProcessSpecial(NewAction, Forwards);
			break;
		case RecordingAction.Action.Destroy:
			if (Forwards)
			{
				DestroyObject(NewAction);
			}
			else
			{
				CreateObject(NewAction, NewAction.m_OldX, NewAction.m_OldY, NewAction.m_OldRotation);
			}
			break;
		case RecordingAction.Action.ShowPlot:
			PlotManager.Instance.GetPlotAtPlot(NewAction.m_X, NewAction.m_Y).SetVisible(Forwards);
			break;
		case RecordingAction.Action.ChangeTile:
			if (Forwards)
			{
				TileManager.Instance.SetTileType(new TileCoord(NewAction.m_X, NewAction.m_Y), (Tile.TileType)NewAction.m_Rotation);
			}
			else
			{
				TileManager.Instance.SetTileType(new TileCoord(NewAction.m_X, NewAction.m_Y), (Tile.TileType)NewAction.m_OldRotation);
			}
			break;
		}
	}

	private void ProcessFrameForward(List<RecordingAction> Actions, bool Forwards)
	{
		if (Forwards)
		{
			for (int i = 0; i < Actions.Count; i++)
			{
				RecordingAction newAction = Actions[i];
				ProcessAction(newAction, Forwards);
			}
		}
		else
		{
			for (int num = Actions.Count - 1; num >= 0; num--)
			{
				RecordingAction newAction2 = Actions[num];
				ProcessAction(newAction2, Forwards);
			}
		}
		CheckMoveBuildings();
	}

	private void PreProcessAction(RecordingAction NewAction, int Frame, bool Forwards)
	{
		if (m_Objects.ContainsKey(NewAction.m_UID) && ObjectTypeList.Instance.GetIsBuilding(m_Objects[NewAction.m_UID].m_TypeIdentifier))
		{
			m_PreProcessActionList.Add(NewAction);
			return;
		}
		switch (NewAction.m_Action)
		{
		case RecordingAction.Action.Create:
			if (Forwards)
			{
				m_PreProcessActionObjects.Add(NewAction.m_UID, NewAction);
				m_PreProcessActionList.Add(NewAction);
				m_ObjectsCreated++;
			}
			else if (m_PreProcessActionObjects.ContainsKey(NewAction.m_UID))
			{
				m_PreProcessActionObjects.Remove(NewAction.m_UID);
				m_PreProcessActionList.Remove(NewAction);
				m_ActionsDropped++;
			}
			else
			{
				m_PreProcessActionList.Add(NewAction);
			}
			break;
		case RecordingAction.Action.Move:
			m_PreProcessActionList.Add(NewAction);
			if (m_PreProcessActionObjects.ContainsKey(NewAction.m_UID))
			{
				m_PreProcessActionObjects.Remove(NewAction.m_UID);
			}
			break;
		case RecordingAction.Action.Destroy:
			if (Forwards)
			{
				if (m_PreProcessActionObjects.ContainsKey(NewAction.m_UID))
				{
					m_PreProcessActionObjects.Remove(NewAction.m_UID);
					m_PreProcessActionList.Remove(NewAction);
					m_ActionsDropped++;
				}
				else
				{
					m_PreProcessActionList.Add(NewAction);
				}
			}
			else
			{
				m_PreProcessActionObjects.Add(NewAction.m_UID, NewAction);
				m_PreProcessActionList.Add(NewAction);
				m_ObjectsCreated++;
			}
			break;
		case RecordingAction.Action.ShowPlot:
			m_PreProcessActionList.Add(NewAction);
			break;
		case RecordingAction.Action.ChangeTile:
			m_PreProcessActionList.Add(NewAction);
			m_TilesChanged++;
			break;
		case RecordingAction.Action.Special:
			break;
		}
	}

	private void PreProcessFrameForward(int Frame, bool Forwards)
	{
		List<RecordingAction> list = m_Timeline[Frame];
		if (Forwards)
		{
			for (int i = 0; i < list.Count; i++)
			{
				RecordingAction newAction = list[i];
				PreProcessAction(newAction, Frame, Forwards);
			}
			return;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			RecordingAction newAction2 = list[num];
			PreProcessAction(newAction2, Frame, Forwards);
		}
	}

	private void ProcessFramesUntilTimeUp(bool Direction, bool New)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (m_Frame != m_OldFrame)
		{
			if (Direction)
			{
				m_OldFrame++;
			}
			if (m_Log)
			{
				Debug.Log("******************************** Frame = " + m_OldFrame);
			}
			if (New)
			{
				PreProcessFrameForward(m_OldFrame, Direction);
			}
			else
			{
				ProcessFrameForward(m_Timeline[m_OldFrame], Direction);
			}
			if (!Direction)
			{
				m_OldFrame--;
			}
			if (Time.realtimeSinceStartup - realtimeSinceStartup > 0.05f)
			{
				float progress = (float)m_OldFrame / (float)m_Timeline.Count;
				m_Slider.SetProgress(progress);
				return;
			}
		}
		m_Slider.SetProgressToTarget();
	}

	private void ProcessFrameChange()
	{
		m_PreProcessActionList = new List<RecordingAction>();
		m_PreProcessActionObjects = new Dictionary<int, RecordingAction>();
		bool flag = false;
		m_ActionsDropped = 0;
		m_ObjectsCreated = 0;
		m_TilesChanged = 0;
		bool flag2 = false;
		flag = ((m_OldFrame < m_Frame) ? true : false);
		ProcessFramesUntilTimeUp(flag, flag2);
		if (flag2)
		{
			m_ObjectsCreated -= m_ActionsDropped;
			Debug.Log(m_PreProcessActionList.Count + " processed : " + m_ObjectsCreated + " created : " + m_ActionsDropped + " Dropped : " + m_TilesChanged + " Tiles");
			ProcessFrameForward(m_PreProcessActionList, flag);
		}
	}

	public void TogglePlay()
	{
		m_Playing = !m_Playing;
		PlaybackControl.Instance.SetPlaying(m_Playing);
		if (m_Playing && m_Frame == m_Timeline.Count - 1)
		{
			m_Time = 0f;
			m_Frame = 0;
			CameraRecordingManager.Instance.UpdateToFrame(m_Frame);
		}
	}

	private void SetSliderToFrame()
	{
		float value = m_Time / ((float)(m_Timeline.Count - 1) * RecordingManager.Instance.m_RecordFrequency);
		m_Slider.SetValue(value);
	}

	private void Update()
	{
		if (m_Slider == null)
		{
			return;
		}
		if (m_Slider.m_UserMoving)
		{
			float value = m_Slider.GetValue();
			m_Time = (float)(m_Timeline.Count - 1) * RecordingManager.Instance.m_RecordFrequency * value;
			m_Frame = (int)(m_Time / RecordingManager.Instance.m_RecordFrequency);
			if (m_Playing)
			{
				TogglePlay();
			}
			if (m_Frame == m_OldFrame)
			{
				m_Slider.SetProgressToTarget();
			}
		}
		if (!m_Slider.m_UserMoving)
		{
			if (m_Playing)
			{
				m_Time += m_PlaybackSpeed * TimeManager.Instance.m_NormalDelta;
				m_Frame = (int)(m_Time / RecordingManager.Instance.m_RecordFrequency);
				if (m_Frame >= m_Timeline.Count)
				{
					m_Frame = m_Timeline.Count - 1;
					m_Time = (float)m_Frame * RecordingManager.Instance.m_RecordFrequency;
					TogglePlay();
				}
				SetSliderToFrame();
			}
			else if (MyInputManager.m_Rewired.GetButtonDown("PrevFrame") || MyInputManager.m_Rewired.GetButtonRepeating("PrevFrame"))
			{
				if (m_Time > 0f)
				{
					m_Time -= m_PlaybackSpeed * TimeManager.Instance.m_NormalDelta;
					if (m_Time < 0f)
					{
						m_Time = 0f;
					}
					m_Frame = (int)(m_Time / RecordingManager.Instance.m_RecordFrequency);
					SetSliderToFrame();
				}
			}
			else if ((MyInputManager.m_Rewired.GetButtonDown("NextFrame") || MyInputManager.m_Rewired.GetButtonRepeating("NextFrame")) && m_Time < (float)m_Timeline.Count * RecordingManager.Instance.m_RecordFrequency)
			{
				m_Time += m_PlaybackSpeed * TimeManager.Instance.m_NormalDelta;
				if (m_Time > (float)m_Timeline.Count * RecordingManager.Instance.m_RecordFrequency)
				{
					m_Time = (float)m_Timeline.Count * RecordingManager.Instance.m_RecordFrequency;
					m_Frame = m_Timeline.Count - 1;
				}
				else
				{
					m_Frame = (int)(m_Time / RecordingManager.Instance.m_RecordFrequency);
				}
				SetSliderToFrame();
			}
		}
		if (m_Frame != m_OldFrame)
		{
			ProcessFrameChange();
			CameraRecordingManager.Instance.UpdateToFrame(m_Frame);
		}
		if (!TimeManager.Instance.m_PauseTimeEnabled)
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Whistle"))
		{
			TogglePlay();
		}
	}
}
