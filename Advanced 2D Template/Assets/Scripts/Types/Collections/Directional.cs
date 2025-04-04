using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Types.Collections
{
    [Serializable]
    public struct Directional<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection, Interfaces.IRotatable<Directional<T>>
    {
        public enum Direction
        {
            North,
            East,
            South,
            West
        }

        [SerializeField] private T _north;
        [SerializeField] private T _east;
        [SerializeField] private T _south;
        [SerializeField] private T _west;

        public readonly int Count => 4;
        public readonly bool IsSynchronized => true;
        public readonly object SyncRoot => default;

        public readonly T this[Direction direction] => direction switch
        {
            Direction.North => _north,
            Direction.East => _east,
            Direction.South => _south,
            Direction.West => _west,
            _ => throw new IndexOutOfRangeException()
        };

        public readonly T this[Vector2 direction]
        {
            get => this[direction.MapTo4Slices(Direction.North, Direction.East, Direction.South, Direction.West)];
        }

        public Directional(T north, T east, T south, T west)
        {
            _north = north;
            _east = east;
            _south = south;
            _west = west;
        }

        public readonly Directional<T> Rotate90() => new(_west, _north, _east, _south);
        public readonly Directional<T> Rotate180() => new(_south, _west, _north, _east);
        public readonly Directional<T> Rotate270() => new(_east, _south, _west, _north);

        public readonly void CopyTo(Array array, int index)
        {
            array.SetValue(_north, 0);
            array.SetValue(_east, 1);
            array.SetValue(_south, 2);
            array.SetValue(_west, 3);
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            yield return _north;
            yield return _east;
            yield return _south;
            yield return _west;
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _north;
            yield return _east;
            yield return _south;
            yield return _west;
        }
    }
}
