using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Settings;
using Zenject;


namespace GameFramework.Installers
{
    public class SerializationProvideSOrInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISerializationProvider>()
                .To<UnityJsonSerialization>()
                .AsSingle()
                .NonLazy();
        }
    }
}

