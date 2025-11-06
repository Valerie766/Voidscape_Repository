using UnityEngine;
using UnityEngine.SceneManagement; 

public class CameraController : MonoBehaviour
{
    // === Propriedades Estáticas (Singleton) ===
    public static CameraController Instance;

    // === Variáveis Configuráveis no Inspector ===
    [Tooltip("O alvo (Player) que a câmera deve seguir.")]
    [SerializeField] private Transform target; 
    
    [Tooltip("Velocidade de suavização.")]
    public float smoothSpeed = 5f; 

    // === Variáveis de Limites Dinâmicos ===
    [HideInInspector] public Vector2 minBounds;
    [HideInInspector] public Vector2 maxBounds;

    // Variáveis Internas para Cálculo de Limites
    private float cameraHalfHeight;
    private float cameraHalfWidth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Se você não quer que a câmera persista, remova o DontDestroyOnLoad do GM para ela.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 1. Inicializa as dimensões da câmera
        InitializeCameraBounds();
        FindPlayerTarget();
        
        // 2. Define limites de segurança (fallback)
        if (minBounds == Vector2.zero && maxBounds == Vector2.zero)
        {
            minBounds = new Vector2(-1000f, -1000f);
            maxBounds = new Vector2(1000f, 1000f);
        }
        // O alinhamento será feito pelo evento OnSceneLoaded ou pelo GM.
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 💡 Ação Pós-Carregamento: Usa HardResetAndSnap com limites seguros
        FindPlayerTarget();
        if (target != null)
        {
            Vector2 safeMin = new Vector2(-1000f, -1000f);
            Vector2 safeMax = new Vector2(1000f, 1000f);
            HardResetAndSnap(target, safeMin, safeMax);
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }
        
        // 1. Posição Desejada
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // 2. Aplica o CLAMP
        Vector3 desiredPosition = CalculateClampedPosition(targetPosition);
        
        // 3. Aplica a Suavização
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            smoothSpeed * Time.deltaTime
        );
        
        transform.position = smoothedPosition;
    }

    // ==========================================================
    //                        MÉTODOS PRIVADOS DE CÁLCULO
    // ==========================================================
    
    private void FindPlayerTarget()
    {
        if (target != null) return;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("Câmera encontrou o novo alvo: " + target.name);
        }
    }

    private void InitializeCameraBounds()
    {
        if (Camera.main != null)
        {
            cameraHalfHeight = Camera.main.orthographicSize;
            cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;
        }
        else
        {
             Debug.LogError("Câmera principal não encontrada! Verifique a tag 'MainCamera'.");
        }
    }

    private Vector3 CalculateClampedPosition(Vector3 positionToFollow)
    {
        if (cameraHalfWidth == 0 || cameraHalfHeight == 0) return positionToFollow; 
        
        float clampedX = Mathf.Clamp(
            positionToFollow.x, 
            minBounds.x + cameraHalfWidth, 
            maxBounds.x - cameraHalfWidth
        );

        float clampedY = Mathf.Clamp(
            positionToFollow.y, 
            minBounds.y + cameraHalfHeight, 
            maxBounds.y - cameraHalfHeight
        );
        
        return new Vector3(clampedX, clampedY, transform.position.z);
    }

    // ==========================================================
    //                        MÉTODOS PÚBLICOS
    // ==========================================================

    /// <summary>
    /// Define a nova Transform de alvo para a câmera.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// Força a câmera a se mover IMEDIATAMENTE para o alvo.
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }
        
        Vector3 snappedPosition = CalculateClampedPosition(target.position);
        transform.position = snappedPosition;
    }

    /// <summary>
    /// 💡 NOVO: Reseta forçadamente os limites e se alinha ao alvo.
    /// Chamado pelo GameManager após o respawn.
    /// </summary>
    public void HardResetAndSnap(Transform newTarget, Vector2 defaultMinBounds, Vector2 defaultMaxBounds)
    {
        target = newTarget;
        minBounds = defaultMinBounds;
        maxBounds = defaultMaxBounds;
        InitializeCameraBounds(); // Re-calcula as dimensões
        
        SnapToTarget(); // Alinha a câmera usando os novos limites (o SnapToTarget está atualizado)
    }
    
    /// <summary>
    /// Define os novos limites para a sala atual.
    /// </summary>
    public void SetNewBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBounds = newMinBounds;
        maxBounds = newMaxBounds;
        
        SnapToTarget(); 
    }
}