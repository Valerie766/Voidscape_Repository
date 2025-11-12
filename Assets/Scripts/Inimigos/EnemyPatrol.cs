using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform[] patrolPoints;	
    public float speed = 2f;	 	 	
    private int currentPoint = 0; // Índice do PONTO ALVO ATUAL

    [Header("Perseguição")]
    public float chaseSpeed = 3.5f;
    public float visionRange = 5f;	 	
    public LayerMask playerLayer;	 	

    [Header("Áudio")]
    public AudioClip chaseAlertSound;	
    private AudioSource audioSource;

    // Componentes e Estados
    private Rigidbody2D rb;	
    private Transform player;
    private bool isChasing = false;
    private int hiddenLayer;	
    private Vector3 initialScale;


    void Start()
    {
        // 1. Inicializa componentes
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
        
        // 2. CORREÇÃO DE INICIALIZAÇÃO DA PATRULHA
        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
            
            if (patrolPoints.Length > 1)
            {
                currentPoint = 1;	
            }
            else
            {
                currentPoint = 0;	
            }
        }
        
        // Tenta encontrar o player persistente
        if (PlayerMovement.Instance != null)
        {
            player = PlayerMovement.Instance.transform;
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        hiddenLayer = LayerMask.NameToLayer("Hidden");
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (chaseAlertSound != null)
        {
            audioSource.clip = chaseAlertSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Checagem se o componente está habilitado
        if (player == null || rb == null || !enabled) return; 

        bool playerHidden = (player.gameObject.layer == hiddenLayer);
        bool playerVisibleAndNear = !playerHidden && Vector2.Distance(transform.position, player.position) < visionRange;

        if (isChasing)
        {
            ChasePlayer(playerHidden);
        }
        else
        {
            Patrol();

            if (playerVisibleAndNear)
            {
                isChasing = true;
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length <= 1)
        {
            rb.linearVelocity = Vector2.zero;	
            return;	
        }

        Transform targetPoint = patrolPoints[currentPoint];
        Vector3 targetPosition = targetPoint.position;
        
        Vector2 moveDirection = (targetPosition - transform.position).normalized;
        rb.linearVelocity = moveDirection * speed;
        
        FlipToTarget(targetPosition);	
        
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        
        if (distanceToTarget < 0.05f)	
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer(bool playerHidden)
    {
        if (playerHidden || Vector2.Distance(transform.position, player.position) > visionRange * 3)
        {
            isChasing = false;
            rb.linearVelocity = Vector2.zero; 
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return;
        }

        Vector2 moveDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = moveDirection * chaseSpeed;
        
        FlipToTarget(player.position);	
    }

    void FlipToTarget(Vector3 targetPosition)
    {
        float direction = targetPosition.x - transform.position.x;
        
        if (Mathf.Abs(direction) > 0.01f)
        {
            if (direction > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
            }
            else if (direction < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
            }
        }
    }
    
    // ======================================================
    // MÓDULO DE RESET PÓS-RESPAWN
    // ======================================================
    // Este método é chamado pelo GameManager após um Respawn.
    public void ResetEnemy()
    {
        // Reativa o componente para que o Update volte a funcionar
        this.enabled = true; 
        
        isChasing = false;
        
        if (rb != null) rb.linearVelocity = Vector2.zero;	
        
        if (patrolPoints.Length > 1)
        {
            currentPoint = 1;
        }
        else
        {
            currentPoint = 0;
        }
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}