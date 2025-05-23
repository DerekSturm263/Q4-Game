using System;
using UnityEngine;
using UnityEngine.Events;

namespace Types.Dialogue
{
    [Serializable]
    public struct DialoguePiece
    {
        public enum SpeakerSide
        {
            Left,
            Right
        }

        [SerializeField] private string _speaker;
        public readonly string Speaker => _speaker;

        [SerializeField] private SpeakerSide _side;
        public readonly SpeakerSide Side => _side;

        [SerializeField][TextArea] private string _text;
        public readonly string Text => _text;

        [SerializeField] private UnityEvent _onDialogue;
        public readonly void InvokeOnDialogue() => _onDialogue?.Invoke();

        [SerializeField] private UnityEvent _onDialogueFinish;
        public readonly void InvokeOnDialogueFinish() => _onDialogueFinish?.Invoke();

        [SerializeField] private Collections.Dictionary<string, UnityEvent> _responses;
        public readonly Collections.Dictionary<string, UnityEvent> Responses => _responses;
    }
}
