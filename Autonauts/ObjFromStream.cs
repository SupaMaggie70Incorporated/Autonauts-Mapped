using System.IO;
using System.Text;
using System.Threading;
using Dummiesman;
using UnityEngine;

public class ObjFromStream : MonoBehaviour
{
	private void Start()
	{
		WWW wWW = new WWW("https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj");
		while (!wWW.isDone)
		{
			Thread.Sleep(1);
		}
		MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes(wWW.text));
		new OBJLoader().Load(input);
	}
}
