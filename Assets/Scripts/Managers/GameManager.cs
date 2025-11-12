using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // REMOVIDOS: startPosition e gameSceneIndex. O RespawnPoint controlará o local.
    
    private TimeTravelManager timeTravelManager;
    private bool isHandlingDeath = false;
    private int defaultPlayerLayer; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnAnySceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        timeTravelManager = GetComponent<TimeTravelManager>();
        
        // Garante que o TimeTravelManager esteja presente para a coordenação de cenas
        if (timeTravelManager == null)
        {
            Debug.LogError("GameManager requer um TimeTravelManager no mesmo GameObject!");
        }
        
        // Tentativa de encontrar a Layer padrão do Player.
        if (PlayerMovement.Instance != null)
        {
             defaultPlayerLayer = PlayerMovement.Instance.gameObject.layer;
        }
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnAnySceneLoaded;
    }

    private void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 💡 CRÍTICO: Se o GameManager estiver manipulando a morte E o Player existe
        if (isHandlingDeath && PlayerMovement.Instance != null)
        {
            // O RecoverPlayerAfterSceneLoad cuida do reposicionamento para o RespawnPoint
            // e da reativação.
            StartCoroutine(RecoverPlayerAfterSceneLoad(PlayerMovement.Instance.gameObject));
        }
        
        // O FinalizeRespawn original (sem isHandlingDeath) era para ajustes na Viagem no Tempo manual.
        // Como o TimeTravelManager agora lida com o estado pós-Viagem no Tempo, este bloco é simplificado.
        // Se a lógica aqui for importante para outros sistemas, mantenha. Caso contrário, pode ser removido.
        /*
        if (!isHandlingDeath)
        {
             FinalizeRespawn(PlayerMovement.Instance?.gameObject);
        }
        */
    }
    
    private IEnumerator RecoverPlayerAfterSceneLoad(GameObject player)
    {
        yield return null; // Aguarda 1 frame para a cena carregar completamente
        
        if (player != null)
        {
            // 1. HARD ACTIVATE
            if (!player.activeSelf)
            {
                player.SetActive(true);
            }
            
            // 2. REPOSICIONAMENTO PARA O RESPAWNPOINT
            if (RespawnPoint.Instance != null)
            {
                // 💡 NOVO: Usa a posição do RespawnPoint na cena recém-carregada
                player.transform.position = RespawnPoint.Instance.GetSpawnPosition();
                Debug.Log($"GM: Player respawnado em {RespawnPoint.Instance.GetSpawnPosition()} na {SceneManager.GetActiveScene().name}.");
                
                // Limpa o PlayerPositionManager para que o próximo 'T' salve a posição atual.
                // Isso previne que o PlayerPositionManager.lastPosition seja a posição da morte
                FindObjectOfType<PlayerPositionManager>()?.SavePosition(RespawnPoint.Instance.GetSpawnPosition());
            }
            else
            {
                // Se o ponto de respawn não for encontrado, ele pode cair no último ponto salvo
                Debug.LogError("RespawnPoint não encontrado na cena! Usando última posição salva.");
            }
            
            // 3. RESTAURAÇÃO DE ESTADO E CONTROLE
            if (player.GetComponent<PlayerMovement>() is PlayerMovement pm)
            {
                pm.enabled = true; // Re-habilita o controle de input
            }
            
            Renderer playerRenderer = player.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                playerRenderer.enabled = true; // Garante visibilidade
            }
            
            player.layer = defaultPlayerLayer; // Garante que volte para a Layer Padrão
            
            // 4. SINCRONIZAÇÃO DA CÂMERA
            if (CameraController.Instance != null)
            {
                Vector2 safeMin = new Vector2(-1000f, -1000f);
                Vector2 safeMax = new Vector2(1000f, 1000f);
                CameraController.Instance.HardResetAndSnap(player.transform, safeMin, safeMax);
            }
        }
        
        // 5. Finaliza o estado
        ResetDeathState();
    }

    public void StartDeathSequence(GameObject player)
    {
        if (isHandlingDeath) return;
        isHandlingDeath = true;
        
        // 1. DESABILITA O INPUT e SALVA LAYER PADRÃO
        if (player.GetComponent<PlayerMovement>() is PlayerMovement pm)
        {
            pm.enabled = false;
            if (defaultPlayerLayer == 0)
            {
                defaultPlayerLayer = player.layer;
            }
            player.SetActive(false); // Desativa o Player
        }

        // 2. Inicia o processo de Respawn
        RespawnPlayer(player);
    }

    // 💡 NOVO/AJUSTADO: Lógica central de respawn.
    public void RespawnPlayer(GameObject player)
    {
        if (timeTravelManager == null) 
        {
            Debug.LogError("TimeTravelManager é NULL. Não é possível respawnar.");
            return;
        }

        // Se a cena atual NÃO for a cena inicial (Scene 1), força o carregamento dela.
        if (SceneManager.GetActiveScene().name != timeTravelManager.initialSceneName)
        {
            timeTravelManager.LoadSceneExplicitly(timeTravelManager.initialSceneName);
            // O resto da rotina será executado em OnAnySceneLoaded -> RecoverPlayerAfterSceneLoad
            return;
        }
        
        // Se já estiver na cena correta, apenas recupera o Player.
        StartCoroutine(RecoverPlayerAfterSceneLoad(player));
    }

    public void ResetDeathState()
    {
        isHandlingDeath = false;
        // O TimeTravelManager também pode precisar de um método de reset de estado aqui.
    }
    
    // ... (Getters e Setters) ...
    public bool IsHandlingDeath() { return isHandlingDeath; }
    public TimeTravelManager GetTimeTravelManager() { return timeTravelManager; }
}