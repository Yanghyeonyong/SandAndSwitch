using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private float _soundValue = 1f;

    public ItemData ItemData => _itemData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.SetNearbyItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.ClearNearbyItem(this);
        }
    }

    public void Pickup()
    {
        if (_itemData.pickupSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(_itemData.pickupSoundClip, transform.position, _soundValue);
        }
        Destroy(gameObject);
    }
}
