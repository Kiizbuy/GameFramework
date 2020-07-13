using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditorExtensions;
using UnityEditor;
#endif

namespace GameFramework.Events
{
    [Serializable]
    public class EventToMethodSubscribeСontainer
    {
        public List<MethodsTemplateData> MethodsTemplateData;
    }

    [Serializable]
    public class MethodsTemplateData
    {
        public UnityMonobehaviourEventInfo UnityMonobehaviourEventInfo;
        public GameObject eventObject;
        public bool IsGlobalEvent;
        public string GlobalEventName;
        public string MonobehaviourMethodName = "None";
        public MonoBehaviour MonobehaviourReference;
    }
#if UNITY_EDITOR
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
                Debug.LogError("Event name attribute is not found. Mark it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, "Event name attribute is not found. Mark it", GetHeaderGUIStyle(Color.red));
                return;
            }

            eventName = eventNameAttribute.EventName;
            methodsTemplateDataProperty = property.FindPropertyRelative("MethodsTemplateData");
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

#endif

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MethodsTemplateData))]
    public class MethodsTemplateDataPropertyDrawer : PropertyDrawer
    {
        private GUIContent selectedMonobehaviourMethodInfoTitle = new GUIContent("None");

        private SerializedProperty monobehaviourMethodNameProperty;
        private SerializedProperty monobehaviourReferenceProperty;
        private SerializedProperty eventObjectProperty;
        private SerializedProperty isGlobalEventProperty;
        private SerializedProperty globalEventNameProperty;
        private SerializedProperty unityMonobehaviourEventInfo;

        private Rect eventObjectRect;
        private Rect isGlobalEventRect;
        private Rect methodsDropdownButtonRect;
        private Rect eventObjectRectTitle;
        private Rect isGlobalEventRectTitle;
        private Rect methodsDropdownButtonRectTitle;
        private Rect globalEventNameTitleRect;


        private GUIStyle GetSlowTextStyle()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 8,
                fontStyle = FontStyle.Bold,
                fixedWidth = 100
            };

            return style;
        }

        private void InitializeProperties(SerializedProperty propertyFromRootPropertyObject)
        {
            monobehaviourMethodNameProperty = propertyFromRootPropertyObject.FindPropertyRelative("MonobehaviourMethodName");
            monobehaviourReferenceProperty = propertyFromRootPropertyObject.FindPropertyRelative("MonobehaviourReference");
            eventObjectProperty = propertyFromRootPropertyObject.FindPropertyRelative("eventObject");
            isGlobalEventProperty = propertyFromRootPropertyObject.FindPropertyRelative("IsGlobalEvent");
            globalEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative("GlobalEventName");
            unityMonobehaviourEventInfo = propertyFromRootPropertyObject.FindPropertyRelative("UnityMonobehaviourEventInfo").FindPropertyRelative("MonobehaviourReference");
        }

        private void InitializePropertyRects(Rect propertyRect)
        {
            eventObjectRect = new Rect(propertyRect.x, propertyRect.y, 100, 20);
            isGlobalEventRect = new Rect(propertyRect.x + 105, propertyRect.y, 50, 20);
            methodsDropdownButtonRect = new Rect(propertyRect.x + 175, propertyRect.y, propertyRect.width - 220, 20);
            eventObjectRectTitle = eventObjectRect;
            isGlobalEventRectTitle = isGlobalEventRect;
            methodsDropdownButtonRectTitle = methodsDropdownButtonRect;
            globalEventNameTitleRect = methodsDropdownButtonRect;

            eventObjectRectTitle.y -= 15;
            isGlobalEventRectTitle.y -= 15;
            methodsDropdownButtonRectTitle.y -= 15;
            globalEventNameTitleRect.y -= 15;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeProperties(property);

            position.y += 20f;
            EditorGUI.BeginProperty(position, label, property);

            InitializePropertyRects(position);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            if (eventObjectProperty.objectReferenceValue == null)
            {
                monobehaviourMethodNameProperty.stringValue = "None";
                monobehaviourReferenceProperty.objectReferenceValue = null;
                isGlobalEventProperty.boolValue = false;
                globalEventNameProperty.stringValue = string.Empty;
            }

            EditorGUI.LabelField(eventObjectRectTitle, "Event Object", GetSlowTextStyle());
            EditorGUI.PropertyField(eventObjectRect, eventObjectProperty, GUIContent.none, true);

            EditorGUI.BeginDisabledGroup(eventObjectProperty.objectReferenceValue == null);

            EditorGUI.LabelField(isGlobalEventRectTitle, "Global event?", GetSlowTextStyle());
            EditorGUI.PropertyField(isGlobalEventRect, isGlobalEventProperty, GUIContent.none, true);

            if (isGlobalEventProperty.boolValue)
            {
                EditorGUI.LabelField(globalEventNameTitleRect, "Global event name", GetSlowTextStyle());
                EditorGUI.PropertyField(methodsDropdownButtonRect, globalEventNameProperty, GUIContent.none, true);
            }
            else
            {
                EditorGUI.LabelField(methodsDropdownButtonRectTitle, "Avaliable methods", GetSlowTextStyle());
                selectedMonobehaviourMethodInfoTitle.text = monobehaviourMethodNameProperty.stringValue;
                if (EditorGUI.DropdownButton(methodsDropdownButtonRect, selectedMonobehaviourMethodInfoTitle, FocusType.Passive))
                {
                    var menu = new GenericMenu();
                    var eventGameObject = eventObjectProperty.objectReferenceValue as GameObject;
                    var allComponents = eventGameObject.GetComponents<MonoBehaviour>().ToList();

                    if (allComponents.Count == 0)
                    {
                        EditorUtility.DisplayDialog("Warning", "НИЧЕГО НЕ НАШЕЛ", "OK");
                        selectedMonobehaviourMethodInfoTitle.text = "None";
                        return;
                    }

                    foreach (var currentComponent in allComponents)
                    {
                        var allComponentMethods = currentComponent.GetType().GetMethods();
                        var filteredMethods = GetFillteredMethods(allComponentMethods);

                        foreach (var currentMethod in filteredMethods)
                        {
                            var componentMethodIndoGUIContent = new GUIContent($"{currentComponent.GetType().Name}/{currentMethod.Name}");
                            var componentMethodInfo = new UnityMonobehaviourMethodInfo(currentComponent, currentMethod);

                            menu.AddItem(componentMethodIndoGUIContent,
                                         monobehaviourMethodNameProperty.stringValue == componentMethodInfo.MonobehaviourMethodInfo.Name,
                                         (x) =>
                                         {
                                             monobehaviourMethodNameProperty.stringValue = componentMethodInfo.MonobehaviourMethodInfo.Name;
                                             monobehaviourReferenceProperty.objectReferenceValue = componentMethodInfo.MonobehaviourReference;
                                             property.serializedObject.ApplyModifiedProperties();
                                             selectedMonobehaviourMethodInfoTitle.text = $"{currentComponent.GetType().Name}/{currentMethod.Name}";
                                         },
                                         componentMethodInfo);
                        }

                    }

                    menu.AddItem(new GUIContent("None"), false, ClearAllMethodInfo, null);
                    menu.ShowAsContext();

                    property.serializedObject.ApplyModifiedProperties();
                }

            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private IEnumerable<MethodInfo> GetFillteredMethods(MethodInfo[] allMonobehaviourMethods)
        {
            return allMonobehaviourMethods.Where(x => x.IsPublic &&
                                                !x.ContainsGenericParameters &&
                                                !x.Name.StartsWith("set_") &&
                                                 x.ReturnType == typeof(void) &&
                                                 x.GetParameters().Length == 1 &&
                                                 x.GetParameters()[0].ParameterType == typeof(EventParameter)
                                                );
        }

        private void SelectMethodFromMonobehaviour(object obj)
        {
            if (obj is UnityMonobehaviourMethodInfo unityMethodInfo)
            {
                selectedMonobehaviourMethodInfoTitle.text = $"{unityMethodInfo.MonobehaviourReference.GetType().Name}/{unityMethodInfo.MonobehaviourMethodInfo.Name}";
                monobehaviourMethodNameProperty.stringValue = unityMethodInfo.MonobehaviourMethodInfo.Name;
                monobehaviourMethodNameProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ClearAllMethodInfo(object obj)
        {
            selectedMonobehaviourMethodInfoTitle.text = "None";
            monobehaviourMethodNameProperty.stringValue = "None";
            monobehaviourMethodNameProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 5f;
        }
    }
#endif
}
