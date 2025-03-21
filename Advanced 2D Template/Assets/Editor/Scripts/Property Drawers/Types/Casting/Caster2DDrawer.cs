using UnityEditor;

namespace Types.Casting
{
    [CustomPropertyDrawer(typeof(Caster2D))]
    internal class Caster2DDrawer : Miscellaneous.PropertyDrawerBase
    {
        public override string[][] GetPropertyNames() => new string[][]
        {
            new string[] { "_direction" },
            new string[] { "_maxDistance" },
            new string[] { "_layerMask" },
            new string[] { "_settings" }
        };
    }
}
