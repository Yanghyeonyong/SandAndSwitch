using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager_Hyeonyong : Singleton<GameManager_Hyeonyong>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //이후엔 실제 퀵슬롯에서 원하는 타입의 아이템이 있는지를 체크할 예정
    [SerializeField] bool _checkItem;
    public bool CheckItem => _checkItem;
    [SerializeField] bool _onProgressGimmick=false;
    PlayerTest playerTest;
    public PlayerTest PlayerTest => playerTest;
    public bool OnProgressGimmick
    {
        get { return _onProgressGimmick; }
        set { _onProgressGimmick = value; }
    }

    private Dictionary<int, bool> _isGimmickClear = new Dictionary<int, bool>();
    public Dictionary<int, bool> IsGimmickClear => _isGimmickClear;

    void Start()
    {

        //해당 값을 찾는 기능은 현재 테스트 용으로  start에 있으나, 이후 플레이어 스폰 지점으로 이동 예정
        playerTest = GameObject.FindFirstObjectByType<PlayerTest>().GetComponent<PlayerTest>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    int _sceneNum = 1;
    public void SceneChange(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (_sceneNum == 1)
            {
                _sceneNum = 0;
                SceneManager.LoadScene(0);
            }
            else if (_sceneNum == 0)
            {
                _sceneNum = 1;
                SceneManager.LoadScene(1);
            }
        }
    }
}
