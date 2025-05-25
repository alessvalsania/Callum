using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList = new List<Item>();

    public event EventHandler OnItemListChanged;

    public Inventory()
    {
        itemList = new List<Item>();
        // Initialize the inventory with some items
        AddItem(new Item { itemType = Item.ItemType.Sword, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.Coin, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.HealthPotion, amount = 1 });
        AddItem(new Item { itemType = Item.ItemType.ManaPotion, amount = 1 });
        Debug.Log("Inventory created with " + itemList.Count + " items.");
    }

    public void AddItem(Item item)
    {
        if (item.IsStackable())
        {
            bool itemAlreadyInInvetory = false;
            foreach (Item existingItem in itemList)
            {
                if (existingItem.itemType == item.itemType)
                {
                    existingItem.amount += item.amount;
                    itemAlreadyInInvetory = true;
                }
            }
            if (!itemAlreadyInInvetory)
            {
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
}
