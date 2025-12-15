using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DoorLock : MonoBehaviour
{
    //관문, 맵 기믹에 사용
    /// <summary>
    /// 플레이어
    /// </summary>
    ////선택지
    //[SerializeField] bool isStageGimmick = false;
    //[SerializeField] GameObject _selection;
    //[SerializeField] Button[] _selectionButtons;
    ////패스워드
    //[SerializeField] GameObject _doorLock;
    [SerializeField] int _password;
    [SerializeField] int _curPassword=-1;
    Button[] _numButtons;
    Button _submitButton;

    [SerializeField] GameObject _testObject;

    Gimmick_DoorLock _gimmick;

    //[SerializeField] GameObject _interactiveUI;

    public void Awake()
    {
        int _count = transform.childCount;
        //리스트로 Add를 통해 추가해도 되지만 List의 경우 정확한 Capacity가 아닌 더 많은 할당량을 적용하여 최적화를 위해 정확한 크기를 가진 배열 사용
        _numButtons=new Button[_count];

        for (int i = 0; i < _count; i++)
        {
            if (i == _count - 1)
            {
                _submitButton = transform.GetChild(i).GetComponent<Button>();
                _submitButton.onClick.AddListener(() => checkPassword());
                continue;
            }
            int index = i;
            _numButtons[i]= transform.GetChild(i).GetComponent<Button>();
            _numButtons[i].onClick.AddListener(() => SetPassword(index + 1));

        }
    }
    void SetPassword(int pressNum)
    {
        _curPassword = pressNum;
    }

    public void InitDoorLock(Gimmick_DoorLock gimmick)
    {
        _gimmick = gimmick;
        _password = _gimmick.Password;
        _testObject= _gimmick.TestObject;
    }

    void checkPassword()
    {
        Debug.Log($"현재 입력 {_curPassword} 정답 : {_password}");
        //패스워드 정답시 발동할 코드
        if (_curPassword == _password)
        {
            GameManager_Hyeonyong.Instance.IsGimmickClear[_gimmick.GimmickId] = true;
            _testObject.SetActive(false);
            gameObject.SetActive(false);
            
            _gimmick.IsClear = true;
            _gimmick=null;  
            GameManager_Hyeonyong.Instance.OnProgressGimmick=false;
            //Debug.Log(GameManager_Hyeonyong.Instance.IsGimmickClear[_gimmick.GimmickId]);
        }
    }

    //public override void StartGimmick()
    //{
    //    _interactiveUI.SetActive(false);
    //    _selection.SetActive(true);
    //    Debug.Log("기믹 시작 : 도어락");
    //}

    //public void ChoiceGimmick()
    //{
    //    _selection.SetActive(false);
    //    _doorLock.SetActive(true);
    //}
    //public void ChoiceItem()
    //{
    //    if (GamaManager_Hyeonyong.Instance.CheckItem)
    //    {
    //        _doorLock.SetActive(false);
    //        _testObject.SetActive(false);
    //        _selection.SetActive(false);
    //    }
    //}
    //public void ExitGimmick()
    //{
    //    _selection.SetActive(false);
    //    _interactiveUI.SetActive(true);
    //}

}
