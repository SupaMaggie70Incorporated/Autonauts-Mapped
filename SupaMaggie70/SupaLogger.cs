using System.IO;
using System.Text;

using UnityEngine;

namespace SupaMaggie70;


/// <summary>
/// Stolen from my github page at https://github.com/SupaMaggie70Incorporated/SupaStuff/blob/master/SupaStuff/Util/Logger.cs
/// </summary>
public struct SupaLogger
{
    static string file = Application.persistentDataPath + "/Supa/log.txt";
    static FileStream stream = File.OpenWrite(file);
    public readonly string Name;
    public static bool IsUnity { get; private set; }
    /// <summary>
    /// Get or create the logger with name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static SupaLogger GetLogger(string name)
    {
        return new SupaLogger(name);
    }
    /// <summary>
    /// Creates a logger with the name
    /// </summary>
    /// <param name="name"></param>
    public SupaLogger(string name)
    {
        Name = name;
    }
    /// <summary>
    /// Logs the message
    /// </summary>
    /// <param name="contents"></param>
    public void Log(object contents)
    {
        string message = "[" + Name + "] " + contents.ToString();
        Debug.Log(message);
        Write("[Log]" + message);
    }
    
    /// <summary>
    /// Logs the warning
    /// </summary>
    /// <param name="contents"></param>
    public void Warn(object contents)
    {
        string message = "[" + Name + "] " + contents.ToString();
        Debug.Log(message);
        Write("[Warn]" + message);
    }
    /// <summary>
    /// Logs the error
    /// </summary>
    /// <param name="contents"></param>
    public void Error(object contents)
    {
        string message = "[" + Name + "] " + contents.ToString();
        Debug.Log(message);
        Write("[Error]" + message);
    }
    static void Write(string message)
    {
        byte[] bytes1 = Encoding.UTF8.GetBytes("\n");
        byte[] bytes2 = Encoding.UTF8.GetBytes(message);
        stream.Write(bytes1,0,bytes1.Length);
        stream.Write(bytes2, 0, bytes2.Length);
        stream.Flush();
    }
    public static void Clear()
    {
        stream.Dispose();
        File.WriteAllBytes(file, new byte[0]);
        stream = File.OpenWrite(file);
    }
}