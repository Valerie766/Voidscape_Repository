using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // CRÍTICO: Importar a biblioteca TMPro

public class ComputerPuzzle : MonoBehaviour
{
    public static ComputerPuzzle Instance;

    // --- VARIÁVEIS DE CACHE DE UI DO PC ATUAL ---
    // Estas variáveis armazenam as referências que o ComputerInteract ativo nos passa.
    private GameObject currentRoot;
    private GameObject currentFileListPanel;
    private GameObject currentNoteDisplayPanel;
    
    // 💡 CORRIGIDO: Esta variável DEVE ser do tipo TextMeshProUGUI
    private TextMeshProUGUI currentNoteText; 
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
            if (currentNoteDisplayPanel != null && currentNoteDisplayPanel.activeSelf)
            {
                CloseNote(); 
            }
            // 2. Se a lista de arquivos está aberta (e não a nota), fecha o puzzle inteiro
            else if (currentFileListPanel != null && currentFileListPanel.activeSelf) 
            {
                ClosePuzzle(); 
            }
        }
    }

    // --- MÉTODO STARTPUZZLE ATUALIZADO ---
    // Assinatura correta: O último parâmetro é TextMeshProUGUI
    public void StartPuzzle(ComputerInteract container, GameObject root, GameObject fileList, GameObject noteDisplay, TextMeshProUGUI noteText)
    {
        if (puzzleActive) return;
        
        currentContainer = container; 
        
        // Armazena as referências do PC que chamou
        currentRoot = root;
        currentFileListPanel = fileList;
        currentNoteDisplayPanel = noteDisplay;
        currentNoteText = noteText; // Armazenando a referência do componente TMPro

        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = false;
        }

        puzzleActive = true;
        
        // Ativa a UI usando as referências cacheada
        currentRoot.SetActive(true);
        currentFileListPanel.SetActive(true); 
        currentNoteDisplayPanel.SetActive(false); 
    }
    
    public void ClosePuzzle()
    {
        puzzleActive = false;
        
        // Usa a referência cacheada para desativar
        if (currentRoot != null)
        {
            currentRoot.SetActive(false);
        }
        
        currentContainer = null;
        
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.enabled = true;
        }
        
        // Limpa as referências cache para evitar erros com o próximo PC
        currentRoot = currentFileListPanel = currentNoteDisplayPanel = null;
        currentNoteText = null;
    }

    public void FileClicked(int fileIndex)
    {
        if (fileIndex < 0 || fileIndex >= fileDataList.Count) return;

        FileData data = fileDataList[fileIndex];
        
        // 1. Exibe o conteúdo da dica (usando a referência cacheada do componente TMPro)
        if (currentNoteText != null) 
        {
            currentNoteText.text = data.fileContent;
        }
        
        // 2. Transiciona a UI
        if (currentFileListPanel != null)
        {
            currentFileListPanel.SetActive(false); 
        }
        if (currentNoteDisplayPanel != null)
        {
            currentNoteDisplayPanel.SetActive(true); 
        }

        // 3. Verifica se este arquivo concede o item
        if (data.givesItem)
        {
            GiveItemAndComplete(data.fileName);
        }
    }
    
    public void CloseNote()
    {
        if (currentNoteDisplayPanel != null)
        {
            currentNoteDisplayPanel.SetActive(false);
        }
        if (currentFileListPanel != null)
        {
            currentFileListPanel.SetActive(true); 
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