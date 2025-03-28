using SingletonBehaviours;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealth : MonoBehaviour
{
    [SerializeField] private Image _fill;

    private void Awake()
    {
        _fill.fillAmount = SaveDataController.Instance.CurrentData.CurrentHealth / SaveDataController.Instance.CurrentData.MaxHealth;
    }
}
