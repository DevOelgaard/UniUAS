using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

internal class JSONPersister : IPersister
{
    public string GetExtension()
    {
        return ".json";
    }

    public T LoadObject<T>(string path)
    {
        Debug.LogWarning("This should implement a Try/Catch if serialization fails");
        if (!File.Exists(path)) return default(T);
        var json = File.ReadAllText(path);
        var deserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return deserialized;
    }

    public void SaveObject<T>(T o, string path)
    {
        var toJson = JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        CreateFile(path);
        File.WriteAllText(path, toJson);
    }

    private void CreateFile(string path)
    {
        var directory = Path.GetDirectoryName(path);
        var fileName = Path.GetFileName(path);
        if (!File.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            var file = File.Create(directory + fileName);
            file.Close();
        }
    }
}