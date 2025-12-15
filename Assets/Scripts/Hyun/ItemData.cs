using UnityEngine;

public enum ItemType
{
    Bomb, Shovel, Potion
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public ItemType type;
    public int quantity;

    [Header("Bomb")]
    public float radius;
    public float delay;

    [Header("Potion")]
    public float amount;

}
