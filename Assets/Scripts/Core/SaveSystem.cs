using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum SaveType
{
    Settings,
    Progress,
}

public static class SaveSystem
{
    private static string GetPath(SaveType type)
    {
        return Path.Combine(Application.persistentDataPath, type.ToString() + ".ldata");
    }

    public static void Save<T>(SaveType type, T data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(GetPath(type), FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
        Debug.Log($"[{type}] Data saved at: {GetPath(type)}");
    }

    public static T Load<T>(SaveType type) where T : class
    {
        string path = GetPath(type);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(stream) as T;
            }
        }
        else
        {
            Debug.LogWarning($"[{type}] No save file found, returning null.");
            return null;
        }
    }

    public static void Delete(SaveType type)
    {
        string path = GetPath(type);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[{type}] Save file deleted.");
        }
    }
}
