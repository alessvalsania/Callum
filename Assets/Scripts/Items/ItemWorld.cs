using TMPro;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Debug.Log(ItemAssets.Instance);
        Transform itemTransform = Instantiate(ItemAssets.Instance.GetItemWorldPrefab(), position, Quaternion.identity);
        ItemWorld itemWorld = itemTransform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        return itemWorld;
    }

    private Item item;
    private SpriteRenderer spriteRenderer;
    private TextMeshProUGUI amountText;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        amountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
        amountText.SetText(item.amount > 1 ? item.amount.ToString() : "");
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
