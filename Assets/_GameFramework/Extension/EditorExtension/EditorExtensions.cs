#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UnityEditorExtensions
{
    public static class EditorExtensions
    {
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

