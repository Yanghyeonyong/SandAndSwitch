

[System.Serializable]
public class QuickSlot
{
    public ItemData data;//아이템정보
    public int count;//수량
    
    
    //읽기전용
    public bool IsEmpty => data == null || count <= 0;//수량이 0이하 혹은 널이면 트루(빈슬롯)

    public void Init(ItemData data, int count)
    {
        this.data = data;
        this.count = count;
    }
    public void Add(int amount)//아이템 추가
    {
        count += amount;
        if (count > data.maxStack)//최대 스택 초과 방지
        {
            count = data.maxStack;
        }
    }
    public void Use(int amount)//아이템 사용
    {
        count -= amount;
        if (count <= 0)
        {
            data = null;
            count = 0;
        }
    }

}
