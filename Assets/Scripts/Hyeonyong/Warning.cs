using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    Image img;
    [SerializeField] float _waitSeconds = 1f;
    WaitForSeconds _wait;
    [SerializeField] CinemachineBasicMultiChannelPerlin m_channels;
    private void Start()
    {
        img = GetComponent<Image>();
        _wait = new WaitForSeconds(_waitSeconds);

        StartCoroutine(WarningEffect());
    }

    IEnumerator WarningEffect()
    {
        m_channels.AmplitudeGain = 1f;
        m_channels.FrequencyGain = 1f;
        //GameObject.SetActive(true);
        img.color=new Color(img.color.r,img.color.g,img.color.b, 0.3f);
        yield return _wait;
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
        m_channels.AmplitudeGain = 0f;
        m_channels.FrequencyGain = 0f;
    }

}
