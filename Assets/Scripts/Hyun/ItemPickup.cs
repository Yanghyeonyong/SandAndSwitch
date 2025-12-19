using UnityEngine;
using System;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private float _soundValue = 1f;
    [SerializeField] private Vector3 uniqueID;

    public ItemData ItemData => _itemData;
    public Vector3 UniqueID => uniqueID;


//#if UNITY_EDITOR
//    //에디터에서 컴포넌트가 변경 될때 자동 실행
//    private void OnValidate()
//    {
//        //프리팹 제외
//        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
//        {
//            uniqueID = string.Empty;
//            return;
//        }

//        // 2. ID가 비어있다면 새 ID 부여
//        if (string.IsNullOrEmpty(uniqueID))
//        {
//            uniqueID = Guid.NewGuid().ToString();
//            UnityEditor.EditorUtility.SetDirty(this); // 변경사항 저장
//        }
//    }
//#endif

    

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
