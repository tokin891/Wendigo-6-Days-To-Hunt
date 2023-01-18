using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RespawnPlayerLobby : MonoBehaviour
{
    [SerializeField] GameObject playerObject;

    private void Awake()
    {
        if(GetComponent<PhotonView>().IsMine)
        {
            Invoke("SetupPlayer", 1f);
        }
    }

    public void SetupPlayer()
    {
        CheckTransform[] Points = FindObjectsOfType<CheckTransform>();
        foreach (CheckTransform one in Points)
        {
            if (one.isFree)
            {
                PhotonNetwork.Instantiate(playerObject.name, one.transform.position, Quaternion.identity).GetComponent<PlayerListItem>().Setup();
                one.Setup();
                break;
            }
        }
    }
}
