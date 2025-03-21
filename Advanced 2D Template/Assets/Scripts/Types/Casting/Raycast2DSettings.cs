using UnityEngine;

namespace Types.Casting
{
    [System.Serializable]
    public struct Raycast2DSettings : Interfaces.ICastable2D
    {
        public readonly RaycastHit2D? GetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask)
        {
            if (TryGetHitInfo(position, direction, maxDistance, layerMask, out RaycastHit2D hit))
                return hit;

            return null;
        }

        public readonly bool TryGetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, out RaycastHit2D hit)
        {
            hit = Physics2D.Raycast(position, direction, maxDistance, layerMask);
            return hit;
        }

        public readonly RaycastHit2D[] GetHitInfoAll(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask)
        {
            return Physics2D.RaycastAll(position, direction, maxDistance, layerMask);
        }

        public readonly int GetHitInfoNonAlloc(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, RaycastHit2D[] results)
        {
            return Physics2D.RaycastNonAlloc(position, direction, results, maxDistance, layerMask);
        }

        public readonly void Draw(Vector2 position)
        {

        }
    }
}
