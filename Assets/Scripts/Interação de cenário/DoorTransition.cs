using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    // O ponto exato para onde o jogador deve ir na próxima sala
    public Transform destinationPoint;

    // Uma variável para saber se o jogador está perto e pode interagir
    private bool playerIsNear = false;
    private GameObject playerObject;
    
    // Variável para verificar se o destino foi ligado
    private bool destinationLinked = false;

    void Awake()
    {
        // Garante que a porta seja um Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Start()
    {
        // Verifica se o ponto de destino foi ligado no Inspector
        if (destinationPoint != null)
        {
            destinationLinked = true;
        }
        else
        {
            Debug.LogError("ERRO: O campo 'Destination Point' não foi ligado no Inspector da porta: " + gameObject.name);
        }
    }

    void Update()
    {
        // Verifica a interação apenas se o jogador estiver perto e pressionar W
        if (playerIsNear && Input.GetKeyDown(KeyCode.W) && destinationLinked)
        {
             TeleportPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou é o Jogador (usando a Tag "Player")
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            playerObject = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            playerObject = null;
        }
    }

    void TeleportPlayer()
    {
        if (playerObject != null && destinationPoint != null)
        {
            // 1. Move o jogador para a nova posição
            playerObject.transform.position = destinationPoint.position;
            
            // 2. ESSENCIAL: Zera a velocidade do Rigidbody para evitar slides
            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // 3. INFORMA A CÂMERA PARA SE AJUSTAR IMEDIATAMENTE (SnapToTarget)
            // Isso resolve o problema de atraso visual após o teletransporte.
            if (CameraController.Instance != null)
            {
                CameraController.Instance.SnapToTarget();
            }
        }
    }
}