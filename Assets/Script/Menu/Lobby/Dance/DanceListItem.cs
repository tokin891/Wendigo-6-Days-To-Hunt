using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DanceListItem : MonoBehaviour
{
    public DanceDetails _structur;

    private void Update()
    {
        if(_structur != null)
        {
            GetComponentInChildren<TMPro.TMP_Text>().text = _structur.GetNameDance;
        }else
            GetComponentInChildren<TMPro.TMP_Text>().text = "";
    }

    public void SetStructure(DanceDetails index)
    {
        _structur = index;
    }
    public void Interact()
    {
        Debug.Log("Dance");
        if (_structur == null)
            return;

        GetPlayerListItem().DanceAboutID(_structur.idDance);
    }

    private PlayerListItem GetPlayerListItem()
    {
        PlayerListItem[] allPlayers = FindObjectsOfType<PlayerListItem>();
        foreach (PlayerListItem item in allPlayers)
        {
            if (item.photonView.Owner.UserId == PhotonNetwork.LocalPlayer.UserId)
            {
                Debug.Log(item);
                return item;
            }
        }

        return null;
    }
}
