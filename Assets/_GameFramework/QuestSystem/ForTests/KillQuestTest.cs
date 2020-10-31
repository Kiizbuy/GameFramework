using GameFramework.Quest;
using UnityEngine;

public class KillQuestTest : MonoBehaviour
{
    public DummyEnemy DummyEnemy;
    public QuestHandler QuestHandler;

    private void Start()
    {
        QuestHandler.OnQuestStarted += (quest) => Debug.Log($"{quest.QuestName} started");
        QuestHandler.OnQuestAdded += (quest) => Debug.Log($"Added new quest {quest.QuestName}");
        QuestHandler.OnQuestStatusHasChanged += (quest, status) => Debug.Log($"quest {quest.QuestName} changed status - {quest.CurrentQuestStatus}");
        QuestHandler.OnQuestComplete += (quest) => Debug.Log($"Quest {quest.QuestName} has been completed");

        var killQuest = new KillQuest("Kill Dummy enemy", 2, DummyEnemy);
        killQuest.AddExperienceReward(20);
        QuestHandler.TryAddQuest(killQuest);
        QuestHandler.StartQuest(killQuest);
    }
}
