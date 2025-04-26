using System;
using UnityEngine;

namespace Types.Miscellaneous
{
    [Serializable]
    public struct Type
    {
        [SerializeField] private string _typeName;
        public readonly System.Type Value => System.Type.GetType(_typeName);

        [SerializeField] private bool _isArray;
        public readonly bool IsArray => _isArray;

        public Type(System.Type type)
        {
            _typeName = type.AssemblyQualifiedName;
            _isArray = type.IsArray;
        }
    }
}
