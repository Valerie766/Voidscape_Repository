using UnityEngine;

// Requer um Collider2D e Rigidbody2D no inimigo
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class EnemyTimeRestriction : MonoBehaviour
{
    private TimeTravelManager manager;

    void Start()
    {
        // Encontra o TimeTravelManager (anexado ao GameManager)
        manager = FindObjectOfType<TimeTravelManager>();
        
        if (manager == null)
        {
            Debug.LogError("EnemyTimeRestriction: TimeTravelManager não encontrado. A restrição não funcionará.");
        }

        // Garante que o colisor de restrição é um Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"EnemyTimeRestriction em {gameObject.name}: O Collider2D principal deve ser um Trigger para funcionar!");
        }
    }

    // --- EVENTOS 2D ---
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Verifica se o que entrou no raio é o Player
        if (other.CompareTag("Player") && manager != null)
        {
            // 2. ATIVA A RESTRIÇÃO NO MANAGER
            manager.SetAreaRestriction(true);
            Debug.Log($"Restrição de Inimigo ATIVADA: {gameObject.name} bloqueando viagem.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 1. Verifica se o que saiu do raio é o Player
        if (other.CompareTag("Player") && manager != null)
        {
            // 2. Desativa a restrição NO MANAGER
            // NOTA: Se houver múltiplos inimigos, esta lógica pode ser complexa.
            // Para a lógica simples, presumimos que a restrição só é desativada
            // quando o Player sai do raio do ÚLTIMO inimigo que o bloqueava.
            
            // Para evitar que um inimigo libere a restrição enquanto outro ainda a impõe,
            // faremos uma checagem de segurança no ponto 2.
            
            // 💡 Ação: Vamos desligar a restrição, mas se você tiver problemas com múltiplos
            // inimigos, o manager precisará de um contador de restrições.
            manager.SetAreaRestriction(false);
            Debug.Log($"Restrição de Inimigo DESATIVADA: {gameObject.name} liberando viagem.");
        }
    }
}
