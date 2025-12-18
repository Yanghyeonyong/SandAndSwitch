using System;
using UnityEngine;

public class ItemRow : TableBase
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
}
