using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WendigoAnm : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] UnityEvent _OnStep;

    public void Step()
    {
        _OnStep.Invoke();
    }
    public void Attack()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient == false)
            return;
        GetComponentInParent<Game.AI.WendigoAI>().Attack();
    }
}
