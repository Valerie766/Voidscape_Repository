using UnityEngine;

public class CutsceneCanvasPersist : MonoBehaviour
{
    void Awake()
    {
        // Garante que este Canvas (e todos os seus filhos, como os VideoPlayers)
        // não seja destruído ao carregar uma nova cena.
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("Cutscene Canvas configurado para persistir entre cenas.");
    }
}