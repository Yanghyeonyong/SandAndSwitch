using JetBrains.Annotations;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Gimmick_DoorLock : Gimmick
{
    //선택지
    [SerializeField] bool _isStageGimmick = false;
    [SerializeField] GameObject _selection;
    public GameObject Selection => _selection;
    [SerializeField] Button[] _selectionButtons;
    //도어락
    [SerializeField] GameObject _doorLockObject;
    public GameObject DoorLockObject => _doorLockObject;    
    [SerializeField] int _password;
    public int Password => _password;
    DoorLock _doorlock;


    [SerializeField] GameObject _interactiveUI;
    [SerializeField] GameObject _testObject;
    public GameObject TestObject => _testObject;
    

    private void Start()
    {
        if (CheckClear())
        {
            //클리어 했으면 클리어된 판정 이벤트 실행
            TestObject.SetActive(false);
            return;
        }
        else
        {
            //CheckClear();
            Debug.Log("실행 자식");
            _doorlock = _doorLockObject.GetComponent<DoorLock>();
            int _count = _selection.transform.childCount;
            if (_count <= 2)
            {
                _isStageGimmick = true;
            }
            _selectionButtons = new Button[_count];
            for (int i = 0; i < _count; i++)
            {
                _selectionButtons[i] = _selection.transform.GetChild(i).GetComponent<Button>();
            }
        }
    }

    public override void StartGimmick()
    {
        if (_isStageGimmick)
        {
            _selectionButtons[0].onClick.AddListener(() => ChoiceGimmick());
            _selectionButtons[1].onClick.AddListener(() => ExitGimmick());
        }
        else
        {
            _selectionButtons[0].onClick.AddListener(() => ChoiceGimmick());
            _selectionButtons[1].onClick.AddListener(() => ChoiceItem());
            _selectionButtons[2].onClick.AddListener(() => ExitGimmick());
        }

        _interactiveUI.SetActive(false);
        _selection.SetActive(true);

        GameManager.Instance.OnProgressGimmick = true;
    }

    public void ChoiceGimmick()
    {
        _selection.SetActive(false);
        _doorLockObject.SetActive(true);
        _doorlock.InitDoorLock(this);
        ResetSelectionButton();
    }
    public void ChoiceItem()
    {
        if (GameManager.Instance.CheckItem)
        {
            _doorLockObject.SetActive(false);
            _testObject.SetActive(false);
            _selection.SetActive(false);

            ResetSelectionButton();
            GameManager.Instance.OnProgressGimmick = false;
        }
    }
    public override void ExitGimmick()
    {
        //_selection.SetActive(false);
        //_interactiveUI.SetActive(true);

        //ResetSelectionButton();
        _doorLockObject.SetActive(false);
        _selection.SetActive(false);
        ResetSelectionButton();
        _interactiveUI.SetActive(true);

        GameManager.Instance.OnProgressGimmick=false;
    }

    public void ResetSelectionButton()
    {
        foreach (var button in _selectionButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void Exit()
    {
        _doorLockObject.SetActive(false);
        _selection.SetActive(false);
        ResetSelectionButton();
        _interactiveUI.SetActive(true);
    }
}
