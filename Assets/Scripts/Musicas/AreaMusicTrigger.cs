using UnityEngine;

// Requer um Collider2D no objeto para detectar a entrada
[RequireComponent(typeof(Collider2D))]
public class AreaMusicTrigger : MonoBehaviour
{
    [Tooltip("A música que deve tocar quando o jogador entrar nesta área.")]
    public AudioClip areaMusic;

    [Tooltip("Volume desejado para esta música (0.0 a 1.0).")]
    [Range(0f, 1f)]
    public float targetVolume = 0.5f;

    void Awake()
    {
        // Garante que o Collider2D seja um Trigger
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
             coll.isTrigger = true;
        }
    }

    // 🚨 ATENÇÃO: Mudança para o método de detecção de colisão 2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou é o Player
        // Lembre-se: o GameObject do seu Player deve ter a tag "Player"
        if (other.CompareTag("Player"))
        {
            if (MusicManager.Instance != null)
            {
                // Inicia a música da nova área
                MusicManager.Instance.PlayMusic(areaMusic, targetVolume);
                Debug.Log($"[MÚSICA] Player entrou na área {gameObject.name} (2D). Tocando: {areaMusic.name}");
            }
        }
    }
}