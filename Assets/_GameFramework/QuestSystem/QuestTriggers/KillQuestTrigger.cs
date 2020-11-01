using System;
using System.Linq;
using UnityEngine;
namespace GameFramework.Quest
{
    [Serializable]
    public class Huy
    {
        [QuestEnemyName]
        public string Huyy;
    }

    public class KillQuestTrigger : MonoBehaviour, IQuestTrigger<KillEnemyQuestInfo>
    {
        public QuestHandler QuestHandler;
        public Huy Huy;
        [QuestEnemyName]
        public string EnemyName;


        public void InvokeTrigger()
        {
            Trigger(QuestHandler, new KillEnemyQuestInfo(EnemyName, 0));
        }

        public void Trigger(QuestHandler questHandler, KillEnemyQuestInfo questData)
        {
            foreach (var collectableQuest in questHandler.GetAllQuests(QuestStatus.InProgress).Cast<KillQuest>())
            {
                collectableQuest.IncreaseKilledEnemy(questData);
            }
        }
    }
}
