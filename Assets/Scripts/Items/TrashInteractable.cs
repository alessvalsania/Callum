using UnityEngine;

public class TrashInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Borrar objeto";
    [SerializeField] private bool requiresSpecificItem = true;
    [SerializeField] private Item.ItemType requiredItemType;
    [SerializeField] private GameObject highlightEffect; // Opcional: efecto visual cuando está en rango

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
        else
        {
            // Interacción normal sin requisitos
            PerformNormalInteraction(player);
        }
    }

    private void PerformNormalInteraction(Player player)
    {
        Debug.Log($"Realizando interacción normal con {gameObject.name}");
        // Aquí puedes agregar la lógica específica del objeto
        // Por ejemplo: abrir una puerta, recoger un item, activar un mecanismo, etc.
    }

    private void PerformSpecialInteraction(Player player)
    {
        Destroy(gameObject); // Destruir el objeto interactuable después de la interacción
    }

    public string GetInteractText()
    {
        if (requiresSpecificItem)
        {
            return $"{interactText} (Requiere: {requiredItemType})";
        }
        return interactText;
    }

    public void OnPlayerEnter(Player player)
    {

        // Activar efecto visual si existe
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
        }

        // Cambiar color del sprite o agregar outline
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow; // Destacar el objeto
        }
    }

    public void OnPlayerExit(Player player)
    {

        // Desactivar efecto visual
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }

        // Restaurar color original
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    // Método para configurar el objeto desde el inspector o código
    public void SetRequiredItem(Item.ItemType itemType)
    {
        requiresSpecificItem = true;
        requiredItemType = itemType;
    }

    public void SetInteractText(string newText)
    {
        interactText = newText;
    }
}
