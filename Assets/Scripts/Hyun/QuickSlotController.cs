using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[3];


    public int CurrentIndex { get; private set; } = 0;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    CurrentIndex = 0;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    CurrentIndex = 1;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    CurrentIndex = 2;
        //}
    }
    public bool TryPickup(ItemData data)
    {
        //중첩
        if (data.IsStackable)
        {
            foreach (QuickSlot slot in _slots)
            {
                if (slot.IsEmpty == false && slot.Data == data && slot.Count < data.maxStack)
                {
                    slot.Add(1);
                    return true;
                }
            }
        }
        //빈슬롯
        foreach (QuickSlot slot in _slots)
        {
            if (slot.IsEmpty)
            {
                slot.Init(data, 1);
                return true;
            }
        }
        return false;
    }

    public void SelectSlot(int index)//슬롯 선택
    {
        CurrentIndex = index;
    }

    public bool TryUseCurrentSlot()//선택된 슬롯 아이템 사용
    {
        QuickSlot slot = _slots[CurrentIndex];
        if (slot.IsEmpty)
        {
            return false;
        }
        if (slot.Data.type != ItemType.Consumable)//소모성아이템이 아닐경우
        {
            return false;
        }
        //아이템 사용
        slot.Consume(1);
        return true;
    }
}
