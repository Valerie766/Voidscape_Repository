using UnityEngine;
using TMPro; // Mantido, embora não estritamente necessário neste script

// Requer um Collider2D no objeto para detectar a proximidade
[RequireComponent(typeof(Collider2D))] 
public class ComputerInteract : MonoBehaviour
{
    [Header("Configuração de Interação")]
    [Tooltip("A tecla que o jogador deve pressionar para iniciar a interação.")]
    public KeyCode interactionKey = KeyCode.W; 
    
    private bool playerIsClose = false;

    [Header("Recompensa deste PC")]
    [Tooltip("O ItemData que o jogador recebe. Deixe NULO se o PC não der item.")]
    public ItemData itemDentro; 
    
    // --- LÓGICA DE INTERAÇÃO ---
    void Update()
    {
        // Se estivermos próximos, e a tecla de interação for pressionada.
        if (playerIsClose && Input.GetKeyDown(interactionKey)) 
        {
            Debug.Log($"[INPUT PC ORIGINAL]: Capturado {interactionKey}. Tentando interagir.");
            Interact();
        }
    }

    public void Interact()
    {
        ComputerPuzzle puzzleManager = ComputerPuzzle.Instance; 
        
        if (puzzleManager != null)
        {
            // Chamada original para o sistema de puzzle
            puzzleManager.StartPuzzle(this);
        }
    }

    // 🔴 CORREÇÃO CRÍTICA: MÉTODOS DE PROXIMIDADE 2D
    
    private void OnTriggerEnter2D(Collider2D other) // <- USANDO COLLIDER2D
    {
        // Debug de diagnóstico para o console
        Debug.Log($"[PC DIAGNÓSTICO 2D]: Objeto {other.gameObject.name} (Tag: {other.tag}) ENTROU no Trigger.");
        
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            Debug.Log("PC: Player ENTROU na zona de interação (2D).");
        }
    }

    private void OnTriggerExit2D(Collider2D other) // <- USANDO COLLIDER2D
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            Debug.Log("PC: Player SAIU da zona de interação (2D).");
        }
    }
}