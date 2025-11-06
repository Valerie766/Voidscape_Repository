using UnityEngine;
using System.Collections; 

public class PausedFlyingPatrol : MonoBehaviour
{
    // ==========================================================
    // CONFIGURAÇÃO PÚBLICA
    // ==========================================================
    
    [Header("Patrulha Voadora")]
    public Transform[] patrolPoints; 
    [Tooltip("Velocidade de movimento do inimigo entre os pontos.")]
    public float speed = 2f; 
    [Tooltip("Tempo (em segundos) que o inimigo espera em cada ponto de patrulha (Idle).")]
    public float patrolWaitTime = 3f; 
    public float arrivalTolerance = 0.05f; // Tolerância para chegada no ponto

    [Header("Ataque Condicional")]
    [Tooltip("O Collider que DEVE ser ativado SOMENTE durante a movimentação/ataque.")]
    public Collider2D attackCollider; 

    [Header("Componentes de Animação")]
    public Animator anim;
    public string idleBoolName = "IsIdle";

    // ==========================================================
    // VARIÁVEIS DE ESTADO
    // ==========================================================

    private Rigidbody2D rb;
    private int currentPoint = 0; 
    private Vector3 initialScale;

    // --- Referências de Sistema (Adicionadas para o HandlePlayerDeath) ---
    // Você não precisa ligar estas, pois a função HandlePlayerDeath já as espera:
    // private int hiddenLayer; 
    // private AudioSource audioSource;
    // ...

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;

        if (anim == null) anim = GetComponent<Animator>();
        if (attackCollider != null) attackCollider.enabled = false;
        
        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
            currentPoint = (patrolPoints.Length > 1) ? 1 : 0; 
            StartCoroutine(PatrolRoutine()); 
        }
    }

    // ======================================================
    // LÓGICA DE PATRULHA COM PAUSA
    // ======================================================

    IEnumerator PatrolRoutine()
    {
        while (patrolPoints.Length > 1) 
        {
            Transform targetPoint = patrolPoints[currentPoint];
            Vector3 targetPosition = targetPoint.position;
            
            FlipToTarget(targetPosition);

            // ANIMAÇÃO: Começa a se mover (IsIdle = false)
            if (anim != null)
            {
                anim.SetBool(idleBoolName, false);
            }

            // --- FASE DE MOVIMENTO ---
            while (Vector2.Distance(transform.position, targetPosition) > arrivalTolerance)
            {
                // ATIVANDO ATAQUE: Somente aqui ele pode matar o player
                if (attackCollider != null) attackCollider.enabled = true;
                
                // Movimentação via MoveTowards
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                yield return null; 
            }
            
            // Garante Parada e Posição Exata
            if (rb != null) rb.linearVelocity = Vector2.zero;
            transform.position = targetPosition; 

            // --- FASE DE PAUSA ---
            
            // ANIMAÇÃO: Entra no estado Parado (IsIdle = true)
            if (anim != null)
            {
                anim.SetBool(idleBoolName, true);
            }

            // DESATIVANDO ATAQUE: Ele é inofensivo
            if (attackCollider != null) attackCollider.enabled = false; 

            yield return new WaitForSeconds(patrolWaitTime);

            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }
    
    // ======================================================
    // MÓDULO DE FLIP (REUTILIZADO)
    // ======================================================
    
    void FlipToTarget(Vector3 targetPosition)
    {
        float direction = targetPosition.x - transform.position.x;
        
        if (direction != 0)
        {
            if (Mathf.Sign(direction) > 0 && transform.localScale.x < 0)
            {
                transform.localScale = initialScale; 
            }
            else if (Mathf.Sign(direction) < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z); 
            }
        }
    }

    // ======================================================
    // 💡 CORREÇÃO CRÍTICA: LÓGICA DE DANO/MORTE CONDICIONAL
    // ======================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // 🚨 O Player só morre se o Collider de ataque estiver ATIVO e o objeto for o Player.
        if (attackCollider != null && attackCollider.enabled && other.CompareTag("Player"))
        {
             HandlePlayerDeath(other.gameObject);
        }
        // Se o Collider estiver desativado (na pausa), a colisão é ignorada!
    }

    // 💡 MÓDULO DE MORTE (Adotado do seu script 'EnemyPatrol')
    void HandlePlayerDeath(GameObject player)
    {
        // 1. Congela o movimento do Player
        if (player.GetComponent<PlayerMovement>() != null)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
        }
        
        // 2. Garante que o inimigo pare o movimento
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        // 3. Desativa o inimigo (Se isso estiver causando o "desaparecimento", é aqui)
        gameObject.SetActive(false); 
        
        // 4. Chama o GameManager/CutsceneManager para lidar com o respawn
        // NOTA: Para usar CutsceneManager.Instance, ele deve ser um Singleton global.
        
        // Tente usar o GameManager como fallback, que é mais comum para respawn
        if (GameManager.Instance != null)
        {
             GameManager.Instance.RespawnPlayer(player);
        }
        else
        {
             Debug.LogError("Gerenciador de Morte/Respawn (GameManager.Instance) não encontrado! Player está travado.");
        }
        
        // Se você precisa do CutsceneManager:
        // if (CutsceneManager.Instance != null)
        // {
        //     CutsceneManager.Instance.StartDeathCutscene(player);
        // }
    }

    // ======================================================
    // MÓDULO DE RESET PÓS-RESPAWN
    // ======================================================
    public void ResetEnemy()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (attackCollider != null) attackCollider.enabled = false;
        
        currentPoint = (patrolPoints.Length > 1) ? 1 : 0;
        
        StopAllCoroutines();
        StartCoroutine(PatrolRoutine());
        
        // Certifica-se de que o inimigo volte a aparecer após o respawn
        gameObject.SetActive(true);
    }
}