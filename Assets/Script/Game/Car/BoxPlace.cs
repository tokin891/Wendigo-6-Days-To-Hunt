using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum isDrop
{
    No,
    Yes
}
public enum typeItemBox
{
    Camera,
    Trap,
    Flashlight
}
public class BoxPlace : MonoBehaviour
{
    private PhotonView PV;
    public isDrop _isdrop = new isDrop();
    public typeItemBox _TypeItemInBox = new typeItemBox();
    public int howManyYouCanPick = 1;
    public ItemGame typeItem;
    public ItemGame removeItem;

    [SerializeField] Material isTriggerM;
    [SerializeField] Material isRealM;

    private void Awake()
    {
        _isdrop = isDrop.No;
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(_isdrop == isDrop.Yes)
        {
            GetComponent<MeshRenderer>().material = isRealM;
            GetComponent<BoxCollider>().isTrigger = false;
        }else
        {
            GetComponent<MeshRenderer>().material = isTriggerM;
        }
    }

    public bool ChangeTrigger()
    {
        PV.RPC(nameof(ChangeTrigger_RPC), RpcTarget.AllBuffered);
        return true;
    }
    [PunRPC]
    private void ChangeTrigger_RPC()
    {
        _isdrop = isDrop.Yes;
    }
}
