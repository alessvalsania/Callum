using UnityEngine;

public class BigMineralInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Examinar objeto";
    [SerializeField] private bool requiresSpecificItem = false;
    [SerializeField] private Item.ItemType requiredItemType;
    [SerializeField] private GameObject highlightEffect; // Opcional: efecto visual cuando está en rango
    [SerializeField] private Item.ItemType spawningItemType; // Opcional: efecto visual cuando está en rango
    [SerializeField] private Transform spawnMineralPoint; // Indica si el objeto está activo para interactuar
    [SerializeField] private GameObject particleEffect; // Efecto de partículas al interactuar
    [SerializeField] private AudioClip breakRockSound; // Sonido al romper la piedra
    [SerializeField] private AudioSource audioSource; // AudioSource propio de la piedra

    private bool isBeingDestroyed = false;

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
        if (particleEffect != null)
        {
            Debug.Log($"Instanciando efecto de partículas en {gameObject.name}");
            Instantiate(particleEffect, transform.position, Quaternion.identity);
        }
        // Reproducir sonido de romper piedra
        if (breakRockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(breakRockSound);
        }
        isBeingDestroyed = true;
        // Ocultar el sprite y colisionador mientras suena el audio
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
        foreach (var col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
        ItemWorld.DropItem(spawnMineralPoint.position, new Item { itemType = spawningItemType, amount = 1 });

        Destroy(gameObject, breakRockSound != null ? breakRockSound.length : 0f); // Espera a que termine el sonido
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
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Debug.Log($"SpriteRenderers encontrados: {spriteRenderers.Length}");
        if (spriteRenderers.Length > 1)
        {
            // Cambiar el color del primer SpriteRenderer encontrado
            if (player.GetInventory().HasItem(requiredItemType))
            {
                spriteRenderers[1].color = Color.yellow; // Cambiar a un color de resaltado
            }
            else
            {
                spriteRenderers[1].color = Color.red; // Cambiar a un color de advertencia
            }
            if (player.GetSelectedItem().itemType == requiredItemType)
            {
                spriteRenderers[1].color = Color.green; // Cambiar a un color de éxito
            }
        }
    }

    public void OnPlayerExit(Player player)
    {
        if (isBeingDestroyed) return;

        // Desactivar efecto visual
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }

        // Restaurar color original
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length > 1)
        {
            // Restaurar el color del primer SpriteRenderer encontrado
            spriteRenderers[1].color = Color.white; // Restaurar al color original
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

