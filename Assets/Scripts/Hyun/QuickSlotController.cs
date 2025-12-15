using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[3];
    private int _currnetIndex = 0;

    public bool TryPickup(ItemData data)
    {
        //¡ﬂ√∏
        if (data.IsStackable)
        {
            foreach (QuickSlot slot in _slots)
            {
                if (slot.IsEmpty == false && slot.Data == data)
                {
                    slot.Add(1);
                    return true;
                }
            }
        }
        //∫ÛΩΩ∑‘
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

    public void SelectSlot(int index)//ΩΩ∑‘ º±≈√
    {
        _currnetIndex = index;
    }
    
    public void UseCurrentSlot()//º±≈√µ» ΩΩ∑‘ æ∆¿Ã≈€ ªÁøÎ
    {
        QuickSlot slot = _slots[_currnetIndex];
        if (slot.IsEmpty)
        {
            return;
        }
        //æ∆¿Ã≈€ ªÁøÎ
        slot.Consume(1);
    }
}
