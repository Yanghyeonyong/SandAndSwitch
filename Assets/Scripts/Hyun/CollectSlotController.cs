using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectSlotController : MonoBehaviour
{
    private Dictionary<int, CollectSlot> _slots = new Dictionary<int, CollectSlot>();
    [SerializeField] ItemData _collectItem;
    //public IReadOnlyDictionary<int, CollectSlot> Slots => _slots;

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
            //if (_slots[data.id].Data != data)
            //{
            //    _slots[data.id].Data = data;
            //}
            GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(data, 1);
            slot.Add(1);
            GameManager.Instance.CollectibleCountText.text = slot.Count + "/" + GameManager.Instance.TotalCollectibleCount;
            if (GameManager.Instance.CollectibleIcon.color.a != 1f)
            {
                GameManager.Instance.CollectibleIcon.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        else
        {
            GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(data, 1);
            _slots.Add(data.id, new CollectSlot
            {
                Data = data,
                Count = 1
            });
            GameManager.Instance.CollectibleIcon.color = new Color(1f, 1f, 1f, 1f);
            GameManager.Instance.CollectibleCountText.text = 1 + "/"+ GameManager.Instance.TotalCollectibleCount;
        }

        //UI갱신
    }
    //보유 여부
    public bool HasItem(int itemId, int requiredCount = 1)
    {
        return _slots.ContainsKey(itemId) && _slots[itemId].Count >= requiredCount;
    }



}
