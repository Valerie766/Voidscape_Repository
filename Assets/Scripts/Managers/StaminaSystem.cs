using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [Header("Configuração da Estamina")]
    public float maxStamina = 5f;          // máximo de estamina
    public float drainRate = 1f;           // quanto gasta por segundo
    public float regenRate = 1.5f;         // quanto regenera por segundo

    [Header("Referências UI")]
    public Image staminaFill;              // a imagem "Fill"
    public CanvasGroup staminaCanvasGroup; // controla visibilidade

    private float currentStamina;
    private bool isDraining;

    void Start()
    {
        currentStamina = maxStamina;

        if (staminaFill != null)
            staminaFill.fillAmount = 1f;

        if (staminaCanvasGroup != null)
            staminaCanvasGroup.alpha = 0f; // começa invisível
    }

    void Update()
    {
        // Consome estamina se o player estiver correndo
        if (isDraining)
        {
            currentStamina -= drainRate * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
        }
        else
        {
            currentStamina += regenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }

        // Atualiza barra
        if (staminaFill != null)
            staminaFill.fillAmount = currentStamina / maxStamina;

        // Só mostra a barra se não estiver cheia
        if (staminaCanvasGroup != null)
            staminaCanvasGroup.alpha = (currentStamina < maxStamina) ? 1f : 0f;
    }

    // Chamado pelo PlayerMovement quando corre
    public void SetDraining(bool draining)
    {
        isDraining = draining;
    }

    // Player pode verificar se ainda tem estamina
    public bool HasStamina()
    {
        return currentStamina > 0;
    }
}

