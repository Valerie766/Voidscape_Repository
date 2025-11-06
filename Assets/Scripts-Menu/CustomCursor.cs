using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    void Start()
    {
        // Oculta o cursor padrão do sistema operacional
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Converte a posição do mouse (em pixels) para a posição do mundo (World Point)
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z - Camera.main.transform.position.z; // Mantém a profundidade (Z)
        
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // 2. Move o cursor customizado para a posição do mouse
        transform.position = worldPosition;
    }

    void OnDestroy()
    {
        // Restaura o cursor do sistema ao sair da cena
        Cursor.visible = true;
    }
}
