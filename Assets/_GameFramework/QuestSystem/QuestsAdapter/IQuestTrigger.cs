namespace GameFramework.Quest
{
    public abstract class AbstarctQuestTrigger<TQuest, TQuestData> where TQuest : IQuest
    {
        public abstract void Trigger<T>(QuestHandler questHandler, TQuestData questData);
    }

    public interface IQuestTrigger<in TQuest, TQuestData> where TQuest : IQuest
    {
        void Trigger(QuestHandler questHandler, TQuestData questData);
    }
}
