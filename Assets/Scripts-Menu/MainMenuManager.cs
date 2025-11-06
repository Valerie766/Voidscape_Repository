using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // O nome da sua primeira cena de jogo
    public string firstLevelSceneName = "Scene_1_Corridor"; 
    
    // Painel de opções (você precisa criá-lo no Canvas)
    public GameObject optionsPanel; 

    public void StartNewGame()
    {
        Debug.Log("Iniciando Novo Jogo...");
        // Opcional: Limpar dados salvos antes de começar
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void LoadSavedGame()
    {
        Debug.Log("Carregando Jogo Salvo...");
        // Implementação real exigiria lógica de PlayerPrefs ou Save/Load de arquivos
        
        // Exemplo: Simplesmente inicia um novo jogo se não houver um sistema de save complexo
        // SceneManager.LoadScene(firstLevelSceneName); 
    }

    public void OpenOptionsMenu()
    {
        Debug.Log("Abrindo Opções...");
        
        // Ativa o painel de opções (Audio, Iluminação, etc.)
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    // Adicione esta função ao botão "Voltar" do seu Painel de Opções
    public void CloseOptionsMenu()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }
    
    // Exemplo de função de Opções
    public void SetVolume(float volume)
    {
        Debug.Log("Volume definido para: " + volume);
        // Exemplo: AudioListener.volume = volume;
    }
}
