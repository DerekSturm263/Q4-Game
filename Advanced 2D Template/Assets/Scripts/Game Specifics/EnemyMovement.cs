using SingletonBehaviours;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EntityStats _stats;

    public void LoadSceneParameters()
    {
        SceneController.Instance.SetSceneParameter("Stats", _stats.Stats);
    }
}
