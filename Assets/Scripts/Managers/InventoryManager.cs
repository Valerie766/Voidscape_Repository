using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // === Singleton Instance ===
    public static InventoryManager Instance { get; private set; }
    
    [Header("UI do Inventário")]
    public GameObject menuInventario;
    public Image[] slotIcons;
    public Image itemPreviewImage;
    public TextMeshProUGUI itemDescriptionText;

    [Header("Configurações de Tecla")]
    public KeyCode inventoryKey = KeyCode.E;

    private List<ItemData> itens = new List<ItemData>();
    private bool inventarioAtivo = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Garante a persistência se o objeto for carregado entre cenas
            // DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (menuInventario != null)
            menuInventario.SetActive(false);

        AtualizarUI(); 
        
        // NOVO: Limpa o painel de preview ao iniciar (caso tenha sobrado lixo de memória)
        ClearPreviewPanel();
    }

    void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            inventarioAtivo = !inventarioAtivo;
            if (menuInventario != null)
                menuInventario.SetActive(inventarioAtivo);
        }
    }

    // =======================================================
    // MÉTODO DE LIMPEZA DO INVENTÁRIO (PARA HARD RESET)
    // =======================================================
    public void ClearInventoryData()
    {
        itens.Clear();
        
        // 🚨 CORREÇÃO PRINCIPAL: Limpar o painel de preview imediatamente
        ClearPreviewPanel();
        
        AtualizarUI();
        UnityEngine.Debug.Log("[INVENTORY] Inventário e UI de preview limpos.");
    }

    // Método auxiliar para limpar o painel de descrição e preview
    private void ClearPreviewPanel()
    {
        if (itemPreviewImage != null)
        {
            itemPreviewImage.sprite = null;
            // Torna o ícone transparente para limpar visualmente o painel
            itemPreviewImage.color = new Color(1f, 1f, 1f, 0f); 
        }
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = ""; // Limpa o texto
        }
    }

    // =======================================================
    // MÉTODOS DE AÇÃO (Adicionar/Verificar/Remover)
    // =======================================================

    public void AdicionarItem(ItemData novoItem)
    {
        if (itens.Count < slotIcons.Length)
        {
            itens.Add(novoItem);
            AtualizarUI();
        }
        else
        {
            Debug.Log("Inventário cheio!");
        }
    }

    public bool HasItem(ItemData itemData)
    {
        if (itemData == null) return false;
        return itens.Contains(itemData);
    }
    
    public bool HasItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return false;
        foreach (var it in itens)
        {
            if (it != null && it.itemName == itemName) 
                return true;
        }
        return false;
    }

    public bool RemoveItem(ItemData itemData)
    {
        if (itemData == null) return false;
        bool removed = itens.Remove(itemData);
        if (removed) AtualizarUI();
        return removed;
    }

    public bool RemoveItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return false;

        for (int i = 0; i < itens.Count; i++)
        {
            if (itens[i] != null && itens[i].itemName == itemName)
            {
                itens.RemoveAt(i);
                AtualizarUI();
                return true;
            }
        }
        return false;
    }

    // Atualiza a UI dos slots
    private void AtualizarUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image icon = slotIcons[i];
            Button btn = icon.GetComponent<Button>();

            if (i < itens.Count && itens[i] != null)
            {
                icon.sprite = itens[i].icon;
                icon.color = Color.white;

                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    int index = i;
                    btn.onClick.AddListener(() => MostrarItem(itens[index]));
                }
            }
            else
            {
                icon.sprite = null;
                icon.color = new Color(1f, 1f, 1f, 0f);

                if (btn != null)
                    btn.onClick.RemoveAllListeners();
            }
        }
    }

    // Mostra o item selecionado no painel da direita
    public void MostrarItem(ItemData item)
    {
        if (itemPreviewImage != null)
        {
            itemPreviewImage.sprite = item.icon;
            itemPreviewImage.color = Color.white;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = item.description;
        }
    }
}