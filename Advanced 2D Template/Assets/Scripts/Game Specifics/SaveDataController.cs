using Extensions;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Types.Miscellaneous;
using Types.Wrappers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SingletonBehaviours
{
    public class SaveDataController : Types.SingletonBehaviour<SaveDataController>
    {
        [SerializeField] private SaveDataAsset _default;

        private SaveData _currentData;
        public ref SaveData CurrentData => ref _currentData;

        public override void Initialize() => Load();

        public void Save()
        {
            Serializable<SaveData> saveData = new(Instance._currentData, "", "", new string[] { }, new Types.Miscellaneous.Tuple<string, string>[] { });

            RenderTextureDescriptor descriptor = new(1024, 512, RenderTextureFormat.ARGB32, 8, 0, RenderTextureReadWrite.sRGB);
            RenderTexture pictureTexture = new(descriptor);
            
            saveData.CreateIcon(Camera.main, output: pictureTexture);
            saveData.Save();
        }

        public void Load(Serializable<SaveData> saveData)
        {
            Instance._currentData = saveData.Value;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Load()
        {
            var saves = SerializationHelper.LoadAllFromDirectory<Serializable<SaveData>>($"{Application.persistentDataPath}/SaveData");

            Instance._currentData = 
                saves.Count() == 0 ?
                _default.Value :
                Instance._currentData = saves.OrderByDescending(item => item.LastEditedDate).ElementAt(0);
        }

        public void EnableOnSaveData(GameObject go)
        {
            go.SetActive(!Instance._currentData.Equals(_default.Value));
        }

        [ContextMenu("Open Directory")]
        public void OpenDirectory()
        {
            Process.Start("explorer.exe", $"{Application.persistentDataPath}/SaveData");
        }

        [ContextMenu("Delete Data")]
        public void DeleteData()
        {
            Helpers.SerializationHelper.Delete($"{_default.name}.json", $"{Application.persistentDataPath}/SaveData");
            Instance._currentData = _default.Value;
        }
    }
}
