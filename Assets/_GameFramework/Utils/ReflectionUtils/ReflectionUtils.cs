using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameFramework.Strategy;
using UnityEngine;

namespace GameFramework.Utils.Reflection
{
    public static class ReflectionUtils
    {
        private static Dictionary<Type, List<Type>> _allInterfaceTypeImplementations = null;
        private static readonly string _mainAssemblyName = "Assembly-CSharp";

#if UNITY_EDITOR

        static ReflectionUtils()
        {
            CacheStrategyInterfaceTypes();
        }

        private static void CacheStrategyInterfaceTypes()
        {
            Debug.Log("Start Caching interface types");

            _allInterfaceTypeImplementations = new Dictionary<Type, List<Type>>();

            var cSharpAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == _mainAssemblyName);

            if (cSharpAssembly == null)
            {
                Debug.LogError("Main Assembly doesn't exsist on this project");
                return;
            }

            var strategies = cSharpAssembly.GetTypes()
                            .Where(x => x.IsInterface && x.GetInterfaces().Contains(typeof(IStrategyContainer)))
                            .ToList();
             
            foreach (var interfaceKeyType in strategies)
                _allInterfaceTypeImplementations.Add(interfaceKeyType, cSharpAssembly.GetAllDerivedTypes(interfaceKeyType));

            Debug.Log("Caching interface types has been complete");

        }
#endif

        public static List<Type> TryGetConcreteStrategyImplementations(Type concreteImplementationType)
        {
            if (_allInterfaceTypeImplementations.TryGetValue(concreteImplementationType, out var strategyList))
                return strategyList;

            Debug.LogError($"{concreteImplementationType.Name} doesn't exsist");
            return null;
        }

        public static List<Type> GetAllDerivedTypes(this AppDomain appDomain, Type type)
        {
            var cSharpAssembly = appDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == _mainAssemblyName);

            if (cSharpAssembly == null)
            {
                Debug.LogError("Main Assembly doesn't exsist on this project");
                return null;
            }

            return cSharpAssembly.GetAllDerivedTypes(type);
        }

        public static List<Type> GetAllDerivedTypes(this Assembly appAssembly, Type type, string assemblyName = "Assembly-CSharp")
        {

            if (appAssembly.GetName().Name != assemblyName)
            {
                Debug.LogError($"Assembly with '{assemblyName}' name doesn't exsist on this project");
                return null;
            }

            return appAssembly.GetTypes()
                  .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract && !typeof(MonoBehaviour).IsAssignableFrom(x))
                  .OrderBy(x => x.Name)
                  .ToList();
        }
    }
}