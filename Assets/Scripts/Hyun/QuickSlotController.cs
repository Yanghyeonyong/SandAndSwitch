using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[3];


    //읽기전용
    public int CurrentIndex { get; private set; } = 0;
    public QuickSlot CurrentSlot => _slots[CurrentIndex];


    private void Awake()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = new QuickSlot();
            }
        }

         

        if (GameManager.Instance != null && GameManager.Instance.GameManagerQuickSlots[0] == null)
        {
            //GameManager의 퀵슬롯 참조
            GameManager.Instance.GameManagerQuickSlots = _slots;
        }


        else if (GameManager.Instance != null && GameManager.Instance.GameManagerQuickSlots[0] != null)
        {
            _slots = GameManager.Instance.GameManagerQuickSlots;
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
        //빈슬롯
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
                        GameManager.Instance.GameManagerQuickSlotIcons[i].color = Color.white;
                        GameManager.Instance.GameManagerQuickSlotIcons[i].gameObject.SetActive(true);
                    }
                }
                return true;
            }
        }
        return false;
    }

    public void SelectSlot(int index)//슬롯 선택
    {
        CurrentIndex = index;
        GameManager.Instance.QuickSlotUIUpdate(index);
    }


    Color clear = new Color(1, 1, 1, 0);
    

    public bool TryUseCurrentSlot(int index)//선택된 슬롯 아이템 사용
    {
        QuickSlot slot = _slots[index];
        if (slot.IsEmpty)
        {
            return false;
        }
        if (slot.Data.type != ItemType.Consumable && slot.Data.type != ItemType.Key)//소모성아이템,키가 아닐경우
        {
            return false;
        }
        //아이템 사용
        slot.Use(1);
        if (slot.Count <= 0)
        {
            GameManager.Instance.GameManagerQuickSlotCountTexts[index].text = "";
            GameManager.Instance.GameManagerQuickSlotIcons[index].gameObject.SetActive(false);
            GameManager.Instance.GameManagerQuickSlotIcons[index].sprite = null;
            GameManager.Instance.GameManagerQuickSlotIcons[index].color = clear;
        }
        else
        {
            GameManager.Instance.GameManagerQuickSlotCountTexts[index].text = slot.Count.ToString();
        }

        //GameManager.Instance.GameManagerQuickSlotCountTexts[CurrentIndex].text = slot.Count.ToString();
        return true;
    }
    public void SelectPreviousSlot()
    {
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex = _slots.Length - 1;
        }

        //슬롯UI업데이트관련호출
    }
    public void SelectNextSlot()
    {
        CurrentIndex++;
        if (CurrentIndex >= _slots.Length)
        {
            CurrentIndex = 0;
        }

        //슬롯UI업데이트관련호출
    }
}
