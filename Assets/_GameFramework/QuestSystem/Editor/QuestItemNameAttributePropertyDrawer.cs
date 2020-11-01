using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Quest
{
    [CustomPropertyDrawer(typeof(QuestItemNameAttribute))]
    public class QuestItemNameAttributePropertyDrawer : PropertyDrawer
    {
        private QuestInfoStorage _questInfoStorage;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_questInfoStorage == null)
                    _questInfoStorage = GetAssetsOfType<QuestInfoStorage>(typeof(QuestInfoStorage), ".asset").FirstOrDefault();

                var propertyString = property.stringValue;
                var questItemNamesList = new List<string> { "(None)" };


                if (_questInfoStorage != null)
                    questItemNamesList.AddRange(_questInfoStorage.AllQuestItemsNames);

                var index = 0;

                if (questItemNamesList.Count == 0)
                    EditorGUILayout.HelpBox("ПОШЕЛ НАХУЙ, ЛИБО НЕ НАШЕЛ ТВОЕ ГОВНО, ЛИБО ИНФА ПУСТАЯ, ДОЛБОЕБИНА", MessageType.Warning);


                for (var i = 1; i < questItemNamesList.Count; i++)
                {
                    if (questItemNamesList[i] != propertyString)
                        continue;
                    index = i;
                    break;
                }

                index = EditorGUI.Popup(position, label.text, index, questItemNamesList.ToArray());
                property.stringValue = index > 0 ? questItemNamesList[index] : string.Empty;
            }
            else
            {
                var message = $"{nameof(QuestItemNameAttributePropertyDrawer)} supports only string fields";
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
        }

        public static T[] GetAssetsOfType<T>(Type type, string fileExtension) where T : Object
        {
            var tempObjects = new List<T>();
            var directory = new DirectoryInfo(Application.dataPath);
            var goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

            var i = 0;
            var goFileInfoLength = goFileInfo.Length;

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

                Debug.Log(tempFilePath + "\n" + Application.dataPath);

                tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(Object)) as Object;
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
    }
}
