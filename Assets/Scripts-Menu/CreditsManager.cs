using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool isPlaying = false;

    void Start()
    {
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false);
    }

    public void PlayCredits()
    {
        if (videoPlayer == null) return;

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();
        isPlaying = true;

        // Quando o vídeo terminar, voltar ao menu
        videoPlayer.loopPointReached += OnCreditsFinished;
    }

    void Update()
    {
        // Permite sair dos créditos com ESC
        if (isPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            StopCredits();
        }
    }

    void OnCreditsFinished(VideoPlayer vp)
    {
        StopCredits();
    }

    void StopCredits()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }

        isPlaying = false;
        // Retorna ao menu principal (ou apenas para o Canvas)
        SceneManager.LoadScene("MainMenu"); 
        // Caso não troque de cena, você pode só reativar o Canvas principal aqui
    }
}
