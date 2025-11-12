using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTravelManager : MonoBehaviour
{
    // Nomes das cenas (Preencha no Inspector)
    public string presentSceneName = "Present_Scene"; 
    public string pastSceneName = "Past_Scene";

    [Header("Respawn Settings")]
    [Tooltip("Nome da cena principal (Scene 1) para onde o Player deve retornar após a morte.")]
    public string initialSceneName = "Scene_1"; // 💡 NOVO CAMPO
    
    [Header("Restrição de Área")]
    [Tooltip("Define se a viagem no tempo está bloqueada por uma TimeRestrictionArea.")]
    public bool isAreaRestricted = false; // Bandeira controlada pela TimeRestrictionArea
    public AudioClip failSound; 
    private AudioSource audioSource; 

    private GameObject player;
    private string currentSceneName;
    // Referência ao GameManager
    private GameManager gameManager; 
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameManager = GetComponent<GameManager>();

        // Configuração do AudioSource (Se este script estiver no GameManager)
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        
        FindPlayerAndMakePersistent(); 
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void FindPlayerAndMakePersistent() 
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                DontDestroyOnLoad(player);
            }
        }
    }

    // O PlayerMovement chama este método para iniciar a viagem
    public void TryTravelThroughTime(Vector3 playerPosition) 
    {
        // 1. CHECAGEM DE BLOQUEIO GERAL (Morte/Cutscene)
        bool isGameLocked = (gameManager != null && gameManager.IsHandlingDeath()) || 
                             (CutsceneManager.Instance != null && CutsceneManager.Instance.IsCutscenePlaying());

        // 2. CHECAGEM FINAL: Se estiver bloqueado por qualquer razão (Geral OU de Área)
        if (isGameLocked || isAreaRestricted)
        {
            HandleTravelFailure();
            return;
        }

        // Se não houver restrição, salva a posição e viaja.
        if (player == null) return;
        
        // 3. Salva a posição antes de carregar (Se necessário para o PlayerPositionManager)
        FindObjectOfType<PlayerPositionManager>()?.SavePosition(playerPosition); 
        
        string sceneToLoad = (currentSceneName == presentSceneName) ? pastSceneName : presentSceneName;
        
        // Desativa o movimento (Isso será reativado em OnSceneLoaded)
        if (player.GetComponent<PlayerMovement>() is PlayerMovement movementScript)
        {
            movementScript.enabled = false;
        }
        
        LoadSceneExplicitly(sceneToLoad);
    }
    
    private void HandleTravelFailure()
    {
        Debug.LogWarning("Viagem no Tempo Bloqueada: Restrição de Jogo ou de Área Ativa.");
        
        if (failSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(failSound);
        }
    }

    // Usado para Viagem no Tempo e pelo Respawn (chamada direta de cena)
    public void LoadSceneExplicitly(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerAndMakePersistent();

        if (player != null)
        {
            currentSceneName = scene.name;
            
            // 💡 AJUSTE CRÍTICO: Se o GameManager ESTIVER LIDANDO COM A MORTE,
            // NÃO REATIVE O MOVIMENTO AQUI. O GameManager fará isso (em RecoverPlayerAfterSceneLoad).
            if (gameManager == null || !gameManager.IsHandlingDeath())
            {
                // Reativa o movimento (SOMENTE se for uma Viagem no Tempo manual)
                if (player.GetComponent<PlayerMovement>() is PlayerMovement movementScript)
                {
                    movementScript.enabled = true;
                    Debug.Log("Viagem no Tempo concluída. Movimento do Player reativado.");
                }
            }
            
            // CHAMA A VERIFICAÇÃO MANUAL DE RESTRIÇÃO APÓS TROCA DE CENA
            CheckAreaRestrictionOnLoad(); 
        }
    }

    // ==========================================================
    // MÉTODOS DE RESTRIÇÃO E GETTERS
    // ==========================================================
    
    // Setter chamado pela TimeRestrictionArea
    public void SetAreaRestriction(bool isRestricted)
    {
        this.isAreaRestricted = isRestricted;
    }

    // Método para verificar se o Player nasce dentro de uma área restrita
    private void CheckAreaRestrictionOnLoad()
    {
        if (player == null) return;
        
        // Encontra todas as TimeRestrictionArea (incluindo EnemyTimeRestriction)
        TimeRestrictionArea[] areas = FindObjectsOfType<TimeRestrictionArea>(); 
        
        foreach(TimeRestrictionArea area in areas)
        {
            Collider2D areaCollider = area.GetComponent<Collider2D>();
            
            // Verifica se a posição do Player sobrepõe o colisor da área.
            if (areaCollider != null && areaCollider.OverlapPoint(player.transform.position))
            {
                SetAreaRestriction(true);
                Debug.Log("RESTRIÇÃO FORÇADA: Player nasceu DENTRO da área de restrição.");
                return; 
            }
        }
        
        // Se a verificação manual não encontrou restrição, garante que a bandeira esteja limpa.
        SetAreaRestriction(false);
    }
    
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    
    public void ForceCurrentSceneName(string newSceneName)
    {
        currentSceneName = newSceneName;
    }
}