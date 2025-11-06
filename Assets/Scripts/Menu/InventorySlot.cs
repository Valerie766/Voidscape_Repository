using UnityEngine;
using UnityEngine.UI;

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
        // Verifica se há InventoryManager na cena
        InventoryManager inv = FindObjectOfType<InventoryManager>();
        if (inv == null) return;

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
