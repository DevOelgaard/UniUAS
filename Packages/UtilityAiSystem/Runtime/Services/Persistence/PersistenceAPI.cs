using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

internal class PersistenceAPI
{
    private IPersister persister;

    internal PersistenceAPI(IPersister persister)
    {
        this.persister = persister;
    }

    internal void SaveObjectPanel(RestoreAble o)
    {
        var extension = FileExtensionService.GetExtension(o);
        var path = EditorUtility.SaveFilePanel("Save object", "", "name", extension);
        if (path == null || path.Length == 0)
        {
            return;
        }
        o.SaveToFile(path, persister);
        //persister.SaveObject(o, path);
    }

    internal void SaveObjectPath<T>(T o, string path)
    {
        path += FileExtensionService.GetExtension(o);
        persister.SaveObject(o, path);
    }

    internal T LoadObjectPanel<T>()
    {
        var extension = FileExtensionService.GetExtension(typeof(T));
        var path = EditorUtility.OpenFilePanel("Load object", "", extension);
        var o = persister.LoadObject<T>(path);
        return o;
    }

    internal T LoadObjectPath<T>(string path)
    {
        return persister.LoadObject<T>(path);
    }
}