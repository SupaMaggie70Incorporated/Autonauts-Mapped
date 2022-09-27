using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraStateMainMenu : CameraState
{
	private float m_Rotation;

	public CameraStateMainMenu(Camera NewCamera)
		: base(NewCamera)
	{
	}

	public override void StartUse()
	{
		int num = (int)(SaveLoadManager.m_MiniMapCameraX / Tile.m_Size) + 2 - TileManager.Instance.m_TilesWide;
		int num2 = (int)((0f - SaveLoadManager.m_MiniMapCameraZ) / Tile.m_Size) + 2 - TileManager.Instance.m_TilesHigh;
		float num3 = (float)num * Tile.m_Size;
		float num4 = (float)num2 * Tile.m_Size;
		m_Camera.transform.position = new Vector3(SaveLoadManager.m_MiniMapCameraX - num3, SaveLoadManager.m_MiniMapCameraY, SaveLoadManager.m_MiniMapCameraZ + num4);
		m_Camera.transform.rotation = Quaternion.Euler(SaveLoadManager.m_MiniMapCameraRotX, SaveLoadManager.m_MiniMapCameraRotY, SaveLoadManager.m_MiniMapCameraRotZ);
		CameraManager.Instance.SetDOFEnabled(Enabled: true);
		CameraManager.Instance.SetAutoDOFEnabled(Enabled: false);
		PostProcessingBehaviour component = CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>();
		DepthOfFieldModel.Settings settings = component.profile.depthOfField.settings;
		settings.focalLength = 201f;
		settings.aperture = 5.6f;
		settings.focusDistance = 30f;
		component.profile.depthOfField.settings = settings;
	}

	public override void EndUse()
	{
		SettingsManager.Instance.SetDOF(SettingsManager.Instance.m_DOFEnabled);
		SettingsManager.Instance.SetAutoDOF(SettingsManager.Instance.m_AutoDOFEnabled);
		SettingsManager.Instance.SetDOFAperture(SettingsManager.Instance.m_DOFAperture);
		SettingsManager.Instance.SetDOFFocalLength(SettingsManager.Instance.m_DOFFocalLength);
		SettingsManager.Instance.SetDOFFocalDistance(SettingsManager.Instance.m_DOFFocalDistance);
	}

	public override void UpdateInput()
	{
	}

	public override void UpdateCamera()
	{
	}
}
