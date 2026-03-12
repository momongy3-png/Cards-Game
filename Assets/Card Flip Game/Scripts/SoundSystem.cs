using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    //singletone property
    public static SoundSystem Instance;

    AudioSource audioSource;

    #region singletone initialize
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }
    #endregion
    #region play sound clips
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    #endregion
}
