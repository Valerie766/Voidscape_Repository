using UnityEngine;
using System.Collections.Generic;

public static class GameResetter
{
    public static void ResetGameData()
    {
        UnityEngine.Debug.Log("[RESETTER] Iniciando limpeza de dados estáticos e estados.");
        
        // 1. LIMPEZA DO INVENTÁRIO (Chama o método no Singleton persistente)
        // Esta chamada é segura e não afeta as cutscenes, mas limpa a UI e os dados.
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearInventoryData();
        }
        else
        {
            UnityEngine.Debug.LogWarning("[RESETTER] InventoryManager.Instance é NULL. Inventário não limpo. Verifique a ordem de inicialização (Awake).");
        }
        
        // =========================================================================
        // 🚨 2. LIMPEZA DA FLAG DE CUTSCENE (SOLUÇÃO PARA A FALHA DE REPRODUÇÃO)
        // =========================================================================
        
        // Você DEVE identificar e forçar a variável de controle da cutscene final
        // para o estado inicial (Geralmente 'false' ou '0').
        
        // EXEMPLO 1 (Se você usa um script estático para estado de jogo):
        // GlobalGameStatus.IsFinalCutscenePlayed = false; 
        
        // EXEMPLO 2 (Se você usa um Manager de Diálogo persistente):
        // DialogueManager.Instance.HasCompletedFinalSequence = false;
        
        // EXEMPLO 3 (Se você usa um objeto estático para controle):
        // FinalTrigger.CanBeActivated = true; 

        // -------------------------------------------------------------------------
        // ADICIONE A LINHA DE CÓDIGO DO SEU PROJETO AQUI.
        // -------------------------------------------------------------------------

        // 3. LIMPEZA DE OUTROS ESTADOS (Opcional, mas recomendado)
        // Ex: PlayerPrefs.DeleteKey("PuzzleCompleted_X");

        // Garante que todas as alterações de PlayerPrefs sejam salvas
        PlayerPrefs.Save(); 
        
        UnityEngine.Debug.Log("[RESETTER] Limpeza de dados concluída. Verifique a flag de cutscene.");
    }
}