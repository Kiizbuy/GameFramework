namespace GameFramework.Quest
{
    public interface IQuestEnemy
    {
        string EnemyName { get; }
        bool EnemyHasDied { get; }
        int EnemyExperienceReward { get; }
        int EnemyLevel { get; }
    }
}
