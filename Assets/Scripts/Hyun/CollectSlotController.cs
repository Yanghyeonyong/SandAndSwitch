using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectSlotController : MonoBehaviour
{
    private Dictionary<int, CollectSlot> _slots = new Dictionary<int, CollectSlot>();
    public IReadOnlyDictionary<int, CollectSlot> Slots => _slots;



    public void Collect(ItemData data)
    {
        if (data == null)
        {
            return;
        }
        if (data.type != ItemType.Collection)
        {
            return;
        }
        if (_slots.TryGetValue(data.id, out CollectSlot slot))
        {
            slot.Add(1);
        }
        else
        {
            _slots.Add(data.id, new CollectSlot
            {
                Data = data,
                Count = 1
            });
        }

        //UI갱신
    }
    //보유 여부
    public bool HasItem(int itemId, int requiredCount = 1)
    {
        return _slots.ContainsKey(itemId) && _slots[itemId].Count >= requiredCount;
    }



}
