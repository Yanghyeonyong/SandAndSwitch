using System.Collections;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    //플레이어가 근접 및 버튼 클릭 시 상호작용 
    //1. 플레이어와 충돌 시 bool 활성화
    //2. 플레이어는 E 버튼 클릭 시 활성화된 오브젝트에서 해당 객체 정보(other) 가져옴
    //3. 해당 객체 정보에서 Gimmick(부모 클래스) 가져오고 StartGimmick 메서드 실행

    Gimmick _gimmick;
    Player _player;
    [SerializeField] GameObject _interactiveUI;
    [SerializeField] bool _isReuse=false;
    [SerializeField] bool _isTeleport=false;
    public bool IsReuse
    {
        get { return _isReuse; }
        set { _isReuse = value; }
    }

    //만약 해당 bool 값이 체크되어져 있다면, 2페이즈에서는 작동하지 않는 기믹이라는 뜻
    [SerializeField] bool _CheckItem=false;

    private void Start()
    {
        _gimmick = GetComponent<Gimmick>();
        _player = GameManager.Instance.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.CheckItem && _CheckItem)
        {
            return;
        }
        if (_gimmick.IsClear && !_isReuse)
        {
            return;
        }
        //E를 눌러 상호작용하라는 ui 활성화
        //플레이어에게 존재하는 bool 활성화
        if (collision.CompareTag("Player"))
        {
            if (_player == null)
            {
                _player = collision.gameObject.GetComponent<Player>();
            }
            //플레이어와 충돌 시 상호작용 버튼 활성화
            _player.CheckGimmick = true;
            _player.CurGimmick = _gimmick;
            _interactiveUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_player == null) 
        { 
            return ;
        }

        if (_gimmick.IsClear)
        {
            //NullReferenceExeption 방지를 위해 && _isReuse 추가
            if (_player.CheckGimmick && _isReuse)
            {
                _player.CheckGimmick = false;
                _interactiveUI.SetActive(false);
            }
            return;
        }
        if (collision.CompareTag("Player"))
        {
            //플레이어와 충돌 시 상호작용 버튼 비활성화
            _player.CheckGimmick = false;
            _interactiveUI.SetActive(false);
        }
    }

    public IEnumerator Teleport()
    {
        yield return null;
        if (_player == null)
        {
            _player = GameManager.Instance.Player;
        }
        Debug.Log("텔레포트");
        _player.CheckGimmick = true;
        _player.CurGimmick = _gimmick;
        _interactiveUI.SetActive(true);
        Debug.Log("텔레포트 성공");
    }
    //public void Teleport()
    //{
    //    if (_player == null)
    //    {
    //        _player = GameManager.Instance.Player;
    //    }
    //    Debug.Log("텔레포트");
    //    _player.CheckGimmick = true;
    //    _player.CurGimmick = _gimmick;
    //    _interactiveUI.SetActive(true);
    //    Debug.Log("텔레포트 성공");
    //}
}
