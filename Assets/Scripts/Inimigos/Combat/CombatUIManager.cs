using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance;

    [Header("Componentes do Canvas")]
    public GameObject combatRoot;            // painel raiz da UI de combate
    public TMP_Text dialogueText;            // caixa de diálogo (TextMeshPro)
    public Button attackButton;              // botão atacar
    public Button talkButton;                // botão conversar
    public Image enemySprite;                // sprite do inimigo

    [Header("Opções de Diálogo")]
    public Transform dialogueOptionsContainer; // container (ex: vertical layout) onde os botões serão instanciados
    public Button dialogueOptionPrefab;        // prefab do botão de opção (com TMP_Text filho)

    private PlayerMovement playerMovement;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        if (combatRoot != null) combatRoot.SetActive(false);

        // registra eventos dos botões principais (assumindo que no Inspector não colocaste OnClick)
        if (attackButton != null) attackButton.onClick.RemoveAllListeners();
        if (talkButton != null) talkButton.onClick.RemoveAllListeners();

        if (attackButton != null) attackButton.onClick.AddListener(OnAttackClicked);
        if (talkButton != null) talkButton.onClick.AddListener(OnTalkClicked);

        playerMovement = PlayerMovement.Instance;
    }

    void Start()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    public void SetupUIForNewTurn(string dialogue, bool showActionMenu)
    {
        if (combatRoot != null) combatRoot.SetActive(true);
        if (dialogueText != null) dialogueText.text = dialogue;

        ShowActionMenu(showActionMenu);
        HideDialogueOptions();

        if (showActionMenu && attackButton != null)
            attackButton.Select();
    }

    public void EndCombat()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            playerMovement.gameObject.SetActive(true);
        }

        if (combatRoot != null) combatRoot.SetActive(false);
    }

    public void ShowActionMenu(bool show)
    {
        if (attackButton != null) attackButton.gameObject.SetActive(show);
        if (talkButton != null) talkButton.gameObject.SetActive(show);
    }

    // ---------- diálogo dinâmico ----------
    public void ShowDialogueOptions(List<string> options)
    {
        HideDialogueOptions();

        if (dialogueOptionPrefab == null || dialogueOptionsContainer == null)
        {
            Debug.LogWarning("CombatUIManager: prefab ou container de opções não atribuídos.");
            return;
        }

        for (int i = 0; i < options.Count; i++)
        {
            string optionText = options[i];
            Button b = Instantiate(dialogueOptionPrefab, dialogueOptionsContainer);
            TMP_Text label = b.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = optionText;

            int index = i; // capture index correctly for closure
            b.onClick.AddListener(() =>
            {
                CombatSystem.Instance.SelectDialogueOption(index);
            });

            b.gameObject.SetActive(true);
        }
    }

    public void HideDialogueOptions()
    {
        if (dialogueOptionsContainer == null) return;
        foreach (Transform child in dialogueOptionsContainer) Destroy(child.gameObject);
    }

    // ---------- botões principais ----------
    private void OnAttackClicked()
    {
        // chama o fim do turno com trigger de ataque
        if (CombatSystem.Instance != null) CombatSystem.Instance.EndPlayerTurn("ATTACK");
    }

    private void OnTalkClicked()
    {
        if (CombatSystem.Instance != null) CombatSystem.Instance.StartDialogueSelection();
    }
}
