using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Strategy
{
    [CustomPropertyDrawer(typeof(StrategyContainerAttribute))]
    public class StartegyAttributePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _serializedProperty;
        private IStrategyContainer _strategyFieldValue;
        private FieldInfo _propertyField;
        private int _strategyImplementationIndex;
        private List<Type> _interfaceTypes;
        private string[] _names;

        private void InitProperty(SerializedProperty property)
        {
            var objectType = property.serializedObject.targetObject.GetType();

            _serializedProperty = property;
            _propertyField = objectType.GetField(property.name);
            _strategyFieldValue = (IStrategyContainer)_propertyField.GetValue(property.serializedObject.targetObject);
            _serializedProperty = property;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitProperty(property);

            EditorGUI.BeginProperty(position, label, property);

            DrawpopupStrategyImplementations(property, label);

            if (_strategyFieldValue != null)
                DrawStrategyImplementationFields(position, property);

            CheckAndChangeStrategyTypeState();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 50f;
        }

        private void DrawpopupStrategyImplementations(SerializedProperty property, GUIContent label)
        {
            if (_propertyField == null)
                return;

            if (_interfaceTypes == null)
            {
                _interfaceTypes = EditorUtils.GetAllOfInterface(_propertyField.FieldType);
            }

            if (_interfaceTypes == null)
            {
                EditorGUILayout.HelpBox("Not found strategy implementations", MessageType.Warning);
                return;
            }

            if (_names == null)
            {
                _names = new string[1];
                _names[0] = "None selected strategy implementation";
                _names = _names.Concat(EditorUtils.GetAllOfInterfaceNames(_propertyField.FieldType)).ToArray();
            }

            if (_strategyFieldValue != null)
                _strategyImplementationIndex = 1 + _interfaceTypes.FindIndex(0, type => type == _strategyFieldValue.GetType());

            EditorGUILayout.BeginHorizontal();
            label.text += " Strategy";
            EditorGUILayout.LabelField(label);
            _strategyImplementationIndex = EditorGUILayout.Popup(_strategyImplementationIndex, _names);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawStrategyImplementationFields(Rect position, SerializedProperty property)
        {
            EditorGUI.indentLevel++;

            ForeachChildProperty(property, (prop) =>
                EditorGUILayout.PropertyField(prop, new GUIContent(prop.name), true, null));

            EditorGUI.indentLevel--;
        }

        private void ForeachChildProperty(SerializedProperty property, Action<SerializedProperty> action)
        {
            var endProperty = property.GetEndProperty();

            property = property.Copy();
            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, endProperty))
            {
                action(property);
                property.NextVisible(false);
            }
        }

        private void CheckAndChangeStrategyTypeState()
        {
            if (_propertyField == null)
                return;

            if (_strategyImplementationIndex == 0)
            {
                ClearStrategy();
                return;
            }

            if (_strategyFieldValue == null || _strategyFieldValue.GetType() != _interfaceTypes[_strategyImplementationIndex - 1])
                CreateStrategy(_interfaceTypes[_strategyImplementationIndex - 1]);
        }

        private void ClearStrategy()
        {
            if (_strategyFieldValue != null)
            {
                _serializedProperty.managedReferenceValue = null;
                _serializedProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private void CreateStrategy(Type strategyType)
        {
            var strategyImplementation = Activator.CreateInstance(strategyType) as IStrategyContainer;

            _strategyFieldValue = strategyImplementation;
            _serializedProperty.managedReferenceValue = _strategyFieldValue;
            _serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}

