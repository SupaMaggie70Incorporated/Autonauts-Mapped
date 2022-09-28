using System;
using System.Collections.Generic;
using DigitalRuby.Tween;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
	public enum State
	{
		Normal,
		Free,
		TrackObject,
		Ceremony,
		MainMenu,
		PlaySequence,
		Total
	}

	public enum ShaderType
	{
		Normal,
		Connected,
		WalledAreas,
		Total
	}

	public static CameraManager Instance;

	public static bool m_ValidInstance = false;

	public State m_State;

	public CameraState[] m_States;

	public CameraState m_CurrentState;

	[HideInInspector]
	public Camera m_Camera;

	private Camera m_SecondCamera;

	private GameObject m_GrabPlane;

	public Vector3 m_CameraPosition;

	public Vector3 m_CameraFinalPosition;

	public Quaternion m_CameraRotation;

	private float m_CameraRotationX;

	public Vector3 m_CameraZoomedPosition;

	private int m_CameraAxisRotation;

	public float m_Distance;

	public static float m_MinDistance = 10f;

	public static float m_MaxDistance = 100f;

	public bool m_Grabbing;

	private Vector3 m_GrabbedPlanePosition;

	private Vector3 m_GrabbedCameraPosition;

	private bool m_MouseWheelFocus;

	public GameObject m_FollowWorker;

	public bool m_Panning;

	private bool m_Orbit;

	private bool m_LookAt;

	public Vector3 m_LookAtPosition;

	public bool m_PlayingSpline;

	private SplineController m_Spline;

	private ShaderType m_CurrentShader;

	private CustomStandaloneInputModule m_EventSystem;

	private bool m_InGame;

	private bool m_AutoDOFEnabled;

	private float m_DOFFocalDistance;

	private bool m_ReadyMoveEvent;

	private Vector3 m_MoveEventPosition;

	private bool m_ReadyZoomEvent;

	private float m_ZoomEventDistance;

	private bool m_FocalPointFound;

	private float m_BestDistance;

	private Vector3 m_BestFocalPoint;

	private float m_CurrentFocalDistance;

	private float m_TargetFocalDistance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			m_ValidInstance = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		Create();
		Restart();
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			m_ValidInstance = false;
		}
	}

	private void Create()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/FollowWorker", typeof(GameObject));
		m_FollowWorker = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, null).gameObject;
		m_FollowWorker.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		m_FollowWorker.SetActive(value: false);
		UnityEngine.Object.DontDestroyOnLoad(m_FollowWorker);
	}

	public void Restart()
	{
		m_MouseWheelFocus = true;
		m_Distance = 13f;
		m_Panning = false;
		m_CameraPosition = new Vector3(0f, 0f, 0f);
		SetDistance(m_Distance);
		SetAxisRotation(0f);
		m_Camera = base.transform.Find("Camera").GetComponent<Camera>();
		m_Camera.transform.position = m_CameraZoomedPosition;
		m_Camera.transform.rotation = m_CameraRotation;
		m_Camera.opaqueSortMode = OpaqueSortMode.NoDistanceSort;
		m_SecondCamera = base.transform.Find("SecondCamera").GetComponent<Camera>();
		m_SecondCamera.transform.position = m_CameraZoomedPosition;
		m_SecondCamera.transform.rotation = m_CameraRotation;
		m_Grabbing = false;
		m_GrabPlane = base.transform.Find("GrabPlane").gameObject;
		m_PlayingSpline = false;
		m_Spline = base.transform.Find("Spline").GetComponent<SplineController>();
		m_States = new CameraState[6];
		m_States[0] = new CameraStateNormal(m_Camera);
		m_States[1] = new CameraStateFree(m_Camera);
		m_States[2] = new CameraStateTrackObject(m_Camera);
		m_States[3] = new CameraStateCeremony(m_Camera);
		m_States[4] = new CameraStateMainMenu(m_Camera);
		m_States[5] = new CameraStatePlaySequence(m_Camera);
		SetState(State.Normal);
		m_CurrentShader = ShaderType.Normal;
		m_EventSystem = GameObject.Find("EventSystem").GetComponent<CustomStandaloneInputModule>();
		m_InGame = false;
		if (SceneManager.GetActiveScene().name == "Main" || SceneManager.GetActiveScene().name == "Playback")
		{
			m_InGame = true;
		}
	}

	public void SetGlobalShader(ShaderType NewType)
	{
		if (!(m_Camera == null))
		{
			m_CurrentShader = NewType;
			if (NewType != 0)
			{
				string text = (new string[3] { "", "Connected", "WalledAreas" })[(int)NewType];
				Shader shader = (Shader)Resources.Load("Shaders/" + text, typeof(Shader));
				m_Camera.SetReplacementShader(shader, "RenderType");
			}
			else
			{
				m_Camera.SetReplacementShader(null, "");
			}
		}
	}

	public void ToggleConnected()
	{
		if (m_CurrentShader == ShaderType.Connected)
		{
			SetGlobalShader(ShaderType.Normal);
		}
		else
		{
			SetGlobalShader(ShaderType.Connected);
		}
	}

	public void ToggleWalledAreas()
	{
		if (m_CurrentShader == ShaderType.WalledAreas)
		{
			SetGlobalShader(ShaderType.Normal);
		}
		else
		{
			SetGlobalShader(ShaderType.WalledAreas);
		}
	}

	public void Focus(Vector3 Position)
	{
		m_CameraPosition = Position;
		m_SecondCamera.transform.position = m_CameraPosition + m_CameraZoomedPosition;
		m_GrabPlane.transform.position = m_CameraPosition;
	}

	public void SetDistance(float Distance)
	{
		m_Distance = Distance;
		UpdateZoomedDistance();
	}

	private void SetAxisRotation(float RotationX)
	{
		m_CameraRotationX = RotationX;
		float num = m_CameraRotationX;
		if (num >= -0.1f && num < 0.1f)
		{
			num = 0.1f;
		}
		m_CameraRotation = Quaternion.Euler(45f, num, 0f);
		UpdateZoomedDistance();
	}

	private Vector3 GetRotatedPosition(Vector3 Old, float Rotation)
	{
		if (Rotation >= -0.1f && Rotation < 0.1f)
		{
			Rotation = 0.1f;
		}
		Rotation = Rotation / 180f * (float)Math.PI;
		return new Vector3(Mathf.Sin(Rotation) * Old.z + Mathf.Cos(Rotation) * Old.x, Old.y, Mathf.Cos(Rotation) * Old.z + Mathf.Sin(Rotation) * Old.x);
	}

	private void UpdateZoomedDistance()
	{
		m_CameraZoomedPosition = GetRotatedPosition(new Vector3(0f, m_Distance, 0f - m_Distance), m_CameraRotationX);
	}

	public void MouseWheelFocusLost()
	{
		m_MouseWheelFocus = false;
	}

	public void UpdateMouseWheel()
	{
		if (m_EventSystem.IsHover())
		{
			return;
		}
		if (m_MouseWheelFocus)
		{
			float axis = MyInputManager.m_Rewired.GetAxis("MouseScrollWheel");
			if (axis > 0f)
			{
				m_Distance -= m_Distance * 0.1f;
				if (m_Distance < m_MinDistance)
				{
					m_Distance = m_MinDistance;
				}
				SetDistance(m_Distance);
			}
			if (axis < 0f)
			{
				m_Distance += m_Distance * 0.1f;
				if (m_Distance > m_MaxDistance)
				{
					m_Distance = m_MaxDistance;
				}
				SetDistance(m_Distance);
			}
		}
		m_MouseWheelFocus = true;
	}

	public void StopMousePan()
	{
		if (m_Grabbing)
		{
			m_Grabbing = false;
			if (!m_Orbit)
			{
				CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: false);
				m_SecondCamera.transform.position = m_CameraPosition + m_CameraZoomedPosition;
				m_GrabPlane.transform.position = m_CameraPosition;
			}
		}
	}

	public void UpdateMousePan(bool AllowPan)
	{
		if ((Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftShift)) || Input.GetMouseButton(2))
		{
			m_SecondCamera.pixelRect = m_Camera.pixelRect;
			if (!m_Grabbing)
			{
				m_Grabbing = true;
				if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(2))
				{
					m_Orbit = true;
					m_GrabbedCameraPosition = Input.mousePosition;
				}
				else
				{
					m_Orbit = false;
					if (AllowPan)
					{
						CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: true);
						m_GrabbedCameraPosition = m_CameraPosition;
						m_SecondCamera.transform.position = m_CameraPosition + m_CameraZoomedPosition;
						m_GrabPlane.transform.position = m_CameraPosition;
						RaycastHit hitInfo = default(RaycastHit);
						if (Physics.Raycast(m_SecondCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000f, 256))
						{
							m_GrabbedPlanePosition = hitInfo.point;
						}
					}
				}
			}
			if (m_Orbit)
			{
				Vector3 vector = Input.mousePosition - m_GrabbedCameraPosition;
				m_GrabbedCameraPosition = Input.mousePosition;
				float num = vector.x / 5f;
				m_CameraRotationX += num;
				SetAxisRotation(m_CameraRotationX);
			}
			else if (AllowPan)
			{
				RaycastHit hitInfo2 = default(RaycastHit);
				if (Physics.Raycast(m_SecondCamera.ScreenPointToRay(Input.mousePosition), out hitInfo2, 1000f, 256))
				{
					Vector3 vector2 = hitInfo2.point - m_GrabbedPlanePosition;
					Vector3 position = m_GrabbedCameraPosition - vector2;
					m_CameraPosition = CheckMapBounds(position);
				}
			}
		}
		else if (AllowPan)
		{
			StopMousePan();
		}
		else
		{
			m_Grabbing = false;
		}
	}

	public Vector3 CheckMapBounds(Vector3 Position)
	{
		if (TileManager.Instance == null)
		{
			return Position;
		}
		if (Position.x < 0f)
		{
			Position.x = 0f;
		}
		float num = (float)TileManager.Instance.m_TilesWide * Tile.m_Size;
		if (Position.x > num)
		{
			Position.x = num;
		}
		if (Position.z > 0f)
		{
			Position.z = 0f;
		}
		num = (float)TileManager.Instance.m_TilesHigh * Tile.m_Size;
		if (Position.z < 0f - num)
		{
			Position.z = 0f - num;
		}
		if (Position.y < 0f)
		{
			Position.y = 0f;
		}
		num = 100f * Tile.m_Size;
		if (Position.y > num)
		{
			Position.y = num;
		}
		return Position;
	}

	public void UpdateRotateReset()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Rotate") && !m_Grabbing)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				m_CameraRotationX = 0f;
			}
			SetAxisRotation(m_CameraRotationX);
		}
	}

	public void UpdateKeyboardPan()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		float num = m_Distance / 10f;
		float num2 = 30f * TimeManager.Instance.m_NormalDeltaUnscaled * num;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			num2 *= 2f;
		}
		float num3 = 0f;
		float num4 = 0f;
		if (MyInputManager.m_Rewired.GetButton("PanUp"))
		{
			num4 = -1f;
		}
		else if (MyInputManager.m_Rewired.GetButton("PanDown"))
		{
			num4 = 1f;
		}
		if (MyInputManager.m_Rewired.GetButton("PanLeft"))
		{
			num3 = -1f;
		}
		else if (MyInputManager.m_Rewired.GetButton("PanRight"))
		{
			num3 = 1f;
		}
		num3 *= num2;
		num4 *= num2;
		float f = m_CameraRotationX / 180f * (float)Math.PI;
		m_CameraPosition.x += Mathf.Cos(f) * num3 - Mathf.Sin(f) * num4;
		m_CameraPosition.z -= Mathf.Cos(f) * num4 + Mathf.Sin(f) * num3;
		m_CameraPosition = CheckMapBounds(m_CameraPosition);
		if (MyInputManager.m_Rewired.GetButton("Recenter"))
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players.Count > 0)
			{
				m_CameraPosition.x = players[0].transform.position.x;
				m_CameraPosition.z = players[0].transform.position.z;
				players[0].GetComponent<FarmerPlayer>().ShowTarget();
				if ((bool)QuestManager.Instance)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.RecentreCamera, Bot: false, null, null);
				}
			}
		}
		UpdateRotateReset();
		if (MyInputManager.m_Rewired.GetButton("PanIn"))
		{
			m_Distance -= m_Distance * 1f * TimeManager.Instance.m_NormalDelta;
			if (m_Distance < m_MinDistance)
			{
				m_Distance = m_MinDistance;
			}
			SetDistance(m_Distance);
		}
		if (MyInputManager.m_Rewired.GetButton("PanOut"))
		{
			m_Distance += m_Distance * 1f * TimeManager.Instance.m_NormalDelta;
			if (m_Distance > m_MaxDistance)
			{
				m_Distance = m_MaxDistance;
			}
			SetDistance(m_Distance);
		}
	}

	public void FinaliseCamera(Vector3 Position, Quaternion Rotation)
	{
		m_CameraFinalPosition = Position;
		if (!m_Panning)
		{
			m_Camera.transform.position = Position;
		}
		m_Camera.transform.rotation = Rotation;
		m_SecondCamera.transform.rotation = Rotation;
	}

	public void TrackObject(GameObject NewObject)
	{
		SetState(State.TrackObject);
		((CameraStateTrackObject)m_CurrentState).SetTrackObject(NewObject);
	}

	public void PanTo(BaseClass NewObject, Vector3 ExtraOffset, float Distance, float Duration)
	{
		Vector3 vector = ((!SpawnAnimationManager.Instance.GetIsObjectSpawning(NewObject)) ? NewObject.transform.position : SpawnAnimationManager.Instance.GetObjectTargetLocation(NewObject));
		vector += ExtraOffset;
		Vector3 vector2 = (m_CameraZoomedPosition = GetRotatedPosition(new Vector3(0f, Distance, 0f - Distance), m_CameraRotationX));
		m_Camera.gameObject.Tween("MoveCamera", m_Camera.transform.position, vector + vector2, Duration, TweenScaleFunctions.CubicEaseInOut, UpdatePan, EndPan);
		m_Panning = true;
		m_LookAt = false;
		m_CameraPosition = vector;
		SetDistance(Distance);
	}

	public void PanTo(Vector3 NewPosition, float Distance, float Duration)
	{
		Vector3 vector = (m_CameraZoomedPosition = GetRotatedPosition(new Vector3(0f, Distance, 0f - Distance), m_CameraRotationX));
		m_Camera.gameObject.Tween("MoveCamera", m_Camera.transform.position, NewPosition + vector, Duration, TweenScaleFunctions.CubicEaseInOut, UpdatePan, EndPan);
		m_Panning = true;
		m_LookAt = false;
		m_CameraPosition = NewPosition;
		SetDistance(Distance);
	}

	public void PanTo(Vector3 NewPosition, float Duration, Vector3 LookAtPosition, bool TweenLookAt = false)
	{
		m_Camera.gameObject.Tween("MoveCamera", m_Camera.transform.position, NewPosition, Duration, TweenScaleFunctions.CubicEaseInOut, UpdatePan, EndPan);
		if (TweenLookAt)
		{
			m_Camera.gameObject.Tween("LookCamera", m_LookAtPosition, LookAtPosition, Duration, TweenScaleFunctions.CubicEaseInOut, UpdateLookAt, EndLookAt);
		}
		m_Panning = true;
		m_LookAt = true;
		if (!TweenLookAt)
		{
			m_LookAtPosition = LookAtPosition;
		}
		m_CameraPosition = NewPosition;
	}

	public void SplineEnd()
	{
		m_PlayingSpline = false;
	}

	public void StartSpline(List<Transform> NewTransforms = null, List<float> NewTimes = null)
	{
		m_PlayingSpline = true;
		m_Spline.AutoStart = true;
		m_Spline.Go(SplineEnd, NewTransforms, NewTimes);
	}

	public void UpdatePan(ITween<Vector3> t)
	{
		m_Camera.transform.position = t.CurrentValue;
		m_CameraFinalPosition = t.CurrentValue;
	}

	public void EndPan(ITween<Vector3> t)
	{
		if (m_LookAt)
		{
			m_Camera.transform.LookAt(m_LookAtPosition);
		}
		m_Panning = false;
	}

	public void UpdateLookAt(ITween<Vector3> t)
	{
		m_LookAtPosition = t.CurrentValue;
	}

	public void EndLookAt(ITween<Vector3> t)
	{
	}

	public bool IsPanning()
	{
		return m_Panning;
	}

	public void SetState(State NewState)
	{
		if (m_CurrentState != null)
		{
			m_CurrentState.EndUse();
		}
		StopMousePan();
		State state = m_State;
		m_State = NewState;
		m_CurrentState = m_States[(int)m_State];
		m_CurrentState.StartUse();
		if (state == State.Free && NewState == State.Normal)
		{
			UpdateInput();
		}
	}

	public void UpdateInput()
	{
		m_CurrentState.UpdateInput();
	}

	public void SetDOFEnabled(bool Enabled)
	{
		if (!GameStateManager.Instance || !GameStateManager.Instance.GetCurrentState() || GameStateManager.Instance.GetCurrentState().m_BaseState != GameStateManager.State.Settings)
		{
			Instance.m_Camera.GetComponent<PostProcessingBehaviour>().profile.depthOfField.enabled = Enabled;
		}
	}

	public void SetAutoDOFEnabled(bool Enabled)
	{
		if (!GameStateManager.Instance || !(GameStateManager.Instance.GetCurrentState() != null) || GameStateManager.Instance.GetCurrentState().m_BaseState != GameStateManager.State.Settings)
		{
			m_AutoDOFEnabled = Enabled;
		}
	}

	public void EnableVignette(bool Enabled)
	{
		PostProcessingBehaviour component = Instance.m_Camera.GetComponent<PostProcessingBehaviour>();
		VignetteModel.Settings settings = component.profile.vignette.settings;
		if (Enabled)
		{
			settings.smoothness = 1f;
		}
		else
		{
			settings.smoothness = 0.2f;
		}
		component.profile.vignette.settings = settings;
	}

	public float GetDOFFocalDistance()
	{
		return m_DOFFocalDistance;
	}

	public void SetDOF(float FocalDistance, float FocalLength, float Aperture)
	{
		if (!GameStateManager.Instance || !(GameStateManager.Instance.GetCurrentState() != null) || GameStateManager.Instance.GetCurrentState().m_BaseState != GameStateManager.State.Settings)
		{
			m_DOFFocalDistance = FocalDistance;
			PostProcessingBehaviour component = Instance.m_Camera.GetComponent<PostProcessingBehaviour>();
			DepthOfFieldModel.Settings settings = component.profile.depthOfField.settings;
			settings.focusDistance = FocalDistance;
			settings.focalLength = FocalLength;
			settings.aperture = Aperture;
			component.profile.depthOfField.settings = settings;
		}
	}

	public void SetPausedDOFEffect()
	{
		SetDOFEnabled(Enabled: true);
		SetAutoDOFEnabled(Enabled: false);
		SetDOF(0f, 0f, 0f);
		EnableVignette(Enabled: true);
		HudManager.Instance.SetIndicatorsVisible(Visible: false);
	}

	public void RestorePausedDOFEffect()
	{
		HudManager.Instance.SetIndicatorsVisible(Visible: true);
		Instance.EnableVignette(Enabled: false);
		SetDOFEnabled(Enabled: false);
	}

	private void TestDistance(GameStateBase NewState, Vector3 Offset)
	{
		Offset.x += HudManager.Instance.m_HalfScaledWidth;
		Offset.y += HudManager.Instance.m_HalfScaledHeight;
		Offset = HudManager.Instance.CanvasToScreen(Offset);
		if ((bool)NewState.TestCollisionOffset(out var CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: true, TestMisc: true, TestWorkersDead: true, Offset))
		{
			float magnitude = (m_Camera.transform.position - CollisionPoint).magnitude;
			if (magnitude < m_BestDistance)
			{
				m_BestDistance = magnitude;
				m_BestFocalPoint = CollisionPoint;
				m_FocalPointFound = true;
			}
		}
	}

	private void UpdateAutoDOF()
	{
		if (!m_AutoDOFEnabled || !m_InGame || !GameStateManager.Instance)
		{
			return;
		}
		GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
		if (!currentState)
		{
			return;
		}
		m_FocalPointFound = false;
		m_BestDistance = 1E+14f;
		float num = 20f;
		TestDistance(currentState, default(Vector3));
		TestDistance(currentState, new Vector3(0f - num, 0f));
		TestDistance(currentState, new Vector3(num, 0f));
		TestDistance(currentState, new Vector3(0f, 0f - num));
		TestDistance(currentState, new Vector3(0f, num));
		if (m_FocalPointFound)
		{
			if ((bool)TileManager.Instance)
			{
				if (m_BestFocalPoint.x < 0f)
				{
					m_BestFocalPoint.x = 0f;
				}
				if (m_BestFocalPoint.y < 0f)
				{
					m_BestFocalPoint.y = 0f;
				}
				if (m_BestFocalPoint.x > (float)TileManager.Instance.m_TilesWide * Tile.m_Size)
				{
					m_BestFocalPoint.x = (float)TileManager.Instance.m_TilesWide * Tile.m_Size;
				}
				if (m_BestFocalPoint.y > (float)TileManager.Instance.m_TilesHigh * Tile.m_Size)
				{
					m_BestFocalPoint.y = (float)TileManager.Instance.m_TilesHigh * Tile.m_Size;
				}
			}
			m_TargetFocalDistance = (m_Camera.transform.position - m_BestFocalPoint).magnitude;
		}
		m_CurrentFocalDistance = m_TargetFocalDistance;
		DepthOfFieldModel.Settings settings = Instance.m_Camera.GetComponent<PostProcessingBehaviour>().profile.depthOfField.settings;
		SetDOF(m_CurrentFocalDistance, settings.focalLength, settings.aperture);
	}

	private void UpdateShadows()
	{
		float num = m_Camera.transform.position.y * 2.5f;
		if (num < 70f)
		{
			num = 70f;
		}
		QualitySettings.shadowDistance = num;
	}

	public void ReadyMoveEvent()
	{
		m_ReadyMoveEvent = true;
		m_MoveEventPosition = m_CameraPosition;
	}

	public void ReadyZoomEvent()
	{
		m_ReadyZoomEvent = true;
		m_ZoomEventDistance = m_Distance;
	}

	private void UpdateTestEvents()
	{
		if ((bool)GameStateManager.Instance && GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
		{
			if (m_ReadyMoveEvent && (m_CameraPosition - m_MoveEventPosition).magnitude > 10f)
			{
				m_ReadyMoveEvent = false;
				QuestManager.Instance.AddEvent(QuestEvent.Type.MoveCamera, Bot: false, null, null);
			}
			if (m_ReadyZoomEvent && Mathf.Abs(m_Distance - m_ZoomEventDistance) > 3f)
			{
				m_ReadyZoomEvent = false;
				QuestManager.Instance.AddEvent(QuestEvent.Type.ZoomCamera, Bot: false, null, null);
			}
		}
	}

	private void Update()
	{
		m_CurrentState.UpdateCamera();
		UpdateAutoDOF();
		float[] layerCullDistances = m_Camera.layerCullDistances;
		layerCullDistances[13] = 75f;
		m_Camera.layerCullDistances = layerCullDistances;
		UpdateTestEvents();
	}

	private void LateUpdate()
	{
		m_CurrentState.LateUpdate();
		if (m_PlayingSpline)
		{
			FinaliseCamera(m_Spline.transform.position, m_Spline.transform.rotation);
		}
		if (m_Panning && m_LookAt)
		{
			m_Camera.transform.LookAt(m_LookAtPosition);
		}
		UpdateShadows();
	}
}
