using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviour
{
    RoomInfo info;
    [SerializeField] GameObject window;
    [SerializeField] TMPro.TMP_Text text_;

    public void AYS_Join(RoomInfo _info)
    {
        info = _info;
        text_.text = "Are you want to join to " + info.Name;

        window.SetActive(true);
    }

    public void yes()
    {
        if (info.MaxPlayers != info.PlayerCount)
        {
            PhotonNetwork.JoinRoom(info.Name);
        }
        else
            no();
    }
    public void no()
    {
        info = null;
        window.SetActive(false);
    }
}
