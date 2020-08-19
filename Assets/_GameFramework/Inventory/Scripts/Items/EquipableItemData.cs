namespace GameFramework.Inventory.Items
{
    public enum EquipmentType
    {
        Head,
        Arms,
        Torso,
        Legs,
        Bag,
        Weapon
    }

    public class EquipableItemData : BaseItemData
    {
        public EquipmentType EquipmentType;
    }
}

