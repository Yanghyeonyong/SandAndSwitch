using UnityEngine;
using System;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private float _soundValue = 1f;
    [SerializeField] private Vector3 uniqueID;



    public ItemData ItemData => _itemData;
    public Vector3 UniqueID => uniqueID;


    private void Start()
    {
        uniqueID = transform.position;
        if (GameManager.Instance.CollectedItemIDs.Contains(uniqueID))
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        Bomb bomb = GetComponent<Bomb>();
        if (bomb != null && bomb.IsThrownBomb)
        {
            return;
        }
        Player player = collision.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        if (_itemData.type == ItemType.Special)
        {
            player.SetNearbyItem(this);
            return;
        }
        if (!_itemData.canQuickSlot)
        {
            Potion potion = GetComponent<Potion>();
            if (potion != null)
            {
                potion.UsePotion();
            }

            Pickup();
            return;
        }
        if (player.Slot != null && player.Slot.TryPickup(_itemData))
        {
            Pickup();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        Player player = collision.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        if (_itemData.type == ItemType.Special)
        {
            player.ClearNearbyItem(this);
        }
    }


    public void Pickup()
    {
        //uniqueID = GameManager.Instance.currentPickedItemID;
        //GameManager.Instance.currentPickedItemID++;
        Debug.Log("Picked up item with ID: " + uniqueID);
        GameManager.Instance.CollectedItemIDs.Add(uniqueID);
        if (_itemData.pickupSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(_itemData.pickupSoundClip, transform.position, _soundValue);
        }
        Destroy(gameObject);
    }

}
