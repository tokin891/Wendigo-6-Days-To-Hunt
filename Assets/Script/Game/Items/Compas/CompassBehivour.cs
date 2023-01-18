using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompassBehivour : MonoBehaviour
{
    private Transform mainPlayer;
    private Vector3 dir;

    public ref readonly Vector3 getDir => ref dir;

    private void Awake()
    {
        mainPlayer = transform.GetComponentInParent<Game.Player.CharacterMove>().transform;
    }
    private void Update()
    {
        dir.z = mainPlayer.eulerAngles.y;
    }
}
