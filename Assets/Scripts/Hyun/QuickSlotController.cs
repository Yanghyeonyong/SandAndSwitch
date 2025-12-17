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



        if (GameManager.Instance != null)
        {
            //GameManagerÀÇ Äü½½·Ô ÂüÁ¶
            GameManager.Instance.GameManagerQuickSlots = _slots;
        }

    }
    public bool TryPickup(ItemData data)
    {
        if (data == null)
        {
            return false;
        }

        //ÁßÃ¸
        if (data.IsStackable)
        {
            foreach (QuickSlot slot in _slots)
            {
                if (slot.IsEmpty == false && slot.Data == data && slot.Count < data.maxStack)
                {
                    slot.Add(1);
                    for ( int i = 0; i < GameManager.Instance.GameManagerQuickSlots.Length; i++)
                    {
                        if (slot.Data == GameManager.Instance.GameManagerQuickSlots[i].Data)
                        {
                            Debug.Log(i);
                            GameManager.Instance.GameManagerQuickSlotCountTexts[i].text = slot.Count.ToString();
                        }
                    }
                    return true;
                }
            }
        }
        //ºó½½·Ô
        foreach (QuickSlot slot in _slots)
        {
            if (slot.IsEmpty)
            {
                slot.Init(data, 1);
                for (int i = 0; i < GameManager.Instance.GameManagerQuickSlots.Length; i++)
                {
                    if (slot.Data == GameManager.Instance.GameManagerQuickSlots[i].Data)
                    {
                        GameManager.Instance.GameManagerQuickSlotCountTexts[i].text = slot.Count.ToString();
                        GameManager.Instance.GameManagerQuickSlotIcons[i].sprite = slot.Data.icon;
                        GameManager.Instance.GameManagerQuickSlotIcons[i].gameObject.SetActive(true);
                    }
                }
                return true;
            }
        }
        return false;
    }

    public void SelectSlot(int index)//½½·Ô ¼±ÅÃ
    {
        CurrentIndex = index;
    }

    public bool TryUseCurrentSlot()//¼±ÅÃµÈ ½½·Ô ¾ÆÀÌÅÛ »ç¿ë
    {
        QuickSlot slot = _slots[CurrentIndex];
        if (slot.IsEmpty)
        {
            return false;
        }
        if (slot.Data.type != ItemType.Consumable)//¼Ò¸ð¼º¾ÆÀÌÅÛÀÌ ¾Æ´Ò°æ¿ì
        {
            return false;
        }
        //¾ÆÀÌÅÛ »ç¿ë
        slot.Use(1);
        if (slot.Count <= 0)
        {
            GameManager.Instance.GameManagerQuickSlotCountTexts[CurrentIndex].text = "";
            GameManager.Instance.GameManagerQuickSlotIcons[CurrentIndex].gameObject.SetActive(false);
            GameManager.Instance.GameManagerQuickSlotIcons[CurrentIndex].sprite = null;
        }

        //GameManager.Instance.GameManagerQuickSlotCountTexts[CurrentIndex].text = slot.Count.ToString();
        return true;
    }
}
