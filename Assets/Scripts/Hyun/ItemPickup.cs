using UnityEngine;
using System;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private float _soundValue = 1f;
    [SerializeField] private string uniqueId;

    public ItemData ItemData => _itemData;
    public string UniqueId => uniqueId;
    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = Guid.NewGuid().ToString();
        }
    }
    private void Start()
    {
        if (GameManager.Instance.CollectedItemIDs.Contains(uniqueId))
        {
            gameObject.SetActive(false);
            return;
        }
    }
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
        GameManager.Instance.CollectedItemIDs.Add(uniqueId);
        if (_itemData.pickupSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(_itemData.pickupSoundClip, transform.position, _soundValue);
        }
        Destroy(gameObject);
    }
}
