using UnityEngine;

public class Gimmick_MapDoor : Gimmick
{
    [SerializeField] GameObject _teleportPos;
    InteractiveObject _interactObj;
    //[SerializeField] GameObject _interactiveUI;
    private void Start()
    {
        _interactObj =_teleportPos.GetComponent<InteractiveObject>();
    }
    public override void StartGimmick()
    {

        GameManager.Instance._player.transform.position= _teleportPos.transform.position;
        StartCoroutine( _interactObj.Teleport());
        //_interactiveUI.SetActive(true);
    }


}
