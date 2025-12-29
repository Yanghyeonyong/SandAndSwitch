using System;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[10];//퀵슬롯 10칸
    //마우스 휠 입력 지연시간
    [SerializeField] private float _wheelCool = 0.1f;
    private float _wheelTimer = 0f;
    //사운드 추가
    [SerializeField] private AudioClip _slotChangeClip;
    [SerializeField] private float _slotChangeVolume = 1f;

    //읽기전용
    public int CurrentIndex { get; private set; } = 0;//현재 선택된 슬롯 인덱스
    public QuickSlot CurrentSlot => _slots[CurrentIndex];//현재 선택된 슬롯


    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        //게임매니저에 슬롯이 아직 없으면 생성 
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
            // 이미 있으면 참조만 가져옴
            _slots = GameManager.Instance.GameManagerQuickSlots;
        }
        //UI 초기 갱신
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
        //중첩
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

        //빈슬롯
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
        SlotChangeSound();
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
        SlotChangeSound();

    }
    public void SelectNextSlot()
    {
        CurrentIndex++;
        if (CurrentIndex >= _slots.Length)
        {
            CurrentIndex = 0;
        }

        GameManager.Instance.QuickSlotUIUpdate(CurrentIndex);
        SlotChangeSound();
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

        //슬롯이 키가 아니면 실패
        if (slot.Data == null || slot.Data.type != ItemType.Key)
        {
            return false;
        }

        //개수 부족하면 실패
        if (slot.Count < consumeCount)
        {
            return false;
        }
        ItemData backupData = slot.Data;
        //개수 차감
        slot.Use(consumeCount);
        GameManager.Instance.ItemLogCanvas.PickupOrUseLogic(backupData, -consumeCount);
        GameManager.Instance.UpdateQuickSlot(index, slot);
        return true;
    }
    private void SlotChangeSound()
    {
        if (_slotChangeClip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(_slotChangeClip, Camera.main.transform.position, _slotChangeVolume);
    }

}
