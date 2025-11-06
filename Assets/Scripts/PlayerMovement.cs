using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance; 
    
    [Header("Movimento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    [Header("Estamina")]
    public float maxStamina = 5f;
    private float currentStamina;
    public float staminaRegenRate = 1f;
    public float staminaDrainRate = 1f;

    [Header("UI da Estamina")]
    public Image staminaFill;
    public CanvasGroup staminaCanvasGroup;

    [Header("Áudio")]
    public AudioClip walkStepSound;
    public AudioClip timeTravelSound; 
    
    // Variáveis Privadas
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    private float moveInput;
    private bool isRunning;
    private bool isMoving;

    void Awake()
    {
        // Lógica de SINGLETON PERSISTENTE
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
            return; 
        }
    }
    
    void Start()
    {
        // 1. Inicialização de Componentes
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // 2. Configuração Inicial de Áudio
        if (audioSource != null && walkStepSound != null)
        {
            audioSource.clip = walkStepSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        // 3. Configuração Inicial de Estamina e UI
        currentStamina = maxStamina;
        if (staminaFill != null)
            staminaFill.fillAmount = 1f;
        if (staminaCanvasGroup != null)
            staminaCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (!enabled) return;
        
        moveInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        isMoving = moveInput != 0;

        // Gatilho da Viagem no Tempo
        if (Input.GetKeyDown(KeyCode.T))
        {
             if (audioSource != null && timeTravelSound != null)
             {
                 audioSource.PlayOneShot(timeTravelSound); 
             }
             
             if (FindObjectOfType<TimeTravelManager>() is TimeTravelManager ttm)
             {
                 FindObjectOfType<PlayerPositionManager>()?.SavePosition(transform.position); 
                 ttm.TravelThroughTime(); 
             }
             return;
        }

        // Lógica de Estamina
        if (isRunning && isMoving)
        {
             currentStamina -= staminaDrainRate * Time.deltaTime;
             if (currentStamina < 0) currentStamina = 0;
             if (currentStamina == 0) isRunning = false;
        }
        else
        {
             currentStamina += staminaRegenRate * Time.deltaTime;
             if (currentStamina > maxStamina) currentStamina = maxStamina;
        }

        // Lógica de UI e Áudio
        if (staminaFill != null) staminaFill.fillAmount = currentStamina / maxStamina;
        if (staminaCanvasGroup != null) staminaCanvasGroup.alpha = (currentStamina < maxStamina) ? 1f : 0f;
        if (isMoving && !audioSource.isPlaying) audioSource.Play();
        else if (!isMoving && audioSource.isPlaying) audioSource.Stop();

        // Lógica de Animação e Flip
        if (moveInput == 0) anim.Play("Idle");
        else if (isRunning) anim.Play("Run");
        else anim.Play("Walk");
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        if (!enabled) return;
        
        float speed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }
    
    /// <summary>
    /// Método a ser chamado por qualquer script (dano, inimigo) para MORTE.
    /// </summary>
    public void HandleDeath()
    {
         if (GameManager.Instance != null)
         {
             GameManager.Instance.StartDeathSequence(gameObject);
         }
         else
         {
             Debug.LogError("PM: GameManager não encontrado para lidar com a morte!");
         }
    }
}
