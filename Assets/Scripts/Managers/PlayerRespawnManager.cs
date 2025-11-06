using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawnManager : MonoBehaviour
{
    private void OnEnable()
    {
        // Garante que o método será chamado quando a cena mudar
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Força o player a ficar ativo
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // Opcional: reposiciona o player no spawn point da cena 1
        if (scene.name == "Scene1")
        {
            Transform spawn = GameObject.FindWithTag("Respawn")?.transform;
            if (spawn != null)
            {
                transform.position = spawn.position;
            }
        }
    }
}
