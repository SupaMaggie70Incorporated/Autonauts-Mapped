using System.IO;
using Dummiesman;
using UnityEngine;

public class ObjFromFile : MonoBehaviour
{
	private string objPath = string.Empty;

	private string error = string.Empty;

	private GameObject loadedObject;

	private void OnGUI()
	{
		objPath = GUI.TextField(new Rect(0f, 0f, 256f, 32f), objPath);
		GUI.Label(new Rect(0f, 0f, 256f, 32f), "Obj Path:");
		if (GUI.Button(new Rect(256f, 32f, 64f, 32f), "Load File"))
		{
			if (!File.Exists(objPath))
			{
				error = "File doesn't exist.";
			}
			else
			{
				if (loadedObject != null)
				{
					Object.Destroy(loadedObject);
				}
				loadedObject = new OBJLoader().Load(objPath);
				error = string.Empty;
			}
		}
		if (!string.IsNullOrWhiteSpace(error))
		{
			GUI.color = Color.red;
			GUI.Box(new Rect(0f, 64f, 320f, 32f), error);
			GUI.color = Color.white;
		}
	}
}
