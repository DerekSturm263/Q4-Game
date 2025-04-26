using System;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;

namespace Types.Miscellaneous
{
    using TupleType = Tuple<string, UnityEngine.Object>;
    
    [Serializable]
    public struct Any : ISerializationCallbackReceiver
    {
        public enum PropertyType
        {
            CSharpObject,
            UnityObject
        }

        [SerializeField] private Type _type;

        [SerializeField] private PropertyType _propertyType;
        [SerializeField] private TupleType _serializableValue;

        [SerializeReference] private object _cSharpObjValue;

        private Any(System.Type type, object value)
        {
            _type = new(type);

            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                _propertyType = PropertyType.UnityObject;
            else
                _propertyType = PropertyType.CSharpObject;

            (_serializableValue, _cSharpObjValue) = _propertyType switch
            {
                PropertyType.UnityObject => (new TupleType("", (UnityEngine.Object)value), null),
                PropertyType.CSharpObject => (new TupleType(JsonConvert.SerializeObject(value), null), value),
                _ => default
            };
        }

        public readonly T Get<T>()
        {
            if (_propertyType == PropertyType.UnityObject)
                return (T)(object)GetUnityObjValue();
            else
                return GetCSharpObjValue<T>();
        }

        private readonly T GetCSharpObjValue<T>()
        {
            try
            {
                if (_cSharpObjValue is not null)
                    return (T)_cSharpObjValue;
                else
                    return default;
            }
            catch
            {
                return default;
            }
        }
        private readonly UnityEngine.Object GetUnityObjValue()
        {
            if (_serializableValue.Item2 != null)
                return _serializableValue.Item2;
            else
                return default;
        }

        public void Set<T>(T value)
        {
            if (typeof(T) != _type.Value)
                throw new ArgumentException($"Given argument \"{typeof(T).Name}\" did not match the current type of the Any.");

            if (_propertyType == PropertyType.UnityObject)
            {
                SetUnityObjValue((UnityEngine.Object)(object)value);
                _cSharpObjValue = null;
            }
            else
            {
                SetCSharpObjValue(value);
                _serializableValue.SetItem2(null);
            }
        }

        private void SetCSharpObjValue<T>(T value) => _cSharpObjValue = value;
        private void SetUnityObjValue(UnityEngine.Object value) => _serializableValue.SetItem2(value);

        public static object GetDefault(System.Type type)
        {
            System.Type thisType = typeof(Any);
            MethodInfo method = thisType.GetMethod("GetDefaultGeneric", BindingFlags.NonPublic | BindingFlags.Static);
            method = method.MakeGenericMethod(type);
            object val = method.Invoke(null, null);

            return val;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static T GetDefaultGeneric<T>() => default;
#pragma warning restore IDE0051 // Remove unused private members

        public readonly override bool Equals(object obj)
        {
            if (obj is Any any)
            {
                if (_propertyType == PropertyType.UnityObject)
                    if (_serializableValue.Item2 != null)
                        return _serializableValue.Item2.Equals(any._serializableValue.Item2);
                    else
                        return obj is null;
                else
                    return Equals(_serializableValue.Item1, any._serializableValue.Item1);
            }
            else
            {
                return false;
            }
        }

        public readonly override int GetHashCode() => HashCode.Combine(_type, _propertyType, _serializableValue);

        public void OnBeforeSerialize()
        {
            _serializableValue.SetItem1(JsonConvert.SerializeObject(_cSharpObjValue));
        }

        public void OnAfterDeserialize()
        {
            _cSharpObjValue = JsonConvert.DeserializeObject(_serializableValue.Item1 ?? string.Empty);
        }

        public static Any FromValue<T>(T value) => new(typeof(T), value);
    }
}
