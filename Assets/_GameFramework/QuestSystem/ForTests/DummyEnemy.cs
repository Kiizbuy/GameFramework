using UnityEngine;

namespace GameFramework.Quest
{
    public class DummyEnemy : MonoBehaviour, IEnemy
    {
        public void DeadEnemy()
        {
            EnemyHasDied = true;
            QuestStarterVisiter.DeadEnemy(this);
        }

        public string EnemyName => "Dummy";
        public int EnemyExperienceReward => 1;
        public int EnemyLevel => 1;
        public bool EnemyHasDied { get; private set; }
    }
}
