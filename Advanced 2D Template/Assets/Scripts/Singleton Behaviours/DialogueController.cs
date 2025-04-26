using MonoBehaviours.Dialogue;
using Types.Dialogue;

namespace SingletonBehaviours
{
    public class DialogueController : SpawnableController<DialogueAsset>
    {
        protected override bool TakeAwayFocus() => true;

        protected override void SetUp(DialogueAsset t)
        {
            _templateInstance.GetComponent<DialogueInstance>().Setup(t.Value);
        }
    }
}
