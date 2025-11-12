using UnityEngine;

// Versão 2D
[RequireComponent(typeof(Collider2D))]
public class TimeRestrictionArea : MonoBehaviour
{
    private TimeTravelManager manager;

    void Start()
    {
        // Encontra a instância do TimeTravelManager (anexado ao GameManager)
        manager = FindObjectOfType<TimeTravelManager>();
        
        if (manager == null)
        {
            Debug.LogError("TimeRestrictionArea: TimeTravelManager não encontrado na cena. A restrição não funcionará.");
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col == null || !col.isTrigger)
        {
             Debug.LogError($"TimeRestrictionArea em {gameObject.name}: Collider2D ausente ou não é Trigger!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && manager != null)
        {
            // 💡 ATIVA A RESTRIÇÃO NO MANAGER
            manager.SetAreaRestriction(true);
            Debug.Log("Restrição ATIVADA: Viagem no tempo bloqueada.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && manager != null)
        {
            // 💡 DESATIVA A RESTRIÇÃO NO MANAGER
            manager.SetAreaRestriction(false);
            Debug.Log("Restrição DESATIVADA: Viagem no tempo liberada.");
        }
    }
}