using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectSlotController : MonoBehaviour
{
    private Dictionary<int, CollectSlot> _slots = new Dictionary<int, CollectSlot>();
    public IReadOnlyDictionary<int, CollectSlot> Slots => _slots;

    public event Action<ItemData, int> OnCollected;//컬렉트 아이템을 획득했다는 이벤트

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

        //UI갱신을 위해 획득했다는 이벤트 발생
        OnCollected?.Invoke(data, _slots[data.id].Count);
    }
    //보유 여부
    public bool HasItem(int itemId, int requiredCount = 1)
    {
        return _slots.ContainsKey(itemId) && _slots[itemId].Count >= requiredCount;
    }



}
