using MonoBehaviours.Text;
using System.Collections.Generic;
using Types.Dialogue;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonoBehaviours.Dialogue
{
    public class DialogueInstance : MonoBehaviour
    {
        [SerializeField] private GameObject _speakerBox;

        [SerializeField] private UnityEvent<string> _onSpeaker;
        [SerializeField] private UnityEvent<string> _onDialogue;

        [SerializeField] private Transform _buttonParent;
        [SerializeField] private GameObject _buttonPrefab;

        [SerializeField] private TMPTextEffectInstance _textEffect;

        private Types.Dialogue.Dialogue _dialogue;
        private DialoguePiece CurrentDialoguePiece => _dialogue.Dialogues[_dialogueIndex];

        private int _dialogueIndex;

        public void Setup(Types.Dialogue.Dialogue dialogue)
        {
            _dialogue = dialogue;
            
            _onSpeaker.Invoke(CurrentDialoguePiece.Speaker);
            _onDialogue.Invoke(CurrentDialoguePiece.Text);

            _speakerBox.SetActive(!string.IsNullOrEmpty(CurrentDialoguePiece.Speaker));
            _textEffect.enabled = !dialogue.IgnoreEffects;

            CurrentDialoguePiece.InvokeOnDialogue();
            _textEffect.onFinished = CurrentDialoguePiece.InvokeOnDialogueFinish;
        }

        public void FinishOrNextDialogue()
        {
            if (!_textEffect.enabled || _textEffect.IsFinished)
                NextDialogue();
            else
                FinishDialogue();
        }

        public void NextDialogue()
        {
            if (_dialogueIndex + 1 >= _dialogue.Dialogues.Count)
            {
                EndDialogue();
                return;
            }

            foreach (Transform buttonTransform in _buttonParent.GetComponentInChildren<Transform>())
            {
                Destroy(buttonTransform.gameObject);
            }

            ++_dialogueIndex;
            _speakerBox.SetActive(!string.IsNullOrEmpty(CurrentDialoguePiece.Speaker));

            _onSpeaker.Invoke(CurrentDialoguePiece.Speaker);
            _onDialogue.Invoke(CurrentDialoguePiece.Text);

            CurrentDialoguePiece.InvokeOnDialogue();
            _textEffect.onFinished = CurrentDialoguePiece.InvokeOnDialogueFinish;
        }

        public void SetDialogueIndex(int index)
        {
            if (index >= _dialogue.Dialogues.Count)
            {
                EndDialogue();
                return;
            }

            _dialogueIndex = index;
        }

        private void EndDialogue()
        {
            _dialogue.InvokeOnDialogueEnd();

            GetComponent<Animator>().SetTrigger("Exit");
        }

        public void FinishDialogue()
        {
            _textEffect.FinishTime();

            SetResponses();
        }

        private void SetResponses()
        {
            List<Button> responseButtons = new();

            foreach (KeyValuePair<string, UnityEvent> response in CurrentDialoguePiece.Responses)
            {
                Button button = Instantiate(_buttonPrefab, _buttonParent).GetComponent<Button>();

                button.GetComponentInChildren<TMPro.TMP_Text>().SetText(response.Key);
                button.onClick.AddListener(() => response.Value.Invoke());

                responseButtons.Add(button);
            }

            if (responseButtons.Count > 0)
                EventSystem.current.SetSelectedGameObject(responseButtons[0].gameObject);
        }
    }
}
