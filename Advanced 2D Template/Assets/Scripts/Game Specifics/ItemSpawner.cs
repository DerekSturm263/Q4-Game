using System.Collections;
using System.Collections.Generic;
using Types.Miscellaneous;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private List<Item> _items;
    [SerializeField] private Range<int> _multiplier;

    [SerializeField] private Range<float> _width;
    [SerializeField] private Range<float> _height;

    [SerializeField] private Range<Vector3> _rotation;
    [SerializeField] private AnimationCurve[] _velocity;
    [SerializeField] private AnimationCurve _iOverTime;

    [SerializeField] private int _translationResolution;
    [SerializeField] private float _translationSpeed;

    [SerializeField] private ItemInstance _itemPrefab;

    public void Spawn()
    {
        int amount = Random.Range(_multiplier.Min, _multiplier.Max);

        for (int i = 0; i < amount; ++i)
        {
            foreach (var item in _items)
            {
                Quaternion rotation = Quaternion.Euler
                (
                    Random.Range(_rotation.Min.x, _rotation.Max.x),
                    Random.Range(_rotation.Min.y, _rotation.Max.y),
                    Random.Range(_rotation.Min.z, _rotation.Max.z)
                );

                var go = ItemInstance.SpawnFromItem(_itemPrefab, item, transform.position, rotation);
                StartCoroutine(Translate(transform.position, go.transform, Random.Range(-1f, 1f), Random.Range(_width.Min, _width.Max), Random.Range(_height.Min, _height.Max), _velocity[Random.Range(0, _velocity.Length)]));
            }
        }
    }

    private IEnumerator Translate(Vector3 originalPosition, Transform trans, float direction, float width, float height, AnimationCurve curve)
    {
        for (int i = 0; i < _translationResolution; ++i)
        {
            float j = _iOverTime.Evaluate(i / (float)_translationResolution);

            float x = direction * (j) * width;
            float y = curve.Evaluate(j) * height;

            trans.position = originalPosition + new Vector3(x, y);

            yield return new WaitForSeconds((1 / (float)_translationResolution) * _translationSpeed);
        }
    }
}
