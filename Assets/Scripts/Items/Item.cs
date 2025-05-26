using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Sword,
        HealthPotion,
        ManaPotion,
        Coin,
        MedKit,
    }

    public ItemType itemType;
    public int amount;

    public Sprite GetSprite()
    {
        return ItemAssets.Instance.GetItemSprite(itemType);
    }

    public bool IsStackable()
    {
        // Define which items are stackable
        switch (itemType)
        {
            default:
            case ItemType.HealthPotion:
            case ItemType.ManaPotion:
            case ItemType.Coin:
                return true;
            case ItemType.Sword:
            case ItemType.MedKit:
                return false;
        }
    }
}
