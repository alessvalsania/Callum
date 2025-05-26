using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;

    Player player;

    private void Awake()
    {
        // itemSlotContainer = transform.Find("itemSlotContainer");
        // itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }

    public void Initialize(Inventory inventory)
    {
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        UpdateInventoryUI();
        player = Player.Instance;
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // Debug.Log("Updating inventory UI with " + inventory.GetItemList().Count + " items.");
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue; // Skip the template
            Destroy(child.gameObject); // Remove existing item slots
        }
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 95f;

        int index = 0;
        foreach (Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            Image image = itemSlotRectTransform.Find("image").GetComponent<Image>();
            image.sprite = item.GetSprite();
            TextMeshProUGUI uiText = itemSlotRectTransform.Find("amountText").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
            {
                uiText.text = item.amount.ToString();
            }
            else
            {
                uiText.text = "";
            }

            if (index == inventory.selectedItemIndex)
            {
                itemSlotRectTransform.Find("backgroundSelected").gameObject.SetActive(true);
                itemSlotRectTransform.Find("background").gameObject.SetActive(false);
                player.SetHoldPointImageVisual(image.sprite);
            }
            index++;
            x++;

        }
    }

}
