using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ClearPortal : MonoBehaviour
{
    bool check;
    [SerializeField] int _itemId = 103;
    int length = 0;
    private void Start()
    {
        //length = GameManager.Instance.GameManagerQuickSlots.Length;
        //if (CheckQuickSlotItem() == -1)
        //{
        //    gameObject.SetActive(false);
        //}
        //else
        //{
        //    Debug.Log(CheckQuickSlotItem() + "에 있다");
        //}
        StartCoroutine(CheckItem());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.LoadVictoryScene();
        }
    }

    //해당 아이템 id가 있으면 true 반환
    private int CheckQuickSlotItem()
    {
        //QuickSlot[] _check = GameManager.Instance.GameManagerQuickSlots;
        for (int i = 0; i < length; i++)
        {
            if (GameManager.Instance.GameManagerQuickSlots[i] != null)
            {
                if (GameManager.Instance.GameManagerQuickSlots[i].Data == null)
                {
                    continue;
                }

                if (GameManager.Instance.GameManagerQuickSlots[i].Data.id == _itemId)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    IEnumerator CheckItem()
    {
        yield return GameManager.Instance != null;
        length = GameManager.Instance.GameManagerQuickSlots.Length;
        if (CheckQuickSlotItem() == -1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GameManager.Instance.CheckItem=true;
            GameManager.Instance.EnterPhaseTwo();
        }

    }
}
