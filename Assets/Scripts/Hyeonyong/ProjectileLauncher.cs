using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : Gimmick_Object
{
    [SerializeField] GameObject arrow;
    [SerializeField] Transform generatePos;
    List<GameObject> arrows=new List<GameObject>();
    GameObject createdArrow;
    public override void TurnOn()
    {
        createdArrow = ArrowPool();
        if (createdArrow == null)
        {
            createdArrow = Instantiate(arrow, generatePos.transform.position, generatePos.transform.rotation);
            arrows.Add(createdArrow);
        }
        //화살 활성화 및 위치와 각도 설정
        else
        {
            createdArrow.SetActive(true);
            createdArrow.transform.position = generatePos.transform.position;
            createdArrow.transform.rotation = generatePos.transform.rotation;
        }
    }
    public override void TurnOff()
    {
    }

    private GameObject ArrowPool()
    {
        foreach (GameObject arrow in arrows)
        {
            if (!arrow.activeSelf)
            {
                return arrow;
            }
        }
        return null;
    }
}
