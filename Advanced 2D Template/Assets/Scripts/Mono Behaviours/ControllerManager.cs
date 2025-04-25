using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] private List<Types.SingletonBehaviourBase> _controllers;
    [SerializeField] private GameObject _transitionCanvas;

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

        _controllers.Select(item => Instantiate(item, transform)).ToList();
        var transitionCanvas = Instantiate(_transitionCanvas);

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(transitionCanvas);
    }
}
