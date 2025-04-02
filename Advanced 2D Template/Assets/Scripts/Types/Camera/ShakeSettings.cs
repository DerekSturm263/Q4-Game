using System;
using UnityEngine;

namespace Types.Camera
{
    [Serializable]
    public struct ShakeSettings
    {
        [SerializeField] private float _frequency;
        public readonly float Frequency => _frequency;

        [SerializeField] private float _amplitude;
        public readonly float Amplitude => _amplitude;

        [SerializeField] private float _time;
        public readonly float Time => _time;
    }
}
