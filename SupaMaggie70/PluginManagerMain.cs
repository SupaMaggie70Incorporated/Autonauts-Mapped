using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SupaMaggie70;

public static class PluginManagerMain
{
	public static SupaLogger logger;

	public static void Init()
	{
		const float timescale = 0.1f;
		logger = new SupaLogger("SUPA");
		SceneManager.sceneLoaded += InputFaker.SceneLoaded;
		SceneManager.sceneUnloaded += InputFaker.SceneUnloaded;

		//SetTimeScale(timescale);
		//logger.Log($"Changed timescale to {timescale}");
	}
	public static void SetTimeScale(float timescale)
    {
		Time.timeScale = timescale;
		Time.fixedDeltaTime *= (1 / timescale);
	}
}
