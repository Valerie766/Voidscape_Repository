using UnityEngine;

// Requer um Collider2D no objeto para detectar a proximidade
[RequireComponent(typeof(Collider2D))] 
public class ItemExchangeInteraction : MonoBehaviour
{
    [Header("Configuração de Interação")]
    public KeyCode interactionKey = KeyCode.W; 
    
    [Header("Requisitos do Puzzle")]
    [Tooltip("O ItemData que o jogador DEVE possuir para interagir (Será checado, não consumido).")]
    public ItemData requiredItem;
    
    [Header("Recompensa")]
    [Tooltip("O ItemData que o jogador recebe após a interação bem-sucedida.")]
    public ItemData rewardItem;

    private bool playerIsClose = false;
    private bool puzzleCompleted = false;

    void Start()
    {
        puzzleCompleted = false;
        
        // Garante que o Collider2D seja um Trigger
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.isTrigger = true;
        }
    }

    void Update()
    {
        // Se o player está perto, o puzzle não foi completado, e o input foi detectado.
        if (playerIsClose && !puzzleCompleted && Input.GetKeyDown(interactionKey))
        {
            Debug.Log($"[INPUT DETECTADO]: Tecla {interactionKey} OK. Verificando requisitos...");
            TryInteraction();
        }
    }

    // **********************************
    // LÓGICA DE PROXIMIDADE (2D)
    // **********************************

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log de Colisão para diagnóstico (Verifica se algum objeto está entrando)
        Debug.Log($"[COLISÃO CHECK]: Objeto {other.gameObject.name} (Tag: {other.tag}) ENTROU.");

        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            Debug.Log("[COLISÃO SUCESSO]: Player ENTROU na zona de interação. playerIsClose = TRUE.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            Debug.Log("[COLISÃO SUCESSO]: Player SAIU da zona de interação. playerIsClose = FALSE.");
        }
    }
    
    // **********************************
    // LÓGICA DE INTERAÇÃO
    // **********************************

    private void TryInteraction()
    {
        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        
        if (inventory == null)
        {
            Debug.LogError("InventoryManager não encontrado na cena.");
            return;
        }

        // Caso 1: Jogador tem o item obrigatório
        if (inventory.HasItem(requiredItem))
        {
            CompletePuzzle(inventory);
        }
        // Caso 2: Jogador não tem o item (Apenas loga)
        else
        {
            Debug.Log($"Interação Falha: Item '{requiredItem.itemName}' ausente. Jogador não possui o item necessário.");
        }
    }

    private void CompletePuzzle(InventoryManager inventory)
    {
        // 1. Dá o item de recompensa
        if (rewardItem != null)
        {
            inventory.AdicionarItem(rewardItem);
            Debug.Log($"SUCESSO: Item '{requiredItem.itemName}' checado. Recompensa '{rewardItem.itemName.ToUpper()}' recebida.");
        }
        
        // 2. Marca como completo e desativa o script para prevenir interações futuras
        puzzleCompleted = true;
        this.enabled = false; 
        
        // Opcional: Se o objeto deve sumir após a interação, descomente a linha abaixo:
        // Destroy(gameObject);
    }
}