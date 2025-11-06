using UnityEngine;

// O script funciona perfeitamente quando anexado ao seu PLAYER_SPAWN_POINT
public class SpawnGizmo : MonoBehaviour
{
    // Configure a cor e o tamanho no Inspector
    public Color spawnColor = Color.cyan;
    public float size = 1f;

    // Esta função é chamada apenas no Editor
    private void OnDrawGizmos()
    {
        // 1. Define a cor
        Gizmos.color = spawnColor;

        // 2. Desenha uma "Caixa" para marcar o corpo do Player
        // Nota: A função DrawWireCube desenha apenas o contorno
        Gizmos.DrawWireCube(transform.position, new Vector3(size, size * 2, 0.1f));

        // 3. Desenha uma Seta para indicar a direção inicial
        Gizmos.color = Color.red;
        
        // Desenha uma seta simples (um raio) na direção X (frente)
        Vector3 direction = transform.right * size;
        Gizmos.DrawRay(transform.position, direction);
        
        // Desenha uma pequena esfera na ponta da seta
        Gizmos.DrawSphere(transform.position + direction, size * 0.15f);
    }
}
