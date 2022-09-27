using System.Linq;
using UnityEngine;

namespace cakeslice;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
	public int color;

	public bool eraseRenderer;

	[HideInInspector]
	public int originalLayer;

	[HideInInspector]
	public Material[] originalMaterials;

	public Renderer Renderer { get; private set; }

	private void Awake()
	{
		Renderer = GetComponent<Renderer>();
	}

	private void OnEnable()
	{
		foreach (OutlineEffect item in from c in Camera.allCameras.AsEnumerable()
			select c.GetComponent<OutlineEffect>() into e
			where e != null
			select e)
		{
			item.AddOutline(this);
		}
	}

	private void OnDisable()
	{
		foreach (OutlineEffect item in from c in Camera.allCameras.AsEnumerable()
			select c.GetComponent<OutlineEffect>() into e
			where e != null
			select e)
		{
			item.RemoveOutline(this);
		}
	}
}
