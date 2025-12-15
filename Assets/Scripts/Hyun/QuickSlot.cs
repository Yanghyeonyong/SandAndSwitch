using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] private ItemData _data;//아이템정보
    [SerializeField] private int _count;//수량
    
    //읽기전용
    public ItemData Data => _data;
    public int Count => _count;
    public bool IsEmpty => _data == null || _count <= 0;//수량이 0이하 혹은 널이면 트루(빈슬롯)

    public void Init(ItemData data, int count)
    {
        
    }
    public void Add()
    {

    }
    public void Consume()
    {

    }
    public void Clear()
    {

    }

}
