using UnityEditor;

namespace Types.Miscellaneous
{
    [CustomPropertyDrawer(typeof(SaveData))]
    internal class SaveDataDrawer : PropertyDrawerBase
    {
        public override string[][] GetPropertyNames() => new string[][]
        {
            new string[] { "_items" },
            new string[] { "_mask" },
            new string[] { "_currentHealth" },
            new string[] { "_maxHealth" },
            new string[] { "_attack" },
            new string[] { "_defense" },
            new string[] { "_storyData" },
            new string[] { "_playerAbilities" },
            new string[] { "_position" }
        };
    }
}
