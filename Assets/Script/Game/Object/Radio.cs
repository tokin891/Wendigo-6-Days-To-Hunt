using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class Radio : MonoBehaviour
{
    [HideInInspector] public bool isOn = false;
    [SerializeField,Space] UnityEvent _ifRadioOn;
    [SerializeField] UnityEvent _ifRadioOff;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if(isOn)
        {
            _ifRadioOn.Invoke();
        }else
        {
            _ifRadioOff.Invoke();
        }
    }

    public void ChangeAvtivity()
    {
        pv.RPC(nameof(ChangeAvtivity_RPC), RpcTarget.All, !isOn);
    }
    [PunRPC]
    private void ChangeAvtivity_RPC(bool index)
    {
        isOn = index;
        transform.Find("Click").GetComponent<AudioSource>().Play();
    }
}
