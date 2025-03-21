using UnityEngine;

namespace Interfaces
{
    public interface ICastable2D
    {
        public RaycastHit2D? GetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask);
        public bool TryGetHitInfo(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, out RaycastHit2D hit);

        public RaycastHit2D[] GetHitInfoAll(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask);
        public int GetHitInfoNonAlloc(Vector2 position, Vector2 direction, float maxDistance, LayerMask layerMask, RaycastHit2D[] results);

        public void Draw(Vector2 position);
    }
}
