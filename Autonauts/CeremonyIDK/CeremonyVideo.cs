using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeremonyVideo : CeremonyBase
{
	public static CeremonyVideo Instance;

	private FarmerPlayer m_Player;

	private CameraStateCeremony m_CameraState;

	private bool m_Done;

	private Coroutine m_Coroutine;

	private Worker m_TargetWorker;

	public static void Init()
	{
		Worker.m_AllDriveInfo[1].m_MoveInitialDelay = 0;
		Worker.m_AllHeadInfo[1].m_FindNearestDelay = 0;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		m_Player = players[0].GetComponent<FarmerPlayer>();
		CameraManager.Instance.SetState(CameraManager.State.Ceremony);
		m_CameraState = (CameraStateCeremony)CameraManager.Instance.m_CurrentState;
		for (int i = 0; i < PlotManager.Instance.m_PlotsHigh; i++)
		{
			for (int j = 0; j < PlotManager.Instance.m_PlotsWide; j++)
			{
				PlotManager.Instance.GetPlotAtPlot(j, i).SetVisible(Visible: true);
			}
		}
		m_Done = false;
		m_Coroutine = StartCoroutine(Stuff());
	}

	private void CreateEmoticon(Vector3 Position, string Name, float Duration)
	{
		Emoticon component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Emoticon, Position, Quaternion.identity).GetComponent<Emoticon>();
		component.SetScale(4f);
		component.SetEmoticon(Name, Duration, "");
		component.SetWorldPosition(Position + new Vector3(1f, 3f, 0f));
	}

	private void CreateEmoticon(BaseClass TargetObject, string Name, float Duration)
	{
		Vector3 position = TargetObject.transform.position;
		CreateEmoticon(position, Name, Duration);
	}

	private void PlayerGoToAndAction(int x, int y, AFO.AT AltAction = AFO.AT.Primary)
	{
		TileCoord tileCoord = new TileCoord(x, y);
		Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
		Actionable actionable = plotAtTile.GetSelectableObjectAtTile(tileCoord);
		if (actionable == null)
		{
			actionable = plotAtTile;
		}
		if ((bool)actionable.GetComponent<Building>())
		{
			tileCoord = actionable.GetComponent<Building>().GetAccessPosition();
		}
		m_Player.GoToAndAction(tileCoord, actionable, AltAction);
	}

	private void PlayerGiveToWorker(int x, int y)
	{
		TileCoord tileCoord = new TileCoord(x, y);
		Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
		Actionable actionable = plotAtTile.GetObjectTypeAtTile(ObjectType.Worker, tileCoord);
		if (actionable == null)
		{
			actionable = plotAtTile;
		}
		m_Player.GoToAndAction(tileCoord, actionable, AFO.AT.Secondary);
	}

	private void PlayerWhistle()
	{
		m_Player.GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.Select);
		m_Player.Whistle(Start: true);
	}

	private void WorkerStartTeach(int x, int y)
	{
		m_Player.Whistle(Start: false);
		TileCoord position = new TileCoord(x, y);
		Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(position);
		m_TargetWorker = plotAtTile.GetObjectTypeAtTile(ObjectType.Worker, position).GetComponent<Worker>();
		AudioManager.Instance.StartEvent("WorkerAcknowledgeLearn", m_TargetWorker);
		m_TargetWorker.StartLearning(m_Player);
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.SetState(GameStateManager.State.TeachWorker);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().SetTarget(m_TargetWorker);
	}

	private void WorkerAddRepeat()
	{
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_ScriptEdit.OnClickRepeat();
	}

	private void WorkerEndTeach()
	{
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_ScriptEdit.OnClickGo();
	}

	private void WorkerHideBrain()
	{
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().ClearSelectedWorkers();
	}

	private void End()
	{
		CameraManager.Instance.SetState(CameraManager.State.Normal);
		m_Done = true;
	}

	private IEnumerator Stuff()
	{
		m_CameraState.SetTrackObject(m_Player.gameObject);
		PlayerGoToAndAction(214, 216);
		while (m_Player.m_State != 0)
		{
			yield return new WaitForSeconds(0f);
		}
		m_Player.SetRotation(6);
		PlayerWhistle();
		yield return new WaitForSeconds(1f);
		WorkerStartTeach(214, 215);
		yield return new WaitForSeconds(1f);
		WorkerEndTeach();
		PlayerGoToAndAction(208, 215);
		while (m_Player.m_State != 0)
		{
			yield return new WaitForSeconds(0f);
		}
		PlayerGoToAndAction(218, 214, AFO.AT.Secondary);
		while (m_Player.m_State != 0)
		{
			yield return new WaitForSeconds(0f);
		}
		m_CameraState.SetTrackObject(null);
		CameraManager.Instance.StartSpline();
		yield return new WaitForSeconds(1f);
		WorkerHideBrain();
		while (CameraManager.Instance.IsPanning())
		{
			yield return new WaitForSeconds(0f);
		}
		End();
	}

	public void RocketFinished()
	{
	}

	private void Update()
	{
		CeremonyManager.Instance.CeremonyEnded();
		if (Input.GetKey(KeyCode.Escape))
		{
			StopCoroutine(m_Coroutine);
			End();
		}
		if (m_Done)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
