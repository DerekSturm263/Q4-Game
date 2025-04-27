using System;
using Types.Collections;
using UnityEngine;

namespace Types.Miscellaneous
{
    [Serializable]
    public struct AnyGroup
    {
        [SerializeField] private Dictionary<string, Any> _values;

        public AnyGroup(Any value)
        {
            _values = new()
            {
                ["Value"] = value
            };
        }

        public void Set<T>(string name, T value)
        {
            _values ??= new();

            if (!_values.TryAdd(name, Any.FromValue(value)))
                _values[name] = Any.FromValue(value);
        }

        public readonly bool TryGet<T>(string name, out T value)
        {
            bool doReturn = _values.TryGetValue(name, out Any anyValue);

            if (doReturn)
                value = anyValue.Get<T>();
            else
                value = default;

            return doReturn;
        }
    }
}
