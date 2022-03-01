using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PersistenceAPI
{
    //public static void PersistBinary(object o, string path, string fileName)
    //{
    //    Debug.LogWarning("This should implement a Try/Catch if serialization fails");
    //    CreateFile(path, fileName);

    //    var bytes = SerializationUtility.SerializeValue(o, DataFormat.Binary);

    //    File.WriteAllBytes(path + fileName, bytes);
    //}

    //public static object LoadBinary(string fullPath)
    //{
    //    Debug.LogWarning("This should implement a Try/Catch if serialization fails");
    //    if (!File.Exists(fullPath)) return false;

    //    var bytes = File.ReadAllBytes(fullPath);

    //    return SerializationUtility.DeserializeValue<object>(bytes, DataFormat.Binary);
    //}

    //https://stackoverflow.com/questions/48910547/jsonconvert-deserialize-an-array-of-abstract-classes/48910729
    public static void PersistJson<T>(object o, string path, string fileName)
    {
        var toJson = JsonConvert.SerializeObject((T)o, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        CreateFile(path, fileName);
        File.WriteAllText(path + fileName, toJson);
    }

    public static object LoadJson<T>(string fullPath)
    {
        Debug.LogWarning("This should implement a Try/Catch if serialization fails");
        if (!File.Exists(fullPath)) return null;
        var json = File.ReadAllText(fullPath);
        var deserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return deserialized;
    }

    public static void CreateFile(string path, string fileName)
    {
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(path);
            var file = File.Create(path + fileName);
            file.Close();
        }
    }
}