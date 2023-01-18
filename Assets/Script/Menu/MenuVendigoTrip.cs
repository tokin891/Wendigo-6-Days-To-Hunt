using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

namespace Menu
{
    public class MenuVendigoTrip : MonoBehaviourPunCallbacks
    {
        //------------Public Static Details
        public static MenuVendigoTrip instance;

        //------------Inspector Details
        [Header("Object")]
        [SerializeField] GameObject MenuObject;
        [SerializeField] GameObject PanelObject;
        [SerializeField] TMP_Text ConnectedText;
        [SerializeField] Button[] buttonsPhoton;
        [SerializeField] TMP_InputField joinRoomInput;
        [SerializeField] TMP_InputField NickInput;
        [SerializeField] GameObject Menu;
        [SerializeField] GameObject JoinMenu;
        [SerializeField] Toggle _isPrivate;
        [SerializeField] Transform roomContent;
        [SerializeField] RoomListing _roomListing;
        [SerializeField] Toggle _toggleFriendlyFire;
        //------------Settings Rooms
        [SerializeField] Dropdown _dropdownMaxPlayers;

        [Header("Proporties")]
        [SerializeField] float timeToShowMenu;
        //-----------Connected Check
        [Header("Photon"), Space]
        [SerializeField] int SerializationRate;
        [SerializeField] int SendRate;
        private List<RoomListing> _listing = new List<RoomListing>();

        private void Awake()
        {
            instance = this;

            //-----------Photon Setup
            PhotonNetwork.SerializationRate = SerializationRate;
            PhotonNetwork.SendRate = SendRate;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = Application.version;
            PhotonNetwork.EnableCloseConnection = true;
        }
        private void Update()
        {
            #region Photon
            #region Txt
            TextConvertLanguage sC = new TextConvertLanguage("Status: Connected", "Status: Polaczono");
            TextConvertLanguage sNC = new TextConvertLanguage("Status: Not connected", "Status: Nie polaczono");
            #endregion
            if (PhotonNetwork.IsConnected)
            {
                ConnectedText.text = TextConvertLanguage.GetText(sC);
                for (int i = 0; i < buttonsPhoton.Length; i++)
                {
                    buttonsPhoton[i].interactable = true;
                }
            }else
            {
                ConnectedText.text = TextConvertLanguage.GetText(sNC);
                for (int i = 0; i < buttonsPhoton.Length; i++)
                {
                    buttonsPhoton[i].interactable = false;
                }
            }
            #endregion
        }

        #region Pun Void
        public void CreateRoom()
        {
            RoomOptions customRoomOptions = new RoomOptions();
            if(_dropdownMaxPlayers != null)
            {
                switch(_dropdownMaxPlayers.value)
                {
                    case 0:
                        customRoomOptions.MaxPlayers = 2;
                        break;
                    case 1:
                        customRoomOptions.MaxPlayers = 3;
                        break;
                    case 2:
                        customRoomOptions.MaxPlayers = 4;
                        break;
                }
            }
            customRoomOptions.IsVisible = !_isPrivate.isOn;
            string nameS = Random.Range(100000, 999999).ToString();
            GameInput.SetupFirendlyFire(_toggleFriendlyFire.isOn);
            PhotonNetwork.CreateRoom(nameS, customRoomOptions);
        }
        public void CreateRoomSP()
        {
            RoomOptions customRoomOptions = new RoomOptions();
            customRoomOptions.MaxPlayers = 1;
            customRoomOptions.IsOpen = false;
            string nameS = Random.Range(100000, 999999).ToString();
            PhotonNetwork.CreateRoom(nameS, customRoomOptions, null, null);
        }
        public void JoinRoom()
        {
            if (joinRoomInput.text.Length == 6)
            {
                PhotonNetwork.JoinRoom(joinRoomInput.text);
            }
            else
                return;
        }
        #endregion
        #region Pun Override
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }
        public override void OnCreatedRoom()
        {
            PhotonNetwork.LoadLevel(1);
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            JoinMenu.SetActive(false);
            Menu.SetActive(true);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (var item in roomList)
            {
                if(item.RemovedFromList) // Removed
                {
                    int index = _listing.FindIndex(x => x.info.Name == item.Name);
                    Destroy(_listing[index].gameObject);
                    _listing.RemoveAt(index);
                }else                   // Added
                {
                    RoomListing pr = Instantiate(_roomListing, roomContent);
                    if (pr != null)
                    {
                        pr.SetupRoom(item);
                        _listing.Add(pr);
                    }
                }              
            }
        }
        public override void OnJoinedRoom()
        {
            foreach(Transform one in roomContent)
            {
                Destroy(one.gameObject);
            }
        }
        #endregion

        public void exitApp() => Application.Quit();
        public void OpenURL(string url) => Application.OpenURL(url);
        public void CountShowMenu()
        {
            StartCoroutine(ShowMenu());
        }
        public void setNick()
        {
            if (NickInput.text.Length > 0)
            {
                PhotonNetwork.NickName = NickInput.text;
            }
            else
                PhotonNetwork.NickName = "Player " + Random.Range(100000, 999999);
        }
        IEnumerator ShowMenu()
        {
            yield return new WaitForSeconds(timeToShowMenu);

            MenuObject.SetActive(true);
            PanelObject.SetActive(false);
        }
    }
}
