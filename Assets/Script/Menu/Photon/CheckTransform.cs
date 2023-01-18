using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CheckTransform : MonoBehaviour
{
    public bool isFree;
    [SerializeField] PhotonView pv;

    public void Setup()
    {
        pv.RPC("Setup_RPC", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void Setup_RPC()
    {
        isFree = false;
    }
}
