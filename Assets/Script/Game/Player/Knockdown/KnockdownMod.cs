using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class KnockdownMod : MonoBehaviourPunCallbacks
{
    List<GameObject> listsCinema = new List<GameObject>();

    public Camera mainCamera;
    public TMP_Text cpText;

    public GameObject windowSpectate;
    public GameObject windowBody;
    [HideInInspector] public GameObject[] playersCam;
    [HideInInspector] public GameObject currentCam;
    [HideInInspector] public int currentWatch = 0;
    [HideInInspector] public bool isSpectate = false;
    bool dontLoop = false;

    public void RightAdd()
    {
        if (currentWatch < playersCam.Length - 1)
        {
            currentCam = playersCam[currentWatch + 1];
            currentWatch += 1;
        }
    }
    public void LeftAdd()
    {
        if (currentWatch > 0)
        {
            currentCam = playersCam[currentWatch - 1];
            currentWatch -= 1;
        }
    }
    public void GoSpectate()
    {
        isSpectate = true;
        windowSpectate.SetActive(true);
        windowBody.SetActive(false);
    }
    public void GoUnspectate()
    {
        isSpectate = false;
        windowSpectate.SetActive(false);
        windowBody.SetActive(true);
    }
    private void Update()
    {
        if(isSpectate == false)
        {
            for (int i = 0; i < listsCinema.Count; i++)
            {
                listsCinema[i].SetActive(false);
            }
            dontLoop = false;
            mainCamera.enabled = true;
        }else if(dontLoop == false)
        {
            //Do spectate
            if (GetObjectCamera() != null)
            {
                playersCam[currentWatch].SetActive(true);

                mainCamera.enabled = false;
            }
            else
                isSpectate = false;           

            currentWatch = 0;

            dontLoop = true;
            return;
        }

        if(isSpectate)
        {
            cpText.text = currentWatch + 1 + "/" + playersCam.Length;
            for (int i = 0; i < currentWatch; i++)
            {
                playersCam[i].SetActive(false);
            }
            for (int i = currentWatch + 1; i < playersCam.Length; i++)
            {
                playersCam[i].SetActive(false);
            }
            playersCam[currentWatch].SetActive(true);
        };
    }
    private void Awake()
    {
        playersCam = GetObjectCamera();
    }
    GameObject[] GetObjectCamera()
    {
        if (listsCinema != null)
            listsCinema.Clear();

        Game.Player.CharacterMove[] allPlayers = FindObjectsOfType<Game.Player.CharacterMove>();
        List<Game.Player.CharacterMove> listsPlayer = new List<Game.Player.CharacterMove>();
        listsPlayer.AddRange(FindObjectsOfType<Game.Player.CharacterMove>());

        foreach (Game.Player.CharacterMove item in allPlayers)
        {
            if(item.PV.Owner.ActorNumber == GetComponentInParent<Game.Player.CharacterMove>().PV.Owner.ActorNumber)
            {
                listsPlayer.Remove(item);

                break;
            }
        }

        for (int i = 0; i < listsPlayer.Count; i++)
        {
            listsCinema.Add(listsPlayer[i].cinemaCam);
        }
        currentWatch = 0;
        currentCam = listsCinema[currentWatch];
        return listsCinema.ToArray();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playersCam = GetObjectCamera();
    }
}
