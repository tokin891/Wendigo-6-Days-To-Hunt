using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotBehaviour : MonoBehaviour
{
    private bool isCorrectArm;
    private bool IsCorrectArm
    {
        set
        {
            isCorrectArm = value;
        }
        get
        {
            return isCorrectArm;
        }
    }
    public ref readonly bool isCorrectBothArm => ref isCorrectArm;

    private void Update()
    {
        RobotArm[] allArms = transform.GetComponentsInChildren<RobotArm>();
        CheckArms(allArms);
    }

    private void CheckArms(RobotArm[] structur)
    {
        if (structur == null && IsCorrectArm == false)
            return;

        foreach(RobotArm one in structur)
        {
            if (one.isCorrect == false)
                return;
            if (one.flareInArm && one.isCorrect)
            {
                if (one.curretlyFlara == false)
                    return;
            }
        }

        OnCorrectylBothArm(structur);
        return;
    }

    public virtual void OnCorrectylBothArm(RobotArm[] structur)
    {
        for (int i = 0; i < structur.Length; i++)
        {
            structur[i].isPossibleToRotate = false;
        }

        IsCorrectArm = true;
    }
}
