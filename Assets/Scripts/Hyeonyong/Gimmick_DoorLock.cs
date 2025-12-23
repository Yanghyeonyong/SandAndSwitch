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
    [SerializeField] string _password;
    public string Password => _password;
    DoorLock _doorlock;


    [SerializeField] GameObject _interactiveUI;
    [SerializeField] GameObject _doorObject;
    public GameObject DoorObject => _doorObject;

    private Gimmick_Object _obj;
    public Gimmick_Object Obj => _obj;

    //사용에 필요한 아이템 id
    [SerializeField] int _itemId;
    int length = 0;
    private void Start()
    {
        _obj=GetComponent<Gimmick_Object>();
        if (CheckClear())
        {
            //클리어 했으면 클리어된 판정 이벤트 실행
            //TestObject.SetActive(false);
            _obj.TurnOn();
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
        length=GameManager.Instance.GameManagerQuickSlots.Length;
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

        //선택지 활성화 여부 확인
        GameManager.Instance.OnSelection = true;
        _interactiveUI.SetActive(false);
        _selection.SetActive(true);

        GameManager.Instance.OnProgressGimmick = true;

        GameManager.Instance.PauseGame();
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
        int index = CheckQuickSlotItem();
        if (index !=-1)
        {
            GameManager.Instance.IsGimmickClear[GimmickId] = true;
            _isClear = true;
           
            ItemData data = GameManager.Instance.GameManagerQuickSlots[index].Data;
            //퀵슬롯이 아니라 퀵슬롯 컨트롤러를 통해서 진행한다
            //GameObject bombObj = Instantiate(data.prefab, transform.position, Quaternion.identity);
            //bombObj.GetComponent<Bomb>().UseBomb();
            GameManager.Instance.Player.Slot.TryUseCurrentSlot(index);

            _doorLockObject.SetActive(false);
            _obj.TurnOn();
            //_testObject.SetActive(false);
            _selection.SetActive(false);

            ResetSelectionButton();
            GameManager.Instance.OnProgressGimmick = false;
            GameManager.Instance.Player.CurGimmick = null;
            GameManager.Instance.ResumeGame();

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

        GameManager.Instance.ResumeGame();
    }

    public void ResetSelectionButton()
    {
        foreach (var button in _selectionButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        GameManager.Instance.OnSelection = false;
    }

    public void Exit()
    {
        _doorLockObject.SetActive(false);
        _selection.SetActive(false);
        ResetSelectionButton();
        _interactiveUI.SetActive(true);
    }


    //해당 아이템 id가 있으면 true 반환
    private int CheckQuickSlotItem()
    {
        //QuickSlot[] _check = GameManager.Instance.GameManagerQuickSlots;
        for (int i = 0; i < length; i++)
        {
            if (GameManager.Instance.GameManagerQuickSlots[i] != null)
            {
                if (GameManager.Instance.GameManagerQuickSlots[i].Data == null)
                {
                    continue;
                }

                if (GameManager.Instance.GameManagerQuickSlots[i].Data.id == _itemId)
                {
                    return i;
                }

            }
        }
        return -1;
    }

    public override void CheckNum(int num)
    {

        if (num == -1)
        {
            _selectionButtons[2].onClick.Invoke();
        }
        num--;
        _selectionButtons[num].onClick.Invoke();
    }
}
