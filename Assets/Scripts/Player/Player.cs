using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // SOUND VARIABLES
    [Header("Sound Settings")] // Puedes usar un Header para organizar en el Inspector
    [SerializeField] private AudioSource audioSource; // Componente AudioSource en el jugador
    [SerializeField] private AudioSource sfxAudioSource; // Componente AudioSource en el jugador
    [SerializeField] private AudioClip walkSound;     // Sonido para caminar
    [SerializeField] private AudioClip sprintSound;   // Sonido para correr
    [SerializeField] private float walkSoundDelay = 0.3f; // Retraso entre sonidos al caminar
    [SerializeField] private float sprintSoundDelay = 0.2f; // Retraso entre sonidos al correr
    [Header("Interaction Sound Settings")]
    [SerializeField] private float soundProximityThreshold = 1f; // Distancia mínima para activar el sonido de un objeto
    [SerializeField] private AudioClip pickupSound; // Sonido al recoger objeto

    private float nextWalkSoundTime;
    private float nextSprintSoundTime;


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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component is not assigned to Player. Add an AudioSource component to the Player GameObject to enable footstep sounds.");
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
            // Reproducir sonido de recogida usando el AudioSource de efectos
            if (pickupSound != null && sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(pickupSound);
            }
            itemWorld.DestroySelf();
        }
        // Esto seguramente funcionará mas adelante
        // // This is called when the collider enters the trigger
        if (other.CompareTag("NextLevel"))
        {
            // Load the next level or scene
            Debug.Log("Next level trigger entered. Loading next level...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // --- NUEVA LÓGICA PARA REPRODUCIR SONIDO AL ENTRAR EN UN TRIGGER ESPECÍFICO ---

        // 1. Opcion: Usar un Tag para identificar el objeto que emite sonido
        if (other.CompareTag("soundObject")) // Asegúrate de que tu GameObject tenga este Tag
        {
            AudioSource objectAudioSource = other.GetComponent<AudioSource>();
            if (objectAudioSource != null && !objectAudioSource.isPlaying) // Solo si no está reproduciendo ya
            {
                objectAudioSource.Play();
                Debug.Log($"Sonido del objeto '{other.name}' iniciado.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // --- NUEVA LÓGICA PARA DETENER SONIDO AL SALIR DE UN TRIGGER ESPECÍFICO ---

        // Puedes detener el sonido si el jugador se aleja
        if (other.CompareTag("soundObject"))
        {
            AudioSource objectAudioSource = other.GetComponent<AudioSource>();
            if (objectAudioSource != null && objectAudioSource.isPlaying)
            {
                objectAudioSource.Stop();
                Debug.Log($"Sonido del objeto '{other.name}' detenido.");
            }
        }
        // --- FIN DE LA NUEVA LÓGICA ---
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
            isWalking = true;
            isSprinting = sprint;
            if (audioSource != null)
            {
                if (isSprinting && sprintSound != null)
                {
                    if (Time.time >= nextSprintSoundTime)
                    {
                        audioSource.PlayOneShot(sprintSound);
                        nextSprintSoundTime = Time.time + sprintSoundDelay;
                    }
                }
                else if (isWalking && walkSound != null)
                {
                    if (Time.time >= nextWalkSoundTime)
                    {
                        audioSource.PlayOneShot(walkSound);
                        nextWalkSoundTime = Time.time + walkSoundDelay;
                    }
                }
            }
        }
        else
        {
            isWalking = false;
            isSprinting = false;
            // Solo detener sonidos de pasos, no efectos
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
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

