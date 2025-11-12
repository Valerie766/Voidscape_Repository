using UnityEngine;

// Classe estática: Não precisa de GameObject para funcionar e é acessível de qualquer lugar.
public static class CombatDataTransfer
{
    private static string enemyToFight = ""; 

    public static void SetEnemyToFight(string enemyName)
    {
        enemyToFight = enemyName;
        Debug.Log("CDT: Inimigo a ser combatido definido como: " + enemyToFight);
    }

    public static string GetEnemyToFight()
    {
        return enemyToFight;
    }

    public static void ClearData()
    {
        enemyToFight = "";
    }
}