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

        private SerializedProperty _mainProperty;
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

        private readonly string _monobehaviourMethodPropertyName = "_monobehaviourMethodName";
        private readonly string _monobehaviourReferencePropertyName = "_monobehaviourReference";
        private readonly string _eventObjectPropertyName = "_eventObject";
        private readonly string _isGlobalEventPropertyName = "_isGlobalEvent";
        private readonly string _globalEventPropertyName = "_globalEventName";
        private readonly string _eventObjectLabel = "Event Object";
        private readonly string _isGlobalEventLabel = "Global event?";
        private readonly string _globalEventNameLabel = "Global event name";
        private readonly string _avaliableMethodsLabel = "Avaliable methods";
        private readonly string _warningTitle = "Warning";
        private readonly string _nothingFoundMessage = "Nothing found";
        private readonly string _okLabel = "OK";
        private readonly string _noneLabel = "None";

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
            _mainProperty = propertyFromRootPropertyObject;

            monobehaviourMethodNameProperty = propertyFromRootPropertyObject.FindPropertyRelative(_monobehaviourMethodPropertyName);
            monobehaviourReferenceProperty = propertyFromRootPropertyObject.FindPropertyRelative(_monobehaviourReferencePropertyName);
            eventObjectProperty = propertyFromRootPropertyObject.FindPropertyRelative(_eventObjectPropertyName);
            isGlobalEventProperty = propertyFromRootPropertyObject.FindPropertyRelative(_isGlobalEventPropertyName);
            globalEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative(_globalEventPropertyName);
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
                monobehaviourMethodNameProperty.stringValue = _noneLabel;
                monobehaviourReferenceProperty.objectReferenceValue = null;
                isGlobalEventProperty.boolValue = false;
                globalEventNameProperty.stringValue = string.Empty;
            }

            EditorGUI.LabelField(eventObjectRectTitle, _eventObjectLabel, GetSlowTextStyle());
            EditorGUI.PropertyField(eventObjectRect, eventObjectProperty, GUIContent.none, true);

            EditorGUI.BeginDisabledGroup(eventObjectProperty.objectReferenceValue == null);

            EditorGUI.LabelField(isGlobalEventRectTitle, _isGlobalEventLabel, GetSlowTextStyle());
            EditorGUI.PropertyField(isGlobalEventRect, isGlobalEventProperty, GUIContent.none, true);

            if (isGlobalEventProperty.boolValue)
            {
                EditorGUI.LabelField(globalEventNameTitleRect, _globalEventNameLabel, GetSlowTextStyle());
                EditorGUI.PropertyField(methodsDropdownButtonRect, globalEventNameProperty, GUIContent.none, true);
            }
            else
            {
                EditorGUI.LabelField(methodsDropdownButtonRectTitle, _avaliableMethodsLabel, GetSlowTextStyle());
                selectedMonobehaviourMethodInfoTitle.text = monobehaviourMethodNameProperty.stringValue;

                if (EditorGUI.DropdownButton(methodsDropdownButtonRect, selectedMonobehaviourMethodInfoTitle, FocusType.Passive))
                {
                    var menu = new GenericMenu();
                    var eventGameObject = eventObjectProperty.objectReferenceValue as GameObject;
                    var allComponents = eventGameObject.GetComponents<MonoBehaviour>().ToList();

                    if (allComponents.Count == 0)
                    {
                        EditorUtility.DisplayDialog(_warningTitle, _nothingFoundMessage, _okLabel);
                        selectedMonobehaviourMethodInfoTitle.text = _noneLabel;
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
                                         (_) =>
                                         {
                                             //Dirty hack - need refactoring
                                             var monobehaviourMethodNamePropertyInternal = property.FindPropertyRelative(_monobehaviourMethodPropertyName);
                                             var monobehaviorReferencePropertyInternal = property.FindPropertyRelative(_monobehaviourReferencePropertyName);

                                             monobehaviourMethodNamePropertyInternal.stringValue = componentMethodInfo.MonobehaviourMethodInfo.Name;
                                             monobehaviorReferencePropertyInternal.objectReferenceValue = componentMethodInfo.MonobehaviourReference;

                                             selectedMonobehaviourMethodInfoTitle = new GUIContent($"{currentComponent.GetType().Name}/{currentMethod.Name}");
                                             _mainProperty.serializedObject.ApplyModifiedProperties();
                                         },
                                         componentMethodInfo);
                        }

                    }

                    menu.AddItem(new GUIContent(_noneLabel), false, ClearAllMethodInfo, null);
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

        private void ClearAllMethodInfo(object obj)
        {
            selectedMonobehaviourMethodInfoTitle.text = _noneLabel;
            monobehaviourMethodNameProperty.stringValue = _noneLabel;
            _mainProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 5f;
        }
    }
}

