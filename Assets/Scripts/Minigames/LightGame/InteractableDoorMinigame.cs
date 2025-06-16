using UnityEngine;

public class DoorMinigame : MonoBehaviour, IInteractable
{

    [SerializeField] private string interactText = "Examinar objeto";
    [SerializeField] private bool requiresSpecificItem = true;
    [SerializeField] private Item.ItemType requiredItemType;


    public string GetInteractText()
    {
        if (requiresSpecificItem)
        {
            return $"{interactText} (Requiere: {requiredItemType})";
        }
        return interactText;
    }

    public void Interact(Player player)
    {
        Item playerItem = player.GetSelectedItem();

        if (requiresSpecificItem)
        {
            if (playerItem != null && playerItem.itemType == requiredItemType)
            {
                PerformSpecialInteraction(player);
            }
            else
            {
                Debug.Log($"Necesitas {requiredItemType} para interactuar con {gameObject.name}");
            }
        }
    }

    public void PerformSpecialInteraction(Player player)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false); // Desactivar el objeto interactuable
    }

    public void OnPlayerEnter(Player player)
    {
        // Cambiar color del sprite o agregar outline
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Cambiar el color del primer SpriteRenderer encontrado
            if (player.GetInventory().HasItem(requiredItemType))
            {
                spriteRenderer.color = Color.yellow; // Cambiar a un color de resaltado
            }
            else
            {
                spriteRenderer.color = Color.red; // Cambiar a un color de advertencia
            }
            if (player.GetSelectedItem().itemType == requiredItemType)
            {
                spriteRenderer.color = Color.green; // Cambiar a un color de éxito
            }
        }
    }

    public void OnPlayerExit(Player player)
    {
        // Restaurar color original
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Restaurar el color del primer SpriteRenderer encontrado
            spriteRenderer.color = Color.white; // Restaurar al color original
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
