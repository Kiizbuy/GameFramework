# GameFramework

Game framework - A set of ready-made solutions that will help make prototype / game development faster and improve development forkflow.

It includes My own solutions:
- Custom .Net events inspector serialization (Alpha) - my own solution that allows you to sign .Net events directly from the inspector from the necessary components. It can work in conjunction with EventBus (it is already built into it)
- Strategy container (Alpha) = my own solution which can help serialize interface serialize reference type in your classes. Example included.
- Basic game components set(In progress)
- Generic singleton behavour
- Edior new created scene Hierarchy Setuper - my own solution which create a true hierarhchy in scene after creating him
- Basic Extension Methods
- Editor Extension Methods
- Game Util class(Needs Refactoring - very awful solution) - this class contains many helpful functions and interesting solutions - UnityJsonListWrapper, Fast Math structures, ProtectionStructures, FastRandom and e.t.c
- uGUI based Inventory (Alpha) - my own cross-project solution that allows you to use inventory in almost all projects
- Buy-cell system (Alpha) - my own solution which can help create base buy-cell system in your projects. Needs to complete
- Craft system for inventory(Alpha)
- MVP_UI Solution - components that allow you to use the Model-View-Presenter pattern
- WindowNavigator (I recommend using Doozy UI for such cases) - solution which can help you create navigation with your gameScreens. This class can save your history on game screen transitions

And Custom OpenSource or free solutions:
- DoTween - Tween library
- Dublicator - A solution that allows you to create a large number of objects in a couple of clicks
- FolderCreator - a solution that allows you to configure the structure in an empty project (includes a basic preset of the project structure)
- PathCreator by sebastian lague - solution for splines
- NaughtyAttributes - set of Property attributes
- EModules_Autosave - backup scenes
- OneLine - editor solution that allows you to draw a large number of fields for components in one line
- SimplestMeshBaker - A solution for baking meshes
- IngameDebugConsole - Debug console solution for ready-made builds

TODO(In next iteration):
- Integrate Zenject
- Refactor .Net events inspector serialization
- Create BeforeBuild And PostBuild system which can make actions with your started build your project and after build(Zip game folder or somethink else)
- Refactor StrategyContainer with cache interface types in editor
- Refactor GameUtil class, because its awful
- Complete inventory system with added modules for him (Buy-cell system, Craft System)
- Add Equipment module to inventory

This framework will be finalized over time.

Enjoy That;)