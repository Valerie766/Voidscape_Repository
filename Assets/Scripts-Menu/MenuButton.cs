using UnityEngine;

public class MenuButton : MonoBehaviour
{
    // Ação que este botão irá executar
    public enum ButtonAction { StartGame }
    public ButtonAction actionType;

    // Referência para o Gerenciador
    private MainMenuManager manager;
    private SpriteRenderer spriteRenderer;

    // Cores para o efeito de HOVER
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    
    void Start()
    {
        manager = FindObjectOfType<MainMenuManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }
    
    // Chamado quando o cursor customizado ENTRA na área do Collider 2D
    void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor;
        }
    }

    // Chamado quando o cursor customizado SAI da área do Collider 2D
    void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    // Chamado quando o botão do mouse é pressionado
    void OnMouseDown()
    {
        if (manager == null)
        {
            Debug.LogError("MenuManager não encontrado!");
            return;
        }
        
        // Executa a ação
        switch (actionType)
        {
            case ButtonAction.StartGame:
                manager.StartNewGame();
                break;
            
        }
    }
}
