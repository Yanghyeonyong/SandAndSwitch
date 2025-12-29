using UnityEngine;

//임시로 제외 대상 주석처리
public enum ItemType
{
    Consumable = 1, Special = 2, Key = 3, Collection = 4, Collections = 5
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

            //20251224 최정욱 parsing 이전에 사용할 수 있도록 구현
            if (typeKor == "" || typeEng == "")
            {
                Debug.Log($"ItemData {itemName}의 typeKor 또는 typeEng이 설정되지 않았습니다. 자동으로 설정합니다.");
            }

            if (typeKor == "")
            {
                switch(type)
                {
                    case ItemType.Consumable:
                        typeKor = "소비 아이템";
                        break;
                    case ItemType.Special:
                        typeKor = "보물";
                        break;
                    case ItemType.Key:
                        typeKor = "열쇠 아이템";
                        break;
                    case ItemType.Collection:
                        typeKor = "수집 아이템";
                        break;
                    case ItemType.Collections:
                        typeKor = "수집 아이템";
                        break;
                    default:
                        typeKor = "기타 아이템";
                        break;
                }
            }
            else if (typeEng == "")
            {
                switch (type)
                {
                    case ItemType.Consumable:
                        typeEng = "Consumable";
                        break;
                    case ItemType.Special:
                        typeEng = "Treasure";
                        break;
                    case ItemType.Key:
                        typeEng = "Key";
                        break;
                    case ItemType.Collection:
                        typeEng = "Collection";
                        break;
                    case ItemType.Collections:
                        typeEng = "Collections";
                        break;
                    default:
                        typeEng = "Other Item";
                        break;
                }
            }

            //게임매니저에 설정된 게임언어 기준
            if (typeKor != "" && GameManager.Instance.currentLanguage == Language.KR)
            {
                

                return typeKor;
            }
            else if (typeEng != "" && GameManager.Instance.currentLanguage == Language.EN)
            {
                
                return typeEng;
            }
            else
            {
                return "Undefined Type";
            }
        }
    }

}
