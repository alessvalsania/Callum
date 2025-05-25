using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private Transform itemWorldPrefab;
    public Transform GetItemWorldPrefab()
    {
        return itemWorldPrefab;
    }


    [SerializeField] private Sprite swordSprite;
    [SerializeField] private Sprite healthPotionSprite;
    [SerializeField] private Sprite manaPotionSprite;
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite medKitSprite;

    public Sprite GetItemSprite(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.Sword:
                return swordSprite;
            case Item.ItemType.HealthPotion:
                return healthPotionSprite;
            case Item.ItemType.ManaPotion:
                return manaPotionSprite;
            case Item.ItemType.Coin:
                return coinSprite;
            case Item.ItemType.MedKit:
                return medKitSprite;
            default:
                return null;
        }
    }
}
