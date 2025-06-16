using System;
using UnityEngine;

public class Player : MonoBehaviour
{

    // INVENTORY VARIABLES
    private Inventory inventory;
    [SerializeField] UI_Inventory uiInventory;


    [SerializeField] private GameInput gameInput; // Agregar esta línea
    [SerializeField] public GameObject itemVisual;

    private bool isWalking = false; // Variable para controlar si el jugador está caminando
    private bool isSprinting = false; // Variable para controlar si el jugador está corriendo
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component for visual feedback
    public Animator animator;



    // MOVEMENT VARIABLES
    private Vector3 lastMoveDirection;
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del jugador
    [SerializeField] private float sprintMultiplier = 1.5f; // Sprint speed multiplier
    [SerializeField] private float movementResponsiveness = 10f; // smoothing factor for acceleration/deceleration
    private Vector2 smoothedVelocity = Vector2.zero;

    private IInteractable currentInteractable; // Reference to the interactable object in front of the player
    [SerializeField] private LayerMask interactableLayerMask; // Layer mask to filter interactable objects

    // Singleton instance for easy access to the Player object
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There are multiple instances of Player");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is not assigned in Player script. Please assign it in the inspector.");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is not assigned in Player script. Please assign it in the inspector.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new Inventory();
        uiInventory.Initialize(inventory);
        if (gameInput != null)
        {
            gameInput.OnInteractAlternateAction += OnInteractAction;
            gameInput.OnNextAction += OnNextAction;
            gameInput.OnPreviousAction += OnPreviousAction;
        }
        else
        {
            Debug.LogError("GameInput is not assigned in Player script. Please assign it in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleInteractWithWorld();
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isSprinting", isSprinting);

        // Debug.Log("Player position: " + transform.position);
        // Debug.Log("Last move direction: " + lastMoveDirection);
    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        Item selectedItem = inventory.GetSelectedItem();
        string itemInHand = selectedItem != null ? selectedItem.itemType.ToString() : "None";
        string objectTouching = currentInteractable != null ? currentInteractable.GetInteractText() : "None";

        Debug.Log($"Item en mano: {itemInHand} | Objeto tocando: {objectTouching}");

        // Interactuar con el objeto si existe
        if (currentInteractable != null)
        {
            currentInteractable.Interact(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            if (inventory.IsFull())
            {
                Debug.Log("Inventory is full");
                return;
            }
            Item item = itemWorld.GetItem();
            inventory.AddItem(item);
            animator.SetTrigger("interact");
            itemWorld.DestroySelf();
        }
        // Esto seguramente funcionará mas adelante
        // // This is called when the collider enters the trigger
        if (other.CompareTag("NextLevel"))
        {
            // Load the next level or scene
            Debug.Log("Next level trigger entered. Loading next level...");
            // Aquí puedes agregar la lógica para cargar el siguiente nivel
            // SceneManager.LoadScene("NextLevelName"); // Asegúrate de tener el using UnityEngine.SceneManagement;
        }
    }

    private void HandleMovement()
    {
        Vector2 moveInput = gameInput.GetMovementVectorNormalized();
        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float currentSpeed = moveSpeed * (sprint ? sprintMultiplier : 1f);
        Vector2 targetVelocity = moveInput * currentSpeed;

        // Smooth acceleration and deceleration
        smoothedVelocity = Vector2.Lerp(smoothedVelocity, targetVelocity, movementResponsiveness * Time.deltaTime);
        GetComponent<Rigidbody2D>().linearVelocity = smoothedVelocity;


        // Girar sprite según la dirección horizontal
        if (moveInput.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveInput.x < -0.01f)
            spriteRenderer.flipX = true;

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
            isWalking = true; // Set walking state to true when there is movement input
            isSprinting = sprint; // Set sprinting state based on input

        }
        else
        {
            isWalking = false; // Set walking state to false when there is no movement input
            isSprinting = false; // Set sprinting state to false when there is no movement input
        }
    }

    private void HandleInteractWithWorld()
    {
        Vector2 moveInput = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0f);
        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }
        Vector3 playerPosition = transform.position;
        float interactDistance = 1.5f;

        // Usar Physics2D.Raycast para 2D
        RaycastHit2D hit = Physics2D.Raycast(playerPosition, lastMoveDirection, interactDistance, interactableLayerMask);

        if (hit.collider != null)
        {
            // Verificar si el objeto tiene un componente IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // Nuevo objeto interactuable encontrado
                if (currentInteractable != interactable)
                {
                    SetCurrentInteractable(interactable);
                }
            }
            else
            {
                // El objeto golpeado no es interactuable
                SetCurrentInteractable(null);
            }
        }
        else
        {
            // No hay objetos en rango
            SetCurrentInteractable(null);
        }
    }

    private void SetCurrentInteractable(IInteractable newInteractable)
    {
        if (currentInteractable != newInteractable)
        {
            // Deseleccionar objeto anterior
            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerExit(this);
            }

            currentInteractable = newInteractable;

            // Seleccionar nuevo objeto
            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerEnter(this);
                Debug.Log("Objeto en rango: " + currentInteractable.GetInteractText());
            }
        }
    }
    public void SetHoldPointImageVisual(Sprite sprite)
    {
        if (itemVisual != null)
        {
            itemVisual.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }



    private void OnPreviousAction(object sender, EventArgs e)
    {
        inventory.SelectPreviousItem();
    }

    private void OnNextAction(object sender, EventArgs e)
    {
        inventory.SelectNextItem();
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public Item GetSelectedItem()
    {
        return inventory.GetSelectedItem();
    }

    public IInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }

}

