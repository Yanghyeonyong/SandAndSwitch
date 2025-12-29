using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectSlotController : MonoBehaviour
{
    //수집아이템이 여러개일것이라 생각하고 딕셔너리기반으로 만들었었는데...
    //키 : id
    //value : 컬렉트슬롯
    private Dictionary<int, CollectSlot> _slots = new Dictionary<int, CollectSlot>();

    [SerializeField] ItemData _collectItem;

    public event Action<CollectSlot> OnCollectChanged;//수집 슬롯 정보가 변경되었을 때 알리기 위한 이벤트

    public Dictionary<int, CollectSlot> Slots
    {
        get { return _slots; }
        set { _slots = value; }
    }

    void Awake()
    {
        _slots.Add(401, new CollectSlot
        {
            Data = _collectItem,
            Count = 0
        });
    }

    public void CollectSlotClear()
    {
        _slots.Clear();
        _slots.Add(401, new CollectSlot
        {
            Data = _collectItem,
            Count = 0
        });
    }

    public void Collect(ItemData data)
    {
  
        if (data == null || data.type != ItemType.Collection)
        {
            return;
        }

        CollectSlot slot;

        if (_slots.TryGetValue(data.id, out slot))
        {
            slot.Add(1);
        }
        else
        {
            slot = new CollectSlot { Data = data, Count = 1 };
            _slots.Add(data.id, slot);
        }
        //UI갱신
        OnCollectChanged?.Invoke(slot);
    }



}
