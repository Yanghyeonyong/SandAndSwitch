using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player"))//태그 기준이라면...
        {
            return;
        }

        QuickSlotController quickSlot = collision.GetComponent<QuickSlotController>();

        if (quickSlot == null)
        {
            return;
        }

        bool success = quickSlot.TryPickup(_itemData);

        if (success)
        {
            Destroy(gameObject);
        }
    }
}
