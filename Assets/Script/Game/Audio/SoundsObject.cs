using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsObject : MonoBehaviour
{
    [SerializeField] PlayerPlace _placeCheck;
    [SerializeField] float smoothDamp = 18;
    [SerializeField] float maxVolume;
    float yRef = 0.0f;

    AudioSource[] allAD;
    private void Awake()
    {
        allAD = GetComponentsInChildren<AudioSource>();
    }
    private void Update()
    {
        GameManager _gm = FindObjectOfType<GameManager>();


        if (_gm._playerPlace == _placeCheck)
        {
            OpenSounds();
        }
        else
            CloseSounds();
    }

    private void OpenSounds()
    {
        for (int i = 0; i < allAD.Length; i++)
        {
            allAD[i].volume = Mathf.SmoothDamp(0, maxVolume, ref yRef, smoothDamp * Time.deltaTime);
        }
    }
    private void CloseSounds()
    {
        for (int i = 0; i < allAD.Length; i++)
        {
            allAD[i].volume = Mathf.SmoothDamp(maxVolume, 0, ref yRef, smoothDamp * Time.deltaTime);
        }
    }
}
