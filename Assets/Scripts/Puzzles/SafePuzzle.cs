using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; 

public class SafePuzzle : MonoBehaviour
{
    public static SafePuzzle Instance;

    [Header("Componentes UI")]
    public GameObject safeRoot;
    public GameObject safeContentPanel;
    public Text feedbackText;
    public Text inputDisplayText;
    [Tooltip("Arraste os 9 botões (1 a 9) aqui, EM ORDEM (Element 0 = Botão 1, Element 8 = Botão 9).")]
    public Button[] numberButtons; // Agora, deve ter 9 elementos no Inspector
    public Button closeButton;

    [Header("Configuração do Cofre")]
    [Tooltip("A sequência correta de números. Use apenas dígitos de 1 a 9.")]
    public List<int> correctSequence = new List<int>();
    
    [Header("Recompensa")]
    public ItemData rewardItem; 
    private SafeInteract currentContainer; 

    private List<int> currentInput = new List<int>();
    private bool puzzleActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        safeRoot.SetActive(false); 
    }
    
    void Start()
    {
        // 1. Liga os botões de 1 a 9 (o array tem 9 elementos)
        for (int i = 0; i < numberButtons.Length; i++)
        {
            // 💡 AJUSTE: O dígito correto é o índice + 1 (0 -> 1, 1 -> 2, etc.)
            int number = i + 1; 
            
            if (numberButtons[i] != null) 
            {
                numberButtons[i].onClick.AddListener(() => ButtonClicked(number));
            }
        }
        
        // 2. Liga o botão de fechar, se houver
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePuzzle);
        }
        
        if (correctSequence.Count == 0)
        {
            Debug.LogError("SafePuzzle: A sequência correta não foi configurada!");
        }
    }

    void Update()
    {
        if (!puzzleActive) return;

        // 1. TRATAMENTO DE SAÍDA COM 'W'
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentContainer != null) 
            {
                ClosePuzzle(); 
                return; 
            }
        }

        // 2. TRATAMENTO DO TECLADO NUMÉRICO (Apenas 1 a 9)
        for (int i = 1; i <= 9; i++)
        {
            // KeyCode.Alpha1 a Alpha9
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i)) 
            {
                ButtonClicked(i);
                return;
            }
        }
    }

    public void StartPuzzle(SafeInteract container)
    {
        if (puzzleActive) return;
        
        currentContainer = container; 
        rewardItem = container.itemDentro; 
        
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = false;
        }

        safeRoot.SetActive(true);
        if (safeContentPanel != null)
        {
            safeContentPanel.SetActive(true);
        }

        puzzleActive = true;
        currentInput.Clear();
        UpdateDisplay("Insira o código.", "");
        
        if (numberButtons.Length > 0 && numberButtons[0] != null)
        {
             numberButtons[0].Select();
        }
    }
    
    public void ClosePuzzle()
    {
        puzzleActive = false;
        safeRoot.SetActive(false);
        currentContainer = null;
        
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = true;
        }
    }

    // O MÉTODO CHAMADO PELOS BOTÕES
    public void ButtonClicked(int number)
    {
        if (!puzzleActive) return;
        
        // Se o número digitado for 0 (o que não deveria acontecer com 9 botões), ignoramos
        if (number == 0) return; 
        
        // DEBUG LOG mantido para testes
        Debug.Log("BOTÃO CLICADO: O dígito " + number + " foi registrado pelo script."); 
        
        if (currentInput.Count >= correctSequence.Count)
        {
             currentInput.Clear();
        }

        currentInput.Add(number);
        UpdateDisplay(null, FormatInput(currentInput)); 

        if (currentInput.Count == correctSequence.Count)
        {
            CheckCode();
        }
    }
    
    private void CheckCode()
    {
        if (currentInput.Count != correctSequence.Count) return;

        bool codeCorrect = true;
        
        Debug.Log($"VERIFICANDO SENHA: Tentativa: {FormatInput(currentInput)} | Correto: {FormatInput(correctSequence)}");

        // LÓGICA DE COMPARAÇÃO MANUAL
        for (int i = 0; i < correctSequence.Count; i++)
        {
            if (currentInput[i] != correctSequence[i])
            {
                codeCorrect = false;
                break;
            }
        }
        
        if (codeCorrect)
        {
            UpdateDisplay("CÓDIGO ACEITO!", null);
            SolveSafe();
        }
        else
        {
            UpdateDisplay("CÓDIGO INCORRETO.", null);
            StartCoroutine(ClearInputAfterDelay(1f));
        }
    }

    private void SolveSafe()
    {
        InventoryManager inventario = FindObjectOfType<InventoryManager>();
        if (inventario != null && rewardItem != null)
        {
            inventario.AdicionarItem(rewardItem);
            Debug.Log("Recompensa do cofre coletada: " + rewardItem.itemName);
        }
        
        if (currentContainer != null)
        {
            Destroy(currentContainer.gameObject); 
        }

        currentContainer = null;
        StartCoroutine(ClosePuzzleAfterDelay(2f));
    }
    
    // --- Métodos de Utilidade e Coroutines ---

    private string FormatInput(List<int> input)
    {
        return string.Join(" - ", input.ConvertAll(i => i.ToString()));
    }
    
    private void UpdateDisplay(string message, string input)
    {
        if (feedbackText != null && !string.IsNullOrEmpty(message))
        {
            feedbackText.text = message;
        }
        if (inputDisplayText != null && !string.IsNullOrEmpty(input))
        {
            inputDisplayText.text = input;
        }
    }
    
    private IEnumerator ClearInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentInput.Clear();
        UpdateDisplay("Insira o código.", "");
    }
    
    private IEnumerator ClosePuzzleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePuzzle();
    }
}