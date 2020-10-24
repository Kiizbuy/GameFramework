using GameFramework.Components;
using GameFramework.Strategy;

namespace GameFramework.WeaponSystem
{
    public interface IShootType : IStrategyContainer
    {
        void TryTakeDamageOnTarget(int Damage, IAttackable attackable);
    }
}
