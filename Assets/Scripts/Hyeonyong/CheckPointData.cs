using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointData : Singleton<CheckPointData>
{
    public bool _onCheck = false;

    public Vector3 _playerPos;
    public Quaternion _playerRot;
    public int _playerHp;
    public int _checkPointScene;
    public QuickSlot[] GameManagerQuickSlots = new QuickSlot[10];
    public Dictionary<int, bool> _isGimmickClear = new Dictionary<int, bool>();
    public Dictionary<int, ItemTransform> _gimmickPos = new Dictionary<int, ItemTransform>();
    //아이템 픽업 관련
    public List<Vector3> CollectedItemIDs = new List<Vector3>();


    [SerializeField] private ItemData[] _data;//아이템정보

    private void Start()
    {
        GameManager.Instance.CheckPointData = this;
    }
    public void Init()
    {
        //_onCheck = true;
        //GameManagerQuickSlots = new QuickSlot[10];
        //for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        //{
        //    GameManagerQuickSlots[i] = new QuickSlot();
        //}

        //_playerPos = GameManager.Instance._player.transform.position;
        //_playerRot = GameManager.Instance._player.transform.rotation;
        //_playerHp = GameManager.Instance.currentPlayerHealth;
        //_checkPointScene = GameManager.Instance.CurScene;

        ////아이템 저장
        //for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        //{
        //    if (GameManager.Instance.GameManagerQuickSlots[i].Data != null)
        //    {

        //        ItemData data = FindItem(GameManager.Instance.GameManagerQuickSlots[i].Data.id);

        //        GameManagerQuickSlots[i].Init(data, GameManager.Instance.GameManagerQuickSlots[i].Count);
        //    }
        //}
        ////이후에 updatequickslot을 호출하여 전부 재설정 가능
        ////for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        ////{
        ////    GameManager.Instance.UpdateQuickSlot(i, GameManagerQuickSlots[i]);
        ////}

        ////기믹 클리어 여부 복사
        //foreach (var d in GameManager.Instance.IsGimmickClear)
        //{
        //    if (_isGimmickClear.ContainsKey(d.Key))
        //    {
        //        _isGimmickClear[d.Key] = d.Value;
        //    }
        //    else
        //    {
        //        _isGimmickClear.Add(d.Key, d.Value);
        //    }
        //}
        ////기믹 위치 정보 복사
        //foreach (var d in GameManager.Instance.GimmickPos)
        //{
        //    ItemTransform itemTransform = new ItemTransform(d.Value.position, d.Value.rotation, d.Value.scale);
        //    if (_gimmickPos.ContainsKey(d.Key))
        //    {
        //        _gimmickPos[d.Key] = itemTransform;
        //    }
        //    else
        //    {
        //        _gimmickPos.Add(d.Key, itemTransform);
        //    }
        //}

        //foreach (Vector3 pos in GameManager.Instance.CollectedItemIDs)
        //{
        //    if (CollectedItemIDs.Contains(pos))
        //    {
        //        continue;
        //    }
        //    CollectedItemIDs.Add(pos);
        //}
        //아이템 생성 여부 복사
        //foreach (Vector3 pos in GameManager.Instance.CollectedItemIDs)
        //{ 
        //    Vector3 itemId = new Vector3(pos.x,pos.y, pos.z);
        //    CollectedItemIDs.Add(itemId);
        //}
        _onCheck = true;
        //플레이어 상태 저장
        _playerPos = GameManager.Instance._player.transform.position;
        _playerRot = GameManager.Instance._player.transform.rotation;
        _playerHp = GameManager.Instance.currentPlayerHealth;
        _checkPointScene = GameManager.Instance.CurScene;
        //퀵슬롯 저장
        GameManagerQuickSlots = new QuickSlot[10];
        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            GameManagerQuickSlots[i] = new QuickSlot();

            QuickSlot src = GameManager.Instance.GameManagerQuickSlots[i];
            if (src != null && src.Data != null && src.Count > 0)
            {
                ItemData data = FindItem(src.Data.id);
                GameManagerQuickSlots[i].Init(data, src.Count);
            }
        }
        //id 저장
        CollectedItemIDs.Clear();
        foreach (Vector3 pos in GameManager.Instance.CollectedItemIDs)
        {
            CollectedItemIDs.Add(pos);
        }
        //기믹 상태 저장
        _isGimmickClear = new Dictionary<int, bool>(GameManager.Instance.IsGimmickClear);
        _gimmickPos = new Dictionary<int, ItemTransform>(GameManager.Instance.GimmickPos);
    }

    public void LoadCheckPointData()
    {

        GameManager.Instance._player.transform.position = _playerPos;
        GameManager.Instance._player.transform.rotation = _playerRot;
        GameManager.Instance.currentPlayerHealth = _playerHp;
        GameManager.Instance.CurScene = _checkPointScene;

        //이거 하니까 아이템 데이터 사라지는 것 확인
        //GameManager.Instance.GameManagerQuickSlots = GameManagerQuickSlots;

        GameManager.Instance.IsGimmickClear = _isGimmickClear;
        GameManager.Instance.GimmickPos = _gimmickPos;

        //GameManager.Instance.CollectedItemIDs = CollectedItemIDs;
        //foreach (Vector3 pos in CollectedItemIDs)
        //{
        //    if (GameManager.Instance.CollectedItemIDs.Contains(pos))
        //    {
        //        continue;
        //    }
        //    GameManager.Instance.CollectedItemIDs.Add(pos);
        //}

        //for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        //{
        //    if (GameManagerQuickSlots[i].Data != null)
        //    {
        //        Debug.Log("아이템 : " + GameManagerQuickSlots[i].Data.id);
        //    }
        //    GameManager.Instance.UpdateQuickSlot(i, GameManagerQuickSlots[i]);
        //}
        //아이템 저장
        //for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        //{
        //    if (GameManagerQuickSlots[i].Data != null)
        //    {
        //        ItemData data = FindItem(GameManager.Instance.GameManagerQuickSlots[i].Data.id);
        //        GameManager.Instance.GameManagerQuickSlots[i].Init(data, GameManagerQuickSlots[i].Count);
        //        GameManager.Instance.UpdateQuickSlot(i, GameManagerQuickSlots[i]);
        //    }
        //}

        //id복원
        GameManager.Instance.CollectedItemIDs.Clear();
        foreach (Vector3 pos in CollectedItemIDs)
        {
            GameManager.Instance.CollectedItemIDs.Add(pos);
        }

        //퀵슬롯 복원
        for (int i = 0; i < GameManager.Instance.GameManagerQuickSlots.Length; i++)
        {
            GameManager.Instance.GameManagerQuickSlots[i].Clear();
        }

        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            if (GameManagerQuickSlots[i].Data != null)
            {
                ItemData data = FindItem(GameManagerQuickSlots[i].Data.id);
                GameManager.Instance.GameManagerQuickSlots[i].Init(data, GameManagerQuickSlots[i].Count);
                //이전 Ui가 남는 문제가 있어서 즉시 반영하도록 추가
                GameManager.Instance.UpdateQuickSlot(i, GameManager.Instance.GameManagerQuickSlots[i]);
            }
        }
        GameManager.Instance.HeartLogic();
        GameManager.Instance.RefreshAllQuickSlotUI();
    }

    public ItemData FindItem(int id)
    {
        foreach (ItemData item in _data)
        {
            if(item.id == id)
            { 
                return item; 
            }
        }
        return null;
    }
    public void Clear()//초기화를 메서드로 만듬
    {
        CollectedItemIDs.Clear();//아이템픽업 관련 초기화
        //기믹 관련 초기화
        _isGimmickClear.Clear();
        _gimmickPos.Clear();
    }
}
