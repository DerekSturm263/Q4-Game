using UnityEngine;
using MonoBehaviours.Dialogue;
using Types;
using Types.Dialogue;

namespace SingletonBehaviours
{
    public class DialogueController : SingletonBehaviour<DialogueController>
    {
        [SerializeField] private DialogueInstance _prefab;

        public void StartDialogue(DialogueAsset dialogue)
        {
            Canvas canvas = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>();

            var instance = Instantiate(_prefab, canvas.transform);
            instance.Setup(dialogue.Value);
        }
    }
}
