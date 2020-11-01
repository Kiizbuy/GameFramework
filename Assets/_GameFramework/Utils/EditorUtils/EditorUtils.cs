#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public static class EditorUtils
{
    public static T[] GetAllAssetsOfType<T>(Type type, string fileExtension) where T : Object
    {
        var tempObjects = new List<T>();
        var directory = new DirectoryInfo(Application.dataPath);
        var goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);
        var goFileInfoLength = goFileInfo.Length;
        var i = 0;

        string tempFilePath;
        FileInfo tempGoFileInfo;
        Object tempGO;

        for (; i < goFileInfoLength; i++)
        {
            tempGoFileInfo = goFileInfo[i];
            if (tempGoFileInfo == null)
                continue;

            tempFilePath = tempGoFileInfo.FullName;
            tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
            tempGO = AssetDatabase.LoadAssetAtPath<Object>(tempFilePath);

            if (tempGO == null)
            {
                Debug.LogWarning("Skipping Null");
                continue;
            }

            if (tempGO.GetType() != type)
            {
                Debug.LogWarning("Skipping " + tempGO.GetType().ToString());
                continue;
            }

            tempObjects.Add(tempGO as T);
        }

        return tempObjects.ToArray();
    }

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
                if (filename != type.Name)
                    continue;

                asset = guid;
                break;
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
