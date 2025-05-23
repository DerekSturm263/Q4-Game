using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonoBehaviours.UI
{
    [AddComponentMenu("UI/Auto Match Size", 13)]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class AutoMatchSize : UIBehaviour
    {
        [SerializeField] private UnityExtensionMethods.Direction _direction;
        [SerializeField] private Vector2 _padding;
        [SerializeField] private Vector2 _parentPadding;

        [SerializeField] private RectTransform _parent;

        private RectTransform _rectTransform;

        protected override void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _rectTransform.SetSizeAuto(_direction, _padding, useMaxParent: !_parent);

            if (_parent)
            {
                Vector2 sizeDelta = _rectTransform.sizeDelta;

                if (!_direction.HasFlag(UnityExtensionMethods.Direction.Horizontal))
                    sizeDelta.x = 0;
                if (!_direction.HasFlag(UnityExtensionMethods.Direction.Vertical))
                    sizeDelta.y = 0;

                _parent.sizeDelta = sizeDelta + _parentPadding;

                _rectTransform.hasChanged = false;
                _parent.hasChanged = false;
            }
        }
    }
}
