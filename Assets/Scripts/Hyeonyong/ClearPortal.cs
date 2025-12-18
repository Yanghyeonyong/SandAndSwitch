using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ClearPortal : MonoBehaviour
{
    bool check;
    [SerializeField] int _itemId;
    private void Start()
    {
        if (GameManager.Instance.CheckItem == false)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance.GameOverCoroutine != null)
            {
                StopCoroutine(GameManager.Instance.GameOverCoroutine);
                GameManager.Instance.GameOverCoroutine = null;
            }
            GameManager.Instance.LoadVictoryScene();
            GameManager.Instance.EnterPhaseOne();
        }
    }

    ////해당 아이템 id가 있으면 true 반환
    //private int CheckQuickSlotItem()
    //{
    //    //QuickSlot[] _check = GameManager.Instance.GameManagerQuickSlots;
    //    for (int i = 0; i < length; i++)
    //    {
    //        if (GameManager.Instance.GameManagerQuickSlots[i] != null)
    //        {
    //            if (GameManager.Instance.GameManagerQuickSlots[i].Data.id == _itemId)
    //            {
    //                return i;
    //            }

    //        }
    //    }
    //    return -1;
    //}
}
