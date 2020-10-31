using GameFramework.Inventory.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Quest
{
    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Complete,
        Failed
    }

    public enum QuestType
    {
        GetDestination,
        KillTarget,
        GetItem,
        Nested
    }

    public enum NestedQuestType
    {
        Sequential,
        Parallel
    }

    public class Quest : IDisposable
    {
        public event Action<Quest> OnStart;
        public event Action<Quest> OnComplete;
        public event Action<Quest> OnFailed;
        public event Action<Quest, QuestStatus> OnStatusChanged;
        public event Action<Quest, QuestStatus> OnChildQuestChanged;

        private Quest _parentQuest;
        private QuestStatus _questStatus = QuestStatus.NotStarted;
        private QuestType _questType;
        private NestedQuestType _childQuestType;
        private Vector3 _destinationPoint;
        private int _experienceReward;

        public readonly string QuestName;
        private readonly List<Quest> _childQuests = new List<Quest>();
        private readonly List<BaseItemData> _rewardItems = new List<BaseItemData>();

        public Vector3 DestinationPoint => _destinationPoint;
        public IEnumerable<Quest> GetChildQuests => _childQuests;
        public IEnumerable<BaseItemData> GetRewardItems => _rewardItems;
        public QuestStatus GetQuestStatus => _questStatus;
        public bool IsChildQuest => _parentQuest != null;
        public int ExperienceReward => _experienceReward;

        public Quest(string questName, QuestType questType)
        {
            QuestName = questName;
            _questType = questType;
        }

        public Quest AddExperienceReward(int expPoints)
        {
            _experienceReward = expPoints;
            return this;
        }

        public Quest AddRewardedItems(IEnumerable<BaseItemData> rewardItems)
        {
            _rewardItems.AddRange(rewardItems);
            return this;
        }

        public Quest TryAddChildQuest(Quest childQuest)
        {
            if (_childQuests.Contains(childQuest))
                return this;

            childQuest.SetParentQuest(this);

            childQuest.OnStatusChanged += (quest, questStatus) =>
            {
                OnChildQuestChanged?.Invoke(quest, questStatus);
            };

            childQuest.OnChildQuestChanged += (quest, newStatus) =>
            {
                OnChildQuestChanged?.Invoke(childQuest, newStatus);
            };

            childQuest.OnStatusChanged += ProcessChildGuestStatus;

            _childQuests.Add(childQuest);

            return this;
        }

        public Quest SetNestedQuestType(NestedQuestType nestedQuestType)
        {
            _childQuestType = nestedQuestType;

            return this;
        }

        public Quest SetQuestTypeToDestination(Vector3 destinationPoint)
        {
            _questType = QuestType.GetDestination;
            _destinationPoint = destinationPoint;

            return this;
        }

        private void SetParentQuest(Quest parentQuest)
        {
            _parentQuest = parentQuest;
        }


        public void ChangeQuestStatus(QuestStatus status)
        {
            _questStatus = status;
            OnStatusChanged?.Invoke(this, _questStatus);
        }

        public void StartQuest()
        {
            if (_questStatus == QuestStatus.InProgress)
                return;

            ChangeQuestStatus(QuestStatus.InProgress);

            if (_questType == QuestType.Nested)
            {
                switch (_childQuestType)
                {
                    case NestedQuestType.Sequential:
                        _childQuests[0].StartQuest();
                        break;
                    case NestedQuestType.Parallel:
                        {
                            foreach (var quest in _childQuests)
                                quest.StartQuest();

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

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

        private void ProcessChildGuestStatus(Quest quest, QuestStatus newStatus)
        {
            switch (newStatus)
            {
                case QuestStatus.InProgress:
                    return;
                case QuestStatus.Failed:
                    _childQuests.ForEach(x => x.FailQuest());
                    FailQuest();
                    return;
                case QuestStatus.Complete:
                    {
                        if (_childQuests.TrueForAll(x => x.GetQuestStatus == QuestStatus.Complete))
                            CompleteQuest();
                        break;
                    }
            }


            if (_childQuestType != NestedQuestType.Sequential)
                return;

            for (var i = 0; i < _childQuests.Count; i++)
            {
                var currentQuest = _childQuests[i];

                if ((i + 1) == _childQuests.Count)
                    break;

                var nextQuest = _childQuests[i + 1];

                if (currentQuest.GetQuestStatus == QuestStatus.Complete && nextQuest.GetQuestStatus == QuestStatus.NotStarted)
                    nextQuest.StartQuest();
            }

        }

        public void Dispose()
        {

        }
    }
}
