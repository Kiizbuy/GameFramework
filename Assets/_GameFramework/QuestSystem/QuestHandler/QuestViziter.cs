
//public interface IQuestViziter
//{
//    void VisitToSubscribeEvents(KillQuest killQuest);
//    void VisitToSubscribeEvents(CollectItemsQuest collectItemsQuest);
//    void VisitToSubscribeEvents(ReachTargetQuest reachTargetQuest);

//    void VisitToUnsubscribeEvents(KillQuest killQuest);
//    void VisitToUnsubscribeEvents(CollectItemsQuest collectItemsQuest);
//    void VisitToUnsubscribeEvents(ReachTargetQuest reachTargetQuest);
//}

//public class QuestViziter : MonoBehaviour, IQuestViziter
//{
//    public static event Action<IQuestEnemy> OnEnemyHasDied;
//    public static event Action<QuestItemData> OnQuestItemHasAdded;

//    public static void DeadEnemy(IQuestEnemy questEnemy) => OnEnemyHasDied?.Invoke(questEnemy);

//    public void VisitToSubscribeEvents(KillQuest killQuest)
//    {
//        OnEnemyHasDied += killQuest.IncreaseKilledEnemy;
//    }

//    public void VisitToSubscribeEvents(CollectItemsQuest collectItemsQuest)
//    {
//        OnQuestItemHasAdded += collectItemsQuest.FetchCollectableItem;
//    }

//    public void VisitToSubscribeEvents(ReachTargetQuest reachTargetQuest)
//    {
//    }

//    public void VisitToUnsubscribeEvents(KillQuest killQuest)
//    {
//        OnEnemyHasDied -= killQuest.IncreaseKilledEnemy;
//    }

//    public void VisitToUnsubscribeEvents(CollectItemsQuest collectItemsQuest)
//    {
//        OnQuestItemHasAdded -= collectItemsQuest.FetchCollectableItem;
//    }

//    public void VisitToUnsubscribeEvents(ReachTargetQuest reachTargetQuest)
//    {
//    }
//}
