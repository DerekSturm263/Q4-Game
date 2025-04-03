using System;
using Types.Collections;
using UnityEngine;

public class Mood : MonoBehaviour
{
    public enum Type
    {
        None,
        Question,
        Exclamation,
        Interact,
        Ellipsis
    }

    private SpriteRenderer _rndr;

    [SerializeField] private Type _type;
    [SerializeField] private Dictionary<Type, Sprite> _sprites;
    [SerializeField] private Vector2 _offset;

    private void Awake()
    {
        GameObject mood = new("Mood");
        mood.transform.SetParent(transform);
        mood.transform.localPosition = _offset;

        _rndr = mood.AddComponent<SpriteRenderer>();
        _rndr.sortingOrder = 99;
    }

    public void SetType(string type)
    {
        _type = (Type)Enum.Parse(typeof(Type), type);

        _rndr.sprite = _sprites[_type];
    }

    public void SetType(Type type)
    {
        _type = type;

        _rndr.sprite = _sprites[_type];
    }

    [ContextMenu("None Mood")]
    public void SetTypeNone() => SetType(Type.None);

    [ContextMenu("Question Mood")]
    public void SetTypeQuestion() => SetType(Type.Question);

    [ContextMenu("Exclamation Mood")]
    public void SetTypeExclamation() => SetType(Type.Exclamation);

    [ContextMenu("Interact Mood")]
    public void SetTypeInteract() => SetType(Type.Interact);

    [ContextMenu("Ellipsis Mood")]
    public void SetTypeEllipsis() => SetType(Type.Ellipsis);
}
