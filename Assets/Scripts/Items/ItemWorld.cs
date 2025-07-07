using TMPro;
using UnityEngine;
using CodeMonkey.Utils;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        Transform itemTransform = Instantiate(ItemAssets.Instance.GetItemWorldPrefab(), position, Quaternion.identity);
        ItemWorld itemWorld = itemTransform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        if (item.itemType == Item.ItemType.Mineral)
        {
            itemTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Adjust scale for mineral
            itemTransform.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 3";
        }
        if (item.itemType == Item.ItemType.Sword)
        {
            itemTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Adjust scale for mineral
            itemTransform.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 3";
        }
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

    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomDir = UtilsClass.GetRandomDir();
        float randomSpeed = Random.Range(2f, 4f);
        ItemWorld itemWorld = SpawnItemWorld(dropPosition, item);
        // itemWorld.GetComponent<Rigidbody2D>().AddForce(randomDir * randomSpeed, ForceMode2D.Impulse);
        return itemWorld;
    }
}
