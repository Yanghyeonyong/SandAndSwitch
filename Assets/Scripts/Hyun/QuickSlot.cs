using UnityEngine;

[System.Serializable]
public class QuickSlot
{
    [SerializeField] private ItemData _data;//아이템정보
    [SerializeField] private int _count;//수량


    //읽기전용
    public ItemData Data => _data;
    public int Count => _count;
    public bool IsEmpty => _data == null || _count <= 0;//수량이 0이하 혹은 널이면 트루(빈슬롯)

    public void Init(ItemData data, int count)
    {
        _data = data;
        _count = count;
    }
    public void Add(int amount)//아이템 추가
    {
        _count += amount;
        if (_count > _data.maxStack)//최대 스택 초과 방지
        {
            _count = _data.maxStack;
        }
    }
    public void Use(int amount)//아이템 사용
    {
        _count -= amount;
        if (_count <= 0)
        {
            _data = null;
            _count = 0;
        }
    }

}
