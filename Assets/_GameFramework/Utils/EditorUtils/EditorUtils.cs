#if UNITY_EDITOR
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

public static class EditorUtils 
{
    //public static List<Type> GetAllOfInterface(Type interfaceType)
    //{
    //    return AppDomain.CurrentDomain.GetAssemblies()
    //          .SelectMany(s => s.GetTypes())
    //          .Where(x => interfaceType.IsAssignableFrom(x) && x != interfaceType && !x.IsAbstract && !typeof(MonoBehaviour).IsAssignableFrom(x))
    //          .OrderBy(x => x.Name)
    //          .ToList();
    //}

    public static string GetMonoScriptPathFor(Type type)
    {
        var asset = string.Empty;
        var guids = AssetDatabase.FindAssets($"{type.Name} t:script");

        if (guids.Length > 1)
        {
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var filename = Path.GetFileNameWithoutExtension(assetPath);
                if (filename == type.Name)
                {
                    asset = guid;
                    break;
                }
            }
        }
        else if (guids.Length == 1)
        {
            asset = guids[0];
        }
        else
        {
            Debug.LogError($"Unable to locate {type.Name}");
            return null;
        }

        return AssetDatabase.GUIDToAssetPath(asset);

    }
}

#endif
