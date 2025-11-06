using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Respawn Settings")]
    public Vector2 startPosition = Vector2.zero;
    public int gameSceneIndex = 1; 
    
    private TimeTravelManager timeTravelManager;
    private bool isHandlingDeath = false;
    private int defaultPlayerLayer; // Usado para restaurar a Layer original

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
        
        if (PlayerMovement.Instance != null)
        {
             // Assumimos que a Layer padrão é definida no Start do PlayerMovement, mas
             // vamos armazenar a Layer padrão para uso no respawn.
             defaultPlayerLayer = LayerMask.NameToLayer("Player"); // Assumindo Layer padrão "Player"
        }
        
        if (!PlayerPositionManager.hasSavedPosition)
        {
            FindObjectOfType<PlayerPositionManager>()?.SavePosition(startPosition);
        }
    }
    
    void OnDestroy()
    {
         SceneManager.sceneLoaded -= OnAnySceneLoaded;
    }

    private void OnAnySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isHandlingDeath && PlayerMovement.Instance != null)
        {
            StartCoroutine(RecoverPlayerAfterSceneLoad(PlayerMovement.Instance.gameObject));
        }
        
        if (!isHandlingDeath)
        {
             FinalizeRespawn(PlayerMovement.Instance?.gameObject);
        }
    }
    
    private IEnumerator RecoverPlayerAfterSceneLoad(GameObject player)
    {
        yield return null; 

        if (player != null)
        {
            // 1. HARD ACTIVATE
            if (!player.activeSelf)
            {
                player.SetActive(true);
                Debug.Log("GM Coroutine: Player HARD ACTIVATED. Input agora deve funcionar.");
            }
            
            // 2. REPOSITION AND RE-ENABLE CONTROL
            if (PlayerPositionManager.hasSavedPosition)
            {
                player.transform.position = PlayerPositionManager.lastPosition;
            }
            
            if (player.GetComponent<PlayerMovement>() is PlayerMovement pm)
            {
                pm.enabled = true; // Re-habilita o controle de input
            }
            
            // 💡 CORREÇÃO CRÍTICA DO HIDEOUTCORRECTION
            Renderer playerRenderer = player.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                 // Garante que o Player esteja visível
                 playerRenderer.enabled = true; 
            }
            
            // Garante que o Player volte para a Layer Padrão (detectável)
            player.layer = defaultPlayerLayer;
            Debug.Log("GM Coroutine: Estado visual e Layer restaurados.");

            // 3. SINCRONIZAÇÃO DA CÂMERA
            if (CameraController.Instance != null)
            {
                Vector2 safeMin = new Vector2(-1000f, -1000f);
                Vector2 safeMax = new Vector2(1000f, 1000f);
                CameraController.Instance.HardResetAndSnap(player.transform, safeMin, safeMax);
                Debug.Log("GM Coroutine: Câmera sincronizada.");
            }
        }
        
        // 4. Finaliza o estado
        isHandlingDeath = false;
        Debug.Log("GM: Sequência de morte concluída. Player deve estar ativo e visível.");
    }

    public void StartDeathSequence(GameObject player)
    {
        if (isHandlingDeath) return;
        isHandlingDeath = true;
        
        // 1. DESABILITA O INPUT
        if (player.GetComponent<PlayerMovement>() is PlayerMovement pm)
        {
            pm.enabled = false;
            // Armazena a Layer padrão APENAS SE AINDA NÃO FOI FEITO
            if (defaultPlayerLayer == 0)
            {
                defaultPlayerLayer = player.layer;
            }
            
            // 2. DESATIVAÇÃO DO OBJETO (USANDO SETACTIVE(FALSE) AGORA, NÃO O DEBUG TRACE)
            // Assumindo que você removeu a função de debug temporária.
            player.SetActive(false); 
        }

        // 3. Inicia o processo de Respawn/Recarregamento de Cena
        RespawnPlayer(player);
    }

    public void RespawnPlayer(GameObject player)
    {
        FindObjectOfType<PlayerPositionManager>()?.SavePosition(startPosition);
        
        if (SceneManager.GetActiveScene().buildIndex != gameSceneIndex)
        {
             SceneManager.LoadScene(gameSceneIndex);
             return;
        }
        
        StartCoroutine(RecoverPlayerAfterSceneLoad(player));
    }
    
    public void FinalizeRespawn(GameObject player)
    {
        if (player == null && PlayerMovement.Instance != null)
        {
             player = PlayerMovement.Instance.gameObject;
        }
        
        if (player == null || isHandlingDeath) return; 

        if (player.activeSelf && CameraController.Instance != null)
        {
             Vector2 safeMin = new Vector2(-1000f, -1000f);
             Vector2 safeMax = new Vector2(1000f, 1000f);
             CameraController.Instance.HardResetAndSnap(player.transform, safeMin, safeMax); 
        }
    }
    
    public bool IsHandlingDeath() { return isHandlingDeath; }
    public TimeTravelManager GetTimeTravelManager() { return timeTravelManager; }
}
