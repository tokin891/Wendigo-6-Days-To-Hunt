using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    PhotonView pv;

    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
    }

    private void Update()
    {
        if (target != null && pv.IsMine)
            transform.LookAt(target);
    }
}
