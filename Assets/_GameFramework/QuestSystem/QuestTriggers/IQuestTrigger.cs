namespace GameFramework.Quest
{
    public interface IQuestTrigger<in TQuestData>
    {
        void InvokeTrigger();
        void Trigger(QuestHandler questHandler, TQuestData questData);
    }
}
