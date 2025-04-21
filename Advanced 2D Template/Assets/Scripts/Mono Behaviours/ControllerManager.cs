using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] private List<Types.SingletonBehaviourBase> _controllers;
    private List<Types.SingletonBehaviourBase> _controllerInstances;

    private static bool _isActive;

    private void Awake()
    {
        if (!_isActive)
        {
            _isActive = true;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _controllerInstances = _controllers.Select(item => Instantiate(item, transform)).ToList();
        DontDestroyOnLoad(gameObject);
    }
}
