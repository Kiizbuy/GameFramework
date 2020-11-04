using UnityEngine;

namespace GameFramework.Quest
{
    public class ReachTargetQuestTest : MonoBehaviour
    {
        public Transform ReachTargetPoint;
        public DummyQuestEnemy DummyQuestEnemy;
        public QuestHandler QuestHandler;

        private void Start()
        {
            QuestHandler.OnQuestStarted += (quest) => Debug.Log($"{quest.QuestName} started");
            QuestHandler.OnQuestAdded += (quest) => Debug.Log($"Added new quest {quest.QuestName}");
            QuestHandler.OnQuestStatusHasChanged += (quest, status) =>
                Debug.Log($"quest {quest.QuestName} changed status - {quest.CurrentQuestStatus}");
            QuestHandler.OnQuestComplete += (quest) => Debug.Log($"Quest {quest.QuestName} has been completed");

            var reachTargetQuest = new ReachTargetQuest("Reach Target", ReachTargetPoint.position).AddExperienceReward(20);
            QuestHandler.TryAddQuest(reachTargetQuest);
            QuestHandler.StartQuest(reachTargetQuest);
        }
    }
}
