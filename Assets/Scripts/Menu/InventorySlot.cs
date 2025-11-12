using UnityEngine;
using UnityEngine.UI;
using TMPro; // Usando TMPro no topo para evitar erro de referência

public class InventorySlot : MonoBehaviour
{
    public ItemData itemData; // ScriptableObject do item
    public Image icon;        // Ícone do item
    public TMPro.TMP_Text itemName; // Nome do item (opcional)

    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClick);
        }

        // Atualiza visualmente o slot se o item estiver definido
        if (itemData != null)
        {
            UpdateSlotUI();
        }
    }

    void UpdateSlotUI()
    {
        if (icon != null && itemData != null)
        {
            icon.sprite = itemData.icon;
            icon.enabled = true;
        }

        if (itemName != null && itemData != null)
        {
            itemName.text = itemData.itemName;
        }
    }

    void OnClick()
    {
        // 🚨 CORREÇÃO: Usar a referência estática (Singleton) em vez de FindObjectOfType
        InventoryManager inv = InventoryManager.Instance;
        
        // Verifica se o InventoryManager está inicializado (Instance não é nulo)
        if (inv == null) 
        {
            Debug.LogError("InventoryManager não está acessível. O slot não pode interagir.");
            return;
        }

        // Se não houver item vinculado, não faz nada
        if (itemData == null) return;

        // ✅ Verifica se o jogador realmente possui o item no inventário
        if (inv.HasItem(itemData))
        {
            inv.MostrarItem(itemData);
        }
        else
        {
            Debug.Log("O jogador não possui este item no inventário!");
        }
    }
}