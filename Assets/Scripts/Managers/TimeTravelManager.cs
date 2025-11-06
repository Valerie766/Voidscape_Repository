using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTravelManager : MonoBehaviour
{
    // Nomes das cenas (Preencha no Inspector)
    public string presentSceneName = "Present_Scene"; 
    public string pastSceneName = "Past_Scene";

    private GameObject player;
    private string currentSceneName;
    
    // Referência ao GameManager para coordenação
    private GameManager gameManager; 
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameManager = GetComponent<GameManager>();
        
        FindPlayerAndMakePersistent(); 
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // 1. CHECAGEM DE BLOQUEIO (CRÍTICO)
        // A viagem no tempo é permitida SOMENTE se o jogo não estiver tratando de morte/cutscene.
        bool isGameLocked = (gameManager != null && gameManager.IsHandlingDeath()) || 
                            (CutsceneManager.Instance != null && CutsceneManager.Instance.IsCutscenePlaying());

        if (Input.GetKeyDown(KeyCode.T) && !isGameLocked)
        {
            TravelThroughTime();
        }
    }

    public void FindPlayerAndMakePersistent() 
    {
        if (player == null)
        {
            // Tenta encontrar o player na cena atual
            player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                DontDestroyOnLoad(player);
                // NOTA: O script PlayerMovement DEVE ser reativado pelo OnSceneLoaded, ou em FinalizeRespawn.
            }
        }
    }

    public void TravelThroughTime()
    {
        if (player == null) return;
        
        string sceneToLoad = (currentSceneName == presentSceneName) ? pastSceneName : presentSceneName;
        
        // Antes de viajar, desativa o movimento para evitar erros no meio do carregamento
        if (player.GetComponent<PlayerMovement>() is PlayerMovement movementScript)
        {
             movementScript.enabled = false;
        }
        
        LoadSceneExplicitly(sceneToLoad);
    }

    // Usado para Viagem no Tempo e pelo Respawn (chamada direta de cena)
    public void LoadSceneExplicitly(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // currentSceneName será atualizado em OnSceneLoaded
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerAndMakePersistent();

        if (player != null)
        {
            // Atualiza o estado da cena
            currentSceneName = scene.name;

            // Se o GameManager não estiver manipulando a morte (ou seja, foi uma Viagem no Tempo manual),
            // REATIVA o PlayerMovement.
            // Se for um Respawn, o GameManager.FinalizeRespawn() cuidará disso.
            if (gameManager == null || !gameManager.IsHandlingDeath())
            {
                if (player.GetComponent<PlayerMovement>() is PlayerMovement movementScript)
                {
                     movementScript.enabled = true;
                     Debug.Log("Viagem no Tempo concluída. Movimento do Player reativado.");
                }
            }
            // Se isHandlingDeath for TRUE, o FinalizeRespawn (chamado pelo GameManager) fará a reativação.
        }
    }
    
    // ==========================================================
    // GETTERS E SETTERS
    // ==========================================================
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    
    public void ForceCurrentSceneName(string newSceneName)
    {
        currentSceneName = newSceneName;
    }
}