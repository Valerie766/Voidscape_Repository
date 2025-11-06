using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LockedDoor : MonoBehaviour
{
    [Header("Configuração da Porta")]
    public Transform destinationPoint;
    public string requiredItemName; // Nome exato do item que o jogador precisa
    public AudioClip lockedDoorSound; // Som quando tenta abrir trancada
    public AudioClip openDoorSound;   // Som quando abre com sucesso

    private bool playerIsNear = false;
    private GameObject playerObject;
    private AudioSource audioSource;
    private bool destinationLinked = false;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (destinationPoint != null)
            destinationLinked = true;
        else
            Debug.LogError("ERRO: Porta " + gameObject.name + " sem destino configurado!");
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.W))
        {
            TryOpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            playerObject = null;
        }
    }

    private void TryOpenDoor()
    {
        if (!destinationLinked)
            return;

        InventoryManager inventory = FindObjectOfType<InventoryManager>();

        if (inventory == null)
        {
            Debug.LogWarning("InventoryManager não encontrado na cena!");
            return;
        }

        // Verifica se o jogador tem o item necessário
        bool hasKey = inventory.HasItem(requiredItemName);

        if (hasKey)
        {
            // Som de porta abrindo
            if (openDoorSound != null)
                audioSource.PlayOneShot(openDoorSound);

            TeleportPlayer();
        }
        else

        {
            // Som de porta trancada
            if (lockedDoorSound != null)
                audioSource.PlayOneShot(lockedDoorSound);

            Debug.Log("A porta está trancada. Você precisa do item: " + requiredItemName);
        }
    }

    private void TeleportPlayer()
    {
        if (playerObject != null && destinationPoint != null)
        {
            playerObject.transform.position = destinationPoint.position;

            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            if (CameraController.Instance != null)
                CameraController.Instance.SnapToTarget();
        }
    }
}
