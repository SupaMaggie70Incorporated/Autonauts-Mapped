using System.Collections.Generic;
using UnityEngine;

public class ObjectUtils : MonoBehaviour
{
	public static Quaternion m_ModelRotator = Quaternion.Euler(-90f, 0f, 180f);

	public static Quaternion m_ModelRotatorInv = Quaternion.Euler(90f, 0f, 0f) * Quaternion.Euler(0f, -180f, 0f);

	public static Rect GUIRectWithObject(GameObject go)
	{
		Rect rect = HudManager.Instance.m_RootTransform.GetComponent<RectTransform>().rect;
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		Rect result = new Rect
		{
			yMin = 100000f,
			yMax = 0f,
			xMin = 100000f,
			xMax = 0f
		};
		Renderer[] array = componentsInChildren;
		foreach (Renderer obj in array)
		{
			Vector3 center = obj.bounds.center;
			Vector3 extents = obj.bounds.extents;
			Vector2 vector = CameraManager.Instance.m_Camera.WorldToScreenPoint(center + extents);
			vector.x = vector.x / (float)CameraManager.Instance.m_Camera.pixelWidth * rect.width;
			if (result.yMin > vector.y)
			{
				result.yMin = vector.y;
			}
			if (result.yMax < vector.y)
			{
				result.yMax = vector.y;
			}
			if (result.xMin > vector.x)
			{
				result.xMin = vector.x;
			}
			if (result.xMax < vector.x)
			{
				result.xMax = vector.x;
			}
			vector = CameraManager.Instance.m_Camera.WorldToScreenPoint(center - extents);
			vector.x = vector.x / (float)CameraManager.Instance.m_Camera.pixelWidth * rect.width;
			if (result.yMin > vector.y)
			{
				result.yMin = vector.y;
			}
			if (result.yMax < vector.y)
			{
				result.yMax = vector.y;
			}
			if (result.xMin > vector.x)
			{
				result.xMin = vector.x;
			}
			if (result.xMax < vector.x)
			{
				result.xMax = vector.x;
			}
		}
		return result;
	}

	public static Bounds ObjectBounds(GameObject go)
	{
		MeshRenderer[] componentsInChildren = go.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		Bounds result = default(Bounds);
		Vector3 min = new Vector3(10000f, 10000f, 10000f);
		Vector3 max = new Vector3(-10000f, -10000f, -10000f);
		if (componentsInChildren.Length != 0)
		{
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				_ = meshRenderer.bounds.center;
				_ = meshRenderer.bounds.extents;
				if (meshRenderer.bounds.min.x < min.x)
				{
					min.x = meshRenderer.bounds.min.x;
				}
				if (meshRenderer.bounds.min.y < min.y)
				{
					min.y = meshRenderer.bounds.min.y;
				}
				if (meshRenderer.bounds.min.z < min.z)
				{
					min.z = meshRenderer.bounds.min.z;
				}
				if (meshRenderer.bounds.max.x > max.x)
				{
					max.x = meshRenderer.bounds.max.x;
				}
				if (meshRenderer.bounds.max.y > max.y)
				{
					max.y = meshRenderer.bounds.max.y;
				}
				if (meshRenderer.bounds.max.z > max.z)
				{
					max.z = meshRenderer.bounds.max.z;
				}
			}
		}
		else
		{
			min = go.transform.position;
			max = go.transform.position;
		}
		result.min = min;
		result.max = max;
		if (result.size.y == -20000f)
		{
			min.x = 1f;
		}
		return result;
	}

	public static Bounds ObjectBoundsFromVertices(GameObject go)
	{
		MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>();
		Bounds result = default(Bounds);
		Vector3 min = new Vector3(10000f, 10000f, 10000f);
		Vector3 max = new Vector3(-10000f, -10000f, -10000f);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			Vector3[] vertices = meshFilter.GetComponent<MeshFilter>().mesh.vertices;
			foreach (Vector3 position in vertices)
			{
				Vector3 vector = meshFilter.transform.TransformPoint(position);
				if (vector.x < min.x)
				{
					min.x = vector.x;
				}
				if (vector.y < min.y)
				{
					min.y = vector.y;
				}
				if (vector.z < min.z)
				{
					min.z = vector.z;
				}
				if (vector.x > max.x)
				{
					max.x = vector.x;
				}
				if (vector.y > max.y)
				{
					max.y = vector.y;
				}
				if (vector.z > max.z)
				{
					max.z = vector.z;
				}
			}
		}
		result.min = min;
		result.max = max;
		return result;
	}

	public static Bounds ObjectBoundsFromVerticesInCameraSpace(GameObject go, Camera NewCamera)
	{
		MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>();
		Bounds result = default(Bounds);
		Vector3 min = new Vector3(10000f, 10000f, 10000f);
		Vector3 max = new Vector3(-10000f, -10000f, -10000f);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter meshFilter in array)
		{
			Vector3[] vertices = meshFilter.GetComponent<MeshFilter>().mesh.vertices;
			foreach (Vector3 position in vertices)
			{
				Vector3 position2 = meshFilter.transform.TransformPoint(position);
				Vector3 vector = NewCamera.WorldToViewportPoint(position2);
				if (vector.x < min.x)
				{
					min.x = vector.x;
				}
				if (vector.y < min.y)
				{
					min.y = vector.y;
				}
				if (vector.z < min.z)
				{
					min.z = vector.z;
				}
				if (vector.x > max.x)
				{
					max.x = vector.x;
				}
				if (vector.y > max.y)
				{
					max.y = vector.y;
				}
				if (vector.z > max.z)
				{
					max.z = vector.z;
				}
			}
		}
		result.min = min;
		result.max = max;
		return result;
	}

	public static Vector2 WorldToGUIPoint(Vector3 world)
	{
		Vector2 result = CameraManager.Instance.m_Camera.WorldToScreenPoint(world);
		result.y = (float)Screen.height - result.y;
		return result;
	}

	public static Transform FindDeepChild(Transform aParent, string aName)
	{
		Transform transform = aParent.Find(aName);
		if (transform != null)
		{
			return transform;
		}
		foreach (Transform item in aParent)
		{
			transform = FindDeepChild(item, aName);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static TileCoordObject FindNearestObject(List<TileCoordObject> Objects, TileCoord Target)
	{
		TileCoordObject result = null;
		float num = 1E+11f;
		foreach (TileCoordObject Object in Objects)
		{
			float num2 = (Object.m_TileCoord - Target).Magnitude();
			if (num2 < num)
			{
				num = num2;
				result = Object;
			}
		}
		return result;
	}

	public static void TransferRectTransform(GameObject NewObject, GameObject OldObject)
	{
		RectTransform component = NewObject.GetComponent<RectTransform>();
		RectTransform component2 = OldObject.GetComponent<RectTransform>();
		component.sizeDelta = component2.sizeDelta;
		component.anchorMin = component2.anchorMin;
		component.anchorMax = component2.anchorMax;
		component.pivot = component2.pivot;
		component.anchoredPosition = component2.anchoredPosition;
	}
}
