using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuração de Câmera")]
    public float smoothSpeed = 0.125f;
    
    // A referência ao nosso Player (que é persistente)
    private Transform target; 
    
    // O ponto de deslocamento (offset) da câmera
    public Vector3 offset = new Vector3(0f, 0f, -10f); 

    void Start()
    {
        // Garante que a câmera seja persistente junto com o Player
        DontDestroyOnLoad(gameObject);
        
        // Tenta encontrar o Player no início da cena
        FindTargetPlayer();
    }

    void FindTargetPlayer()
    {
        // Encontra o Player pela Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("Câmera encontrou o Player.");
        }
    }

    // Usamos LateUpdate para garantir que a câmera se mova DEPOIS que o Player se moveu.
    void LateUpdate()
    {
        // CRÍTICO: Se o alvo for nulo, tenta encontrá-lo novamente (acontece após load de cena)
        if (target == null)
        {
            FindTargetPlayer();
        }

        // Se o alvo for válido, move a câmera
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            
            // Suaviza o movimento
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}