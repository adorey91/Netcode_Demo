using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClientMusicPlayer : Singleton<ClientMusicPlayer>
{
    private AudioSource _audioSource;
    [SerializeField] private AudioClip fireClip;

    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayFireball()
    {
        _audioSource.PlayOneShot(fireClip);
    }
}
