using SingletonBehaviours;
using Types.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SaveState : MonoBehaviour
{
    [SerializeField] private Dictionary<string, UnityEvent> _events;

    private void Awake()
    {
        Resolve();
    }

    public void Resolve()
    {
        foreach (var unityEvent in _events)
        {
            if (SaveDataController.Instance.CurrentData.StoryData.Contains(unityEvent.Key))
            {
                unityEvent.Value.Invoke();
            }
        }
    }
}
