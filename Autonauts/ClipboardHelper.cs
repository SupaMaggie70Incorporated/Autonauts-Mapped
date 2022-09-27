using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

public class ClipboardHelper
{
	public static string clipBoard
	{
		get
		{
			return GUIUtility.systemCopyBuffer;
		}
		set
		{
			GUIUtility.systemCopyBuffer = value;
		}
	}

	public static void CopyToClipboard(Texture2D texture)
	{
		MemoryStream memoryStream = new MemoryStream(texture.width * texture.height);
		byte[] array = texture.EncodeToPNG();
		memoryStream.Write(array, 0, array.Length);
		Clipboard.SetImage(Image.FromStream(memoryStream));
		memoryStream.Close();
		memoryStream.Dispose();
	}
}
