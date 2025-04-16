namespace SingletonBehaviours
{
    public class SaveStateController : Types.SingletonBehaviour<SaveStateController>
    {
        public void AddState(string id)
        {
            SaveDataController.Instance.CurrentData.StoryData.Add(id);
        }

        public void RemoveState(string id)
        {
            SaveDataController.Instance.CurrentData.StoryData.Remove(id);
        }
    }
}
