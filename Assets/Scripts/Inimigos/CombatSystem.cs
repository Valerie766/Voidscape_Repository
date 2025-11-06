using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance;

    [Header("Cena / UI")]
    public CombatUIManager uiManager;            // arraste o CombatUIManager no inspector
    public int mainGameSceneIndex = 1;           // cena para voltar após combate (opcional)

    [Header("Inimigo (atribuir por cena)")]
    public EnemyData currentEnemy;               // arraste o EnemyData específico da cena

    // estado
    private int currentHP;
    private List<int> currentDialogueSequence = new List<int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        if (uiManager == null) Debug.LogError("CombatSystem: uiManager não atribuído no Inspector!");
        if (currentEnemy == null) Debug.LogError("CombatSystem: currentEnemy não atribuído na cena!");
        if (currentEnemy != null && uiManager != null) StartCombatSequence();
    }

    private void StartCombatSequence()
    {
        currentDialogueSequence.Clear();
        currentHP = currentEnemy.maxHP;

        uiManager.enemySprite.sprite = currentEnemy.visualSprite;
        uiManager.SetupUIForNewTurn(currentEnemy.initialDialogue, true);
    }

    // Chamado pelos botões (Attack)
    public void EndPlayerTurn(string trigger)
    {
        bool shouldEnemyAttack = false;
        if (trigger == "ATTACK") shouldEnemyAttack = true;
        else if (trigger == "DIALOGUE_TRIGGER") shouldEnemyAttack = true;

        if (shouldEnemyAttack)
            StartCoroutine(EnemyAttackPhase(0.8f));
        else
            StartCoroutine(WaitAndShowActionMenu(0.5f));
    }

    private IEnumerator EnemyAttackPhase(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Mensagem de ataque
        string attackText = currentEnemy.enemyName + " está atacando!";
        if (currentEnemy.attackPattern != null && currentEnemy.attackPattern.Count > 0)
        {
            // escolhe ataque aleatório para variar o texto
            attackText = currentEnemy.enemyName + " usa " + currentEnemy.attackPattern[Random.Range(0, currentEnemy.attackPattern.Count)] + "!";
        }

        uiManager.SetupUIForNewTurn(attackText, false);

        // A lógica de aplicar dano ao jogador deve ser implementada por você futuramente.
        // Aqui apenas espera um tempo e volta pro menu.
        yield return new WaitForSeconds(1.2f);
        uiManager.SetupUIForNewTurn("O que você fará a seguir?", true);
    }

    private IEnumerator WaitAndShowActionMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        uiManager.SetupUIForNewTurn("O que você fará a seguir?", true);
    }

    // ======= DIALOGO ========
    // Inicia a seleção de opções (chamado pelo UI ao clicar em CONVERSAR)
    public void StartDialogueSelection()
    {
        if (currentEnemy == null || uiManager == null) return;

        uiManager.ShowActionMenu(false);
        uiManager.dialogueText.text = "O que você dirá?";
        uiManager.ShowDialogueOptions(currentEnemy.talkOptions);
    }

    // Chamado quando o jogador clica em uma opção (index 0-based)
    public void SelectDialogueOption(int optionIndex)
    {
        if (currentEnemy == null || uiManager == null) return;

        uiManager.HideDialogueOptions();

        // registra na sequência atual
        currentDialogueSequence.Add(optionIndex);

        // mostra reação do inimigo, se houver
        if (optionIndex < currentEnemy.enemyReactions.Count)
        {
            uiManager.SetupUIForNewTurn(currentEnemy.enemyReactions[optionIndex], false);
        }

        // checa se essa opção é uma trigger de ataque
        if (currentEnemy.triggersAttackIndices != null && currentEnemy.triggersAttackIndices.Contains(optionIndex))
        {
            EndPlayerTurn("DIALOGUE_TRIGGER");
            return;
        }

        // checa vitória pacifista parcial / completa
        CheckPacifistVictory();

        // se ainda não atacou, volta ao menu após pequeno delay
        StartCoroutine(WaitAndShowActionMenu(0.8f));
    }

    private void CheckPacifistVictory()
    {
        if (currentEnemy.defeatSequence == null || currentEnemy.defeatSequence.Count == 0) return;

        // se currentDialogueSequence é maior que defeatSequence, reseta (erro)
        if (currentDialogueSequence.Count > currentEnemy.defeatSequence.Count)
        {
            currentDialogueSequence.Clear();
            uiManager.SetupUIForNewTurn("O inimigo parece confuso... sequência reiniciada!", false);
            return;
        }

        // compara os elementos já escolhidos com o começo da defeatSequence
        for (int i = 0; i < currentDialogueSequence.Count; i++)
        {
            if (currentDialogueSequence[i] != currentEnemy.defeatSequence[i])
            {
                // erro na sequência -> inimigo ataca
                currentDialogueSequence.Clear();
                uiManager.SetupUIForNewTurn("O inimigo ficou irritado com sua fala!", false);
                StartCoroutine(EnemyAttackPhase(0.6f));
                return;
            }
        }

        // se completou a sequência corretamente
        if (currentDialogueSequence.Count == currentEnemy.defeatSequence.Count)
        {
            // vitória pacifista
            uiManager.SetupUIForNewTurn(currentEnemy.pacifistVictoryDialogue, false);
            StartCoroutine(WaitAndReturnToMainScene(2.0f));
        }
    }

    private IEnumerator WaitAndReturnToMainScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        // aqui você pode executar recompensas, xp, drop etc.
        uiManager.EndCombat();
        // opcional: retornar à cena principal
        // SceneManager.LoadScene(mainGameSceneIndex);
    }
}
