using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMain : RobotBehaviour
{
    [SerializeField] ParticleSystem _effects;
    [SerializeField] Door _chagneState;
    [SerializeField] AudioSource _ad;

    public override void OnCorrectylBothArm(RobotArm[] structur)
    {
        base.OnCorrectylBothArm(structur);

        FinishPuzzle();
        Debug.Log("You Finish Puzzle");
    }

    private void FinishPuzzle()
    {
        _effects.Play();
        _chagneState.UnlockKey();
        _ad.Play();
    }
}
