using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorExtensions;
using UnityEditorInternal;
#endif

namespace GameFramework.Events
{
    [Serializable]
    public class MethodToEventSubscribeContainer
    {
        public List<EventsTemplateData> EventsTemplateData;
    }

    [Serializable]
    public class EventsTemplateData
    {
        public UnityMonobehaviourMethodInfo UnityMonobehaviourMethodInfo;
        public GameObject eventObject;
        public bool IsGlobalEvent;
        public string GlobalEventName;
        public string MonobehaviourEventName = "None";
        public MonoBehaviour MonobehaviourReference;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MethodToEventSubscribeContainer))]
    public class MethodToEventSubscribeContainerPropertyDrawer : PropertyDrawer
    {
        private readonly Color headerColor = new Color(1, 0.45f, 0);
        private ReorderableList methodsTemplateDataReorderableList;
        private MethodToEventSubscribeContainer propertyObjectInstance;

        private SerializedProperty eventsTemplateDataProperty;

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
            var methodNameAttribute = (MethodNameAttribute)fieldInfo.GetCustomAttribute(typeof(MethodNameAttribute), false);
            var methodName = string.Empty;

            if (methodNameAttribute == null)
            {
                Debug.LogError("Method name attribute is not found. Mark it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, "Method name attribute is not found. Mark it", GetHeaderGUIStyle(Color.red));
                return;
            }

            methodName = methodNameAttribute.MethodName;
            eventsTemplateDataProperty = property.FindPropertyRelative("EventsTemplateData");
            propertyObjectInstance = property.GetObjectValueFromSerializedProperty<MethodToEventSubscribeContainer>();

            if (ObjectDoesntHaveMethod(objectWhoUseProperty, methodName))
            {
                Debug.LogError($"{methodName} event doesn't exsist on object, Please, use an existing eventName on object if you have it");
                GUI.Box(position, GUIContent.none);
                GUI.Label(position, $"''{methodName}'' event doesn't exsist on object", GetHeaderGUIStyle(Color.red));
                return;
            }

            InitializeRects(position);

            property.isExpanded = EditorGUI.Foldout(foldoutButtonRect, property.isExpanded, GUIContent.none, true);

            if (methodsTemplateDataReorderableList == null)
                methodsTemplateDataReorderableList = BuildReorderableListFromProperty(eventsTemplateDataProperty, methodName);

            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.isExpanded)
            {
                methodsTemplateDataReorderableList.DoList(reordableListRect);
            }
            else
            {
                var headerGUIContent = new GUIContent(methodName);
                GUI.Box(foldinBoxRect, GUIContent.none);
                GUI.Label(foldoutLabelRect, headerGUIContent, GetHeaderGUIStyle(headerColor));
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private bool ObjectDoesntHaveMethod(object eventableObject, string methodName)
        {
            return eventableObject.GetType().GetMethod(methodName) == null;
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
                var headerGUIContent = new GUIContent(headerName);
                GUI.Label(rect, headerGUIContent, GetHeaderGUIStyle(headerColor));
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
    [CustomPropertyDrawer(typeof(EventsTemplateData))]
    public class EventsTemplateDataPropertyDrawer : PropertyDrawer
    {
        private GUIContent selectedMonobehaviourEventInfoTitle = new GUIContent("None");

        private SerializedProperty monobehaviourEventNameProperty;
        private SerializedProperty monobehaviourReferenceProperty;
        private SerializedProperty eventObjectProperty;
        private SerializedProperty isGlobalEventProperty;
        private SerializedProperty globalEventNameProperty;
        private SerializedProperty unityMonobehaviourMethodInfo;

        private Rect eventObjectRect;
        private Rect isGlobalEventRect;
        private Rect eventsDropdownButtonRect;
        private Rect eventObjectRectTitle;
        private Rect isGlobalEventRectTitle;
        private Rect eventsDropdownButtonRectTitle;
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
            monobehaviourEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative("MonobehaviourEventName");
            monobehaviourReferenceProperty = propertyFromRootPropertyObject.FindPropertyRelative("MonobehaviourReference");
            eventObjectProperty = propertyFromRootPropertyObject.FindPropertyRelative("eventObject");
            isGlobalEventProperty = propertyFromRootPropertyObject.FindPropertyRelative("IsGlobalEvent");
            globalEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative("GlobalEventName");
            unityMonobehaviourMethodInfo = propertyFromRootPropertyObject.FindPropertyRelative("UnityMonobehaviourMethodInfo").FindPropertyRelative("MonobehaviourReference");
        }

        private void InitializePropertyRects(Rect propertyRect)
        {
            eventObjectRect = new Rect(propertyRect.x, propertyRect.y, 100, 20);
            isGlobalEventRect = new Rect(propertyRect.x + 105, propertyRect.y, 50, 20);
            eventsDropdownButtonRect = new Rect(propertyRect.x + 175, propertyRect.y, propertyRect.width - 220, 20);
            eventObjectRectTitle = eventObjectRect;
            isGlobalEventRectTitle = isGlobalEventRect;
            eventsDropdownButtonRectTitle = eventsDropdownButtonRect;
            globalEventNameTitleRect = eventsDropdownButtonRect;

            eventObjectRectTitle.y -= 15;
            isGlobalEventRectTitle.y -= 15;
            eventsDropdownButtonRectTitle.y -= 15;
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
                monobehaviourEventNameProperty.stringValue = "None";
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
                EditorGUI.PropertyField(eventsDropdownButtonRect, globalEventNameProperty, GUIContent.none, true);
            }
            else
            {
                EditorGUI.LabelField(eventsDropdownButtonRectTitle, "Avaliable events", GetSlowTextStyle());
                selectedMonobehaviourEventInfoTitle.text = monobehaviourEventNameProperty.stringValue;
                if (EditorGUI.DropdownButton(eventsDropdownButtonRect, selectedMonobehaviourEventInfoTitle, FocusType.Passive))
                {
                    var menu = new GenericMenu();
                    var eventGameObject = eventObjectProperty.objectReferenceValue as GameObject;
                    var allComponents = eventGameObject.GetComponents<MonoBehaviour>().ToList();

                    if (allComponents.Count == 0)
                    {
                        EditorUtility.DisplayDialog("Warning", "НИЧЕГО НЕ НАШЕЛ", "OK");
                        selectedMonobehaviourEventInfoTitle.text = "None";
                        return;
                    }

                    foreach (var currentComponent in allComponents)
                    {
                        var allComponentEvents = currentComponent.GetType().GetEvents();
                        var filteredEvents = GetFillteredMethods(allComponentEvents);

                        foreach (var currentEvent in filteredEvents)
                        {
                            var componentMethodIndoGUIContent = new GUIContent($"{currentComponent.GetType().Name}/{currentEvent.Name}");
                            var componentMethodInfo = new UnityMonobehaviourEventInfo(currentComponent, currentEvent);

                            menu.AddItem(componentMethodIndoGUIContent,
                                         monobehaviourEventNameProperty.stringValue == componentMethodInfo.MonobehaviourEventInfo.Name,
                                         (x) =>
                                         {
                                             monobehaviourEventNameProperty.stringValue = componentMethodInfo.MonobehaviourEventInfo.Name;
                                             monobehaviourReferenceProperty.objectReferenceValue = componentMethodInfo.MonobehaviourReference;
                                             property.serializedObject.ApplyModifiedProperties();
                                             selectedMonobehaviourEventInfoTitle.text = $"{currentComponent.GetType().Name}/{currentEvent.Name}";
                                         },
                                         componentMethodInfo);
                        }

                    }

                    menu.AddItem(new GUIContent("None"), false, ClearAllEventInfo, property);
                    menu.ShowAsContext();

                    property.serializedObject.ApplyModifiedProperties();
                }

            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        private IEnumerable<EventInfo> GetFillteredMethods(EventInfo[] allMonobehaviourMethods)
             => allMonobehaviourMethods.Where(x => x.EventHandlerType == typeof(Action<EventParameter>));

        private void SelectMethodFromMonobehaviour(object obj)
        {
            if (obj is UnityMonobehaviourEventInfo unityEventInfo)
            {
                selectedMonobehaviourEventInfoTitle.text = $"{unityEventInfo.MonobehaviourReference.GetType().Name}/{unityEventInfo.MonobehaviourEventInfo.Name}";
                monobehaviourEventNameProperty.stringValue = unityEventInfo.MonobehaviourEventInfo.Name;
                monobehaviourEventNameProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ClearAllEventInfo(object obj)
        {
            monobehaviourEventNameProperty.stringValue = "None";
            selectedMonobehaviourEventInfoTitle.text = "None";
            monobehaviourEventNameProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 5f;
        }
    }
#endif
}