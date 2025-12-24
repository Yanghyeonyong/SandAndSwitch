using UnityEngine;
using UnityEngine.UIElements;

public class Gimmick_Gate : Gimmick
{
    [SerializeField] int _itemId;
    [SerializeField] int _requireCount;
    int length;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CheckClear())
        {
            //문이 열려있고 씬 이동이 활성화되어있는 상태

            return;
        }
        length = GameManager.Instance.GameManagerQuickSlots.Length;
    }

    public override void StartGimmick()
    {
        int index = CheckQuickSlotItem();
        if (IsClear || index != -1)
        {
            IsClear = true;
            if (index != -1)
            {
                for (int i = 0; i < _requireCount; i++)
                {
                    Debug.Log("열쇠 사용");
                    //GameManager.Instance.Player.Slot.TryUseCurrentSlot(index);
                    GameManager.Instance.Player.Slot.ConsumeKeySlot(index, 1);
                }
                //키 사용이 끝난후에 비어있는 슬롯 이동
                GameManager.Instance.Player.Slot.ShiftSlots();
                //UI갱신
                GameManager.Instance.RefreshAllQuickSlotUI();
            }
            //NullReferenceExeption 방지를 위해 IsReuse를 false로 전환
            GetComponent<InteractiveObject>().IsReuse = false;
            GameManager.Instance.IsGimmickClear[GimmickId] = true;
            GameManager.Instance.LoadNextScene();
            //gameObject.SetActive(false);

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
                    if(GameManager.Instance.GameManagerQuickSlots[i].Count >=_requireCount)
                        return i;
                    else 
                        return -1;
                }

            }
        }
        return -1;
    }
}
