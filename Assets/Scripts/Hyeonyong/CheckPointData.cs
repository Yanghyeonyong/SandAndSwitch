using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointData:MonoBehaviour
{
    public Vector3 _playerPos;
    public Quaternion _playerRot;
    public int _playerHp;
    public int _checkPointScene;
    public QuickSlot[] GameManagerQuickSlots = new QuickSlot[10];
    public Dictionary<int, bool> _isGimmickClear = new Dictionary<int, bool>();
    public Dictionary<int, ItemTransform> _gimmickPos = new Dictionary<int, ItemTransform>();
    //아이템 픽업 관련
    public List<Vector3> CollectedItemIDs = new List<Vector3>();

    public CheckPointData() 
    {
        GameManagerQuickSlots = new QuickSlot[10];
        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            GameManagerQuickSlots[i] = new QuickSlot();
        }

        _playerPos = GameManager.Instance._player.transform.position;
        _playerRot = GameManager.Instance._player.transform.rotation;
        _playerHp =GameManager.Instance.currentPlayerHealth;
        _checkPointScene = GameManager.Instance.CurScene;

        //아이템 저장
        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            if (GameManager.Instance.GameManagerQuickSlots[i].Data != null)
            {
                ItemData data = Instantiate(GameManager.Instance.GameManagerQuickSlots[i].Data) as ItemData;
                Debug.Log("아이템 데이터 정보 : " + data.id);
                
                if (data == null)
                {
                    Debug.Log("데이터가 없다");
                }
                GameManagerQuickSlots[i].Init(data, GameManager.Instance.GameManagerQuickSlots[i].Count);
            }
        }
        //이후에 updatequickslot을 호출하여 전부 재설정 가능
        //for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        //{
        //    GameManager.Instance.UpdateQuickSlot(i, GameManagerQuickSlots[i]);
        //}

        //기믹 클리어 여부 복사
        foreach (var d in GameManager.Instance.IsGimmickClear)
        {
            _isGimmickClear.Add(d.Key, d.Value);
        }
        //기믹 위치 정보 복사
        foreach (var d in GameManager.Instance.GimmickPos)
        {
            ItemTransform itemTransform = new ItemTransform(d.Value.position, d.Value.rotation,d.Value.scale);
            _gimmickPos.Add(d.Key, itemTransform);
        }
        //아이템 생성 여부 복사
        foreach (Vector3 pos in GameManager.Instance.CollectedItemIDs)
        { 
            Vector3 itemId = new Vector3(pos.x,pos.y, pos.z);
            CollectedItemIDs.Add(itemId);
        }

    }

    public void LoadCheckPointData()
    {
        GameManager.Instance._player.transform.position = _playerPos;
        GameManager.Instance._player.transform.rotation = _playerRot;
        GameManager.Instance.currentPlayerHealth = _playerHp;
        GameManager.Instance.CurScene = _checkPointScene;
        GameManager.Instance.GameManagerQuickSlots = GameManagerQuickSlots;
        GameManager.Instance.IsGimmickClear = _isGimmickClear;
        GameManager.Instance.GimmickPos = _gimmickPos;
        GameManager.Instance.CollectedItemIDs = CollectedItemIDs;


        for (int i = 0; i < GameManagerQuickSlots.Length; i++)
        {
            GameManager.Instance.UpdateQuickSlot(i, GameManagerQuickSlots[i]);
        }
        GameManager.Instance.HeartLogic();
    }

}
