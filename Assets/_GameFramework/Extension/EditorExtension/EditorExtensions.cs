#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEditorExtensions
{
    public static class EditorExtensions
    {
        public static List<Type> GetAllDerivedTypes(this AppDomain appDomain, Type interfaceType)
        {
            var cSharpAssembly = appDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");

            if(cSharpAssembly == null)
            {
                Debug.LogError("Main Assembly doesn't exsist on this project");
                return null;
            }

            return cSharpAssembly.GetTypes()
                   //.Where(x => interfaceType.IsAssignableFrom(x) && x != interfaceType && !x.IsAbstract && !typeof(MonoBehaviour).IsAssignableFrom(x))
                   .OrderBy(x => x.Name)
                   .ToList();
        }

        public static T GetObjectValueFromSerializedProperty<T>(this SerializedProperty property) where T : class
        {
            object obj = property.serializedObject.targetObject;
            FieldInfo field = null;

            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path);
                obj = field.GetValue(obj);
            }

            return obj as T;
        }

        public static object GetObjectWhoUseTheProperty(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            FieldInfo field = null;

            foreach (var path in property.propertyPath.Split('.').Where(x => !x.Contains(property.name)))
            {
                var type = obj.GetType();
                field = type.GetField(path);
                obj = field.GetValue(obj);
            }

            return obj;
        }
    }
}

#endif

