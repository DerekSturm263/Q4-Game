using Types.Miscellaneous;
using Types.Wrappers;
using UnityEngine;

public class PopulateSaveDatas : PopulateSerializables<SaveData>
{
    [SerializeField] private string _format;

    protected override string Name(Serializable<SaveData> item) => item.LastEditedDate.ToString(_format);
    protected override Sprite Icon(Serializable<SaveData> item) => item.Icon;
}
