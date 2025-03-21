using System;
using UnityEngine;

namespace Types.Casting
{
    [Serializable]
    public struct Caster2D
    {
        [SerializeField] private Vector2 _direction;
        [SerializeField] private float _maxDistance;
        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private Miscellaneous.Variant<BoxCast2DSettings, CircleCast2DSettings, CapsuleCast2DSettings, Raycast2DSettings> _settings;

        public readonly RaycastHit2D? GetHitInfo(Transform transform, Vector2 offset = default)
        {
            return _settings.Get<Interfaces.ICastable2D>().GetHitInfo((Vector2)transform.position + offset, _direction, _maxDistance, _layerMask);
        }

        public readonly bool TryGetHitInfo(Transform transform, out RaycastHit2D hit)
        {
            return _settings.Get<Interfaces.ICastable2D>().TryGetHitInfo(transform.position, _direction, _maxDistance, _layerMask, out hit);
        }

        public readonly RaycastHit2D[] GetHitInfoAll(Transform transform)
        {
            return _settings.Get<Interfaces.ICastable2D>().GetHitInfoAll(transform.position, _direction, _maxDistance, _layerMask);
        }

        public readonly int GetHitInfoNonAlloc(Transform transform, RaycastHit2D[] results)
        {
            return _settings.Get<Interfaces.ICastable2D>().GetHitInfoNonAlloc(transform.position, _direction, _maxDistance, _layerMask, results);
        }

        public readonly void Draw(Transform transform, Vector2 offset = default)
        {
            _settings.Get<Interfaces.ICastable2D>().Draw((Vector2)transform.position + offset);
        }
    }
}
