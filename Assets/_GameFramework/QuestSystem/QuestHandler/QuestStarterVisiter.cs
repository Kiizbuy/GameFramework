using GameFramework.Inventory.Items;
using GameFramework.Quest;
using System;
using UnityEngine;

public interface IQuestVisiter
{
    void Visit(KillQuest killQuest);
    void Visit(CollectItemsQuest collectItemsQuest);
}

public class QuestStarterVisiter : MonoBehaviour, IQuestVisiter
{
    public static event Action<IEnemy> OnEnemyHasDied;
    public static event Action<QuestItemData> OnQuestItemHasAdded;

    public void Visit(KillQuest killQuest)
    {
        OnEnemyHasDied += killQuest.IncreaseKilledEnemy;
    }

    public void Visit(CollectItemsQuest collectItemsQuest)
    {
        OnQuestItemHasAdded += collectItemsQuest.FetchCollectableItem;
    }
}
