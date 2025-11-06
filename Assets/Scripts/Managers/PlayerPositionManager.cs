using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    // Tornamos as variáveis públicas e estáticas para fácil acesso
    public static Vector3 lastPosition; 
    public static bool hasSavedPosition; 

    void Awake()
    {
        // Se este manager já existe, destrua a duplicata.
        if (FindObjectsOfType<PlayerPositionManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        // Garante que o Manager persista
        DontDestroyOnLoad(this.gameObject);
    }

    public void SavePosition(Vector3 pos)
    {
        lastPosition = pos;
        hasSavedPosition = true;
    }

    // Você pode adicionar um método para limpar a posição salva se necessário.
    public static void ClearSavedPosition()
    {
        lastPosition = Vector3.zero;
        hasSavedPosition = false;
    }
}