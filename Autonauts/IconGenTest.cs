using UnityEngine;

public class IconGenTest : MonoBehaviour
{
	private void Awake()
	{
		new ObjectTypeList();
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/ModelManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/TextureAtlasManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/MaterialManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/IconManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		ModelManager.Instance.MakeModelList();
		ModelManager.Instance.Init();
		IconManager.Instance.Init();
	}
}
