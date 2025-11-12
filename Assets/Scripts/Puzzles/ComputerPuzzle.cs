using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // CRÍTICO: Importar a biblioteca TMPro

public class ComputerPuzzle : MonoBehaviour
{
    public static ComputerPuzzle Instance;

    // 💡 REFERÊNCIAS ESTÁTICAS DE UI: Devem ser preenchidas no Inspector deste script.
    [Header("Componentes UI (Fixos)")]
    [Tooltip("O GameObject Root/Canvas que contém toda a UI do computador.")]
    public GameObject computerRoot;
    public GameObject fileListPanel;
    public GameObject noteDisplayPanel;
    public TextMeshProUGUI noteTextDisplay; 
    // ---------------------------------------------
    
    [Header("Arquivos e Dicas")]
    public Button[] fileButtons;
    
    [System.Serializable]
    public class FileData
    {
        public string fileName;
        [TextArea(3, 10)]
        public string fileContent; 
        public bool givesItem; 
    }
    public List<FileData> fileDataList = new List<FileData>();

    private bool puzzleActive = false;
    public bool IsActive => puzzleActive;
    // O container ainda é necessário para saber qual PC dar item
    private ComputerInteract currentContainer; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (fileButtons.Length != fileDataList.Count)
        {
            Debug.LogError("ComputerPuzzle: O número de botões de arquivos não corresponde ao número de FileData. Verifique o Inspector!");
        }

        // Configura os Listeners dos botões
        for (int i = 0; i < fileButtons.Length; i++)
        {
            int index = i; 
            fileButtons[i].onClick.AddListener(() => FileClicked(index));
        }
    }

    void Update()
    {
        if (!puzzleActive) return;

        // Lógica de fechamento: Pressionar 'W'
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 1. Se a nota está aberta, fecha a nota
            if (noteDisplayPanel != null && noteDisplayPanel.activeSelf)
            {
                CloseNote(); 
            }
            // 2. Se a lista de arquivos está aberta (e não a nota), fecha o puzzle inteiro
            else if (fileListPanel != null && fileListPanel.activeSelf) 
            {
                ClosePuzzle(); 
            }
        }
    }

    // --- MÉTODO STARTPUZZLE ORIGINAL ---
    // Apenas recebe a referência do PC que iniciou a interação
    public void StartPuzzle(ComputerInteract container)
    {
        if (puzzleActive) return;
        
        currentContainer = container; 
        
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = false;
        }

        puzzleActive = true;
        
        // 💡 ATIVA A UI USANDO OS CAMPOS ESTÁTICOS DESTE PRÓPRIO SCRIPT
        if (computerRoot != null) computerRoot.SetActive(true);
        if (fileListPanel != null) fileListPanel.SetActive(true); 
        if (noteDisplayPanel != null) noteDisplayPanel.SetActive(false); 
    }
    
    public void ClosePuzzle()
    {
        puzzleActive = false;
        
        // 💡 DESATIVA A UI USANDO OS CAMPOS ESTÁTICOS
        if (computerRoot != null)
        {
            computerRoot.SetActive(false);
        }
        
        currentContainer = null;
        
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = true;
        }
    }

    public void FileClicked(int fileIndex)
    {
        if (fileIndex < 0 || fileIndex >= fileDataList.Count) return;

        FileData data = fileDataList[fileIndex];
        
        // 1. Exibe o conteúdo da dica (usando a referência estática do componente TMPro)
        if (noteTextDisplay != null) 
        {
            noteTextDisplay.text = data.fileContent;
        }
        
        // 2. Transiciona a UI
        if (fileListPanel != null)
        {
            fileListPanel.SetActive(false); 
        }
        if (noteDisplayPanel != null)
        {
            noteDisplayPanel.SetActive(true); 
        }

        // 3. Verifica se este arquivo concede o item
        if (data.givesItem)
        {
            GiveItemAndComplete(data.fileName);
        }
    }
    
    public void CloseNote()
    {
        if (noteDisplayPanel != null)
        {
            noteDisplayPanel.SetActive(false);
        }
        if (fileListPanel != null)
        {
            fileListPanel.SetActive(true); 
        }
    }

    private void GiveItemAndComplete(string fileName)
    {
        Debug.Log($"Arquivo '{fileName}' encontrado. Dando item ao jogador e fechando puzzle.");
        
        InventoryManager inventario = FindObjectOfType<InventoryManager>();
        
        // Verifica se o PC TEM um item para dar
        if (inventario != null && currentContainer != null && currentContainer.itemDentro != null)
        {
            inventario.AdicionarItem(currentContainer.itemDentro);
        }
        
        // Fecha o Puzzle
        ClosePuzzle(); 

        // Desabilita o script de interação
        if (currentContainer != null)
        {
            currentContainer.enabled = false;
        }
    }
}