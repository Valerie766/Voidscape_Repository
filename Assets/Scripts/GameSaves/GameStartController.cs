using UnityEngine;

public class GameStartController : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    [Tooltip("Tag do GameObject que serve como ponto de spawn (Ex: 'Spawn').")]
    public string spawnPointTag = "Spawn";
    [Tooltip("Tag do GameObject do Player (Ex: 'Player').")]
    public string playerTag = "Player";
    
    void Awake()
    {
        // === 1. LÓGICA DE HARD RESET (ISOLADA AQUI) ===

        // Verifica se a flag de reset foi definida pelo Menu Principal
        if (PlayerPrefs.HasKey("PerformHardReset"))
        {
            UnityEngine.Debug.Log("[GAME START] Hard Reset detectado. Executando limpeza total.");
            
            // Limpa o Inventário, Puzzles, e Flags de Diálogo
            GameResetter.ResetGameData(); 

            // Remove a flag para que a próxima troca de cena não acione o reset
            PlayerPrefs.DeleteKey("PerformHardReset");
            
            // === 2. LÓGICA DE SPAWN FORÇADO ===
            
            ForcePlayerSpawn();
        }
        else
        {
            // O jogo foi carregado via Time Travel (troca de cena normal).
            // NENHUM HARD RESET é executado. O estado do jogo é mantido.
            UnityEngine.Debug.Log("[GAME START] Carregamento normal via Time Travel. Progresso mantido.");
        }
        
        // Auto-destrói-se após a checagem, a não ser que seja um Manager persistente
        Destroy(gameObject); 
    }
    
    // Reposiciona o jogador para o ponto de spawn
    private void ForcePlayerSpawn()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
            UnityEngine.Debug.Log("[SPAWN] Jogador reposicionado para o SpawnPoint.");
        }
        else
        {
            UnityEngine.Debug.LogError("[SPAWN ERROR] Certifique-se de que os GameObjects com as tags 'Spawn' e 'Player' existem na cena.");
        }
    }
}