using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.Extension
{
    public static class ReflectionExtension
    {
        private static readonly string _mainAssemblyName = "Assembly-CSharp";

        public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }

        public static List<Type> GetAllDerivedTypes(this AppDomain appDomain, Type baseType)
        {
            var cSharpAssembly = appDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == _mainAssemblyName);

            if (cSharpAssembly == null)
            {
                Debug.LogError("Main Assembly doesn't exsist on this project");
                return null;
            }

            /// TODO: Add Fillter by type
            return cSharpAssembly.GetTypes()
                   //.Where(x => interfaceType.IsAssignableFrom(x) && x != interfaceType && !x.IsAbstract && !typeof(MonoBehaviour).IsAssignableFrom(x))
                   .OrderBy(x => x.Name)
                   .ToList();
        }
    }
}

