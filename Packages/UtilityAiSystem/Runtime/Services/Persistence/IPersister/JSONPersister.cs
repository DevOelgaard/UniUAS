using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

internal class JSONPersister : IPersister
{
    //public string GetExtension()
    //{
    //    return ".json";
    //}

    public T LoadObject<T>(string path)
    {
        try
        {
            if (!File.Exists(path)) return default(T);
            var json = File.ReadAllText(path);
            var deserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return deserialized;
        } catch(Exception ex)
        {
            throw new Exception("Loading failed: ", ex);
        }
    }

    public void SaveObject<T>(T o, string path)
    {
        try
        {
            var toJson = JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            path += Consts.FileExtension_JSON;
            CreateFile(path);
            File.WriteAllText(path, toJson);
        }
        catch(Exception ex)
        {
            throw new Exception("File Not Saved: ", ex);
        }
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