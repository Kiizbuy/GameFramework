using GameFramework.Events;
using UnityEngine;
using Zenject;

namespace GameFramework.Installers
{
    public class GameLogicInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _globalEventsRouterPrefab;

        public override void InstallBindings()
        {
            Container.Bind<GlobalEventsRouter>().FromComponentInNewPrefab(_globalEventsRouterPrefab)
                .AsSingle();
        }
    } 
}

