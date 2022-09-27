using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	private static int m_MaxAudioRadiusInTiles = 50;

	public static AudioManager Instance;

	private PlaylistController m_MusicController;

	private MasterAudio m_Master;

	private bool m_Started;

	private static float m_MusicVolume = 1f;

	private static bool m_MusicRequested = false;

	public string m_MusicName;

	private float m_SFXVolume;

	private Dictionary<TileCoordObject, int> m_NonPlotAmbientAudio;

	private Dictionary<PlaySound, int> m_ActiveAmbientSounds;

	private Dictionary<string, AudioClip> ResetEvents;

	private Dictionary<string, float> ResetEventVolumes;

	private Dictionary<string, float> ResetEventPitches;

	public bool AllowDayNightChange { get; private set; }

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
		m_Master = GameObject.Find("MasterAudio").GetComponent<MasterAudio>();
		m_MusicController = m_Master.GetComponent<PlaylistController>();
		m_Started = false;
		m_NonPlotAmbientAudio = new Dictionary<TileCoordObject, int>();
		m_ActiveAmbientSounds = new Dictionary<PlaySound, int>();
		AllowDayNightChange = true;
		ResetEvents = new Dictionary<string, AudioClip>();
		ResetEventVolumes = new Dictionary<string, float>();
		ResetEventPitches = new Dictionary<string, float>();
	}

	private void Start()
	{
		m_Master.playlistControllerPrefab = m_Master.transform;
	}

	public bool IsEventActive(string EventName, TileCoordObject Object = null)
	{
		return false;
	}

	public PlaySound StartEvent(string EventName, TileCoordObject Object = null, bool Remember = false, bool Follow = false)
	{
		MasterAudio.AudioGroupInfo groupInfo = MasterAudio.GetGroupInfo(EventName);
		if (groupInfo != null && (bool)Object && groupInfo.Sources.Count > 0)
		{
			if (groupInfo.Sources[0].Source.loop)
			{
				PlaySound playSound = new PlaySound(EventName, Object, groupInfo.Sources[0].Source.maxDistance);
				if (Object.m_AmbientSounds == null)
				{
					Object.m_AmbientSounds = new List<PlaySound>();
				}
				Object.m_AmbientSounds.Add(playSound);
				if ((bool)Object.GetComponent<TileCoordObject>().m_Plot)
				{
					Object.GetComponent<TileCoordObject>().m_Plot.m_AmbientObjects.Add(playSound);
				}
				else if (!m_NonPlotAmbientAudio.ContainsKey(Object))
				{
					m_NonPlotAmbientAudio.Add(Object, 0);
				}
				return playSound;
			}
			if ((CameraManager.Instance.m_Camera.transform.position - Object.transform.position).magnitude - groupInfo.Sources[0].Source.maxDistance > 0f)
			{
				return null;
			}
		}
		PlaySound result = null;
		if (Remember)
		{
			PlaySoundResult result2 = ((!Object) ? MasterAudio.PlaySound(EventName) : ((!Follow) ? MasterAudio.PlaySound3DAtVector3(EventName, Object.transform.position) : MasterAudio.PlaySound3DFollowTransform(EventName, Object.transform)));
			result = new PlaySound(result2);
		}
		else if ((bool)Object)
		{
			if (Follow)
			{
				MasterAudio.PlaySound3DFollowTransformAndForget(EventName, Object.transform);
			}
			else
			{
				MasterAudio.PlaySound3DAtVector3AndForget(EventName, Object.transform.position);
			}
		}
		else
		{
			MasterAudio.PlaySoundAndForget(EventName);
		}
		return result;
	}

	public PlaySound StartEventAmbient(string EventName, GameObject Object = null, bool Remember = false, bool Follow = false)
	{
		PlaySound result = null;
		if (Remember)
		{
			PlaySoundResult result2 = ((!Object) ? MasterAudio.PlaySound(EventName) : ((!Follow) ? MasterAudio.PlaySound3DAtVector3(EventName, Object.transform.position) : MasterAudio.PlaySound3DFollowTransform(EventName, Object.transform)));
			result = new PlaySound(result2);
		}
		else if ((bool)Object)
		{
			if (Follow)
			{
				MasterAudio.PlaySound3DFollowTransformAndForget(EventName, Object.transform);
			}
			else
			{
				MasterAudio.PlaySound3DAtVector3AndForget(EventName, Object.transform.position);
			}
		}
		else
		{
			MasterAudio.PlaySoundAndForget(EventName);
		}
		return result;
	}

	public void StopEvent(PlaySound Sound)
	{
		if (Sound != null)
		{
			if ((bool)Sound.m_Object && Sound.m_Object.GetComponent<TileCoordObject>() != null && Sound.m_Object.GetComponent<TileCoordObject>().m_Plot != null)
			{
				Sound.m_Object.GetComponent<TileCoordObject>().m_Plot.m_AmbientObjects.Remove(Sound);
			}
			if (m_ActiveAmbientSounds.ContainsKey(Sound))
			{
				m_ActiveAmbientSounds.Remove(Sound);
			}
			if ((bool)Sound.m_Object && Sound.m_Object.m_AmbientSounds != null)
			{
				Sound.m_Object.m_AmbientSounds.Remove(Sound);
			}
			if ((bool)Sound.m_Object && m_NonPlotAmbientAudio.ContainsKey(Sound.m_Object))
			{
				m_NonPlotAmbientAudio.Remove(Sound.m_Object);
			}
			if (Sound != null && Sound.m_Result != null && (bool)Sound.m_Result.ActingVariation)
			{
				Sound.m_Result.ActingVariation.Stop();
			}
		}
	}

	public void SetEventVolume(PlaySound Sound, float Volume)
	{
		if (Sound != null && Sound.m_Result != null && (bool)Sound.m_Result.ActingVariation)
		{
			Sound.m_Result.ActingVariation.AdjustVolume(Volume);
		}
	}

	public void GlideEventPitch(PlaySound Sound, float Pitch, float Time)
	{
		float pitchAddition = Pitch - Sound.m_Result.ActingVariation.VarAudio.pitch;
		Sound.m_Result.ActingVariation.GlideByPitch(pitchAddition, Time);
	}

	public float GetSFXVolume()
	{
		return MasterAudio.GrabBusByName("UI").volume;
	}

	public void SetSFXVolume(float Volume)
	{
		MasterAudio.SetBusVolumeByName("UI", Volume);
		MasterAudio.SetBusVolumeByName("World", Volume);
		m_SFXVolume = Volume;
	}

	public void DuckWorldSFX(float Volume)
	{
		MasterAudio.SetBusVolumeByName("World", Volume * m_SFXVolume);
	}

	public void RestoreWorldSFX()
	{
		MasterAudio.SetBusVolumeByName("World", m_SFXVolume);
	}

	public void StartMusic(string Name)
	{
		if (m_MusicController.CurrentPlaylist == null || m_MusicController.PlaylistName != Name || !IsMusicPlaying())
		{
			m_MusicController.StartPlaylist(Name);
			m_MusicRequested = true;
			m_MusicName = Name;
			SetMusicVolume(m_MusicVolume);
		}
	}

	public bool IsMusicPlaying()
	{
		return m_MusicController.PlaylistState == PlaylistController.PlaylistStates.Playing;
	}

	public void StopMusic()
	{
		m_MusicController.StopPlaylist();
	}

	public float GetMusicVolume()
	{
		return m_MusicController.PlaylistVolume;
	}

	public void SetCurrentMusicVolume(float Volume)
	{
		m_MusicController.PlaylistVolume = m_MusicVolume * Volume;
	}

	public void SetMusicVolume(float Volume)
	{
		m_MusicVolume = Volume;
		m_MusicController.PlaylistVolume = Volume;
	}

	public void Pause(bool Pause)
	{
		if (MasterAudio.SafeInstance != null)
		{
			if (Pause)
			{
				MasterAudio.PauseEverything();
			}
			else
			{
				MasterAudio.UnpauseEverything();
			}
			m_MusicController.UnpausePlaylist();
		}
	}

	public void StopSFX()
	{
		MasterAudio.StopBus("UI");
		MasterAudio.StopBus("World");
	}

	public void PlotObjectAdded(Plot NewPlot, TileCoordObject NewObject)
	{
		if (m_NonPlotAmbientAudio.ContainsKey(NewObject))
		{
			m_NonPlotAmbientAudio.Remove(NewObject);
		}
		if (NewObject.m_AmbientSounds == null || NewObject.m_AmbientSounds.Count == 0)
		{
			return;
		}
		foreach (PlaySound ambientSound in NewObject.m_AmbientSounds)
		{
			NewPlot.m_AmbientObjects.Add(ambientSound);
		}
	}

	public void PlotObjectRemoved(Plot NewPlot, TileCoordObject NewObject)
	{
		if (NewObject.m_AmbientSounds == null || NewObject.m_AmbientSounds.Count == 0)
		{
			return;
		}
		foreach (PlaySound ambientSound in NewObject.m_AmbientSounds)
		{
			NewPlot.m_AmbientObjects.Remove(ambientSound);
		}
		m_NonPlotAmbientAudio.Add(NewObject, 0);
	}

	private void CheckActiveAmbientSounds()
	{
		Vector3 position = CameraManager.Instance.m_Camera.transform.position;
		List<PlaySound> list = new List<PlaySound>();
		foreach (KeyValuePair<PlaySound, int> activeAmbientSound in m_ActiveAmbientSounds)
		{
			PlaySound key = activeAmbientSound.Key;
			if (key.m_Object != null)
			{
				if ((position - key.m_Object.transform.position).magnitude - key.m_MaxRange >= 0f || !key.m_Object.gameObject.activeSelf)
				{
					list.Add(key);
				}
			}
			else
			{
				list.Add(key);
			}
		}
		foreach (PlaySound item in list)
		{
			m_ActiveAmbientSounds.Remove(item);
			if (item != null && item.m_Result != null)
			{
				item.m_Result.ActingVariation.Stop();
			}
		}
	}

	private void CheckNewAmbientSounds()
	{
		if (PlotManager.Instance == null)
		{
			return;
		}
		Vector3 position = CameraManager.Instance.m_Camera.transform.position;
		TileCoord position2 = new TileCoord(position);
		PlotManager.Instance.GetArea(position2, m_MaxAudioRadiusInTiles, out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = PlotManager.Instance.GetPlotAtPlot(j, i);
				if (!(plotAtPlot != null))
				{
					continue;
				}
				foreach (PlaySound ambientObject in plotAtPlot.m_AmbientObjects)
				{
					if (!m_ActiveAmbientSounds.ContainsKey(ambientObject) && ambientObject.m_Object.gameObject.activeSelf && (position - ambientObject.m_Object.transform.position).magnitude - ambientObject.m_MaxRange < 0f)
					{
						ambientObject.m_Result = MasterAudio.PlaySound3DFollowTransform(ambientObject.m_EventName, ambientObject.m_Object.transform);
						if (ambientObject.m_Result != null)
						{
							m_ActiveAmbientSounds.Add(ambientObject, 0);
						}
					}
				}
			}
		}
	}

	private void CheckNonPlotAmbientSounds()
	{
		if (PlotManager.Instance == null)
		{
			return;
		}
		Vector3 position = CameraManager.Instance.m_Camera.transform.position;
		foreach (KeyValuePair<TileCoordObject, int> item in m_NonPlotAmbientAudio)
		{
			TileCoordObject key = item.Key;
			if (!key || key.m_AmbientSounds == null || !key.gameObject.activeSelf)
			{
				continue;
			}
			foreach (PlaySound ambientSound in key.m_AmbientSounds)
			{
				if (!m_ActiveAmbientSounds.ContainsKey(ambientSound) && (position - key.transform.position).magnitude - ambientSound.m_MaxRange < 0f)
				{
					ambientSound.m_Result = MasterAudio.PlaySound3DFollowTransform(ambientSound.m_EventName, key.transform);
					if (ambientSound.m_Result != null)
					{
						m_ActiveAmbientSounds.Add(ambientSound, 0);
					}
				}
			}
		}
	}

	private void Update()
	{
		if (!m_Started)
		{
			m_Started = true;
			m_Master.transform.Find("_Followers").Find("~ListenerFollower~").gameObject.SetActive(value: false);
			Physics.autoSimulation = false;
		}
		if (SceneManager.GetActiveScene().name == "Main")
		{
			CheckActiveAmbientSounds();
			CheckNewAmbientSounds();
			CheckNonPlotAmbientSounds();
		}
		if (m_MusicRequested && m_MusicController.PlaylistState == PlaylistController.PlaylistStates.Playing)
		{
			m_MusicRequested = false;
			SetMusicVolume(m_MusicVolume);
		}
	}

	public void Mod_SoundEffect(string EventName, AudioClip NewClip)
	{
		AudioClip variationClip = MasterAudio.GetVariationClip(EventName, changeAllVariations: true, "");
		if (!ResetEvents.ContainsKey(EventName))
		{
			ResetEvents.Add(EventName, variationClip);
		}
		else
		{
			ResetEvents[EventName] = variationClip;
		}
		MasterAudio.ChangeVariationClip(EventName, changeAllVariations: true, "", NewClip);
	}

	public void Mod_SoundEffectVolume(string EventName, float NewVolume)
	{
		float variationVolume = MasterAudio.GetVariationVolume(EventName, changeAllVariations: true, "");
		if (!ResetEventVolumes.ContainsKey(EventName))
		{
			ResetEventVolumes.Add(EventName, variationVolume);
		}
		else
		{
			ResetEventVolumes[EventName] = variationVolume;
		}
		MasterAudio.ChangeVariationVolume(EventName, changeAllVariations: true, "", NewVolume);
	}

	public void Mod_SoundEffectPitch(string EventName, float NewPitch)
	{
		float variationPitch = MasterAudio.GetVariationPitch(EventName, changeAllVariations: true, "");
		if (!ResetEventPitches.ContainsKey(EventName))
		{
			ResetEventPitches.Add(EventName, variationPitch);
		}
		else
		{
			ResetEventPitches[EventName] = variationPitch;
		}
		MasterAudio.ChangeVariationPitch(EventName, changeAllVariations: true, "", NewPitch);
	}

	public void Mod_AllMusic(AudioClip NewClip)
	{
		m_MusicController.SetCurrentPlaylistClip("MusicGame", NewClip);
		m_MusicController.SetCurrentPlaylistClip("MusicGameNight", NewClip);
	}

	public void Mod_DayMusic(AudioClip NewClip)
	{
		m_MusicController.SetCurrentPlaylistClip("MusicGame", NewClip);
	}

	public void Mod_NightMusic(AudioClip NewClip)
	{
		m_MusicController.SetCurrentPlaylistClip("MusicGameNight", NewClip);
	}

	public void Mod_MenuMusic(AudioClip NewClip)
	{
		m_MusicController.SetCurrentPlaylistClip("MusicCover", NewClip);
	}

	public void Mod_LoadingMusic(AudioClip NewClip)
	{
		m_MusicController.SetCurrentPlaylistClip("MusicLoading", NewClip);
	}

	public void Mod_AboutMusic(AudioClip NewClip)
	{
		m_MusicController.SetCurrentPlaylistClip("MusicAbout", NewClip);
	}

	public void Mod_AllowDayNightMusic(bool Change)
	{
		AllowDayNightChange = Change;
	}

	public void Mod_MusicVolume(float NewVolume)
	{
		SetCurrentMusicVolume(NewVolume);
		SetMusicVolume(NewVolume);
	}

	public List<AudioSource> GetAllSounds()
	{
		return MasterAudio.MasterAudioSources;
	}

	public void ResetAllModSounds()
	{
		m_MusicController.ResetModClips();
		foreach (KeyValuePair<string, AudioClip> resetEvent in ResetEvents)
		{
			MasterAudio.ChangeVariationClip(resetEvent.Key, changeAllVariations: true, "", resetEvent.Value);
		}
		ResetEvents.Clear();
		foreach (KeyValuePair<string, float> resetEventVolume in ResetEventVolumes)
		{
			MasterAudio.ChangeVariationVolume(resetEventVolume.Key, changeAllVariations: true, "", resetEventVolume.Value);
		}
		ResetEventVolumes.Clear();
		foreach (KeyValuePair<string, float> resetEventPitch in ResetEventPitches)
		{
			MasterAudio.ChangeVariationPitch(resetEventPitch.Key, changeAllVariations: true, "", resetEventPitch.Value);
		}
		ResetEventPitches.Clear();
	}
}
