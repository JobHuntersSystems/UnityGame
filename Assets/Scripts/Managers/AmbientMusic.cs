using UnityEngine;
using UnityEngine.UI;

public class AmbientMusic : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private AudioSource audioSource;
    private const string VolumeKey = "MusicVolume";

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.4f);
        audioSource.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    private void OnSliderChanged(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
    }
}
