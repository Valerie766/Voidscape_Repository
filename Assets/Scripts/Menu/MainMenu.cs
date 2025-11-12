using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("O índice da primeira cena de gameplay na Build Settings.")]
    public int initialGameplaySceneIndex = 1;

    public void PlayGame()
    {
        UnityEngine.Debug.Log("[MENU] Botão Play acionado. Iniciando Hard Reset.");

        // 1. Define uma flag para sinalizar que a cena deve ter um HARD RESET
        // Esta é a CHAVE que isola a lógica de reset!
        PlayerPrefs.SetInt("PerformHardReset", 1);
        PlayerPrefs.Save(); // Garante que o valor foi escrito imediatamente

        // 2. Limpa Managers Persistentes (a parte que NÃO depende da cena)
        
        // Destrói o Cutscene Sequencer persistente
        CutsceneSequencer sequencer = FindObjectOfType<CutsceneSequencer>();
        if (sequencer != null)
        {
            Destroy(sequencer.gameObject);
        }
        
        // Destrói o Jogador persistente
        GameObject player = GameObject.FindGameObjectWithTag("Player"); 
        if (player != null)
        {
            Destroy(player.gameObject);
        }

        // 3. Carrega a primeira cena de gameplay.
        // O restante do reset e o spawn do player ocorrerão no script da cena.
        SceneManager.LoadSceneAsync(initialGameplaySceneIndex);
    }
}