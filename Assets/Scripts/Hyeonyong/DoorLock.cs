using System.Collections;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DoorLock : MonoBehaviour
{
    AudioSource _audioSource;
    // 0 : 오답 1 : 정답 2: 버튼 클릭
    [SerializeField] AudioClip[] _audio;
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
    [SerializeField] string _password;
    [SerializeField] StringBuilder _curPassword = new StringBuilder();
    Button[] _numButtons;
    Button _submitButton;

    [SerializeField] GameObject _testObject;

    Gimmick_DoorLock _gimmick;

    [SerializeField] int _selectCount = 4;
    [SerializeField] int _curCount = 0;

    Gimmick_Object _obj;

    //[SerializeField] GameObject _interactiveUI;

    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        int _count = transform.childCount;
        //리스트로 Add를 통해 추가해도 되지만 List의 경우 정확한 Capacity가 아닌 더 많은 할당량을 적용하여 최적화를 위해 정확한 크기를 가진 배열 사용
        _numButtons = new Button[_count];

        for (int i = 0; i < _count; i++)
        {
            int index = i;
            if (i == _count - 1)
            {
                _numButtons[i] = transform.GetChild(i).GetComponent<Button>();
                _numButtons[i].onClick.AddListener(() => SetPassword(0));
            }
            _numButtons[i] = transform.GetChild(i).GetComponent<Button>();
            _numButtons[i].onClick.AddListener(() => SetPassword(index + 1));

        }
    }
    void SetPassword(int pressNum)
    {
        _curCount++;
        //PlaySound(_audio[2]);
        SoundEffectManager.Instance.PlayEffectSound(_audio[2]);
        //_curPassword = pressNum;
        _curPassword.Append(pressNum.ToString());
        if (_curCount == _selectCount)
        {
            checkPassword();
        }
    }

    public void InitDoorLock(Gimmick_DoorLock gimmick)
    {
        _gimmick = gimmick;
        _password = _gimmick.Password;
        _testObject = _gimmick.DoorObject;
        _obj = _gimmick.Obj;
    }

    void checkPassword()
    {
        //Debug.Log($"현재 입력 {_curPassword} 정답 : {_password}");
        ////패스워드 정답시 발동할 코드
        //if (_curPassword == _password)
        if(_curPassword.ToString().Equals(_password))
        {
            //PlaySound(_audio[1]);
            SoundEffectManager.Instance.PlayEffectSound(_audio[1]);
            GameManager.Instance.IsGimmickClear[_gimmick.GimmickId] = true;
            _testObject.SetActive(false);

            _gimmick.IsClear = true;
            _gimmick = null;
            GameManager.Instance.OnProgressGimmick = false;
            GameManager.Instance.Player.CurGimmick = null;
            GameManager.Instance.ResumeGame();
            gameObject.SetActive(false);
            _obj.TurnOn();
            //StartCoroutine(CollectPassword());
            return;
            //Debug.Log(GameManager.Instance.IsGimmickClear[_gimmick.GimmickId]);
        }

        //PlaySound(_audio[0]);
        SoundEffectManager.Instance.PlayEffectSound(_audio[0]);
        //GameManager.Instance.OnProgressGimmick = false;
        //GameManager.Instance.Player.CurGimmick = null;
        //GameManager.Instance.ResumeGame();
        _gimmick.ExitGimmick();
        gameObject.SetActive(false);

    }

    private void PlaySound(AudioClip audio)
    {
        _audioSource.clip = audio;
        _audioSource.Play();
    }

    IEnumerator CollectPassword()
    {
        Debug.Log("도어락 사운드 실행");
        PlaySound(_audio[1]);
        GameManager.Instance.IsGimmickClear[_gimmick.GimmickId] = true;
        _testObject.SetActive(false);

        _gimmick.IsClear = true;
        _gimmick = null;
        GameManager.Instance.OnProgressGimmick = false;
        yield return new WaitForSeconds(_audioSource.clip.length);
        _audioSource.clip = null;
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        _curCount = 0;
        _curPassword.Clear();
        _password = null;
        //GameManager.Instance.OnProgressGimmick = false;
        //GameManager.Instance.Player.CurGimmick = null;
        //GameManager.Instance.ResumeGame();

    }

}
