using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class AssetDatabaseService
{
    public static string GetAssetPath(string filter, string type)
    {
        var GUIDS = AssetDatabase.FindAssets(filter);

        var paths = new List<string>();
        foreach (var guid in GUIDS)
        {
            paths.Add(AssetDatabase.GUIDToAssetPath(guid));
        }

        foreach (var p in paths)
        {
            if (p.Contains(type))
            {
                return p;
            }
        }

        return null;
    }

    public static VisualTreeAsset GetVisualTreeAsset(string name)
    {
        if (name.Contains("`"))
        {
            name = name.Substring(0, name.IndexOf("`"));
        }
        var path = AssetDatabaseService.GetAssetPath(name, "uxml");
        return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
    }

    public static TemplateContainer GetTemplateContainer(Type type) {
        return GetTemplateContainer(TypeDescriptor.GetClassName(type));
    }
    public static TemplateContainer GetTemplateContainer(string name)
    {
        var template = GetVisualTreeAsset(name);
        return template.CloneTree();
    }

    public static List<T> GetInstancesOfType<T>()
    {
        var result = new List<T>();
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblie in assemblies)
        {
            var types = assemblie.GetTypes();
            foreach (var type in types)
            {
                if (typeof(T).IsAssignableFrom(type) &&
                    !type.IsAbstract)
                {
                    // Guarding against Test files
                    if (!type.ToString().Contains("Mock") &&
                        !type.ToString().Contains("Stub"))
                    {
                        var instance = (T)Activator.CreateInstance(type);
                        result.Add(instance);
                    }
                }
            }
        }
        return result;
    }

    public static T GetInstanceOfType<T>(string typeName)
    {
        var result = new List<T>();
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assemblie in assemblies)
        {
            var types = assemblie.GetTypes();

            var type = types.FirstOrDefault(t => t.ToString() == typeName);
            if (type != null)
            {
                var instance = (T)Activator.CreateInstance(type);
                return instance;
            }
        }
        return default(T);
    }
}
