using UnityEngine;

public class Gimmick_MapDoor : Gimmick
{
    [SerializeField] Transform _teleportPos;
    public override void StartGimmick()
    {
        GameManager.Instance._player.transform.position= _teleportPos.position;
    }


}
