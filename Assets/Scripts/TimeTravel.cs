using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTraveler : MonoBehaviour
{
    [Header("Configuração de Viagem no Tempo")]
    [Tooltip("A tecla que ativa a viagem no tempo.")]
    public KeyCode timeTravelKey = KeyCode.T;
    
    [Tooltip("O nome da cena para onde o jogador viaja no tempo.")]
    public string targetSceneName = "FutureScene";

    [Header("Restrição de Área")]
    [Tooltip("Define se a viagem no tempo está bloqueada por uma TimeRestrictionArea.")]
    public bool isTimeTravelRestricted = false;

    void Update()
    {
        // Se a tecla de viagem for pressionada...
        if (Input.GetKeyDown(timeTravelKey))
        {
            // ...e NÃO estiver restrito, execute a viagem.
            if (!isTimeTravelRestricted)
            {
                TravelToNewTime();
            }
            else
            {
                Debug.LogWarning("Viagem no Tempo Bloqueada: O jogador está em uma área de restrição!");
                // Opcional: Adicionar feedback visual ou sonoro de falha aqui.
            }
        }
    }

    private void TravelToNewTime()
    {
        // Verificação básica para evitar erros
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("TimeTraveler: O nome da cena alvo está vazio. Não é possível viajar.");
            return;
        }

        Debug.Log($"Viajando no tempo para a cena: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }
}