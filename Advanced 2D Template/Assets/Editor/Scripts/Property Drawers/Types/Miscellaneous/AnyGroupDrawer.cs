using UnityEditor;

namespace Types.Miscellaneous
{
    [CustomPropertyDrawer(typeof(AnyGroup))]
    internal class AnyGroupDrawer : PropertyDrawerBase
    {
        public override string[][] GetPropertyNames() => new string[][]
        {
            new string[] { "_values" }
        };
    }
}
