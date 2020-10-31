using GameFramework.Inventory.Items;
using GameFramework.Quest;
using System;
using UnityEngine;

public interface IQuestVisiter
{
    void Visit(KillQuest killQuest);
    void Visit(CollectItemsQuest collectItemsQuest);
    void Visit(ReachTargetQuest reachTargetQuest);
}

public class QuestStarterVisiter : MonoBehaviour, IQuestVisiter
{
    public static event Action<IEnemy> OnEnemyHasDied;
    public static event Action<QuestItemData> OnQuestItemHasAdded;

    public static void DeadEnemy(IEnemy enemy) => OnEnemyHasDied?.Invoke(enemy);

    public void Visit(KillQuest killQuest)
    {
        OnEnemyHasDied += killQuest.IncreaseKilledEnemy;
    }

    public void Visit(CollectItemsQuest collectItemsQuest)
    {
        OnQuestItemHasAdded += collectItemsQuest.FetchCollectableItem;
    }

    public void Visit(ReachTargetQuest reachTargetQuest)
    {
    }
}
