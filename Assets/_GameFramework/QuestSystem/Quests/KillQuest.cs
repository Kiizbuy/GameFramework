using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;

namespace GameFramework.Quest
{
    public readonly struct KillEnemyQuestInfo
    {
        public readonly string EnemyName;
        public readonly int MaxKilledEnemies;

        public KillEnemyQuestInfo(string enemyName, int maxKilledEnemies)
        {
            EnemyName = enemyName;
            MaxKilledEnemies = maxKilledEnemies;
        }
    }

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

        private int _currentDeadEnemiesCount;

        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();
        private readonly KillEnemyQuestInfo _questEnemyType;

        public KillQuest(string questName, KillEnemyQuestInfo questEnemyType)
        {
            QuestName = questName;
            _questEnemyType = questEnemyType;
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

        public void IncreaseKilledEnemy(KillEnemyQuestInfo questEnemy)
        {
            if (CurrentQuestStatus == QuestStatus.NotStarted)
                return;

            if (questEnemy.EnemyName == _questEnemyType.EnemyName)
                CurrentDeadEnemiesCount++;

            ChangeQuestStatus(CurrentQuestStatus);
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
            if (CurrentQuestStatus == QuestStatus.NotStarted || CurrentQuestStatus == QuestStatus.Complete)
                return;

            if (CurrentDeadEnemiesCount >= _questEnemyType.MaxKilledEnemies)
                CompleteQuest();
        }

        private void ChangeQuestStatus(QuestStatus status)
        {
            CurrentQuestStatus = status;
            OnStatusChanged?.Invoke(this, CurrentQuestStatus);
        }
    }
}
