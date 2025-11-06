using UnityEngine;

// Este script lida com a coleta imediata de itens em contêineres simples.
public class ContainerInteract : MonoBehaviour
{
    [Header("Configuração de Item")]
    public ItemData itemDentro; // O Scriptable Object do item que o player vai pegar
    
    // Mudamos de 'private' para 'protected' para que o 'SafeInteract' (subclasse)
    // possa acessar esta variável em seu próprio método Update().
    protected bool playerProximo; 

    void Update()
    {
        // Lógica de interação: Se o jogador está perto e pressionou 'W' (Interagir)
        if (playerProximo && Input.GetKeyDown(KeyCode.W))
        {
            // O cofre NÃO usará esta lógica; ele usará a do SafeInteract.
            // Esta lógica é para baús/caixas que dão o item imediatamente.

            InventoryManager inventario = FindObjectOfType<InventoryManager>();
            
            if (inventario != null && itemDentro != null)
            {
                inventario.AdicionarItem(itemDentro);
                Debug.Log("Item coletado: " + itemDentro.itemName);
                
                // O contêiner some após a coleta
                Destroy(gameObject); 
            }
            else if (itemDentro == null)
            {
                 Debug.LogWarning("ContainerInteract: ItemData não configurado no Inspector.");
            }
        }
    }

    // Gerenciamento da proximidade do jogador (usando Collider2D marcado como Is Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerProximo = true;
            // Opcional: Adicionar feedback visual aqui (ex: ícone de '!')
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerProximo = false;
            // Opcional: Remover feedback visual aqui
        }
    }
}