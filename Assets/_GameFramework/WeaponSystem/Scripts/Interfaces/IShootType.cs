using GameFramework.Components;
using GameFramework.Strategy;

namespace GameFramework.WeaponSystem
{
    public interface IShootType : IStrategyContainer, IStrategyDrawGizmos
    {
        bool TryShoot(int damage, IAttackable attackable);
    }
}
