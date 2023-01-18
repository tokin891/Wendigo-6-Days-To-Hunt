using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _players;
    [SerializeField] TMP_Text _private;
    [SerializeField] TMP_Text _inGame;

    [SerializeField] Button _joinButt;

    public RoomInfo info;

    public void SetupRoom(RoomInfo _info)
    {
        info = _info;
    }

    private void Update()
    {
        if (info == null)
            return;

        _name.text = info.Name;
        if (info.MaxPlayers != 1)
        {
            _players.text = info.PlayerCount.ToString() + "/" + info.MaxPlayers.ToString();
        }
        else
            _players.text = "Alone ;-(";

        if (info.IsVisible)
        {
            _private.text = "No";
            if(info.IsVisible)
            {
                _joinButt.interactable = true;
            }
        }
        else
        {
            _private.text = "Yes";
            _joinButt.interactable = false;
        }
        if(info.IsOpen)
        {
            _inGame.text = "in Lobby";
            if (info.IsOpen)
            {
                _joinButt.interactable = true;
            }
        }
        else
        {
            _inGame.text = "in Game";
            _joinButt.interactable = false;
        }
    }

    public void Join()
    {
        FindObjectOfType<RoomListingMenu>().AYS_Join(info);
    }
}
