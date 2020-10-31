using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;

namespace GameFramework.Quest
{
    public class KillQuest : IQuest
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


        public int CurrentDeadEnemiesCount
        {
            get => _currentDeadEnemiesCount;
            private set
            {
                _currentDeadEnemiesCount = value;
                EvaluateQuestCompletion();
            }
        }

        private int _maxDeadEnemiesCountToCompleteQuest;
        private int _currentDeadEnemiesCount;

        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();
        private readonly IEnemy _enemyType;

        public KillQuest(string questName, int maxDeadEnemiesCountToCompleteQuest, IEnemy enemyType)
        {
            QuestName = questName;
            _maxDeadEnemiesCountToCompleteQuest = maxDeadEnemiesCountToCompleteQuest;
            _enemyType = enemyType;
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

        public void IncreaseKilledEnemy(IEnemy enemy)
        {
            if (CurrentQuestStatus == QuestStatus.NotStarted)
                return;

            if (enemy.EnemyHasDied && enemy.EnemyName == _enemyType.EnemyName && enemy.GetType() == _enemyType.GetType())
                CurrentDeadEnemiesCount++;

            ChangeQuestStatus(QuestStatus.InProgress);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void StartQuest()
        {
            ChangeQuestStatus(QuestStatus.InProgress);
            OnStart?.Invoke(this);
        }

        public void Accept(IQuestVisiter visiter)
        {
            visiter.Visit(this);
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
            if (CurrentDeadEnemiesCount >= _maxDeadEnemiesCountToCompleteQuest)
                CompleteQuest();
        }

        private void ChangeQuestStatus(QuestStatus status)
        {
            CurrentQuestStatus = status;
            OnStatusChanged?.Invoke(this, CurrentQuestStatus);
        }
    }
}
