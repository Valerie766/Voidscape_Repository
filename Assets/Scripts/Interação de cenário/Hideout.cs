using UnityEngine;

public class Hideout : MonoBehaviour
{
    private bool playerIsNear = false;
    private GameObject playerObject = null;

    [Tooltip("A Tag que o Player tem (Geralmente 'Player').")]
    public string playerTag = "Player";

    // Variável para armazenar a camada original do Player (para restaurar depois)
    private int originalPlayerLayer; 

    void Update()
    {
        // Verifica se o Player está perto e se a tecla de interação foi pressionada
        if (playerIsNear && Input.GetKeyDown(KeyCode.W))
        {
            if (playerObject != null)
            {
                // Se o Player está escondido, ele sai. Se não, ele entra.
                if (IsPlayerHidden(playerObject))
                {
                    ExitHideout(playerObject);
                }
                else
                {
                    EnterHideout(playerObject);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = true;
            playerObject = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Se o Player sair da área, forçamos a saída do esconderijo
            if (IsPlayerHidden(other.gameObject))
            {
                 ExitHideout(other.gameObject);
            }
            
            playerIsNear = false;
            playerObject = null;
        }
    }

    private void EnterHideout(GameObject player)
    {
        // 1. Esconde visualmente o Player
        player.GetComponent<Renderer>().enabled = false;
        
        // 2. Desabilita o movimento (opcional, mas recomendado)
        player.GetComponent<PlayerMovement>().enabled = false; 
        
        // 3. Torna o Player indetectável para inimigos (mudando a Layer)
        originalPlayerLayer = player.layer;
        player.layer = LayerMask.NameToLayer("Hidden"); // Usaremos uma nova Layer chamada "Hidden"
        
        Debug.Log("Player Escondido.");
    }

    private void ExitHideout(GameObject player)
    {
        // 1. Revela visualmente o Player
        player.GetComponent<Renderer>().enabled = true;
        
        // 2. Habilita o movimento
        player.GetComponent<PlayerMovement>().enabled = true; 
        
        // 3. Restaura a Layer original (Torna-o detectável novamente)
        player.layer = originalPlayerLayer;
        
        Debug.Log("Player Saiu do Esconderijo.");
    }
    
    private bool IsPlayerHidden(GameObject player)
    {
        // Verifica se a Layer do Player é a de "Hidden"
        return player.layer == LayerMask.NameToLayer("Hidden");
    }
}
