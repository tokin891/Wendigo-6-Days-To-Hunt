using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView PV;
    [SerializeField] TMP_Text nickNamePlayer;

    [SerializeField] TMP_Text CharacterName;
    [SerializeField,Space] TMP_Text readyText;
    [SerializeField] Color32 colorRed;
    [SerializeField] Color32 colorGreen;

    [Header("Characters"), Space]
    [SerializeField] GameObject Justin;
    [SerializeField] GameObject Sunil;
    [SerializeField] GameObject Mayank;
    [SerializeField] GameObject Praveen;
    [SerializeField] GameObject[] ButtonChange;
    [SerializeField] UnityEngine.UI.Button ButtonReady;
    [SerializeField] Animator[] anm;
    [SerializeField] GameObject _danceWindow;

    [HideInInspector] public bool isReady = false;
    [HideInInspector] public Player player;
    [HideInInspector] public Character _Character = new Character();

    private Vector3 _firstPos;

    private void Awake()
    {
        _firstPos = transform.position;
        Setup();
        if (PV.IsMine == false)
        {
            for (int i = 0; i < ButtonChange.Length; i++)
            {
                ButtonChange[i].SetActive(false);
            }
            ButtonReady.interactable = false;
        }
    }
    private void Update()
    {
        #region text
        TextConvertLanguage readyT = new TextConvertLanguage("Ready", "Gotowy");
        TextConvertLanguage nreadyT = new TextConvertLanguage("Not Ready", "Nie Gotowy");
        #endregion

        if (isReady)
        {
            readyText.text = TextConvertLanguage.GetText(readyT) +  "(R)";
            readyText.color = colorGreen;
            for (int i = 0; i < ButtonChange.Length; i++)
            {
                ButtonChange[i].GetComponent<Button>().interactable = false;
            }
        }else
        {
            readyText.text = TextConvertLanguage.GetText(nreadyT) + "(R)";
            readyText.color = colorRed;
            for (int i = 0; i < ButtonChange.Length; i++)
            {
                ButtonChange[i].GetComponent<Button>().interactable = true;
            }
        }

        if(PV.IsMine)
        {
            if(Input.GetKeyDown(KeyCode.R))
            { ChangeReady(); }

            if (Input.GetKeyDown(KeyCode.B))
                _danceWindow.SetActive(!_danceWindow.activeSelf);
        }
    }
    public void Setup()
    {
        player = PV.Owner;
        nickNamePlayer.text = PV.Owner.NickName;
        CharacterName.text = _Character.ToString();
        Debug.Log(PV.Owner.NickName);
    }
    public void CharacterAdd()
    {
        if((int)_Character < 3)
        {
            _Character = (Character)((int)_Character + 1);
        }else
            _Character = 0;

        PV.RPC("SetCharacter_RPC", RpcTarget.AllBuffered, (int)_Character);
        RoomManager.Instance._Character = this._Character;
    }
    public void CharacterLess()
    {
        if ((int)_Character > 0)
        {
            _Character = (Character)((int)_Character - 1);
        }
        else
            _Character = (Character)3;

        PV.RPC("SetCharacter_RPC", RpcTarget.AllBuffered, (int)_Character);
        RoomManager.Instance._Character = this._Character;
    }
    public void ChangeCharacter()
    {
        if(_Character == Character.JustinLeblanc)
        {
            _Character = Character.MayankRoss;

            PV.RPC("SetCharacter_RPC", RpcTarget.AllBuffered, (int)_Character);
            RoomManager.Instance._Character = this._Character;
            return;
        }
        if (_Character == Character.MayankRoss)
        {
            _Character = Character.PraveenLeblanc;

            PV.RPC("SetCharacter_RPC", RpcTarget.AllBuffered, (int)_Character);
            RoomManager.Instance._Character = this._Character;
            return;
        }
        if (_Character == Character.PraveenLeblanc)
        {
            _Character = Character.SunilMoore;

            PV.RPC("SetCharacter_RPC", RpcTarget.AllBuffered, (int)_Character);
            RoomManager.Instance._Character = this._Character;
            return;
        }
        if (_Character == Character.SunilMoore)
        {
            _Character = Character.JustinLeblanc;

            PV.RPC("SetCharacter_RPC", RpcTarget.AllBuffered, (int)_Character);
            RoomManager.Instance._Character = this._Character;
            return;
        }
    }
    public void ChangeReady()
    {
        PV.RPC("ChangeReady_RPC", RpcTarget.AllBuffered, !isReady);
    }
    [PunRPC]
    public void ChangeReady_RPC(bool ready)
    {
        isReady = ready;
    }

    [PunRPC]
    public void SetCharacter_RPC(int character)
    {
        if(character == 0)
        {
            Justin.SetActive(true);
            Sunil.SetActive(false);
            Mayank.SetActive(false);
            Praveen.SetActive(false);
            if(!PV.IsMine)
            {
                _Character = Character.JustinLeblanc;
            }
        }
        if (character == 1)
        {
            Justin.SetActive(false);
            Sunil.SetActive(true);
            Mayank.SetActive(false);
            Praveen.SetActive(false);
            if (!PV.IsMine)
            {
                _Character = Character.SunilMoore;
            }
        }
        if (character == 2)
        {
            Justin.SetActive(false);
            Sunil.SetActive(false);
            Mayank.SetActive(true);
            Praveen.SetActive(false);
            if (!PV.IsMine)
            {
                _Character = Character.MayankRoss;
            }
        }
        if (character == 3)
        {
            Justin.SetActive(false);
            Sunil.SetActive(false);
            Mayank.SetActive(false);
            Praveen.SetActive(true);
            if (!PV.IsMine)
            {
                _Character = Character.PraveenLeblanc;
            }
        }

        CharacterName.text = _Character.ToString();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public void StartGame() => StartGame_RPC();

    [PunRPC]
    private void StartGame_RPC()
    {
        CanvasGroup obCG = GameObject.Find("Panel PW").GetComponent<CanvasGroup>();
        obCG.alpha = 1;
        obCG.interactable = true;
        obCG.blocksRaycasts = true;
    }

    #region Dancing
    public void DanceAboutID(int id)
    {
        for (int i = 0; i < anm.Length; i++)
        {
            if(anm[i] != null)
            {
                anm[i].SetInteger("DanceID", id);
            }
        }

        StartCoroutine(_stopDancing(8f));
    }
    public void StopDance()
    {
        for (int i = 0; i < anm.Length; i++)
        {
            if (anm[i] != null)
            {
                anm[i].SetInteger("DanceID", 0);
            }
        }
    }

    private IEnumerator _stopDancing(float waitToStop)
    {
        yield return new WaitForSeconds(waitToStop);

        StopDance();
    }
    #endregion
}
