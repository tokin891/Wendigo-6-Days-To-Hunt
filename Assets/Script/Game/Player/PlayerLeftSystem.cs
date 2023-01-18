using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerLeftSystem : MonoBehaviourPunCallbacks
{
    [Header("Message Item")]
    [SerializeField] Transform _parentMessagePlayer;
    [SerializeField] MessageItem _prefabMessagePlayer;

    private void ShowMessagePlayerLeft(Player player_)
    {
        MessageItem mess = Instantiate(_prefabMessagePlayer, _parentMessagePlayer);
        mess.SetupPlayerMessage(player_);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {

        }
        else
        {
            ShowMessagePlayerLeft(otherPlayer);
        }
    }
}
