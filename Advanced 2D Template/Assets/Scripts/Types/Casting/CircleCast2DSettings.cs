using System;
using UnityEngine;

namespace Types.Casting
{
    [Serializable]
    public struct CircleCast2DSettings : Interfaces.ICastable2D
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _radius;

        public readonly RaycastHit2D? GetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask)
        {
            if (TryGetHitInfo(position, direction, maxDistance, layerMask, out RaycastHit2D hit))
                return hit;

            return null;
        }

        public readonly bool TryGetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, out RaycastHit2D hit)
        {
            hit = Physics2D.CircleCast(position + _offset, _radius, direction, maxDistance, layerMask);
            return hit;
        }

        public readonly RaycastHit2D[] GetHitInfoAll(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask)
        {
            return Physics2D.CircleCastAll(position + _offset, _radius, direction, maxDistance, layerMask);
        }

        public readonly int GetHitInfoNonAlloc(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, RaycastHit2D[] results)
        {
            return Physics2D.CircleCastNonAlloc(position + _offset, _radius, direction, results, maxDistance, layerMask);
        }

        public readonly void Draw(Vector2 position)
        {
            Gizmos.DrawSphere(position + _offset, _radius);
        }
    }
}
