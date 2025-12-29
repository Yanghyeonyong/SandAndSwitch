using UnityEngine;
using System;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private float _soundValue = 1f;
    [SerializeField] private Vector3 uniqueID;//중복 획득 방지용 위치기반 ID



    public ItemData ItemData => _itemData;
    public Vector3 UniqueID => uniqueID;


    private void Start()
    {
        uniqueID = transform.position;//위치기반 ID 설정
        if (GameManager.Instance.CollectedItemIDs.Contains(uniqueID))//이미 획득한 아이템은 비활성화
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
        if (bomb != null && bomb.IsThrownBomb)//던져진 폭탄은 다시 주울수 없도록 처리
        {
            return;
        }
        Player player = collision.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        //아이템타입에 따라 분기
        switch (_itemData.type)
        {
            case ItemType.Collection://컬렉션 슬롯에 들어가는 수집아이템은 컬렉션쪽
                GameManager.Instance.GetComponent<CollectSlotController>().Collect(_itemData);
                Pickup();
                break;
            case ItemType.Collections://퀵슬롯에 들어가는 수집아이템은 퀵슬롯쪽
                if (player.Slot != null && player.Slot.TryPickup(_itemData))
                {
                    Pickup();
                }
                break;
            case ItemType.Consumable:
                if (_itemData.canQuickSlot)
                {
                    //퀵슬롯에 들어가는 소모형 아이템
                    if (player.Slot != null && player.Slot.TryPickup(_itemData))
                    {
                        Pickup();
                    }
                }
                else
                {
                    //퀵슬롯에 들어가지 않는 소모형 아이템
                    if (GameManager.Instance.CurrentPlayerHealth < 3)//조건 : 플레이어 체력이 3미만
                    {
                        //GameManager.Instance.PlayerHeal(1);
                        Potion.ConsumableUsed(_itemData);//포션에 요청
                        Pickup();
                    }
                }
                break;

            case ItemType.Special:
                player.SetNearbyItem(this);
                break;

            case ItemType.Key:
                if (player.Slot != null && player.Slot.TryPickup(_itemData))
                {
                    Pickup();
                }
                break;
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
        GameManager.Instance.CollectedItemIDs.Add(uniqueID);//중복 방지용 ID저장
        if (_itemData.pickupSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(_itemData.pickupSoundClip, transform.position, _soundValue);
        }
        //Destroy(gameObject);
        gameObject.SetActive(false);//파괴대신 비활성화
    }

}
