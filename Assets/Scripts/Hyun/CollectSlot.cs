[System.Serializable]
public class CollectSlot
{
    public ItemData Data;
    public int Count;

    public void Init(ItemData data, int count)
    {
        Data = data;
        Count = count;
    }

    public void Add(int amount)
    {
        Count += amount;
    }

    public bool IsEmpty => Data == null || Count <= 0;
}
