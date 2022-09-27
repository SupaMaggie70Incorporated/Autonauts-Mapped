using System.IO;
using UnityEngine;

namespace SupaMaggie70;

public class SupaLogger
{

	readonly static string folder = Application.persistentDataPath + "/supa/";
	readonly static string outputFile = folder + "log.txt";
	public SupaLogger()
	{
		if(!Directory.Exists(folder)) Directory.CreateDirectory(folder);
		File.Create(outputFile);
	}
}
