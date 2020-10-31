using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;

namespace GameFramework.Quest
{
    public class SequenceQuest : IQuest
    {
        public event Action<IQuest> OnStart;
        public event Action<IQuest> OnComplete;
        public event Action<IQuest> OnFailed;
        public event Action<IQuest, QuestStatus> OnStatusChanged;


        private readonly List<IQuest> _childrenQuests = new List<IQuest>();
        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();

        public IEnumerable<BaseItemData> RewardItems => _rewardItems;

        public QuestStatus CurrentQuestStatus { get; private set; }
        public string QuestName { get; private set; }
        public int ExperienceReward { get; private set; }
        public bool QuestHasBeenComplete { get; private set; }
        public void StartQuest()
        {
            throw new NotImplementedException();
        }

        public void Accept(IQuestVisiter visiter)
        {
            throw new NotImplementedException();
        }

        public void CompleteQuest()
        {
            throw new NotImplementedException();
        }

        public void FailQuest()
        {
            throw new NotImplementedException();
        }

        public SequenceQuest(string questName)
        {
            QuestName = questName;
        }

        public IQuest AddExperienceReward(int expPoints)
        {
            ExperienceReward = expPoints;
            return this;
        }

        public IQuest AddRewardedItems(IEnumerable<BaseItemData> rewardItems)
        {
            return this;
        }

        public IQuest AddQuest(IQuest quest)
        {
            if (!_childrenQuests.Contains(quest))
                _childrenQuests.Add(quest);

            return this;
        }


        public void EvaluateQuestCompletion()
        {
        }

        private void UnsubscribeAllEvents()
        {
            if (OnStart != null)
                foreach (var action in OnStart.GetInvocationList())
                    OnStart -= action as Action<IQuest>;

            if (OnComplete != null)
                foreach (var action in OnComplete.GetInvocationList())
                    OnComplete -= action as Action<IQuest>;

            if (OnFailed != null)
                foreach (var action in OnFailed.GetInvocationList())
                    OnFailed -= action as Action<IQuest>;

            if (OnStatusChanged != null)
                foreach (var action in OnStatusChanged.GetInvocationList())
                    OnStatusChanged -= action as Action<IQuest, QuestStatus>;
        }

        public void Dispose()
        {
            UnsubscribeAllEvents();
        }
    }
}
