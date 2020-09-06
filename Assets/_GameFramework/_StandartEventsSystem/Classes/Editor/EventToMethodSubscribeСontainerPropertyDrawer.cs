using System.Reflection;
using UnityEditor;
using UnityEditorExtensions;
using UnityEditorInternal;
using UnityEngine;

namespace GameFramework.Events
{
    [CustomPropertyDrawer(typeof(EventToMethodSubscribeСontainer))]
    public class EventToMethodSubscribeСontainerPropertyDrawer : PropertyDrawer
    {
        private ReorderableList methodsTemplateDataReorderableList;
        private EventToMethodSubscribeСontainer propertyObjectInstance;

        private SerializedProperty methodsTemplateDataProperty;

        private Rect reordableListRect;
        private Rect foldoutButtonRect;
        private Rect foldoutLabelRect;
        private Rect foldinBoxRect;

        private readonly string _methodsTemplateDataLabel = "_methodsTemplateData";
        private readonly string _eventWarrningMessageLabel = "Event name attribute is not found. Mark it";

        private GUIStyle GetHeaderGUIStyle(Color labelColor)
        {
            var labelGUIStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };

            labelGUIStyle.normal.textColor = labelColor;

            return labelGUIStyle;
        }


        private void InitializeRects(Rect propertyRect)
        {
            reordableListRect = propertyRect;
            foldoutButtonRect = propertyRect;
            foldoutLabelRect = propertyRect;
            foldinBoxRect = propertyRect;

            foldinBoxRect.xMin = propertyRect.x + 2;

            foldoutButtonRect.width = 100;
            foldoutButtonRect.height = 20;

            foldoutLabelRect.x += 6;
            foldoutLabelRect.width = 400;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectWhoUseProperty = property.GetObjectWhoUseTheProperty();
            var eventNameAttribute = (EventNameAttribute)fieldInfo.GetCustomAttribute(typeof(EventNameAttribute), false);
            var eventName = string.Empty;

            if (eventNameAttribute == null)
            {
                Debug.LogError(_eventWarrningMessageLabel);
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, _eventWarrningMessageLabel, GetHeaderGUIStyle(Color.red));
                return;
            }

            eventName = eventNameAttribute.EventName;
            methodsTemplateDataProperty = property.FindPropertyRelative(_methodsTemplateDataLabel);
            propertyObjectInstance = property.GetObjectValueFromSerializedProperty<EventToMethodSubscribeСontainer>();

            if (ObjectDoesntHaveEvent(objectWhoUseProperty, eventName))
            {
                Debug.LogError($"{eventName} event doesn't exsist on object, Please, use an existing eventName on object if you have it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, $"''{eventName}'' event doesn't exsist on object", GetHeaderGUIStyle(Color.red));
                return;
            }

            InitializeRects(position);

            property.isExpanded = EditorGUI.Foldout(foldoutButtonRect, property.isExpanded, GUIContent.none, true);

            if (methodsTemplateDataReorderableList == null)
                methodsTemplateDataReorderableList = BuildReorderableListFromProperty(methodsTemplateDataProperty, eventName);

            EditorGUI.BeginProperty(position, label, property);


            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.isExpanded)
            {
                methodsTemplateDataReorderableList.DoList(reordableListRect);
            }
            else
            {
                var headerGUIContent = new GUIContent(eventName);
                GUI.Box(foldinBoxRect, GUIContent.none);
                GUI.Label(foldoutLabelRect, headerGUIContent, GetHeaderGUIStyle(Color.blue));
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private bool ObjectDoesntHaveEvent(object eventableObject, string eventName)
        {
            return eventableObject.GetType().GetEvent(eventName) == null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return GetPropertyHeightFromReorderableList(methodsTemplateDataReorderableList);

            return EditorGUIUtility.singleLineHeight;
        }

        private float GetPropertyHeightFromReorderableList(ReorderableList list)
        {
            return (list != null ?
                    list.headerHeight + list.footerHeight + (list.elementHeight * Mathf.Max(list.count, 1) + 10f) :
                    80);
        }

        private ReorderableList BuildReorderableListFromProperty(SerializedProperty property, string headerName)
        {
            var newReordableList = new ReorderableList(property.serializedObject, property, true, false, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), true);
                },

            };

            newReordableList.drawHeaderCallback += (rect) =>
            {
            //var headerGUIContent = new GUIContent(property.FindPropertyRelative("EventName").stringValue);
            var headerGUIContent = new GUIContent(headerName);
                GUI.Label(rect, headerGUIContent, GetHeaderGUIStyle(Color.blue));
            };

            newReordableList.onRemoveCallback += (list) =>
            {
                property.DeleteArrayElementAtIndex(list.index);
            };

            newReordableList.elementHeight = 40f;

            return newReordableList;
        }
    }
}