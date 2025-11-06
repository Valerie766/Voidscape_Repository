using UnityEngine;
using TMPro; // Necessário para usar TextMeshProUGUI

// Este script vai no GameObject que representa o PC no cenário.
public class ComputerInteract : MonoBehaviour
{
    // 💡 CORREÇÃO: Campo da tecla de interação adicionado de volta
    [Header("Configuração de Interação")]
    [Tooltip("A tecla que o jogador deve pressionar para iniciar a interação.")]
    public KeyCode interactionKey = KeyCode.W; 
    
    // Variável de proximidade (gerenciada por OnTriggerEnter/Exit)
    private bool playerIsClose = false;

    [Header("Recompensa deste PC")]
    [Tooltip("O ItemData que o jogador recebe. Deixe NULO se o PC não der item.")]
    public ItemData itemDentro; 
    
    [Header("Componentes de UI Específicos")]
    [Tooltip("O GameObject Root/Canvas que contém toda a UI do computador.")]
    public GameObject computerRoot;
    
    [Tooltip("O painel que lista os botões dos arquivos.")]
    public GameObject fileListPanel;
    
    [Tooltip("O painel que exibe a nota/conteúdo do arquivo.")]
    public GameObject noteDisplayPanel;
    
    [Tooltip("O componente TextMeshPro que exibirá o texto do arquivo.")]
    public TextMeshProUGUI noteTextDisplay;

    // --- LÓGICA DE INTERAÇÃO ---
    void Update()
    {
        // Se estivermos próximos, e a tecla de interação for pressionada.
        if (playerIsClose && Input.GetKeyDown(interactionKey)) // 💡 USANDO O CAMPO DO INSPECTOR
        {
            // Debug de diagnóstico para garantir que o input está sendo capturado
            Debug.Log($"[INPUT PC]: Capturado {interactionKey}. Tentando interagir.");
            Interact();
        }
    }

    public void Interact()
    {
        ComputerPuzzle puzzleManager = ComputerPuzzle.Instance; 
        
        if (puzzleManager != null)
        {
            puzzleManager.StartPuzzle(
                this, 
                computerRoot, 
                fileListPanel, 
                noteDisplayPanel,
                noteTextDisplay
            );
        }
    }

    // --- MÉTODOS DE PROXIMIDADE (Deixe o que você está usando - 3D ou 2D) ---
    // (Mantive os métodos 3D como padrão, você pode usar os 2D se for o caso)
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            Debug.Log("PC: Player ENTROU na zona de interação.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            Debug.Log("PC: Player SAIU da zona de interação.");
        }
    }
}