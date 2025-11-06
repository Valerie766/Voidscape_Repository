using UnityEngine;

[CreateAssetMenu(fileName = "Novo Item", menuName = "Inventário/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
}
