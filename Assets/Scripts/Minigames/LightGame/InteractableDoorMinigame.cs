using UnityEngine;

public class DoorMinigame : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Examinar objeto";
    [SerializeField] private bool requiresSpecificItem = true;
    [SerializeField] private Item.ItemType requiredItemType;

    // AÑADE ESTA LÍNEA: Referencia al GameObject de tu Canvas o Panel
    // Asegúrate de arrastrar el Canvas (o el Panel dentro) a esta variable en el Inspector de Unity.
    [SerializeField] private GameObject minigameCanvas; // O un Panel, si es lo que quieres mostrar/ocultar


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
        else // Si no requiere un ítem específico, simplemente realiza la interacción especial
        {
            PerformSpecialInteraction(player);
        }
    }

    public void PerformSpecialInteraction(Player player)
    {
        // Aquí es donde mostramos el Canvas/Panel del minijuego
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true); // Activa el GameObject, haciéndolo visible
            Debug.Log("Canvas del minijuego mostrado.");
            
        }
        else
        {
            Debug.LogError("minigameCanvas no está asignado en el Inspector de " + gameObject.name);
        }

        // Originalmente tenías esto para desactivar el objeto interactuable (la puerta).
        // Decide si quieres que la puerta se desactive inmediatamente o después del minijuego.
        // gameObject.SetActive(false); // Desactivar el objeto interactuable (la puerta)
    }

    public void OnPlayerEnter(Player player)
    {
        // Cambiar color del sprite o agregar outline
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Cambiar el color del primer SpriteRenderer encontrado
            // Verifica si el jugador tiene el ítem, no solo el seleccionado
            if (player.GetInventory() != null && player.GetInventory().HasItem(requiredItemType))
            {
                spriteRenderer.color = Color.yellow; // Cambiar a un color de resaltado si tiene el ítem
            }
            else
            {
                spriteRenderer.color = Color.red; // Cambiar a un color de advertencia si no tiene el ítem
            }

            // Si el ítem seleccionado es el requerido
            if (player.GetSelectedItem() != null && player.GetSelectedItem().itemType == requiredItemType)
            {
                spriteRenderer.color = Color.green; // Cambiar a un color de éxito si tiene el ítem seleccionado
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
        // Opcional: Asegúrate de que el Canvas del minijuego esté oculto al inicio
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Si necesitas una forma de cerrar el minijuego, podrías añadir algo aquí,
        // por ejemplo, si el minigameCanvas está activo y se presiona una tecla.
        // Sin embargo, lo más común es que el propio script del minijuego lo cierre.
    }
}
