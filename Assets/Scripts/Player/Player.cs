using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Inventory inventory;
    [SerializeField] UI_Inventory uiInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            Item item = itemWorld.GetItem();
            inventory.AddItem(item);
            itemWorld.DestroySelf();
        }
        // Esto seguramente funcionará mas adelante
        // // This is called when the collider enters the trigger
        // if (other.CompareTag("Item"))
        // {
        // }
    }

    void Awake()
    {
        // This is called when the script instance is being loaded
        Debug.Log("Player Awake");
        inventory = new Inventory();
        uiInventory.Initialize(inventory);
    }

}

