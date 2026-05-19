using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    public static AmbientMusic Instance { get; private set; }

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
    }
}
