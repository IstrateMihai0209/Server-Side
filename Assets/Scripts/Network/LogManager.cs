using System.IO;
using System.IO.Compression;
using System;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    string openedAt = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";

    string zipPath = Environment.CurrentDirectory + "\\Builds\\logs\\" + DateTime.Today.ToString("MM.dd.yyyy") + ".zip";
    string dir = Environment.CurrentDirectory + "\\Builds\\logs\\";
    string filePath = Environment.CurrentDirectory + "\\Builds\\logs\\" + DateTime.Today.ToString("MM.dd.yyyy") + ".txt";

    public static string newLog = null;

    private void OnApplicationQuit()
    {
        //Creates a zip file that cointains the resulted text file with logs
        using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
        {
            //Saving the text file in the zip 
            archive.CreateEntryFromFile(filePath, $"{openedAt} - { DateTime.Now.Hour}:{ DateTime.Now.Minute}:{ DateTime.Now.Second}.txt");
        }

        if (File.Exists(filePath)) File.Delete(filePath); //deletes the text file from the folder, so it only stays compressed in the zip
    }

    public void Update()
    {
        if (newLog != null) WriteLog(newLog); //called whenever there is something new to save in the file
    }

    public void WriteLog(string text)
    {
        //Creates directory and text file if they don't exist and writes the text inside
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        if (!File.Exists(filePath)) File.WriteAllText(filePath, text + Environment.NewLine);
        else File.AppendAllText(filePath, text + Environment.NewLine);

        newLog = null;
    }

    public static void WriteInfo(string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] [INFO] ");
        Console.ForegroundColor = ConsoleColor.Gray;

        Console.WriteLine(text);

        newLog = $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] [INFO] " + text;
    }

    public static void WriteError(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] [ERROR] ");
        Console.ForegroundColor = ConsoleColor.Gray;

        Console.WriteLine(text);

        newLog = $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}] [ERROR] " + text;
    }
}
