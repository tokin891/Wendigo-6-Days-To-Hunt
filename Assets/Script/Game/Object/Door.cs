using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public enum isKeyLock
{
    No,
    Yes
}
public class Door : MonoBehaviour
{
    private PhotonView PV;

    [SerializeField] Transform pointClose;
    [SerializeField, Tooltip
        ("Close door on awake")] Transform pointOpen;
    [SerializeField] float smoothWayDoor;
    [SerializeField] bool isRotation = true;

    [SerializeField, Header("Events"), Space]
    UnityEvent _onDoorOpenClose;
    public isKeyLock _Lock = new isKeyLock();
    public ItemGame _keyItem;

    [HideInInspector] public bool isOpen = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        pointClose.position = transform.position;
        pointClose.rotation = transform.rotation;
    }
    private void Update()
    {
        if(_Lock == isKeyLock.No)
        {
            if(isRotation)
            {
                if (isOpen)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, pointOpen.rotation, smoothWayDoor * Time.deltaTime);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, pointClose.rotation, smoothWayDoor * Time.deltaTime);
                }
            }else
            {
                if (isOpen)
                {
                    transform.position = Vector3.Lerp(transform.position, pointOpen.position, smoothWayDoor * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, pointClose.position, smoothWayDoor * Time.deltaTime);
                }
            }
        }
        else
        {
            isOpen = false;
            if(isRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, pointClose.rotation, smoothWayDoor * Time.deltaTime);
            }else
                transform.position = Vector3.Lerp(transform.position, pointClose.position, smoothWayDoor * Time.deltaTime);
        }          
    }

    public void UnlockKey()
    {
        PV.RPC(nameof(UnlockKey_RPC), RpcTarget.AllBuffered);
    }
    [PunRPC]
    private void UnlockKey_RPC()
    {
        _Lock = isKeyLock.No;
    }
    public void ChangeStateDoor()
    {
        PV.RPC(nameof(ChangeStateDoor_Master), RpcTarget.MasterClient);
    }
    [PunRPC]
    private void ChangeStateDoor_Master()
    {
        if (isOpen)
        { isOpen = false; } else
            isOpen = true;

        _onDoorOpenClose.Invoke();
        PV.RPC(nameof(ChangeStateDoor_RPC), RpcTarget.OthersBuffered, isOpen);
    }
    [PunRPC]
    private void ChangeStateDoor_RPC(bool state)
    {
        _onDoorOpenClose.Invoke();
        isOpen = state;
    }
}
