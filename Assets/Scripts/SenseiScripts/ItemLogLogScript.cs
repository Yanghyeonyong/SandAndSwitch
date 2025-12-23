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
        _logText.text = data.itemName + " È¹µæ+1";
    }

    public void UseItemLog(ItemData data, int usecount)
    {
        _logText.text = data.itemName + " »ç¿ë" + usecount;
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
