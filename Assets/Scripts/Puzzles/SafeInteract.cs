using UnityEngine;

// Assumimos que ItemData, playerProximo e itemDentro são definidos em ContainerInteract
public class SafeInteract : ContainerInteract
{
    // A única alteração está no método Update, que substitui a coleta direta pelo início do puzzle.

    void Update()
    {
        // Verifica se o jogador está perto E se a tecla de interação (W) foi pressionada
        if (playerProximo && Input.GetKeyDown(KeyCode.W))
        {
            // CRÍTICO: Verifica se o ItemData está ligado ANTES de começar o puzzle
            if (itemDentro == null)
            {
                 Debug.LogWarning("SafeInteract: Cofre sem ItemData configurado no Inspector. Interação cancelada.");
                 return;
            }
            
            // 💡 NOVO FLUXO: Inicia o Puzzle em vez de dar o item
            SafePuzzle puzzleManager = SafePuzzle.Instance;
            
            if (puzzleManager != null)
            {
                // Passa a referência DESTE cofre (this) para o Puzzle Manager
                // O Puzzle Manager usa essa referência para dar o item e destruir o objeto
                puzzleManager.StartPuzzle(this); 
            }
            else
            {
                 Debug.LogError("SafeInteract: SafePuzzle Manager não encontrado na cena! Verifique se ele está ativo e configurado como Singleton.");
            }
        }
    }

    // Os métodos 'OnTriggerEnter2D', 'OnTriggerExit2D' e as variáveis 'itemDentro' e 'playerProximo'
    // são herdados diretamente do seu script original 'ContainerInteract', que precisa estar no mesmo projeto/namespace.
}