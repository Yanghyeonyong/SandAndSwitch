using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[3];


    public int CurrentIndex { get; private set; } = 0;

    private void Awake()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = new QuickSlot();
            }
        }
    }
    public bool TryPickup(ItemData data)
    {
        if (data == null)
        {
            return false;
        }

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
        slot.Use(1);
        return true;
    }
}
