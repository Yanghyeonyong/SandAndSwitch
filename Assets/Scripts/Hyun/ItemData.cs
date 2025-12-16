using UnityEngine;

public enum ItemType
{
    Consumable, Special//, Key
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public GameObject prefab;
    //public float weight;

    [Header("Stack")]
    public int maxStack;
    public bool IsStackable => maxStack > 1;

    [Header("Consumable/Bomb")]
    public float radius;
    public float delay;
    public LayerMask targetLayer;

    //[Header("Consumable/Potion")]
    //public float amount;
    //[Header("Key")]
    
    //[Header("Special")]

}
