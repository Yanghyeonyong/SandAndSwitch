using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemLogCanvasScript : MonoBehaviour
{
    List<ItemLogLogScript> _itemLogs = new List<ItemLogLogScript>();

    [SerializeField] List<float> _positionZeroToFour= new List<float>();

    void Awake()
    {



        if (_positionZeroToFour.Count < 5)
        {
            _positionZeroToFour = new List<float>() { -344f, -290f, -236f, -182f, -128f};
        }



        foreach (Transform child in transform)
        {
            ItemLogLogScript log = child.GetComponent<ItemLogLogScript>();
            if (log != null)
            {
                _itemLogs.Add(log);
            }
        }


    }

    void Start()
    {
        foreach (var log in _itemLogs)
        {
            log.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            log.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        }
        GameManager.Instance.ItemLogs = _itemLogs;
        GameManager.Instance.ItemLogCanvas = this;
    }

    [SerializeField] float _moveUpSpeed = 2f;

    [SerializeField] float _closeEnoughThreshold = 0.1f;
    WaitForSecondsRealtime _moveUpTime = new WaitForSecondsRealtime(0.02f);

    Queue<ItemData> _itemDataQueue = new Queue<ItemData>();
    Queue<int> _itemCountQueue = new Queue<int>();


    public void PickupOrUseLogic(ItemData data, int count)
    {
        _itemDataQueue.Enqueue(data);
        _itemCountQueue.Enqueue(count);

    }


    IEnumerator MoveUp()
    {
        List<float> _targetPosition = new List<float>();
        GameObject[] _tempLogs = new GameObject[5];

        foreach (var log in _itemLogs)
        {
            if (log.gameObject.GetComponent<RectTransform>().anchoredPosition.y == _positionZeroToFour[4])
            {
                _tempLogs[4] = log.gameObject;
            }
            else if (log.gameObject.GetComponent<RectTransform>().anchoredPosition.y == _positionZeroToFour[3])
            {
                _tempLogs[3] = log.gameObject;
            }
            else if (log.gameObject.GetComponent<RectTransform>().anchoredPosition.y == _positionZeroToFour[2])
            {
                _tempLogs[2] = log.gameObject;
            }
            else if (log.gameObject.GetComponent<RectTransform>().anchoredPosition.y == _positionZeroToFour[1])
            {
                _tempLogs[1] = log.gameObject;
            }
            else if (log.gameObject.GetComponent<RectTransform>().anchoredPosition.y == _positionZeroToFour[0])
            {
                _tempLogs[0] = log.gameObject;
                if (_itemCountQueue.Peek()>0)
                {
                    log.PickupItemLog(_itemDataQueue.Dequeue());
                    _itemCountQueue.Dequeue();
                }
                else
                {
                    log.UseItemLog(_itemDataQueue.Dequeue(), _itemCountQueue.Dequeue());
                }
            }
            //_tempLogs.Add(log.gameObject);
        }





        for (int i =0; i <_tempLogs.Length; i++)
        {
            _targetPosition.Add(_tempLogs[i].GetComponent<RectTransform>().anchoredPosition.y + (_positionZeroToFour[2] -_positionZeroToFour[1]));

        }

        while (_targetPosition[0] - _tempLogs[0].GetComponent<RectTransform>().anchoredPosition.y > _closeEnoughThreshold)
        {
            for (int i = 0; i < _tempLogs.Length; i++)
            {
                _tempLogs[i].GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(_tempLogs[i].GetComponent<RectTransform>().anchoredPosition, new Vector2(_tempLogs[i].GetComponent<RectTransform>().anchoredPosition.x, _targetPosition[i]),_moveUpSpeed);
                if (_tempLogs[i].GetComponent<RectTransform>().anchoredPosition.y > _positionZeroToFour[3])
                {
                    Color color = _tempLogs[i].GetComponent<Image>().color;
                    if (color.a > 0.2f)
                    {
                        color.a -= 0.2f;
                    }
                    _tempLogs[i].GetComponent<Image>().color = color;
                    _tempLogs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color;
                }

                else if(_tempLogs[i].GetComponent<RectTransform>().anchoredPosition.y <= _positionZeroToFour[1])
                {
                    Color color = _tempLogs[i].GetComponent<Image>().color;

                    if (color.a < 0.8f)
                    {
                        color.a += 0.2f;
                    }
                    _tempLogs[i].GetComponent<Image>().color = new Color (1f,1f,1f,1f);
                    _tempLogs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 1f);
                }


            }


            yield return _moveUpTime;
        }

        for (int i = 0; i < _tempLogs.Length; i++)
        {
            _tempLogs[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(_tempLogs[i].GetComponent<RectTransform>().anchoredPosition.x, _targetPosition[i]);

            

        }


        _tempLogs[4].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        _tempLogs[4].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        _tempLogs[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(_tempLogs[4].GetComponent<RectTransform>().anchoredPosition.x, _positionZeroToFour[0]);
        _tempLogs[3].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        _tempLogs[3].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 0f);
        _tempLogs[0].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        
        


        _moveCoroutine = null;
    }

    //bool _isMoving = false;

    Coroutine _moveCoroutine;

    [SerializeField] float _totalInvisibleTime = 2f;

    //List<ItemLogLogScript> _activeLogs = new List<ItemLogLogScript>();

    float _invisibleTimer = 0f;
    

    // Update is called once per frame
    void Update()
    {
        


        if (_itemDataQueue.Count > 0 && _moveCoroutine == null)
        {

            _invisibleTimer = _totalInvisibleTime;
            //_isMoving = true;
            _moveCoroutine = StartCoroutine(MoveUp());
        }
        _invisibleTimer -= Time.deltaTime;

        if (_invisibleTimer < 0f)
        {

            foreach (var log in _itemLogs)
            {
                if (log.GetComponent<Image>().color.a >= 0f)
                {
                    Color color = log.GetComponent<Image>().color;
                    color.a -= Time.deltaTime;

                    log.SetLogAlpha(color);


                }
            }

        }


    }
}
