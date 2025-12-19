using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private QuickSlot[] _slots = new QuickSlot[10];
    [SerializeField] protected float _wheelCool = 0.1f;//ÈÙ¼Óµµ

    private float _wheelTimer = 0f;

    //ÀÐ±âÀü¿ë
    public int CurrentIndex { get; private set; } = 0;
    public QuickSlot CurrentSlot => _slots[CurrentIndex];
    public int SlotCount => _slots.Length;

    private void Awake()
    {
        _slots = GameManager.Instance.GameManagerQuickSlots;

        //if (GameManager.Instance != null && GameManager.Instance.GameManagerQuickSlots[0] == null)
        //{
        //    //GameManagerÀÇ Äü½½·Ô ÂüÁ¶
        //    GameManager.Instance.GameManagerQuickSlots = _slots;
        //}


        //else if (GameManager.Instance != null && GameManager.Instance.GameManagerQuickSlots[0] != null)
        //{
        //    _slots = GameManager.Instance.GameManagerQuickSlots;
        //}

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

        //ÁßÃ¸
        if (data.IsStackable)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (!_slots[i].IsEmpty && _slots[i].Data == data)
                {
                    _slots[i].Add(1);

                    GameManager.Instance.QuickSlotUI(i);
                    return true;
                }
            }
        }
        //ºó½½·Ô
        for (int i = 0; i < SlotCount; i++)
        {
            if (_slots[i].IsEmpty)
            {
                _slots[i].Init(data, 1);

                GameManager.Instance.QuickSlotUI(i);
                return true;
            }
        }
        return false;
    }

    public void SelectSlot(int index)//½½·Ô ¼±ÅÃ
    {
        CurrentIndex = index;
    }

    public void OnSelectPrev(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        CurrentIndex--;
        if (CurrentIndex < 0)
        {
            CurrentIndex = SlotCount - 1;
        }
        //UpdateHighlight();
    }
    public void OnSelectNext(InputAction.CallbackContext ctx)
    {
        if (!ctx.started)
        {
            return;
        }
        CurrentIndex++;
        if (CurrentIndex >= SlotCount)
        {
            CurrentIndex = 0;
        }
        //UpdateHighlight();
    }
    public void OnWheel(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        if (Mathf.Abs(value) < 0.1f)
        {
            return;
        }

        if (_wheelTimer > 0)// ÄðÅ¸ÀÓ
        {
            return;
        }

        if (value > 0)
        {
            OnSelectPrev(ctx);//ÈÙ ¾÷
        }
        else if (value < 0)
        {
            OnSelectNext(ctx);//ÈÙ ´Ù¿î
        }

        _wheelTimer = _wheelCool;
    }
    public bool TryUseCurrentSlot(int index)//¼±ÅÃµÈ ½½·Ô ¾ÆÀÌÅÛ »ç¿ë
    {
        QuickSlot slot = _slots[index];
        if (slot.IsEmpty)
        {
            return false;
        }

        if (slot.Data.type != ItemType.Consumable && slot.Data.type != ItemType.Key)//¼Ò¸ð¼º¾ÆÀÌÅÛ,Å°°¡ ¾Æ´Ò°æ¿ì
        {
            return false;
        }

        slot.Use(1);//¾ÆÀÌÅÛ »ç¿ë

        GameManager.Instance.QuickSlotUI(index);
        return true;
    }
}
