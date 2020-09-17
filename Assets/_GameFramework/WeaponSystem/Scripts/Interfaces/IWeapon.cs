namespace GameFramework.WeaponSystem
{
    public interface IWeapon
    {
        void Attack();
        bool CanAttack();
        int Damage { get; }
    }
}

