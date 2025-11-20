using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip bgm;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;    
        audioSource.clip = bgm;    
        audioSource.volume = .6f;     

        audioSource.Play();         
    }
}
