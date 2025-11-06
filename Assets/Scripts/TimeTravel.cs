using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTravel : MonoBehaviour
{
    public string presentScene = "CidadePresente";
    public string pastScene = "CidadePassado";

    private string currentScene;

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Salva posição antes de viajar
            PlayerPositionManager.lastPosition = transform.position;
            PlayerPositionManager.hasSavedPosition = true;

            // Alterna cena
            if (currentScene == presentScene)
            {
                SceneManager.LoadScene(pastScene);
            }
            else if (currentScene == pastScene)
            {
                SceneManager.LoadScene(presentScene);
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;

        // Reposiciona se existir posição salva
        if (PlayerPositionManager.hasSavedPosition)
        {
            transform.position = PlayerPositionManager.lastPosition;
        }
    }
}


   