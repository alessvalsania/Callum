using UnityEngine;

public class PickObject : MonoBehaviour
{
    private GameObject heldObject; // Objeto que el personaje está sosteniendo
    public Transform holdPoint;   // Punto donde se sostendrá el objeto
    public float pickUpRange = 1.5f; // Rango para agarrar objetos

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Tecla para agarrar/soltar
        {
            if (heldObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    void TryPickUpObject()
    {
        // Detecta objetos cercanos con un Collider2D
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Agarrable")) // Asegúrate de que el objeto tenga la etiqueta "Pickable"
            {
                heldObject = collider.gameObject;
                heldObject.GetComponent<Rigidbody2D>().isKinematic = true; // Desactiva la física
                heldObject.GetComponent<Collider2D>().enabled = false; // Desactiva el collider
                heldObject.transform.position = holdPoint.position; // Mueve el objeto al punto de agarre
                heldObject.transform.SetParent(holdPoint); // Lo hace hijo del punto de agarre
                break;
            }
        }
    }

    void DropObject()
    {
        // Reactiva la física del objeto
        heldObject.GetComponent<Rigidbody2D>().isKinematic = false;
        heldObject.GetComponent<Collider2D>().enabled = true;

        // Desvincula el objeto del personaje
        heldObject.transform.SetParent(null);

        // Ajusta la posición del objeto para evitar superposición
        Vector3 dropOffset = new Vector3(0, -0.5f, 0); // Desplaza ligeramente hacia abajo
        heldObject.transform.position += dropOffset;

        // Limpia la referencia al objeto sostenido
        heldObject = null;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el rango de agarre en la vista de escena
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }
}