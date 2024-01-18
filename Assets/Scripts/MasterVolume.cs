using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolume : MonoBehaviour
{
    public Slider VolumeSlider;
    float masterVolume = 1.0f;
    public void Example()
    {
        masterVolume = VolumeSlider.value;
        AudioListener.volume = masterVolume;
    }
    public void AdjustVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
    }
}
