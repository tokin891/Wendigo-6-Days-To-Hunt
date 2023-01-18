using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CarTask : MonoBehaviour
{
    //-------------Private Details
    private PhotonView pv;

    [HideInInspector] public int Boxs;

    //-------------Inspector Details
    public ItemGame boxItem;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        Boxs = 3;
    }

    public bool GetBox()
    {
        if (Boxs <= 0)
            return false;

        Boxs--;
        pv.RPC(nameof(SetBoxs), RpcTarget.OthersBuffered, Boxs);

        return true;
    }

    [PunRPC]
    private void SetBoxs(int index)
    {
        Boxs = index;
    }
}
