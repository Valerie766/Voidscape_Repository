using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Combate/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identificação")]
    public string enemyID; // opcional, só se quiser
    public string enemyName = "Inimigo Desconhecido";
    public Sprite visualSprite;

    [Header("Estatísticas Base")]
    public int maxHP = 10;
    public int attackValue = 2;

    [Header("Diálogo / Mecânica de Paciência")]
    [TextArea(2,4)]
    public string initialDialogue = "Um ser espreita pelas sombras.";

    [Tooltip("Opções que o jogador pode escolher (na ordem).")]
    public List<string> talkOptions = new List<string> {
        "Perguntar o nome",
        "Fazer um elogio",
        "Tentar contar uma piada"
    };

    [Tooltip("Respostas do inimigo correspondentes às talkOptions (mesma ordem).")]
    [TextArea(1,4)]
    public List<string> enemyReactions = new List<string> {
        "Eu sou um vulto.",
        "Fico lisonjeado.",
        "Não entendo piadas."
    };

    [Header("Comportamento por Diálogo")]
    [Tooltip("Índices (0-based) de talkOptions que fazem o inimigo atacar imediatamente.")]
    public List<int> triggersAttackIndices = new List<int>();

    [Tooltip("Sequência de índices que, quando o jogador fala em ordem, causa a vitória pacifista.")]
    public List<int> defeatSequence = new List<int>(); // e.g. {1,0,1}

    [TextArea(2,4)]
    public string pacifistVictoryDialogue = "O inimigo se acalma e some em paz.";

    [TextArea(2,4)]
    public string attackVictoryDialogue = "O inimigo foi derrotado pela força.";

    [Header("Padrão de Ataque (texto/descritivo)")]
    public List<string> attackPattern = new List<string>();
}
