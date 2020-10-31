namespace GameFramework.Quest
{
    public interface IEnemy
    {
        string EnemyName { get; }
        bool EnemyHasDied { get; }
        int EnemyExperienceReward { get; }
        int EnemyLevel { get; }
    }
}
