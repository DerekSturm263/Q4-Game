using UnityEditor;

namespace Types.Miscellaneous
{
    [CustomPropertyDrawer(typeof(SaveData))]
    internal class SaveDataDrawer : PropertyDrawerBase
    {
        public override string[][] GetPropertyNames() => new string[][]
        {
            new string[] { "_stats" },
            new string[] { "_items" },
            new string[] { "_mask" },
            new string[] { "_storyData" },
            new string[] { "_playerAbilities" },
            new string[] { "_position" },
            new string[] { "_questsCompleted" },
            new string[] { "_time" },
            new string[] { "_interactStates" }
        };
    }
}
