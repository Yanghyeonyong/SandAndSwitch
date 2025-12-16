using UnityEngine;

public class SoundEffectManager:Singleton<SoundEffectManager>
{
    [SerializeField] private AudioSource _effectAudio;
    //효과음은 한 번에 여러개 들릴 수 있으니 PlayOneShot 활용
    public void PlayEffectSound(AudioClip audio, Vector3 pos)
    {
        transform.position = pos;   
        if (_effectAudio.clip == null)
        {
            _effectAudio.clip = audio;
            _effectAudio.Play();
        }
        else
        {
            _effectAudio.PlayOneShot(audio);
        }
    }
}
