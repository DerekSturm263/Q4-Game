using UnityEditor;

namespace Types.Camera
{
    [CustomPropertyDrawer(typeof(ShakeSettings))]
    internal class ShakeSettingsDrawer : Miscellaneous.PropertyDrawerBase
    {
        public override string[][] GetPropertyNames() => new string[][]
        {
            new string[] { "_frequency" },
            new string[] { "_amplitude" },
            new string[] { "_time" }
        };
    }
}
