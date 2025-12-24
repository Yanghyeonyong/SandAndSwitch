using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[10];
    [SerializeField] private float _wheelCool = 0.1f;

    private float _wheelTimer = 0f;

    //읽기전용
    public int CurrentIndex { get; private set; } = 0;
    public QuickSlot CurrentSlot => _slots[CurrentIndex];

    public event Action<ItemData, int> OnItemUsed;//외부에 아이템 사용을 알리는 이벤트

    private void Awake()
    {
        //for (int i = 0; i < _slots.Length; i++)
        //{
        //    if (_slots[i] == null)
        //    {
        //        _slots[i] = new QuickSlot();
        //    }
        //}



        //if (GameManager.Instance != null && GameManager.Instance.GameManagerQuickSlots[0] == null)
        //{
        //    //GameManager의 퀵슬롯 참조
        //    GameManager.Instance.GameManagerQuickSlots = _slots;
        //}


        //else if (GameManager.Instance != null && GameManager.Instance.GameManagerQuickSlots[0] != null)
        //{
        //    _slots = GameManager.Instance.GameManagerQuickSlots;
        //}
        if (GameManager.Instance == null)
        {
            return;
        }

        // GameManager 슬롯이 아직 없으면 생성 (최초 1회)
        if (GameManager.Instance.GameManagerQuickSlots[0] == null)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = new QuickSlot();
            }

            GameManager.Instance.GameManagerQuickSlots = _slots;
        }
        else
        {
            // 이미 있으면 참조만 가져옴 (절대 새로 만들지 않음)
            _slots = GameManager.Instance.GameManagerQuickSlots;
        }
        GameManager.Instance.QuickSlotUIUpdate(CurrentIndex);

    }

    private void Update()
    {
        if (_wheelTimer > 0)
        {
            _wheelTimer -= Time.deltaTime;
        }
    }
    public bool TryPickup(ItemData data)
    {
        if (data == null)
        {
            return false;
        }
        if (!data.canQuickSlot)
        {
            return false;
        }
        //기존 중첩코드
        //if (data.IsStackable)
        //{
        //    foreach (QuickSlot slot in _slots)
        //    {
        //        if (slot.IsEmpty == false && slot.Data == data && slot.Count < data.maxStack)
        //        {
        //            slot.Add(1);
        //            for (int i = 0; i < GameManager.Instance.GameManagerQuickSlots.Length; i++)
        //            {
        //                if (slot.Data == GameManager.Instance.GameManagerQuickSlots[i].Data)
        //                {
        //                    Debug.Log(i);
        //                    GameManager.Instance.GameManagerQuickSlotCountTexts[i].text = slot.Count.ToString();
        //                }
        //            }
        //            return true;
        //        }
        //    }
        //}

        //변경된 중첩
        if (data.IsStackable)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                 QuickSlot slot = _slots[i];

                if (!slot.IsEmpty && slot.Data == data && slot.Count < data.maxStack)
                {
                    slot.Add(1);

                    // UI 업데이트는 GameManager 호출
                    GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(slot.Data, 1);
                    GameManager.Instance.UpdateQuickSlot(i, slot);
                    return true;
                }
            }
        }

        //기존 빈슬롯 코드
        //foreach (QuickSlot slot in _slots)
        //{
        //    if (slot.IsEmpty)
        //    {
        //        slot.Init(data, 1);
        //        for (int i = 0; i < GameManager.Instance.GameManagerQuickSlots.Length; i++)
        //        {
        //            if (slot.Data == GameManager.Instance.GameManagerQuickSlots[i].Data)
        //            {
        //                GameManager.Instance.GameManagerQuickSlotCountTexts[i].text = slot.Count.ToString();
        //                GameManager.Instance.GameManagerQuickSlotIcons[i].sprite = slot.Data.icon;
        //                GameManager.Instance.GameManagerQuickSlotIcons[i].color = Color.white;
        //                GameManager.Instance.GameManagerQuickSlotIcons[i].gameObject.SetActive(true);
        //            }
        //        }
        //        return true;
        //    }
        //}

        //변경된 빈슬롯
        for (int i = 0; i < _slots.Length; i++)
        {
            QuickSlot slot = _slots[i];

            if (slot.IsEmpty)
            {
                slot.Init(data, 1);
                GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(slot.Data, 1);
                GameManager.Instance.UpdateQuickSlot(i, slot);
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


    //Color clear = new Color(1, 1, 1, 0);


    public bool TryUseCurrentSlot(int index)//선택된 슬롯 아이템 사용
    {
        QuickSlot slot = _slots[index];
        if (slot.IsEmpty)
        {
            return false;
        }
        if (slot.Data.type != ItemType.Consumable)//소모성아이템이 아닐경우
        {
            return false;
        }
        int useCount = 1;

        //아이템 사용
        GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(slot.Data, -useCount);
        slot.Use(useCount);
        //기존코드
        //if (slot.Count <= 0)
        //{
        //    GameManager.Instance.GameManagerQuickSlotCountTexts[index].text = "";
        //    GameManager.Instance.GameManagerQuickSlotIcons[index].gameObject.SetActive(false);
        //    GameManager.Instance.GameManagerQuickSlotIcons[index].sprite = null;
        //    GameManager.Instance.GameManagerQuickSlotIcons[index].color = clear;
        //}
        //else
        //{
        //    GameManager.Instance.GameManagerQuickSlotCountTexts[index].text = slot.Count.ToString();
        //}

        //GameManager.Instance.GameManagerQuickSlotCountTexts[CurrentIndex].text = slot.Count.ToString();

        //변경된 코드
        GameManager.Instance.UpdateQuickSlot(index, slot);
        if (slot.Count <= 0)
        {
            ShiftSlots();//슬롯이동

            for (int i = 0; i < _slots.Length; i++)
            {
                GameManager.Instance.UpdateQuickSlot(i, _slots[i]);//슬롯이동후 UI갱신
            }

            //현재 인덱스도 0으로 초기화
            CurrentIndex = 0;
            GameManager.Instance.QuickSlotUIUpdate(CurrentIndex);
        }

        OnItemUsed?.Invoke(slot.Data, useCount);//아이템 사용 알림

        return true;
    }
    public void SelectPreviousSlot()
    {
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex = _slots.Length - 1;
        }

        GameManager.Instance.QuickSlotUIUpdate(CurrentIndex);
    }
    public void SelectNextSlot()
    {
        CurrentIndex++;
        if (CurrentIndex >= _slots.Length)
        {
            CurrentIndex = 0;
        }

        GameManager.Instance.QuickSlotUIUpdate(CurrentIndex);
    }
    public void WheelScroll(float scroll)
    {
        if (_wheelTimer > 0)
        {
            return;
        }
        if (scroll > 0)
        {
            SelectPreviousSlot();
        }
        else if (scroll < 0)
        {
            SelectNextSlot();
        }
        _wheelTimer = _wheelCool;
    }
    public void ShiftSlots()
    {
        int writeIndex = 0;

        for (int readIndex = 0; readIndex < _slots.Length; readIndex++)
        {
            if (!_slots[readIndex].IsEmpty)
            {
                if (writeIndex != readIndex)
                {
                    _slots[writeIndex].SlotCopy(_slots[readIndex]);
                    _slots[readIndex].Clear();
                }
                writeIndex++;
            }
        }
    }
    public bool ConsumeKeySlot(int index, int consumeCount)
    {
        QuickSlot slot = _slots[index];

        // 슬롯이 비었거나 키가 아니면 실패
        if (slot.IsEmpty || slot.Data == null || slot.Data.type != ItemType.Key)
        {
            return false;
        }

        // 개수 부족하면 실패
        if (slot.Count < consumeCount)
        {
            return false;
        }

        // 개수 차감
        GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(slot.Data, -consumeCount);
        slot.Use(consumeCount);
        GameManager.Instance.UpdateQuickSlot(index, slot);

        return true;
    }
}
