using UnityEngine;
using UnityEngine.Video; // CRÍTICO: Para usar o VideoPlayer
using UnityEngine.SceneManagement; // Para usar o SceneManager no futuro

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance; // Singleton
    
    [Header("Vídeos das Cutscenes")]
    public VideoPlayer timeTravelVideoPlayer; // Mantido para o futuro, se necessário
    public VideoPlayer deathVideoPlayer; 
    
    private GameObject playerRef;
    private bool isCutscenePlaying = false; // Flag para controlar o estado da cutscene

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Garante que o CutsceneManager persista
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Assina o evento de conclusão para a Morte
        if (deathVideoPlayer != null)
        {
            deathVideoPlayer.loopPointReached += OnDeathVideoFinished;
        }
    }
    
    // ======================================================
    // MODO: MORTE (Chamado pelo EnemyPatrol.cs)
    // ======================================================
    public void StartDeathCutscene(GameObject player)
    {
        // Bloqueia se já estiver rodando ou se não houver vídeo
        if (isCutscenePlaying || deathVideoPlayer == null) return;
        
        isCutscenePlaying = true;
        playerRef = player; // Guarda a referência do Player

        // 1. CONGELA O JOGO (para pausar toda a física e lógica de Update)
        Time.timeScale = 0f;
        
        // 2. DESATIVA O PLAYER (desaparece visualmente durante a cutscene)
        if (playerRef != null)
        {
            playerRef.SetActive(false);
        }
        
        // 3. ATIVA E INICIA O VÍDEO
        deathVideoPlayer.gameObject.SetActive(true);
        deathVideoPlayer.Play();
    }
    
    private void OnDeathVideoFinished(VideoPlayer vp)
    {
        // 1. DESATIVA VISUAL
        vp.gameObject.SetActive(false);
        
        // 2. RESPONSÁVEL PELO RESPAWN DO PLAYER
        if (playerRef != null && GameManager.Instance != null)
        {
            // O GameManager cuidará de reativar o Player e movê-lo (para a startPosition)
            GameManager.Instance.RespawnPlayer(playerRef);
            // NOTA: A reativação do PlayerMovement é feita dentro do GameManager.FinalizeRespawn
        }
        
        // 3. RESPONSÁVEL PELO RESPONSÁVEL E RESET DO INIMIGO
        // O FindGameObjectWithTag é custoso, considere usar uma referência direta se possível.
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            enemy.SetActive(true);
            
            if (enemy.GetComponent<EnemyPatrol>() is EnemyPatrol enemyScript)
            {
                enemyScript.ResetEnemy(); 
            }
        }
        
        // 4. DESCONGELA O JOGO
        Time.timeScale = 1f;
        isCutscenePlaying = false; // CRÍTICO: Libera a flag.
    }
    
    // ======================================================
    // NOVO MÉTODO: CHAVE PARA BLOQUEAR VIAGEM NO TEMPO
    // ======================================================
    public bool IsCutscenePlaying()
    {
        return isCutscenePlaying;
    }
    
    // Função placeholder para Viagem no Tempo, se for usada.
    public void StartTimeTravelCutscene() { }
}