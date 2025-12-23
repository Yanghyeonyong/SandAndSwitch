using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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
        GameManager.Instance.ItemLogs = _itemLogs;
    }

    [SerializeField] float _moveUpSpeed = 2f;

    [SerializeField] float _closeEnoughThreshold = 0.1f;
    WaitForSecondsRealtime _moveUpTime = new WaitForSecondsRealtime(0.02f);


    IEnumerator MoveUp()
    {
        List<float> _targetPosition = new List<float>();
        for (int i =0; i <_itemLogs.Count; i++)
        {
            _targetPosition.Add(_itemLogs[i].transform.position.y + (_positionZeroToFour[2] -_positionZeroToFour[1]));

        }

        while (_targetPosition[0] - _itemLogs[0].transform.position.y > _closeEnoughThreshold)
        {
            for (int i = 0; i < _itemLogs.Count; i++)
            {
                _itemLogs[i].transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x,_targetPosition[i]),_moveUpSpeed * 0.02f);
            }


            yield return _moveUpTime;
        }

        for (int i = 0; i < _itemLogs.Count; i++)
        {
            _itemLogs[i].transform.position = new Vector2(_itemLogs[i].transform.position.x, _targetPosition[i]);
        }


    }


    

    // Update is called once per frame
    void Update()
    {
        
    }
}
