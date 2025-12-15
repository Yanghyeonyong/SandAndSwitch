using UnityEngine;

public enum ItemType
{
    Consumable, Key, Special
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public GameObject prefab;
    public float weight;

    [Header("Stack/Consume")]
    [SerializeField] private bool _isConsumable = true;
    public int maxStack = 99;
    public bool IsConsumable => _isConsumable;
    public bool IsStackable => maxStack > 1;

    [Header("Consumable/Bomb")]
    public float radius = 1.5f;
    public float delay = 2f;

    [Header("Consumable/Potion")]
    public float amount;
    //[Header("Key")]
    
    //[Header("Special")]

}
