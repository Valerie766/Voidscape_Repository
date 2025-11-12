using UnityEngine;

// Este script deve ser anexado ao Collider filho que representa o dano/morte do inimigo.
[RequireComponent(typeof(Collider2D))]
public class EnemyDamageCollider : MonoBehaviour
{
    // Referência ao script pai para desabilitar o movimento após a morte.
    private EnemyPatrol enemyPatrol;

    void Start()
    {
        // Encontra o script EnemyPatrol no objeto pai (Enemy)
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        
        Collider2D col = GetComponent<Collider2D>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogError("EnemyDamageCollider: O Collider 2D deve estar presente e ser um TRIGGER!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Este é o ÚNICO lugar onde a morte é acionada
        if (other.CompareTag("Player"))
        {
            Debug.Log("COLISÃO DE DANO DETECTADA: Player atingido!");
            
            // 1. Encontra a instância persistente do Player
            GameObject playerToKill = PlayerMovement.Instance != null ? PlayerMovement.Instance.gameObject : other.gameObject;
            
            HandlePlayerDeath(playerToKill);
        }
    }

    void HandlePlayerDeath(GameObject player)
    {
        // 1. Garante que o inimigo pare
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false; // Desabilita o movimento do inimigo
            if (enemyPatrol.GetComponent<Rigidbody2D>() is Rigidbody2D rb)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        // 2. Chama o GameManager para COORDENAR A MORTE e o Respawn.
        if (GameManager.Instance != null)
        {
            if (FindObjectOfType<CutsceneManager>() is CutsceneManager cm)
            {
                cm.StartDeathCutscene(player); 
            }
            else
            {
                GameManager.Instance.StartDeathSequence(player);
            }
        }
        else
        {
            Debug.LogError("ERRO FATAL: GameManager não encontrado.");
        }
    }
}