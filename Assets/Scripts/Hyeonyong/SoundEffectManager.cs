using UnityEngine;

public class SoundEffectManager:Singleton<SoundEffectManager>
{
    [SerializeField] private AudioSource _bgmAudio;
    [SerializeField] private AudioSource _effectAudio;

    //BGM 실행
    public void PlayBGM(AudioClip audio)
    {
        _bgmAudio.clip = audio;
        _bgmAudio.Play();
    }
    //효과음은 한 번에 여러개 들릴 수 있으니 PlayOneShot 활용
    public void PlayEffectSound(AudioClip audio)
    {   
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
