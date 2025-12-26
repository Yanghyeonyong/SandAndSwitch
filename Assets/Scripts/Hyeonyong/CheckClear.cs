using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckClear : MonoBehaviour
{
    bool check;
    [SerializeField] int _itemId = 103;
    int length = 0;
    [SerializeField] GameObject _clearPortal;
    [SerializeField] GameObject _phaseTwoUI;
    [SerializeField] string[] _tableId;
    [SerializeField] TextMeshProUGUI[] _text;

    public GameObject PhaseTwoUI => _phaseTwoUI;

    private void Start()
    {
        GameManager.Instance.SenseiCheckClear = this;
        StartCoroutine(CheckItem());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.LoadVictoryScene();
        }
    }

    //해당 아이템 id가 있으면 true 반환
    public int CheckQuickSlotItem()
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

    IEnumerator CheckItem()
    {
        yield return GameManager.Instance != null;
        length = GameManager.Instance.GameManagerQuickSlots.Length;
        if (CheckQuickSlotItem() == -1)
        {
            if (_clearPortal != null)
            {
                _clearPortal.SetActive(false);
            }
        }
        //else
        //{
        //    //만약 _phaseTwoUI가 설정되어져 있지 않다면 이미 이전 단계에서 해당 스크립트를 실행하였다는 뜻
        //    if (_phaseTwoUI != null)
        //    {
        //        for (int i = 0; i < _text.Length; i++)
        //        {
        //            _text[i].text = GetStringFromTable(_tableId[i]);
        //        }

        //        GameManager.Instance.PrimedForPhaseTwo = true;


        //        GameManager.Instance.PauseGame();
        //        _phaseTwoUI.SetActive(true);
        //        GameManager.Instance.CheckItem = true;
        //        yield return new WaitWhile (() => _phaseTwoUI.activeSelf);
        //        GameManager.Instance.ResumeGame();
        //        GameManager.Instance.EnterPhaseTwo();
        //    }
        //}
    }

    public IEnumerator SenseiStartPhaseTwo()
    {
        if (_phaseTwoUI != null)
        {
            for (int i = 0; i < _text.Length; i++)
            {
                _text[i].text = GetStringFromTable(_tableId[i]);
            }
            GameManager.Instance.PauseGame();
            _phaseTwoUI.SetActive(true);
            GameManager.Instance.CheckItem = true;
            yield return new WaitWhile(() => _phaseTwoUI.activeSelf);
            GameManager.Instance.ResumeGame();
            GameManager.Instance.EnterPhaseTwo();
            GameManager.Instance.PhaseTwoCoroutine = null;  
        }
    }

    // CSV 데이터 가져오는 헬퍼 함수
    private string GetStringFromTable(string key)
    {
        // GameManager 싱글톤과 StringTable이 존재하는지 확인
        if (GameManager.Instance != null && GameManager.Instance.StringTable != null)
        {
            // Table 클래스의 인덱서([])를 사용하여 데이터 조회
            var data = GameManager.Instance.StringTable[key];

            if (data != null)
            {
                if (GameManager.Instance.currentLanguage == Language.KR)
                {
                    return data.kr; // StringTableData의 'kr' 변수 값 반환 (예: "아야!")
                }
                else if (GameManager.Instance.currentLanguage == Language.EN)
                {
                    return data.en; // StringTableData의 'en' 변수 값 반환 (예: "Ouch!")
                }
            }
        }

        // 데이터를 못 찾았을 경우 기본값 반환
        return "아야!";
    }
}
