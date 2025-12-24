using UnityEngine;

//임시로 제외 대상 주석처리
public enum ItemType
{
    Consumable = 1, Special = 2, Key = 3, Collection = 4
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public string EnItemName;
    public string description;
    public string EnDescription;
    public ItemType type;
    public GameObject prefab;
    public Sprite icon;
    public bool canQuickSlot = true;
    public string ItemNameText
    {
        get
        {
            if (GameManager.Instance.currentLanguage == Language.KR)
            {
                return itemName;
            }
            else
            {
                return EnItemName;
            }
        }
    }
    public string ItemDescription
    {
        get
        {
            if (GameManager.Instance.currentLanguage == Language.KR)
            {
                return description;
            }
            else
            {
                return EnDescription;
            }
        }
    }

    //public float weight;

    [Header("Stack")]
    public int maxStack;
    public bool IsStackable => maxStack > 1;

    [Header("Consumable/Bomb")]
    public float radius;
    public float delay;
    public LayerMask targetLayer;

    [Header("Sound")]
    public AudioClip pickupSoundClip;

    [Header("Type")]
    //외부에서 타입명을 주입할수있도록 열어뒀습니다
    public string typeKor;
    public string typeEng;
    public string TypeText
    {
        get
        {
            //게임매니저에 설정된 게임언어 기준
            if (GameManager.Instance.currentLanguage == Language.KR)
            {

                return typeKor;
            }
            else
            {
                return typeEng;
            }
        }
    }

}
