﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Events
{
    [CustomPropertyDrawer(typeof(EventsTemplateData))]
    public class EventsTemplateDataPropertyDrawer : PropertyDrawer
    {
        private GUIContent _selectedMonobehaviourEventInfoTitle = new GUIContent("None");

        private SerializedProperty _monobehaviourEventNameProperty;
        private SerializedProperty _monobehaviourReferenceProperty;
        private SerializedProperty _eventObjectProperty;
        private SerializedProperty _isGlobalEventProperty;
        private SerializedProperty _globalEventNameProperty;

        private Rect _eventObjectRect;
        private Rect _isGlobalEventRect;
        private Rect _eventsDropdownButtonRect;
        private Rect _eventObjectRectTitle;
        private Rect _isGlobalEventRectTitle;
        private Rect _eventsDropdownButtonRectTitle;
        private Rect _globalEventNameTitleRect;

        private readonly string _monobehaviourEventNamePropertyName = "_monobehaviourEventName";
        private readonly string _monobehaviourReferencePropertyName = "_monobehaviourReference";
        private readonly string _eventObjectPropertyName = "_eventObject";
        private readonly string _isGlobalEventPropertyName = "_isGlobalEvent";
        private readonly string _globalEventNamePropertyName = "_globalEventName";
        private readonly string _noneLabel = "None";
        private readonly string _eventObjectLabel = "Event Object";
        private readonly string _globalEventLabel = "Global event?";
        private readonly string _globalEventNameLabel = "Global event name";
        private readonly string _avaliableEventsLabel = "Avaliable events";
        private readonly string _warningTitleLabel = "Warning";
        private readonly string _nothingFoundMessageLabel = "Nothing Found";
        private readonly string _okLabel = "OK";

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
            _monobehaviourEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative(_monobehaviourEventNamePropertyName);
            _monobehaviourReferenceProperty = propertyFromRootPropertyObject.FindPropertyRelative(_monobehaviourReferencePropertyName);
            _eventObjectProperty = propertyFromRootPropertyObject.FindPropertyRelative(_eventObjectPropertyName);
            _isGlobalEventProperty = propertyFromRootPropertyObject.FindPropertyRelative(_isGlobalEventPropertyName);
            _globalEventNameProperty = propertyFromRootPropertyObject.FindPropertyRelative(_globalEventNamePropertyName);
        }

        private void InitializePropertyRects(Rect propertyRect)
        {
            _eventObjectRect = new Rect(propertyRect.x, propertyRect.y, 100, 20);
            _isGlobalEventRect = new Rect(propertyRect.x + 105, propertyRect.y, 50, 20);
            _eventsDropdownButtonRect = new Rect(propertyRect.x + 175, propertyRect.y, propertyRect.width - 220, 20);
            _eventObjectRectTitle = _eventObjectRect;
            _isGlobalEventRectTitle = _isGlobalEventRect;
            _eventsDropdownButtonRectTitle = _eventsDropdownButtonRect;
            _globalEventNameTitleRect = _eventsDropdownButtonRect;

            _eventObjectRectTitle.y -= 15;
            _isGlobalEventRectTitle.y -= 15;
            _eventsDropdownButtonRectTitle.y -= 15;
            _globalEventNameTitleRect.y -= 15;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeProperties(property);
            position.y += 20f;

            EditorGUI.BeginProperty(position, label, property);

            InitializePropertyRects(position);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            if (_eventObjectProperty.objectReferenceValue == null)
            {
                _monobehaviourEventNameProperty.stringValue = _noneLabel;
                _monobehaviourReferenceProperty.objectReferenceValue = null;
                _isGlobalEventProperty.boolValue = false;
                _globalEventNameProperty.stringValue = string.Empty;
            }

            EditorGUI.LabelField(_eventObjectRectTitle, _eventObjectLabel, GetSlowTextStyle());
            EditorGUI.PropertyField(_eventObjectRect, _eventObjectProperty, GUIContent.none, true);

            EditorGUI.BeginDisabledGroup(_eventObjectProperty.objectReferenceValue == null);

            EditorGUI.LabelField(_isGlobalEventRectTitle, _globalEventLabel, GetSlowTextStyle());
            EditorGUI.PropertyField(_isGlobalEventRect, _isGlobalEventProperty, GUIContent.none, true);

            if (_isGlobalEventProperty.boolValue)
            {
                EditorGUI.LabelField(_globalEventNameTitleRect, _globalEventNameLabel, GetSlowTextStyle());
                EditorGUI.PropertyField(_eventsDropdownButtonRect, _globalEventNameProperty, GUIContent.none, true);
            }
            else
            {
                EditorGUI.LabelField(_eventsDropdownButtonRectTitle, _avaliableEventsLabel, GetSlowTextStyle());
                _selectedMonobehaviourEventInfoTitle.text = _monobehaviourEventNameProperty.stringValue;
                if (EditorGUI.DropdownButton(_eventsDropdownButtonRect, _selectedMonobehaviourEventInfoTitle, FocusType.Passive))
                {
                    var menu = new GenericMenu();
                    var eventGameObject = _eventObjectProperty.objectReferenceValue as GameObject;
                    var allComponents = eventGameObject.GetComponents<MonoBehaviour>().ToList();

                    if (allComponents.Count == 0)
                    {
                        EditorUtility.DisplayDialog(_warningTitleLabel, _nothingFoundMessageLabel, _okLabel);
                        _selectedMonobehaviourEventInfoTitle.text = _noneLabel;
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
                                         _monobehaviourEventNameProperty.stringValue == componentMethodInfo.MonobehaviourEventInfo.Name,
                                         (x) =>
                                         {
                                             _monobehaviourEventNameProperty.stringValue = componentMethodInfo.MonobehaviourEventInfo.Name;
                                             _monobehaviourReferenceProperty.objectReferenceValue = componentMethodInfo.MonobehaviourReference;
                                             property.serializedObject.ApplyModifiedProperties();
                                             _selectedMonobehaviourEventInfoTitle.text = $"{currentComponent.GetType().Name}/{currentEvent.Name}";
                                         },
                                         componentMethodInfo);
                        }

                    }

                    menu.AddItem(new GUIContent(_noneLabel), false, ClearAllEventInfo, property);
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
                _selectedMonobehaviourEventInfoTitle.text = $"{unityEventInfo.MonobehaviourReference.GetType().Name}/{unityEventInfo.MonobehaviourEventInfo.Name}";
                _monobehaviourEventNameProperty.stringValue = unityEventInfo.MonobehaviourEventInfo.Name;
                _monobehaviourEventNameProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ClearAllEventInfo(object obj)
        {
            _monobehaviourEventNameProperty.stringValue = "None";
            _selectedMonobehaviourEventInfoTitle.text = "None";
            _monobehaviourEventNameProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 5f;
        }
    }
}