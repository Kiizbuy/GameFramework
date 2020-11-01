using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;

namespace GameFramework.Quest
{
    public class CollectItemsQuest : IQuest
    {
        public event Action<IQuest> OnStart;
        public event Action<IQuest> OnComplete;
        public event Action<IQuest> OnFailed;
        public event Action<IQuest, QuestStatus> OnStatusChanged;

        public QuestStatus CurrentQuestStatus { get; private set; }
        public IEnumerable<BaseItemData> RewardItems { get; }
        public string QuestName { get; }
        public int ExperienceReward { get; private set; }
        public bool QuestHasBeenComplete { get; }

        public int CurrentCollectableItemsCount
        {
            get => _currentCollectableItemsCount;
            private set
            {
                _currentCollectableItemsCount = value;
                EvaluateQuestCompletion();
            }
        }

        private int _currentCollectableItemsCount;

        private readonly int _maxItemsCountToCompleteQuest;
        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();
        private readonly QuestItemData _collectableItemType;

        public CollectItemsQuest(string questName, int maxItemsCountToCompleteQuest, QuestItemData collectableItemType)
        {
            QuestName = questName;
            _maxItemsCountToCompleteQuest = maxItemsCountToCompleteQuest;
            _collectableItemType = collectableItemType;
        }

        public IQuest AddExperienceReward(int expPoints)
        {
            ExperienceReward = expPoints;
            return this;
        }

        public IQuest AddRewardedItems(IEnumerable<BaseItemData> rewardItems)
        {
            _rewardItems.AddRange(rewardItems);
            return this;
        }

        public void FetchCollectableItem(QuestItemData questItem)
        {
            if (questItem.Title == _collectableItemType.Title && _collectableItemType.GetType() == questItem.GetType())
                CurrentCollectableItemsCount++;

            ChangeQuestStatus(CurrentQuestStatus);
        }

        public void Dispose()
        {
        }

        public void StartQuest()
        {
            ChangeQuestStatus(QuestStatus.InProgress);
            OnStart?.Invoke(this);
        }

        public void CompleteQuest()
        {
            ChangeQuestStatus(QuestStatus.Complete);
            OnComplete?.Invoke(this);
        }

        public void FailQuest()
        {
            ChangeQuestStatus(QuestStatus.Failed);
            OnFailed?.Invoke(this);
        }

        public void EvaluateQuestCompletion()
        {
            if (CurrentCollectableItemsCount >= _maxItemsCountToCompleteQuest)
                CompleteQuest();
        }


        private void ChangeQuestStatus(QuestStatus status)
        {
            CurrentQuestStatus = status;
            OnStatusChanged?.Invoke(this, CurrentQuestStatus);
        }
    }
}
