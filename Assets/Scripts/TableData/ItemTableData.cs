using System;
using UnityEngine;

public class ItemTableData : TableBase
{
    public int ID;
    public string Name;
    public string Desc;
    public ItemType Type;
    public int MaxStack;

    // CSV에서 읽는 값(문자열 경로/키)
    public string Prefab;
    public string Icon;

    // 로드해서 쓰는 실제 오브젝트(테이블 후처리에서 채움)
    [NonSerialized] public GameObject PrefabObject;
    [NonSerialized] public Sprite IconSprite;

    public string NameText
    {
        get
        {
            if (GameManager.Instance != null && GameManager.Instance.StringTable != null)
            {
                // StringTable에 해당 키가 있으면 kr 텍스트 반환, 없으면 키값 자체 반환(에러 방지)
                var data = GameManager.Instance.StringTable[Name];
                return data != null ? data.kr : Name;
            }
            return Name;
        }
    }

    public string DescText
    {
        get
        {
            if (GameManager.Instance != null && GameManager.Instance.StringTable != null)
            {
                var data = GameManager.Instance.StringTable[Desc];
                return data != null ? data.kr : Desc;
            }
            return Desc;
        }
    }
}
