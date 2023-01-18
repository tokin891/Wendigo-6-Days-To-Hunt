using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickupSystem : MonoBehaviour
{
    public ItemGame index;
    public int howMany = 1;

    public ItemGame[] indexMultiply;
    public int[] howManyMultiply;

    public bool onClick = false;
    public bool isImportant = false;
    public bool isMultiply = false;
    public bool isEnabled = true;

    PhotonView PV;
    CameraHit cm = null;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Pickup(CameraHit _cm)
    {
        PV.RPC(nameof(Pickup_Master), RpcTarget.AllBuffered);

        cm = _cm;
    }
    [PunRPC]
    public void Pickup_Master()
    {
        Destroy(gameObject);
        onClick = true;
    }
    public void SetupObject(int id, int hw)
    {
        PV.RPC(nameof(SetupObject_RPC), RpcTarget.AllBuffered, id, hw);
    }
    public void SetupMultiplayObject(Dictionary<ItemGame,int> index)
    {
        
    }
    [PunRPC]
    public void SetupObject_RPC(int id, int _howMany)
    {
        index = RoomManager.GetItemByID(id);
        howMany = _howMany;
    }
    [PunRPC]
    public void SetupMultiplyObject_RPC(Dictionary<int, int> index)
    {
        isMultiply = true;
    }

    private void OnDestroy()
    {
        if(cm != null && onClick)
        {
            cm.GetComponentInChildren<Inventory>().AddItem(index, howMany);
        }
    }
}
