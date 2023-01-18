using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Safe : MonoBehaviour
{
    [Header("Details")]
    [HideInInspector]
    public bool isUsePlayer = false;
    [HideInInspector]
    public bool isfree = false;
    [HideInInspector]
    public bool isOpen = false;
    [SerializeField]
    int code;

    [SerializeField, Header("Object")]
    TMP_Text textCode;
    [SerializeField]
    Animator DoorAnm;
    [SerializeField]
    GameObject objectCanvas;

    private PhotonView PV;
    public Game.Player.CharacterMove playerD;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        textCode.text = "";
    }
    private void Update()
    {
        if(playerD != null)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInputCode();
            }
        }
    }
    public void AddNumber(int number)
    {
        if (textCode.text.Length < 4)
        {
            textCode.text += number.ToString();
            Debug.Log("Input is " + textCode.text);
        }
        else
            Debug.Log("Enough Number");
    }
    public void Delete() => textCode.text = "";
    public void Enter() => TryToOpen();

    public void InteractWithDoor()
    {
        if (isOpen)
            PV.RPC(nameof(CloseDoor), RpcTarget.All);
        if (!isOpen)
            PV.RPC(nameof(OpenDoor), RpcTarget.All);
    }
    public void EnterInputCode(Game.Player.CharacterMove player)
    {
        PV.RPC(nameof(EnterInputCode_RPC), RpcTarget.All);

        playerD = player;
        objectCanvas.SetActive(true);
    }
    public void CloseInputCode()
    {
        PV.RPC(nameof(CloseInputCode_RPC), RpcTarget.All);

        objectCanvas.SetActive(false);
        playerD.dontMoveAndRotate = false;
        playerD.SetCursor(false);
        playerD = null;
    }

    #region System
    private void TryToOpen()
    {
        if (textCode.text == code.ToString())
        {
            PV.RPC(nameof(OpenIt), RpcTarget.AllBuffered);
            CloseInputCode();
        }
        else
            Debug.Log("Wrong Code");
    }
    [PunRPC]
    private void OpenIt() => isfree = true;

    [PunRPC]
    private void EnterInputCode_RPC()
    {
        isUsePlayer = true;
    }
    [PunRPC]
    private void CloseInputCode_RPC()
    {
        isUsePlayer = false;
    }


    [PunRPC]
    private void OpenDoor()
    {
        DoorAnm.SetBool("isOpen", true);
        isOpen = true;
    }
    [PunRPC]
    private void CloseDoor()
    {
        DoorAnm.SetBool("isOpen", false);
        isOpen = false;
    }
    #endregion
}
