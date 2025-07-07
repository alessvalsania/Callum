using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;
    public float chaseRange = 5f;
    public float stopChaseRange = 7f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float dashSpeed = 8f;
    public float dashDuration = 0.2f;
    public float pauseBeforeDash = 0.3f;
    public float attackCooldown = 1.5f;

    [Header("Dash Visuals")]
    public TrailRenderer trailRenderer;
    public ParticleSystem dashParticles;
    public Vector3 dashScale = new Vector3(1.3f, 0.7f, 1f);

    [Header("Death Visuals")]
    [SerializeField] private GameObject deathParticlesPrefab;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private float directionTimer;
    private Transform playerTransform;
    private bool isChasing = false;

    // Attack state
    private bool isAttacking = false;
    private bool isAttackOnCooldown = false;
    private float attackCooldownTimer = 0f;
    private float dashTimer = 0f;
    private float pauseTimer = 0f;
    private Vector2 dashDirection;
    private Vector3 originalScale;
    private bool hasDealtDamageThisDash = false;

    private Animator animator;
    private bool isDying = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        directionTimer = changeDirectionTime;
        originalScale = transform.localScale;
        if (trailRenderer != null)
            trailRenderer.enabled = false;
    }

    void Start()
    {
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró el jugador (Player.Instance) en la escena.");
        }
        PickRandomDirection();
    }

    void Update()
    {
        if (isDying) return;
        if (isAttackOnCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                isAttackOnCooldown = false;
            }
        }

        if (isAttacking)
        {
            if (pauseTimer > 0f)
            {
                pauseTimer -= Time.deltaTime;
                if (pauseTimer <= 0f)
                {
                    dashTimer = dashDuration;
                    if (playerTransform != null)
                        dashDirection = (playerTransform.position - transform.position).normalized;
                    else
                        dashDirection = movementDirection;
                    if (trailRenderer != null)
                        trailRenderer.enabled = true;
                    transform.localScale = dashScale;
                    if (dashParticles != null)
                        dashParticles.Play();
                    hasDealtDamageThisDash = false;
                }
            }
            else if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                {
                    isAttacking = false;
                    isAttackOnCooldown = true;
                    attackCooldownTimer = attackCooldown;
                    if (trailRenderer != null)
                        trailRenderer.enabled = false;
                    transform.localScale = originalScale;
                }
            }
            return;
        }

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (!isChasing && distanceToPlayer <= chaseRange)
            {
                isChasing = true;
            }
            else if (isChasing && distanceToPlayer > stopChaseRange)
            {
                isChasing = false;
                PickRandomDirection();
                directionTimer = changeDirectionTime;
            }
            if (isChasing && !isAttacking && !isAttackOnCooldown && distanceToPlayer <= attackRange)
            {
                isAttacking = true;
                pauseTimer = pauseBeforeDash;
                dashTimer = 0f;
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        if (!isChasing)
        {
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0f)
            {
                PickRandomDirection();
                directionTimer = changeDirectionTime;
            }
        }

        // Control de animación de persecución
        if (animator != null)
        {
            animator.SetBool("IsChasing", isChasing);
        }
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            if (pauseTimer > 0f)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else if (dashTimer > 0f)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        if (isChasing && playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = movementDirection * moveSpeed;
        }
    }

    void PickRandomDirection()
    {
        movementDirection = Random.insideUnitCircle.normalized;
    }

    public void Die()
    {
        if (isDying) return;
        isDying = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
        if (animator != null)
        {
            animator.SetBool("IsDying", true);
        }
        // Desactivar colisiones y lógica
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        this.enabled = false; // Desactiva el script para evitar lógica extra
    }

    // Llamado por un Animation Event al final de la animación de muerte
    public void OnDeathAnimationEnd()
    {
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && !hasDealtDamageThisDash && collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1);
                hasDealtDamageThisDash = true;
                Debug.Log($"Slime hizo daño al jugador en dash. hasDealtDamageThisDash ahora es {hasDealtDamageThisDash}");
            }
        }
        else if (isAttacking && hasDealtDamageThisDash && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Slime ya hizo daño este dash, no vuelve a hacer daño hasta el próximo dash.");
        }
    }
}
