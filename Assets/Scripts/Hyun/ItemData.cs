using UnityEngine;

//임시로 제외 대상 주석처리
public enum ItemType
{
    Consumable = 1, Special = 2//, Key
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public ItemType type;
    public GameObject prefab;
    public Sprite icon;
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
