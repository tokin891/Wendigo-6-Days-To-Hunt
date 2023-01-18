using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShotgunPickSystem : MonoBehaviour
{
    [Header("Item")]
    public ItemGame itemShotgun;
    public ItemGame itemForestock;
    public ItemGame itemStructur;

    public bool isForestock = false;
    public bool isStructur = false;

    //----- Materials & Objects
    [Header("Materials & Objects")]
    [SerializeField] Material exist;
    [SerializeField] Material nonExist;
    [SerializeField, Tooltip
        ("0 is Forestock & 1 is Structur")] GameObject[] ObjectMaterials;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (isForestock && isStructur)
        {
            GetComponent<PickupSystem>().enabled = true;
        }

        #region Forestock & Structur
        if (isForestock)
        {
            ObjectMaterials[0].GetComponent<MeshRenderer>().material = exist;
        }else
            ObjectMaterials[0].GetComponent<MeshRenderer>().material = nonExist;

        if (isStructur)
        {
            ObjectMaterials[1].GetComponent<MeshRenderer>().material = exist;
        }
        else
            ObjectMaterials[1].GetComponent<MeshRenderer>().material = nonExist;
        #endregion
    }

    public void GetItem()
    {
        PV.RPC(nameof(GetItem_Master), RpcTarget.All);
    }
    [PunRPC]
    public void GetItem_Master()
    {
        Destroy(gameObject);
    }
    #region Update Forestock & Structur
    public void ForestockApply()
    {
        PV.RPC(nameof(ForestockApply_RPC), RpcTarget.AllBuffered);
    }
    [PunRPC]
    private void ForestockApply_RPC()
    {
        isForestock = true;
    }
    public void StructurApply()
    {
        PV.RPC(nameof(StructurApply_RPC), RpcTarget.AllBuffered);
    }
    [PunRPC]
    private void StructurApply_RPC()
    {
        isStructur = true;
    }
    #endregion
}
