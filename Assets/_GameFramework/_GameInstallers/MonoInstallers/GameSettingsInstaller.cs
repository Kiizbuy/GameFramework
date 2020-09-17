using GameFramework.Extension;
using GameFramework.Settings;
using System.Linq;
using UnityEngine;
using Zenject;

namespace GameFramework.Installers
{
    public class GameSettingsInstaller : MonoInstaller
    {
        [SerializeField] private bool _bindGameSettings = false;

        public override void InstallBindings()
        {
            if (_bindGameSettings)
                BindGameSettings();
        }

        private void BindGameSettings()
        {
            var allGameSettings = Resources.LoadAll<GameSettingsSOData>(string.Empty);

            foreach (var currentGameSettings in allGameSettings)
            {

                currentGameSettings.InitDefaultSettings();

                var gameSettingsInheritanceTree = currentGameSettings.GetType()
                                                    .GetInheritanceHierarchy()
                                                    .Where(x => x.IsSubclassOf(typeof(GameSettingsSOData)) || x == typeof(GameSettingsSOData));

                Container.Bind(gameSettingsInheritanceTree)
                    .FromInstance(currentGameSettings)
                    .AsCached()
                    .NonLazy();
            }
        }
    }
}

