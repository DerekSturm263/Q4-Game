using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SingletonBehaviours
{
    public class SaveDataController : Types.SingletonBehaviour<SaveDataController>
    {
        [SerializeField] private Types.Miscellaneous.SaveDataAsset _default;

        private Types.Miscellaneous.SaveData _currentData;
        public Types.Miscellaneous.SaveData CurrentData => _currentData;

        public override void Initialize()
        {
            _currentData = Helpers.SerializationHelper.Load(_default.Value, $"{Application.persistentDataPath}/SaveData", $"{_default.name}.json");
        }

        public override void Shutdown()
        {
            Helpers.SerializationHelper.Save(_currentData, $"{Application.persistentDataPath}/SaveData", $"{_default.name}.json");
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
        }
    }
}
