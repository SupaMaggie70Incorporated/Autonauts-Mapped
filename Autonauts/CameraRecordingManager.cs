using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class CameraRecordingManager : MonoBehaviour
{
	public static CameraRecordingManager Instance;

	private List<CameraSnap> m_Snaps;

	private int m_LastFrame;

	private CameraSnap m_LastSnap;

	private void Awake()
	{
		Instance = this;
		m_Snaps = new List<CameraSnap>();
	}

	private static int SortSnapsByFrame(CameraSnap p1, CameraSnap p2)
	{
		return p1.f - p2.f;
	}

	public void RecordCameraSnap(bool Normal)
	{
		CameraSnap cameraSnap = new CameraSnap();
		cameraSnap.f = PlaybackManager.Instance.m_Frame;
		cameraSnap.m_Free = Normal;
		cameraSnap.m_Position = CameraManager.Instance.m_Camera.transform.position;
		cameraSnap.m_Rotation = CameraManager.Instance.m_Camera.transform.rotation;
		cameraSnap.m_DOFEnabled = SettingsManager.Instance.m_DOFEnabled;
		cameraSnap.m_AutoDOFEnabled = SettingsManager.Instance.m_AutoDOFEnabled;
		cameraSnap.m_DOFFocalDistance = SettingsManager.Instance.m_DOFFocalDistance;
		cameraSnap.m_DOFFocalLength = SettingsManager.Instance.m_DOFFocalLength;
		cameraSnap.m_DOFAperture = SettingsManager.Instance.m_DOFAperture;
		m_Snaps.Add(cameraSnap);
		m_Snaps.Sort(SortSnapsByFrame);
		Debug.Log("Grab " + PlaybackManager.Instance.m_Frame);
	}

	private void DeleteCameraSnap()
	{
		CameraSnap cameraSnap = null;
		foreach (CameraSnap snap in m_Snaps)
		{
			if (snap.f <= PlaybackManager.Instance.m_Frame)
			{
				cameraSnap = snap;
			}
		}
		if (cameraSnap != null)
		{
			m_Snaps.Remove(cameraSnap);
			m_Snaps.Sort(SortSnapsByFrame);
			Debug.Log("Delete " + cameraSnap.f);
		}
	}

	public void PlayCameraSnap(CameraSnap NewSnap)
	{
		CameraManager.Instance.m_Camera.transform.position = NewSnap.m_Position;
		if (NewSnap.m_Rotation.eulerAngles.y == 0f)
		{
			NewSnap.m_Rotation.eulerAngles = new Vector3(NewSnap.m_Rotation.eulerAngles.x, 0.01f, NewSnap.m_Rotation.eulerAngles.z);
		}
		CameraManager.Instance.m_Camera.transform.rotation = NewSnap.m_Rotation;
		SettingsManager.Instance.SetDOF(NewSnap.m_DOFEnabled);
		SettingsManager.Instance.SetAutoDOF(NewSnap.m_AutoDOFEnabled);
		SettingsManager.Instance.SetDOFFocalDistance(NewSnap.m_DOFFocalDistance);
		SettingsManager.Instance.SetDOFFocalLength(NewSnap.m_DOFFocalLength);
		SettingsManager.Instance.SetDOFAperture(NewSnap.m_DOFAperture);
		((CameraStateFree)CameraManager.Instance.m_States[1]).ResetFirstUse();
		CameraManager.Instance.SetState(CameraManager.State.Free);
		Debug.Log("Play " + NewSnap.f);
	}

	public void UpdateToFrame(int Frame)
	{
		if (Frame == m_LastFrame)
		{
			return;
		}
		m_LastFrame = Frame;
		CameraSnap cameraSnap = null;
		foreach (CameraSnap snap in m_Snaps)
		{
			if (snap.f <= m_LastFrame)
			{
				cameraSnap = snap;
			}
		}
		if (cameraSnap != null && m_LastSnap != cameraSnap)
		{
			m_LastSnap = cameraSnap;
			PlayCameraSnap(cameraSnap);
		}
	}

	public void Save()
	{
		string fileName = Application.persistentDataPath + "/CameraData.txt";
		JSONNode jSONNode = new JSONObject();
		JSONArray jSONArray = (JSONArray)(jSONNode["Snaps"] = new JSONArray());
		int num = 0;
		foreach (CameraSnap snap in m_Snaps)
		{
			JSONNode jSONNode3 = new JSONObject();
			snap.Save(jSONNode3);
			jSONArray[num] = jSONNode3;
			num++;
		}
		string finalString = jSONNode.ToString();
		SaveFile.Save(fileName, finalString);
		Debug.Log("Saved " + m_Snaps.Count);
	}

	public void Load()
	{
		JSONArray asArray = JSON.Parse(File.ReadAllText(Application.persistentDataPath + "/CameraData.txt"))["Snaps"].AsArray;
		m_Snaps.Clear();
		m_LastFrame = 0;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			CameraSnap cameraSnap = new CameraSnap();
			cameraSnap.Load(asObject);
			m_Snaps.Add(cameraSnap);
		}
		Debug.Log("Loaded " + m_Snaps.Count);
	}

	private void Update()
	{
	}
}
