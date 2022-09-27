using UnityEngine;

public class ModelManagerTest : MonoBehaviour
{
	private void Awake()
	{
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/ModelManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/TextureAtlasManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/MaterialManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		ModelManager.Instance.MakeModelList();
		ModelManager.Instance.Init();
	}
}
