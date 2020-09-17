using NaughtyAttributes;
using System.IO;
using UnityEngine;
using Zenject;

namespace GameFramework.Settings
{
    public class GameSettingsSOData : ScriptableObject
    {
        [SerializeField, Header("Base Data Settings")]
        private string _fileNameExtension = ".json";
        [SerializeField, ReadOnly]
        private string _defaultSettings = _noneSaveInfo;
        [SerializeField, ReadOnly]
        private string _saveFileDataSettings = _noneSaveInfo;
      
        private bool _defaultSettingsHasInitialized = false;
        [Inject]
        private ISerializationProvider _serializationProvider;

        protected const string _noneSaveInfo = "(None)";

        public GameSettingsSOData InitDefaultSettings()
        {
            //return this;

            if (_defaultSettingsHasInitialized)
                return this;
            //_serializationProvider = new UnityJsonSerialization();

            //_defaultSettings = _serializationProvider.SerializeObject(this);
            _defaultSettingsHasInitialized = true;

            //Load();
            OnInitializationHasComplete();

            //Debug.LogError(_serializationProvider.GetType().Name);

            return this;
        }

        protected virtual string GetSavePath
            => Application.persistentDataPath;
        protected string GetFullPath(string filename = "")
            => Path.Combine(GetSavePath, GetSavePath);
        protected string GetFullFileName()
            => GetType().Name.ToString() + _fileNameExtension;
        protected bool FileDoesntExsist(FileInfo saveFile)
            => saveFile.Directory != null && !saveFile.Directory.Exists;
        protected bool CanLoadData()
            => _saveFileDataSettings != string.Empty || _saveFileDataSettings != _noneSaveInfo;

        public virtual void OnInitializationHasComplete() { }

        public virtual void Save()
        {
            var fullSaveFilePath = GetFullPath(GetFullFileName());
            var saveFile = new FileInfo(fullSaveFilePath);

            _saveFileDataSettings = _serializationProvider.SerializeObject(this);

            if (FileDoesntExsist(saveFile))
                saveFile.Directory.Create();

            File.WriteAllText(fullSaveFilePath, _saveFileDataSettings);
        }


        public virtual void Load()
        {
            if (CanLoadData())
            {
                _serializationProvider.DeserializeObject(_saveFileDataSettings, this);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }


        public virtual void ResetSettings()
        {
            if (_defaultSettingsHasInitialized)
            {
                _serializationProvider.DeserializeObject(_defaultSettings, this);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

    }
}

