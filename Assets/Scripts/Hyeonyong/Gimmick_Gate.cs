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
        if (IsClear|| CheckQuickSlotItem())
        {
            GameManager.Instance.IsGimmickClear[GimmickId] = true;
            GameManager.Instance.LoadNextScene();
        }
    }

    //해당 아이템 id가 있으면 true 반환
    private bool CheckQuickSlotItem()
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
                        return true;
                    else 
                        return false;
                }

            }
        }
        return false;
    }
}
