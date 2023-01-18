using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LauncherLobby : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text codeText;
    [SerializeField] UnityEngine.UI.Button buttonStart;
    [SerializeField] GameObject errorPlayers;

    private void Awake()
    {
        codeText.text = "Code: " + PhotonNetwork.CurrentRoom.Name;
    }
    private void Update()
    {
        buttonStart.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        if(PhotonNetwork.IsMasterClient)
        {
            buttonStart.interactable = CheckAllBool();
        }

        if (CheckPlayerCharacters() == false)
        {
            errorPlayers.SetActive(true);
        }
        else
            errorPlayers.SetActive(false);
    }
    bool CheckAllBool()
    {
        if (CheckReady() && CheckPlayerCharacters() && FindObjectOfType<PlayerListItem>() != null)
            return true;

        return false;
    }
    private bool CheckReady()
    {
        PlayerListItem[] allPlayersInLobby = FindObjectsOfType<PlayerListItem>();
        foreach(PlayerListItem one in allPlayersInLobby)
        {
            if(!one.isReady)
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckPlayerCharacters()
    {
        List<PlayerListItem> playersJustin = new List<PlayerListItem>();
        List<PlayerListItem> playersSunil = new List<PlayerListItem>();
        List<PlayerListItem> playersMayank = new List<PlayerListItem>();
        List<PlayerListItem> playersPraveen = new List<PlayerListItem>();

        foreach(PlayerListItem one in FindObjectsOfType<PlayerListItem>())
        {
            if(one._Character == Character.JustinLeblanc)
            {
                playersJustin.Add(one);
            }
            if (one._Character == Character.SunilMoore)
            {
                playersSunil.Add(one);
            }
            if (one._Character == Character.MayankRoss)
            {
                playersMayank.Add(one);
            }
            if (one._Character == Character.PraveenLeblanc)
            {
                playersPraveen.Add(one);
            }
        }

        if (playersJustin.Count > 1)
            return false;
        if (playersSunil.Count > 1)
            return false;
        if (playersMayank.Count > 1)
            return false;
        if (playersPraveen.Count > 1)
            return false;

        Debug.Log(playersJustin.Count);

        return true;
    }

    public void CopyCode()
    {
        TextEditor te = new TextEditor();
        te.text = PhotonNetwork.CurrentRoom.Name;
        te.SelectAll();
        te.Copy();
    }
    public void Play()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        GetPlayerByPV().StartGame();

        PhotonNetwork.LoadLevel(2);
    }

    public void QuitRoom()
    {
        RoomManager.Instance.DisconnectPlayer();
    }

    public void AddCharacter()
    {
        GetPlayerByPV().CharacterAdd();
    }
    public void LessCharacter()
    {
        GetPlayerByPV().CharacterLess();
    }
    public void ChagenReady()
    {
        GetPlayerByPV().ChangeReady();
    }

    private PlayerListItem GetPlayerByPV()
    {
        PhotonView[] allPV = FindObjectsOfType<PhotonView>();

        foreach(PhotonView one in allPV)
        {
            if(one.IsMine)
            {
                PlayerListItem player = one.GetComponent<PlayerListItem>();
                if(one != null)
                {
                    return player;
                }
            }
        }

        return null;
    }
}
