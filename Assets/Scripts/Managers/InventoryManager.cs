using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // <-- Importante! Necessário para TextMeshProUGUI

public class InventoryManager : MonoBehaviour
{
    [Header("UI do Inventário")]
    public GameObject menuInventario;        // Painel principal do inventário
    public Image[] slotIcons;                // Ícones dos slots do inventário
    public Image itemPreviewImage;           // Imagem grande do item selecionado
    public TextMeshProUGUI itemDescriptionText; // Descrição (TextMeshProUGUI)

    [Header("Configurações de Tecla")]
    public KeyCode inventoryKey = KeyCode.E; // Tecla para abrir/fechar inventário

    private List<ItemData> itens = new List<ItemData>();
    private bool inventarioAtivo = false;

    void Start()
    {
        if (menuInventario != null)
            menuInventario.SetActive(false);

        AtualizarUI();
    }

    void Update()
    {
        // Alterna o inventário com a tecla configurada (E)
        if (Input.GetKeyDown(inventoryKey))
        {
            inventarioAtivo = !inventarioAtivo;
            if (menuInventario != null)
                menuInventario.SetActive(inventarioAtivo);
        }
    }

    // Adiciona um novo item ao inventário
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
    // ---------- Adicione no seu InventoryManager.cs ----------

/// <summary>
/// Verifica se o inventário contém um item com o mesmo nome.
/// Útil se você só tiver o nome da chave em LockedDoor.requiredItemName.
/// </summary>
public bool HasItem(string itemName)
{
    if (string.IsNullOrEmpty(itemName)) return false;

    // Se o seu inventário usa ItemData (ScriptableObject), adapte o campo conforme o nome real.
    foreach (var it in itens) // 'itens' é a lista interna de ItemData no InventoryManager
    {
        if (it != null && it.itemName == itemName)
            return true;
    }
    return false;
}

/// <summary>
/// Verifica se o inventário contém exatamente a referência ao ItemData passado.
/// </summary>
public bool HasItem(ItemData itemData)
{
    if (itemData == null) return false;
    return itens.Contains(itemData);
}

/// <summary>
/// Remove a primeira ocorrência de um item com o mesmo nome.
/// Retorna true se removeu com sucesso.
/// </summary>
public bool RemoveItem(string itemName)
{
    if (string.IsNullOrEmpty(itemName)) return false;

    for (int i = 0; i < itens.Count; i++)
    {
        if (itens[i] != null && itens[i].itemName == itemName)
        {
            itens.RemoveAt(i);
            AtualizarUI(); // atualiza a UI para refletir a remoção
            return true;
        }
    }
    return false;
}

/// <summary>
/// Remove a primeira ocorrência da referência ItemData.
/// Retorna true se removeu com sucesso.
/// </summary>
public bool RemoveItem(ItemData itemData)
{
    if (itemData == null) return false;
    bool removed = itens.Remove(itemData);
    if (removed) AtualizarUI();
    return removed;
}


    // Atualiza a UI dos slots
    private void AtualizarUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image icon = slotIcons[i];
            Button btn = icon.GetComponent<Button>(); // botão no mesmo objeto do ícone

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
            itemDescriptionText.text = item.description; // <-- Exibe o texto do item
        }
    }
}