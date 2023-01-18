using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnm : MonoBehaviour
{
    [SerializeField] AudioSource clipPlay;
    [SerializeField] AudioClip[] clip;

    public void PlaySoundsFootstep()
    {
        if (GetComponentInParent<CharacterController>().isGrounded == false)
            return;

        GetComponentInParent<Game.Player.CharacterMove>().FootstepPlay();
        clipPlay.clip = clip[Random.Range(0, clip.Length)];
        clipPlay.Play();
    }
}
