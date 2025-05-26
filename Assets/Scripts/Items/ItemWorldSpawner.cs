using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        // This is called when the script instance is being loaded
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);
    }
}
