using UnityEditor;

namespace Types.Miscellaneous
{
    [CustomPropertyDrawer(typeof(Stats))]
    internal class StatsDrawer : PropertyDrawerBase
    {
        public override string[][] GetPropertyNames() => new string[][]
        {
            new string[] { "_name" },
            new string[] { "_currentHealth" },
            new string[] { "_maxHealth" },
            new string[] { "_attack" },
            new string[] { "_defense" },
            new string[] { "_actions" }
        };
    }
}
