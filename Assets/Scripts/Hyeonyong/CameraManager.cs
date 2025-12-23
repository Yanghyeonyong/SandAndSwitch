using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineCamera _cam;
    CinemachinePositionComposer _camPos;
    GameObject _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = GetComponent<CinemachineCamera>();
        _camPos= GetComponent<CinemachinePositionComposer>();
        StartCoroutine(TrackedPlayer());
    }

    IEnumerator TrackedPlayer()
    {
        while (true)
        {
            if (_player == null)
            {
                _player = GameManager.Instance._player;

            }
            if (_player != null)
            {
                //transform.position = _player.transform.position + new Vector3(0f, 0f, -2f);
                //_camera.transform.position=transform.position;
                break;
            }
            yield return null;
        }
        //transform.position = _player.transform.position + new Vector3(0f,0f,-2f);
        _cam.Follow=_player.transform;

        yield return new WaitForSeconds(0.1f);
        _camPos.Damping = Vector3.one;
    }

}
