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
        // 🚨 CORREÇÃO: Usar a referência estática (Singleton) em vez de FindObjectOfType
        InventoryManager inventory = InventoryManager.Instance;
        
        if (inventory == null)
        {
            // O InventoryManager persiste, então essa mensagem agora indica que ele não foi inicializado corretamente
            Debug.LogError("InventoryManager não encontrado na cena. Verifique se ele inicializou o Singleton (Awake()).");
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
            // Nota: Adicione uma verificação de Nulo para requiredItem aqui
            string requiredName = requiredItem != null ? requiredItem.itemName : "ITEM REQUERIDO (NULL)";
            Debug.Log($"Interação Falha: Item '{requiredName}' ausente. Jogador não possui o item necessário.");
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
    }
}