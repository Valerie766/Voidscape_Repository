using UnityEngine;
using TMPro; 
using System.Collections;

public class NPC : MonoBehaviour
{
    // Referências à UI e Dados
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText; 
    public string[] dialogue;

    [Header("Configurações")]
    public KeyCode startKey = KeyCode.W;       // Tecla para iniciar o diálogo
    public KeyCode advanceKey = KeyCode.Space; // Tecla para avançar o diálogo (Barra de Espaço)
    public float wordSpeed = 0.05f; 
    
    // Variáveis de Controle
    private int index;
    private bool isTyping = false;
    public bool playerIsClose;

    // -----------------------------------------------------
    // DETECÇÃO DE COLISÃO 2D
    // -----------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }

    // -----------------------------------------------------
    // LÓGICA DE INPUT (Ativação e Avanço)
    // -----------------------------------------------------

    void Update()
    {
        // === 1. Lógica de INICIAR/FECHAR a Conversa (Tecla W) ===
        if (playerIsClose && Input.GetKeyDown(startKey) && !isTyping)
        {
            if (dialogPanel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialogPanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }
        
        // === 2. Lógica de AVANÇAR/COMPLETAR (Tecla de Avanço/Space) ===
        if (dialogPanel != null && dialogPanel.activeInHierarchy && Input.GetKeyDown(advanceKey))
        {
            // O LOG de diagnóstico foi removido do Update para simplificar o console na versão final.

            if (isTyping)
            {
                CompleteSentence();
            }
            else
            {
                NextLine();
            }
        }
    }

    // -----------------------------------------------------
    // FUNÇÕES AUXILIARES
    // -----------------------------------------------------

    // Função que limpa e desativa a UI
    public void zeroText()
    {
        StopAllCoroutines(); 
        isTyping = false;
        
        dialogText.text = "";
        index = 0;
        dialogPanel.SetActive(false);
    }

    // Efeito de Digitação
    IEnumerator Typing()
    {
        isTyping = true;
        
        foreach (char letter in dialogue[index].ToCharArray())
        {
            if (!isTyping) break; 
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
    }
    
    // Função para completar o texto imediatamente
    private void CompleteSentence()
    {
        StopAllCoroutines(); 
        dialogText.text = dialogue[index]; 
        isTyping = false; 
    }

    // -----------------------------------------------------
    // AVANÇAR PARA A PRÓXIMA LINHA (Chamada da Sequência Final)
    // -----------------------------------------------------
    
    public void NextLine()
    {
        // Se ainda houver frases para exibir
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogText.text = ""; 
            StartCoroutine(Typing());
        }
        // Se for a ÚLTIMA frase, inicia a sequência de cutscenes.
        else
        {
            zeroText(); // Fecha o diálogo
            
            // Chama o novo Sequencer (CutsceneSequencer)
            if (CutsceneSequencer.Instance != null)
            {
                CutsceneSequencer.Instance.StartCutsceneSequence();
                UnityEngine.Debug.Log("NPC: Chamada para CutsceneSequencer bem-sucedida. Cutscenes devem iniciar na cena atual.");
            }
            else
            {
                // Este log só aparece se o Manager não estiver na cena!
                UnityEngine.Debug.LogError("NPC: ERRO! CutsceneSequencer.Instance é NULO. O Manager não está ativo ou não inicializou.");
            }
        }
    }
}