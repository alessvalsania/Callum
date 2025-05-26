using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList = new List<Item>();
    public int selectedItemIndex;

    private const int maxItemCount = 7;

    public event EventHandler OnItemListChanged;

    public Inventory()
    {
        itemList = new List<Item>();

        UpdateSelectedItem();
        Debug.Log("Inventory created with " + itemList.Count + " items.");
        Debug.Log("Selected Item: " + (selectedItemIndex != -1 ? itemList[selectedItemIndex].itemType.ToString() : "None"));
    }

    public void AddItem(Item item)
    {
        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item existingItem in itemList)
            {
                if (existingItem.itemType == item.itemType)
                {
                    existingItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                    break;
                }
            }
            if (!itemAlreadyInInventory)
            {
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);
        }

        UpdateSelectedItem();
        Debug.Log("Selected Item: " + (selectedItemIndex != -1 ? itemList[selectedItemIndex].itemType.ToString() : "None"));

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveItem(Item item)
    {
        if (itemList.Contains(item))
        {
            itemList.Remove(item);

            // Si el item removido era el seleccionado, actualizar selección
            if (selectedItemIndex != -1 && itemList[selectedItemIndex] == item)
            {
                UpdateSelectedItem();
            }

            Debug.Log("Item removed: " + item.itemType);
            Debug.Log("Selected Item: " + (selectedItemIndex != -1 ? itemList[selectedItemIndex].itemType.ToString() : "None"));

            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveItem(Item.ItemType itemType, int amount = 1)
    {
        for (int i = itemList.Count - 1; i >= 0; i--)
        {
            if (itemList[i].itemType == itemType)
            {
                itemList[i].amount -= amount;

                if (itemList[i].amount <= 0)
                {
                    Item removedItem = itemList[i];
                    itemList.RemoveAt(i);

                    // Si el item removido era el seleccionado, actualizar selección
                    if (selectedItemIndex != -1 && itemList[selectedItemIndex] == removedItem)
                    {
                        UpdateSelectedItem();
                    }
                }
                break;
            }
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateSelectedItem()
    {
        if (itemList.Count > 0 && selectedItemIndex == -1)
        {
            selectedItemIndex = 0;
        }
        else if (itemList.Count == 0)
        {
            selectedItemIndex = -1;
        }
        // Si el selectedItem ya no está en la lista, seleccionar el primero disponible
        else if (selectedItemIndex != -1 && !itemList.Contains(itemList[selectedItemIndex]))
        {
            selectedItemIndex = itemList.Count > 0 ? 0 : -1;
        }
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public int GetItemCount()
    {
        return itemList.Count;
    }

    public bool IsFull()
    {
        return itemList.Count >= maxItemCount;
    }

    public bool HasItem(Item.ItemType itemType)
    {
        foreach (Item item in itemList)
        {
            if (item.itemType == itemType)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItemAmount(Item.ItemType itemType)
    {
        foreach (Item item in itemList)
        {
            if (item.itemType == itemType)
            {
                return item.amount;
            }
        }
        return 0;
    }

    internal void SelectNextItem()
    {
        if (itemList.Count == 0) return;

        selectedItemIndex++;
        if (selectedItemIndex >= itemList.Count)
        {
            selectedItemIndex = 0;
        }

        Debug.Log("Selected Item Index: " + selectedItemIndex);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void SelectPreviousItem()
    {
        if (itemList.Count == 0) return;

        selectedItemIndex--;
        if (selectedItemIndex < 0)
        {
            selectedItemIndex = itemList.Count - 1;
        }

        Debug.Log("Selected Item Index: " + selectedItemIndex);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public Item GetSelectedItem()
    {
        if (selectedItemIndex >= 0 && selectedItemIndex < itemList.Count)
        {
            return itemList[selectedItemIndex];
        }
        return null;
    }
}