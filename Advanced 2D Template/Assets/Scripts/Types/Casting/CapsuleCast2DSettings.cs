using System;
using UnityEngine;
using static UnityEngine.GridBrushBase;

namespace Types.Casting
{
    [Serializable]
    public struct CapsuleCast2DSettings : Interfaces.ICastable2D
    {
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Vector2 _size;
        [SerializeField] private CapsuleDirection2D _capsuleDirection;
        [SerializeField] private float _rotation;

        public readonly RaycastHit2D? GetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask)
        {
            if (TryGetHitInfo(position, direction, maxDistance, layerMask, out RaycastHit2D hit))
                return hit;

            return null;
        }

        public readonly bool TryGetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, out RaycastHit2D hit)
        {
            hit = Physics2D.CapsuleCast(position + _offset, _size, _capsuleDirection, _rotation, direction, maxDistance, layerMask);
            return hit;
        }

        public readonly RaycastHit2D[] GetHitInfoAll(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask)
        {
            return Physics2D.CapsuleCastAll(position + _offset, _size, _capsuleDirection, _rotation, direction, maxDistance, layerMask);
        }

        public readonly int GetHitInfoNonAlloc(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, RaycastHit2D[] results)
        {
            return Physics2D.CapsuleCastNonAlloc(position + _offset, _size, _capsuleDirection, _rotation, direction, results, maxDistance, layerMask);
        }

        public readonly void Draw(Vector2 position)
        {

        }
    }
}
