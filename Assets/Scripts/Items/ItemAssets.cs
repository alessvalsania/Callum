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
    [SerializeField] private Sprite broomSprite;
    [SerializeField] private Sprite pickaxeSprite;
    [SerializeField] private Sprite mineralSprite;
    [SerializeField] private Sprite smallMineralSprite;



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
            case Item.ItemType.Broom:
                return broomSprite;
            case Item.ItemType.Pickaxe:
                return pickaxeSprite;
            case Item.ItemType.Mineral:
                return mineralSprite;
            case Item.ItemType.SmallMineral:
                return smallMineralSprite;
            default:
                return null;
        }
    }
}
