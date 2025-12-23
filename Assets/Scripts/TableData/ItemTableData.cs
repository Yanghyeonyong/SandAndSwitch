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
            if (GameManager.Instance == null || GameManager.Instance.StringTable == null) return Name;

            var data = GameManager.Instance.StringTable[Name];
            if (data == null) return Name;

            //언어 설정에 따라 분기
            return GameManager.Instance.currentLanguage == Language.KR ? data.kr : data.en;
        }
    }

    // 설명 가져오기
    public string DescText
    {
        get
        {
            if (GameManager.Instance == null || GameManager.Instance.StringTable == null) return Desc;

            var data = GameManager.Instance.StringTable[Desc];
            if (data == null) return Desc;

            //언어 설정에 따라 분기
            return GameManager.Instance.currentLanguage == Language.KR ? data.kr : data.en;
        }
    }
}
