using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video; 
using System.Collections;
using System.Diagnostics;

public class CutsceneSequencer : MonoBehaviour
{
    public static CutsceneSequencer Instance;

    [Header("Cutscene 1: Monitor")]
    [Tooltip("O Canvas/Root que contém a cutscene (DESATIVADO).")]
    public GameObject monitorRoot;
    [Tooltip("O componente VideoPlayer do Monitor.")]
    public VideoPlayer monitorVideoPlayer;

    [Header("Cutscene 2: Créditos")]
    public GameObject creditsRoot;
    public VideoPlayer creditsVideoPlayer;

    [Header("Reset")]
    [Tooltip("O nome da cena do menu para carregar ao final da sequência.")]
    public string menuSceneName = "Main_Menu"; 
    
    private int currentCutscene = 0; // 1=Monitor, 2=Créditos, 3=Reset
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Garante que o manager não seja destruído
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // Se já existe outra instância, destrói esta nova
            Destroy(gameObject);
        }
        
        // Garante que o estado comece do zero quando o objeto for criado
        currentCutscene = 0; 

        // Desativação inicial das cutscenes
        if (monitorRoot != null) monitorRoot.SetActive(false);
        if (creditsRoot != null) creditsRoot.SetActive(false);
        
        // Assina o evento de término para ambos os players
        if (monitorVideoPlayer != null)
        {
            monitorVideoPlayer.loopPointReached += OnVideoFinished;
        }
        if (creditsVideoPlayer != null)
        {
            creditsVideoPlayer.loopPointReached += OnVideoFinished;
        }
    }
    
    // CHAMADA PELO NPC.CS
    public void StartCutsceneSequence()
    {
        UnityEngine.Debug.Log("[SEQUENCER] Sequência de Cutscenes Iniciada. Tocando na cena atual.");
        currentCutscene = 1; // Começa na primeira fase
        
        PlayNextCutscene(); 
    }

    private void PlayNextCutscene()
    {
        UnityEngine.Debug.Log($"[SEQUENCER] Iniciando Fase {currentCutscene}.");

        if (currentCutscene == 1)
        {
            // === DIAGNÓSTICO E REPRODUÇÃO DO MONITOR ===
            
            if (monitorVideoPlayer == null || monitorRoot == null || monitorVideoPlayer.clip == null)
            {
                UnityEngine.Debug.LogError("[MONITOR] FALHA: Referências faltando. Pulando Cutscene.");
                currentCutscene = 2; 
                PlayNextCutscene();
                return;
            }
            
            monitorRoot.SetActive(true);
            monitorVideoPlayer.Prepare();
            monitorVideoPlayer.isLooping = false;
            monitorVideoPlayer.prepareCompleted += (vp) => vp.Play(); 
            UnityEngine.Debug.Log("[MONITOR] SUCESSO: Monitor Cutscene INICIADA.");
        }
        else if (currentCutscene == 2)
        {
            // === DIAGNÓSTICO E REPRODUÇÃO DOS CRÉDITOS ===

            // Desativa a cutscene anterior (Monitor)
            if (monitorRoot != null) monitorRoot.SetActive(false); 
            
            if (creditsVideoPlayer == null || creditsRoot == null || creditsVideoPlayer.clip == null)
            {
                UnityEngine.Debug.LogError("[CRÉDITOS] FALHA: Referências faltando. Indo para Reset.");
                currentCutscene = 3; 
                PlayNextCutscene();
                return;
            }
            
            creditsRoot.SetActive(true);
            creditsVideoPlayer.Prepare();
            creditsVideoPlayer.isLooping = false;
            creditsVideoPlayer.prepareCompleted += (vp) => vp.Play();
            UnityEngine.Debug.Log("[CRÉDITOS] SUCESSO: Créditos Cutscene INICIADA.");
        }
        else if (currentCutscene == 3)
        {
            // O vídeo dos créditos terminou. AGORA sim, chamamos o carregamento da cena.
            LoadMenuScene();
        }
    }

    // Chamado automaticamente quando um VideoPlayer termina (via loopPointReached)
    private void OnVideoFinished(VideoPlayer vp)
    {
        // Limpa o handler de prepareCompleted para evitar problemas
        vp.prepareCompleted -= (player) => player.Play(); 
        
        if (vp == monitorVideoPlayer)
        {
            UnityEngine.Debug.Log("[MONITOR] Vídeo Finalizado. Avançando para Créditos.");
            currentCutscene = 2; 
        }
        else if (vp == creditsVideoPlayer)
        {
            UnityEngine.Debug.Log("[CRÉDITOS] Vídeo Finalizado. Iniciando Reset.");
            currentCutscene = 3;
        }
        
        // Inicia a próxima fase (ou o reset)
        PlayNextCutscene();
    }
    
    private void LoadMenuScene()
    {
        UnityEngine.Debug.Log("[RESET] Fim da Sequência de Vídeos. Carregando Cena de Menu.");
        
        // Destrói este manager ANTES de carregar a cena, pois ele não é mais necessário
        Destroy(gameObject); 

        // Carrega a cena de menu.
        SceneManager.LoadScene(menuSceneName);
    }
    
    void OnDestroy()
    {
        // Limpa as assinaturas para evitar erros se o Manager for destruído
        if (monitorVideoPlayer != null)
        {
            monitorVideoPlayer.loopPointReached -= OnVideoFinished;
        }
        if (creditsVideoPlayer != null)
        {
            creditsVideoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}