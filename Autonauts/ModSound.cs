using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModSound
{
	public void ChangeSound(string EventName, string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeSound '" + EventName + "' - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_SoundEffect(EventName, audioClip);
		}
	}

	public void ChangeVolume(string EventName, float Volume)
	{
		if (EventName.Equals("AmbienceDayTime"))
		{
			DayNightManager.Instance.Mod_SetAmbienceDayTimeVolume(Volume);
		}
		else if (EventName.Equals("AmbienceNightTime"))
		{
			DayNightManager.Instance.Mod_SetAmbienceNightTimeVolume(Volume);
		}
		else if (EventName.Equals("AmbienceRain"))
		{
			DayNightManager.Instance.Mod_SetAmbienceRainVolume(Volume);
		}
		else if (EventName.Equals("AmbienceWind"))
		{
			DayNightManager.Instance.Mod_SetAmbienceWindVolume(Volume);
		}
		AudioManager.Instance.Mod_SoundEffectVolume(EventName, Volume);
	}

	public void ChangePitch(string EventName, float Pitch)
	{
		AudioManager.Instance.Mod_SoundEffectPitch(EventName, Pitch);
	}

	public void ChangeMusicVolume(float Volume)
	{
		AudioManager.Instance.Mod_MusicVolume(Volume);
	}

	public void AllowDayNightCycleMusic(bool Allow)
	{
		AudioManager.Instance.Mod_AllowDayNightMusic(Allow);
	}

	public void ChangeAllGameMusic(string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeAllMusic - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_AllMusic(audioClip);
		}
	}

	public void ChangeDayGameMusic(string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeDayGameMusic - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_DayMusic(audioClip);
		}
	}

	public void ChangeNightGameMusic(string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeNightGameMusic - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_NightMusic(audioClip);
		}
	}

	public void ChangeMenuMusic(string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeMenuMusic - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_MenuMusic(audioClip);
		}
	}

	public void ChangeAboutMusic(string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeAboutMusic - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_AboutMusic(audioClip);
		}
	}

	public void ChangeLoadingMusic(string ReplacementSound)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(ReplacementSound.ToLower());
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.ChangeLoadingMusic - WAV File not found for file '" + ReplacementSound + "'";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			audioClip.LoadAudioData();
			AudioManager.Instance.Mod_LoadingMusic(audioClip);
		}
	}

	public void PlayCustomSound(string AudioFile)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(AudioFile);
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.PlayCustomSound '" + AudioFile + "' Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else if ((bool)audioClip && audioClip.LoadAudioData())
		{
			AudioSource audioSource = ModManager.Instance.GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = ModManager.Instance.gameObject.AddComponent<AudioSource>();
			}
			if (audioSource != null)
			{
				audioSource.clip = audioClip;
				audioSource.volume *= AudioManager.Instance.GetSFXVolume();
				audioSource.Play();
			}
		}
	}

	public void PlayCustom3DSound(string AudioFile, int UID, float Pitch = 1f, float Volume = 1f)
	{
		AudioClip audioClip = ModManager.Instance.FindModAudioClip(AudioFile);
		if (audioClip == null)
		{
			string descriptionOverride = "Error: ModSound.PlayCustomSound '" + AudioFile + "' Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		Vector3 position = default(Vector3);
		if (objectFromUniqueID != null)
		{
			position = objectFromUniqueID.transform.position;
		}
		if (!(objectFromUniqueID == null) && (bool)audioClip && audioClip.LoadAudioData())
		{
			AudioSource audioSource = ModManager.Instance.GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = ModManager.Instance.gameObject.AddComponent<AudioSource>();
			}
			if (audioSource != null)
			{
				audioSource.clip = audioClip;
				audioSource.volume = Volume;
				audioSource.pitch = Pitch;
				AudioSource.PlayClipAtPoint(audioSource.clip, position);
			}
		}
	}

	public void PlayCustom3DSound(string AudioFile, string UID, float Pitch = 1f, float Volume = 1f)
	{
	}
}
