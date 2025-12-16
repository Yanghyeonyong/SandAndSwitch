using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;

    private bool _canPickup;
    private QuickSlotController _cachedSlot;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        _cachedSlot = collision.GetComponent<QuickSlotController>();
        _canPickup = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        _cachedSlot = null;
        _canPickup = false;
    }

    private void Update()
    {
        if (!_canPickup)
        {
            return;
        }

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    if (_cachedSlot != null && _cachedSlot.TryPickup(_itemData))
        //    {
        //        Destroy(gameObject);
        //    }
        //}
    }
}
