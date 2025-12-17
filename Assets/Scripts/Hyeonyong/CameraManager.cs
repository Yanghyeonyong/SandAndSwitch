using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineCamera _cam;
    GameObject _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = GetComponent<CinemachineCamera>();
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
                break;
            }
            yield return null;
        }

        _cam.Follow=_player.transform;
    }

}
