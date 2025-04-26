using Callbacks;
using MonoBehaviours;
using Types.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace SingletonBehaviours
{
    public class PopupController : SpawnableController<PopupAsset>
    {
        [SerializeField] private GameObject _buttonTemplate;
        [SerializeField] private GameObject _inputFieldTemplate;

        private PopupCallbackContext _continueContext;

        protected override bool TakeAwayFocus() => true;

        public void InsertEvent(InvokeableEvent ctx)
        {
            _continueContext = new(() => ctx.Invoke());
        }

        public void ClearEvent()
        {
            _continueContext = default;
        }

        protected override void SetUp(PopupAsset popup)
        {
            TMPro.TMP_Text[] texts = _templateInstance.transform.GetComponentsInChildren<TMPro.TMP_Text>();
            var layoutGroups = _templateInstance.GetComponentsInChildren<LayoutGroup>();

            texts[0].SetText(popup.Value.Title);
            texts[1].SetText(popup.Value.Description);

            Transform buttonParent = layoutGroups[0].transform;
            foreach (var response in popup.Value.Responses)
            {
                Button button = Instantiate(_buttonTemplate, buttonParent).GetComponent<Button>();

                button.GetComponentInChildren<TMPro.TMP_Text>().SetText(response.Key);
                button.onClick.AddListener(() => response.Value.Invoke(_continueContext));
            }

            Transform inputFieldParent = layoutGroups[1].transform;
            if (popup.Value.InputResponse.Item2 is not null && popup.Value.InputResponse.Item2.GetPersistentEventCount() > 0)
            {
                TMPro.TMP_InputField inputField = Instantiate(_inputFieldTemplate, inputFieldParent).GetComponentInChildren<TMPro.TMP_InputField>();

                inputField.SetTextWithoutNotify(popup.Value.InputResponse.Item1);
                inputField.onEndEdit.AddListener(popup.Value.InputResponse.Item2.Invoke);
            }
        }

        public void ContinueAction(PopupCallbackContext ctx)
        {
            ctx.Invoke();
            ClearEvent();
        }
    }
}
