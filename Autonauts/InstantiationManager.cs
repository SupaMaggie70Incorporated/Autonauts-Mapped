using System.Collections.Generic;
using UnityEngine;

public class InstantiationManager : MonoBehaviour
{
	public static InstantiationManager Instance;

	private static Dictionary<ObjectType, List<BaseClass>> m_ReusableObjects;

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
		m_ReusableObjects = new Dictionary<ObjectType, List<BaseClass>>();
	}

	public void Clear()
	{
		foreach (KeyValuePair<ObjectType, List<BaseClass>> reusableObject in m_ReusableObjects)
		{
			foreach (BaseClass item in reusableObject.Value)
			{
				if ((bool)item)
				{
					Object.DestroyImmediate(item.gameObject);
				}
			}
		}
		m_ReusableObjects.Clear();
	}

	public void AddBaseClass(BaseClass NewCollectable)
	{
		if (!m_ReusableObjects.ContainsKey(NewCollectable.m_TypeIdentifier))
		{
			m_ReusableObjects.Add(NewCollectable.m_TypeIdentifier, new List<BaseClass>());
		}
		List<BaseClass> value = null;
		if (m_ReusableObjects.TryGetValue(NewCollectable.m_TypeIdentifier, out value))
		{
			if (value.Contains(NewCollectable))
			{
				ErrorMessage.LogError(string.Concat(NewCollectable.m_TypeIdentifier, ":", NewCollectable.m_UniqueID, " already exists in the reusable object list"));
			}
			else
			{
				value.Add(NewCollectable);
			}
		}
	}

	public void RemoveBaseClass(BaseClass NewCollectable)
	{
		if (m_ReusableObjects.ContainsKey(NewCollectable.m_TypeIdentifier))
		{
			if (m_ReusableObjects.TryGetValue(NewCollectable.m_TypeIdentifier, out var value))
			{
				value.Remove(NewCollectable);
			}
			else
			{
				ErrorMessage.LogError("RemoveBaseClass : object doesn't exist in prefab list " + NewCollectable.m_TypeIdentifier);
			}
		}
		else
		{
			ErrorMessage.LogError("RemoveBaseClass : couldn't find prefab name " + NewCollectable.m_TypeIdentifier);
		}
	}

	public void SetLayer(GameObject NewObject, Layers Layer)
	{
		if (NewObject.layer == (int)Layer)
		{
			return;
		}
		NewObject.layer = (int)Layer;
		for (int i = 0; i < NewObject.transform.childCount; i++)
		{
			GameObject gameObject = NewObject.transform.GetChild(i).gameObject;
			if (gameObject.activeSelf)
			{
				SetLayer(gameObject, Layer);
			}
		}
	}

	public GameObject CreateObject(ObjectType NewType, string PrefabName, string ModelName, Vector3 Position, Quaternion Rotation)
	{
		GameObject obj = (GameObject)Resources.Load("Prefabs/" + PrefabName, typeof(GameObject));
		if (obj == null)
		{
			ErrorMessage.LogError("Prefab " + PrefabName + " not found");
		}
		GameObject gameObject = Object.Instantiate(obj, Position, Rotation, null);
		if (gameObject == null)
		{
			ErrorMessage.LogError("Couldn't instantiate " + PrefabName);
		}
		else
		{
			gameObject.transform.SetParent(ObjectTypeList.Instance.GetParentFromIdentifier(NewType));
		}
		BaseClass component = gameObject.GetComponent<BaseClass>();
		if (component == null)
		{
			ErrorMessage.LogError("Can't get BaseClass " + PrefabName);
		}
		if (ModelName != "")
		{
			GameObject gameObject2 = ModelManager.Instance.Instantiate(NewType, ModelName, gameObject.transform, RandomVariants: true);
			if (gameObject2 == null)
			{
				Debug.Log("Can't instantiate " + ModelName);
			}
			component.m_ModelName = gameObject2.name;
			gameObject2.name = "Model";
			SetLayer(gameObject2, (Layers)gameObject.gameObject.layer);
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject2.transform.localRotation = Quaternion.identity;
		}
		if ((bool)gameObject.transform.Find("Model"))
		{
			component.m_ModelRoot = gameObject.transform.Find("Model").gameObject;
		}
		else
		{
			component.m_ModelRoot = null;
		}
		component.m_TypeIdentifier = NewType;
		component.PostCreate();
		return gameObject;
	}

	public BaseClass GetReusableObject(ObjectType IdentifierType, Vector3 Position, Quaternion Rotation)
	{
		if (m_ReusableObjects.TryGetValue(IdentifierType, out var value) && value.Count > 0)
		{
			BaseClass baseClass = value[0];
			value.RemoveAt(0);
			if ((bool)baseClass)
			{
				baseClass.transform.position = Position;
				baseClass.transform.rotation = Rotation;
				baseClass.transform.localScale = new Vector3(1f, 1f, 1f);
				baseClass.transform.SetParent(ObjectTypeList.Instance.GetParentFromIdentifier(baseClass.GetComponent<BaseClass>().m_TypeIdentifier));
				return baseClass;
			}
		}
		return CreateObject(IdentifierType, ObjectTypeList.Instance.GetPrefabFromIdentifier(IdentifierType), ObjectTypeList.Instance.GetModelNameFromIdentifier(IdentifierType), Position, Rotation).GetComponent<BaseClass>();
	}

	public void ReturnBaseClassObject(BaseClass Object)
	{
		Object.gameObject.SetActive(value: false);
		if ((bool)MapManager.Instance)
		{
			Transform transform = null;
			transform = MapManager.Instance.m_UnusedRootTransform;
			Object.transform.SetParent(transform);
			AddBaseClass(Object);
		}
	}

	public void LoadModel(BaseClass NewObject, string ModelName, bool RandomVariants)
	{
		if (NewObject.transform.childCount > 0)
		{
			Object.DestroyImmediate(NewObject.m_ModelRoot);
		}
		if (ModelName != "")
		{
			GameObject gameObject = ModelManager.Instance.Instantiate(NewObject.m_TypeIdentifier, ModelName, NewObject.transform, RandomVariants);
			NewObject.m_ModelName = gameObject.name;
			gameObject.name = "Model";
			SetLayer(gameObject, (Layers)NewObject.gameObject.layer);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localRotation = Quaternion.identity;
			NewObject.m_ModelRoot = gameObject;
		}
		else
		{
			NewObject.m_ModelRoot = null;
		}
	}

	public GameObject LoadModel(ObjectType NewType, GameObject Parent, string ModelName, string NodeName)
	{
		if (ModelName != "")
		{
			GameObject gameObject = ModelManager.Instance.Instantiate(NewType, ModelName, Parent.transform, RandomVariants: false);
			gameObject.name = NodeName;
			SetLayer(gameObject, (Layers)Parent.layer);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localRotation = Quaternion.identity;
			return gameObject;
		}
		return null;
	}
}
