using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    // O AudioSource que tocará a música. Deve estar no mesmo GameObject.
    [SerializeField] private AudioSource audioSource;
    
    [Tooltip("Tempo em segundos para fazer o fade (suavizar) entre as músicas.")]
    public float fadeDuration = 1.5f;

    private AudioClip currentClip;
    private float targetVolume;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Garante que o Manager não seja destruído ao trocar de cena (se necessário)
            // DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        // Garante que o AudioSource está configurado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        audioSource.loop = true;
        targetVolume = audioSource.volume; // Salva o volume base
    }

    /// <summary>
    /// Inicia uma nova música com fade-in.
    /// </summary>
    public void PlayMusic(AudioClip newClip, float volume = 0.5f)
    {
        if (newClip == currentClip && audioSource.isPlaying)
        {
            return; // Já está tocando esta música, ignora.
        }

        currentClip = newClip;
        targetVolume = volume;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // Inicia a transição
        fadeCoroutine = StartCoroutine(FadeMusic(newClip, targetVolume));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float newVolume)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        // Fase 1: Fade-out da música atual
        if (audioSource.isPlaying)
        {
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                yield return null;
            }
        }

        // Fase 2: Toca a nova música
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.volume = 0f; // Começa silencioso
        audioSource.Play();

        timer = 0f;
        
        // Fase 3: Fade-in da nova música
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, newVolume, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = newVolume; // Garante o volume final
    }
}