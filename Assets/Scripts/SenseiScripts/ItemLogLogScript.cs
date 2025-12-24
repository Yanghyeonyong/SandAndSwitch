using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemLogLogScript : MonoBehaviour
{

    Image _logImage;
    TextMeshProUGUI _logText;
    //Text _logText; 

    void Awake()
    {
        _logImage = GetComponent<Image>();
        _logText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void PickupItemLog(ItemData data)
    {
        if (GameManager.Instance.currentLanguage == Language.KR)
        {
            if (GameManager.Instance.ItemTable[data.id] == null)
            {
                _logText.text = $"ItemTable에 ID {data.id}가 없습니다.";
            }
            else
            {
                _logText.text = GameManager.Instance.ItemTable[data.id].NameText + " 획득 +1";
            }
        }
        else
        {
            if (GameManager.Instance.ItemTable[data.id] == null)
            {
                _logText.text = $"ItemTable에 ID {data.id}가 없습니다.";
            }
            else
            {
                _logText.text = GameManager.Instance.ItemTable[data.id].NameText + " Get +1";
            }
        }
    }

    public void UseItemLog(ItemData data, int usecount)
    {
        if (GameManager.Instance.currentLanguage == Language.KR)
        {
            if (GameManager.Instance.ItemTable[data.id] == null)
            {
                _logText.text = $"ItemTable에 ID {data.id}가 없습니다.";
            }
            else
            {
                _logText.text = GameManager.Instance.ItemTable[data.id].NameText + " 사용 " + usecount;
            }
        }
        else
        {
            if (GameManager.Instance.ItemTable[data.id] == null)
            {
                _logText.text = $"ItemTable에 ID {data.id}가 없습니다.";
            }
            else
            {
                _logText.text = GameManager.Instance.ItemTable[data.id].NameText + " Use " + usecount;
            }
        }
    }
    public void SetLogText(string text)
    {
        _logText.text = text;
    }


    public void SetLogAlpha(Color color)
    {
        //Color color = _logImage.color;
        //color.a = alpha;
        _logImage.color = color;
        _logText.color = color;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
