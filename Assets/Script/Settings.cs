using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    public void SetSettings(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void VolumeAll(float index)
    {
        mixer.SetFloat("All", index);
    }
    public void VolumeMusic(float index)
    {
        mixer.SetFloat("Music", index);
    }
}
