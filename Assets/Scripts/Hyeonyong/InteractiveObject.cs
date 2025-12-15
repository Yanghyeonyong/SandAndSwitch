using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    //플레이어가 근접 및 버튼 클릭 시 상호작용 
    //1. 플레이어와 충돌 시 bool 활성화
    //2. 플레이어는 E 버튼 클릭 시 활성화된 오브젝트에서 해당 객체 정보(other) 가져옴
    //3. 해당 객체 정보에서 Gimmick(부모 클래스) 가져오고 StartGimmick 메서드 실행

    Gimmick _gimmick;
    PlayerTest _playerTest;
    [SerializeField] GameObject _interactiveUI;

    private void Start()
    {
        _gimmick = GetComponent<Gimmick>();
        _playerTest = GameManager_Hyeonyong.Instance.GetComponent<PlayerTest>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_gimmick.IsClear)
        {
            return;
        }
        //E를 눌러 상호작용하라는 ui 활성화
        //플레이어에게 존재하는 bool 활성화
        if (collision.CompareTag("Player"))
        {
            if (_playerTest == null)
            {
                _playerTest = collision.gameObject.GetComponent<PlayerTest>();
            }
            //플레이어와 충돌 시 상호작용 버튼 활성화
            _playerTest.CheckGimmick = true;
            _playerTest.CurGimmick = _gimmick;
            _interactiveUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_playerTest == null) 
        { 
            return ;
        }
        if (_gimmick.IsClear)
        {
            if (_playerTest.CheckGimmick)
            {
                _playerTest.CheckGimmick = false;
                _interactiveUI.SetActive(false);
            }
            return;
        }
        if (collision.CompareTag("Player"))
        {
            //플레이어와 충돌 시 상호작용 버튼 비활성화
            _playerTest.CheckGimmick = false;
            _interactiveUI.SetActive(false);
        }
    }
}
