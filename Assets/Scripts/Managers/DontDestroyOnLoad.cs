using UnityEngine;

/// <summary>
/// Anexado a um GameObject, faz com que ele não seja destruído
/// quando novas cenas são carregadas.
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        // Verifica se já existe um objeto com o mesmo nome e do mesmo tipo.
        // Isso impede a duplicação se você voltar para a cena inicial.
        
        // CRÍTICO: Se você já tem um Singleton Pattern em outro lugar (como no GameManager),
        // e ele já está chamando DontDestroyOnLoad, este script pode ser simplificado.
        
        // No entanto, para um objeto Canvas, é mais seguro implementar uma verificação de duplicatas.
        
        GameObject[] objs = GameObject.FindGameObjectsWithTag(gameObject.tag);
        
        // Se este objeto tiver a tag 'Inventory' ou 'UI' (se você configurou), 
        // e já houver outro objeto com a mesma tag, destrua o novo.
        if (objs.Length > 1 && objs[0] != gameObject)
        {
            // Assume que o primeiro encontrado é o correto e destrói esta duplicata
            Destroy(gameObject);
        }
        else
        {
            // Se for o primeiro, marque-o para não ser destruído
            DontDestroyOnLoad(gameObject);
        }
        
        // Dica: Certifique-se de que o seu Canvas de Inventário tenha uma Tag única (ex: "InventoryUI")
        // e configure essa Tag no Inspector.
    }
}