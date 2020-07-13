using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace GameFramework.Editor.Utils
{
    [InitializeOnLoad]
    public class NewCreatedSceneSetuper
    {
        private static readonly string debugConsolePath = "Assets/_GameFramework/Prefabs/DebugConsole/IngameDebugConsolePlaceholder.prefab";

        static NewCreatedSceneSetuper()
        {
            EditorSceneManager.newSceneCreated += SetupNewScene;
        }

        private static void SetupNewScene(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            if (setup == NewSceneSetup.EmptyScene)
                return;

            var mainCamera = Camera.main?.transform;
            var directionalLight = GameObject.Find("Directional Light")?.transform;

            var ui = new GameObject("UI").transform;
            var logic = new GameObject("Logic").transform;
            var debug = new GameObject("Debug").transform;
            var singletons = new GameObject("Singletons").transform;
            var environment = new GameObject("Environment").transform;
            var geo = new GameObject("Geo").transform;
            var staticObjects = new GameObject("Static").transform;
            var dynamicObjects = new GameObject("Dynamic").transform;
            var runtime = new GameObject("Runtime").transform;
            var lighting = new GameObject("Lights").transform;
            var cameras = new GameObject("Cameras").transform;
            var vfx = new GameObject("VFX").transform;
            var audio = new GameObject("Audios").transform;
            var sfx = new GameObject("SFX").transform;
            var music = new GameObject("Music").transform;
            var entites = new GameObject("Entites").transform;

            lighting.SetParent(environment.transform);
            mainCamera?.SetParent(cameras.transform);

            debug.SetParent(logic);

            cameras.SetParent(environment.transform);
            directionalLight?.SetParent(lighting.transform);

            geo.SetParent(environment);
            singletons.SetParent(logic);

            staticObjects.SetParent(geo);
            dynamicObjects.SetParent(geo);

            vfx.SetParent(environment);

            audio.SetParent(environment);
            sfx.SetParent(audio);
            music.SetParent(audio);

            staticObjects.gameObject.isStatic = true;
            CreateSceneLogicPrefabs(logic);
        }

        private static void CreateSceneLogicPrefabs(Transform logicObject)
        {
            var debugConsoleAsset = AssetDatabase.LoadAssetAtPath<GameObject>(debugConsolePath);

            if (debugConsoleAsset != null)
            {
                var debugConsoleObject = PrefabUtility.InstantiatePrefab(debugConsoleAsset) as GameObject;
                var debugPlaceholder = logicObject.Find("Debug");
                debugConsoleObject.transform.SetParent(debugPlaceholder);
            }
            else
            {
                Debug.LogError($"Debug console is not found on path: {debugConsolePath}");
            }

            var globalEventsRouter = new GameObject("Global Events").AddComponent<Events.GlobalEventsRouter>();
            var singletonsPlaceholder = logicObject.Find("Singletons");
            var uiEventSystem = new GameObject("EventSystem").AddComponent<EventSystem>().transform;

            uiEventSystem.gameObject.AddComponent<StandaloneInputModule>();
            uiEventSystem.SetParent(logicObject);

            globalEventsRouter.transform.SetParent(singletonsPlaceholder);
        }
    }
}

