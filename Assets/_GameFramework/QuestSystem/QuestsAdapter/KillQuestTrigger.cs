using System.Linq;
using UnityEngine;
namespace GameFramework.Quest
{
    public class KillQuestTrigger : MonoBehaviour, IQuestTrigger<KillQuest, IQuestEnemy>
    {
        public QuestHandler QuestHandler;

        private DummyQuestEnemy _dummyQuestEnemy;

        private void Awake()
        {
            _dummyQuestEnemy = GetComponent<DummyQuestEnemy>();
        }

        public void InvokeTrigger()
        {
            Trigger(QuestHandler, _dummyQuestEnemy);
        }

        public void Trigger(QuestHandler questHandler, IQuestEnemy questData)
        {
            foreach (var collectableQuest in questHandler.GetAllQuests(QuestStatus.InProgress).Cast<KillQuest>())
            {
                collectableQuest.IncreaseKilledEnemy(questData);
            }
        }
    }
}
