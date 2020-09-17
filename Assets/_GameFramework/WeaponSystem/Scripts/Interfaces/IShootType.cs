using GameFramework.Components;
using GameFramework.Strategy;

namespace GameFramework.WeaponSystem
{
    public interface IShootType : IStrategyContainer
    {
        bool HitInTarget(out IHealth healthTarget);
    }
}
