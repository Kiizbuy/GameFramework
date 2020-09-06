using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Events
{
    [CustomPropertyDrawer(typeof(MethodsTemplateData))]
    public class MethodsTemplateDataPropertyDrawer : PropertyDrawer
    {
        private GUIContent selectedMonobehaviourMethodInfoTitle = new GUIContent("None");

        private SerializedProperty monobehaviourMethodNameProperty;
        private SerializedProperty monobehaviourReferenceProperty;
        private SerializedProperty eventObjectProperty;
        private SerializedProperty isGlobalEventProperty;
        private SerializedProperty globalEventNameProperty;

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
            monobehaviourMethodNameProperty = propertyFromRootPropertyObject.FindPropertyRelative("_monobehaviourMethodName");
            monobehaviourReferenceProperty = propertyFromRootPropertyObject.FindPropertyRelative("_monobehaviourReference");
            eventObjectProperty = propertyFromRootPropertyObject.FindPropertyRelative("_eventObject");
            isGlobalEventProperty = propertyFromRootPropertyObject.FindPropertyRelative("_isGlobalEvent");
            globalEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative("_globalEventName");
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
}

