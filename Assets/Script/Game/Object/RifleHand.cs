using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class RifleHand : MonoBehaviour
{
    //---------- Public
    [Header("Proporties")]
    public ItemGame itemRifle;
    public int id = 0;
    public bool activeStatus = false;

    [SerializeField,Space, Header(
        "Objects")] GameObject Object;

    [SerializeField, Header("Events")]
    UnityEvent _onShot;

    //---------- Private
    private PhotonView PV;
    private bool isUse = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        if (itemRifle != null)
            id = itemRifle.ID;
    }
    private void Update()
    {
        if (PV.IsMine)
        {
            Object.SetActive(false);
        }else
            Object.SetActive(isUse);
    }

    public void ChangeStateRifle(bool index)
    {
        PV.RPC(nameof(ChangeStateRifle_RPC), RpcTarget.AllBuffered, index);
        Debug.Log("Change State Rifle");
    }
    public void ShotRifle()
    {
        PV.RPC(nameof(ShotRifle_RPC), RpcTarget.Others);
    }

    #region RPC
    [PunRPC]
    private void ChangeStateRifle_RPC(bool index)
    {
        isUse = index;
        activeStatus = index;
    }
    [PunRPC]
    private void ShotRifle_RPC()
    {
        _onShot.Invoke();
    }
    #endregion
}
