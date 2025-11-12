using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    // CUIDADO: Este é um Singleton de CENAS. 
    // Garante que só haja um ponto de respawn principal por cena.
    public static RespawnPoint Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // NÃO USAR DONTDESTROYONLOAD AQUI. Queremos que ele seja destruído quando a cena é descarregada.
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // Método para ser chamado pelo GameManager para obter a posição
    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }
}