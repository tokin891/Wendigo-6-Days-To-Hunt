using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Realtime;

namespace Game.Player
{
    public class CharacterMove : MonoBehaviourPunCallbacks
    {
        #region Varibals
        //---------------Move
        [Header("Move & Status"), Space]
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool isCrouch = false;
        private bool isRun = false;
        private float heightAwake = 0f;
        private float speedAwake;
        [SerializeField] public PlayerStatus _playerDetails;
        [SerializeField] private float playerSpeed = 2.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravityValue = -9.81f;

        //---------------Mouse
        [Header("Mouse"), Space]
        [SerializeField] private float mouseSens = 100f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] Animator cameraAnm;
        private Transform playerBody;
        float xRot = 0f;
        [HideInInspector] public bool dontMoveAndRotate = false;

        //---------------Photon
        [HideInInspector] public PhotonView PV;
        [HideInInspector] public isKnockedDown isKnocked = new isKnockedDown();

        //---------------Readonly
        private bool isMoving;
        public ref readonly bool IsMoving => ref isMoving;
        public ref readonly bool IsRun => ref isRun;

        [Header("Events")]
        [SerializeField] UnityEvent _onKnockdown;
        [SerializeField] UnityEvent _onRunEndOutStamina;
        [SerializeField] UnityEvent _onJump;
        [SerializeField] UnityEvent _onGetDamage;

        [Header("Objects")]
        [SerializeField] TMP_Text nicknameTMP;
        [SerializeField] TMP_Text timeTMP;
        [SerializeField] TMP_Text dayTMP;
        [SerializeField] Image playerMap;
        [SerializeField] Color32 colorPV;
        [SerializeField] Color32 colorNotPV;
        [HideInInspector] Animator CharacterAnm;
        [SerializeField, Space] GameObject menu;
        [SerializeField] GameObject inv;
        [SerializeField] GameObject map;
        [SerializeField] AudioSource fsS;
        [SerializeField] GameObject projectile;
        [SerializeField] GameObject[] hideOnKnockedown;
        [SerializeField] Transform GC;
        [SerializeField] float GD;
        [SerializeField] float yVel = 0.0f;
        [SerializeField] Animator endAnm;
        [SerializeField] GameObject KnockdownWindow;
        [SerializeField] Slider staminaSlider;
        [SerializeField] Slider healthSlider;
        [SerializeField] CheckTrigger _CrunchTrigger;
        [SerializeField] AudioSource _Hurt;
        [SerializeField] TMP_Text _cordText;
        [SerializeField] TMP_Text _cordTextx;
        [SerializeField] TMP_Text _cordTexty;

        [Header("Characters"), Space]
        [SerializeField] GameObject[] Justins;
        [SerializeField] GameObject[] Sunils;
        [SerializeField] GameObject[] Mayanks;
        [SerializeField] GameObject[] Praveens;
        [SerializeField] GameObject Justin;
        [SerializeField] GameObject Sunil;
        [SerializeField] GameObject Mayank;
        [SerializeField] GameObject Praveen;

        [Header("Items System"), Space]
        [SerializeField] Transform PointDrop;

        //---------------Public
        [HideInInspector] public Inventory inv_;
        [HideInInspector] public bool inHouse = false;
        public GameObject cinemaCam;
        public Character GetCharacter;

        //---------------Private
        bool menuIsOpen = false;
        bool invIsOpen = false;
        bool mapIsOpen = false;
        bool isGrounded = false;

        bool isRifle = false;
        bool LoopRifle = false;
        bool isRunPossible = true;
        int rifleID = 0;
        int pistolID = 0;
        #endregion

        #region Update & Awake & Start
        private void Awake()
        {
            PV = GetComponent<PhotonView>();
            controller = GetComponent<CharacterController>();
            heightAwake = controller.height;
            speedAwake = playerSpeed;
            playerBody = transform;
            if (PV.IsMine)
            {
                SetCursor(false);
                nicknameTMP.text = "";
                RoomManager RM = FindObjectOfType<RoomManager>();
                PV.RPC(nameof(ShowCharacter_RPC), RpcTarget.OthersBuffered, (int)RM._Character);
                SetupCharacter((int)RM._Character);
                inv_ = GetComponentInChildren<Inventory>();

                playerMap.color = colorPV;
                Debug.Log(_playerDetails.health);
            }
            else
            {
                Destroy(playerCamera.gameObject);
                nicknameTMP.text = PV.Owner.NickName;
                playerMap.color = colorNotPV;
            }
        }
        void Update()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (PV.IsMine)
                {
                    if(isKnocked == isKnockedDown.No)
                    {
                        if(SomeWindowIsOpen() == false)
                        {
                            PlayerMove();
                            PlayerRotation();

                            int xC = (int)transform.position.x;
                            int yC = (int)transform.position.z;

                            PlayerCoord(xC, yC);
                        }

                        isGrounded = Physics.CheckSphere(GC.position, GD);

                        for (int i = 0; i < hideOnKnockedown.Length; i++)
                        {
                            hideOnKnockedown[i].SetActive(true);
                        }

                        #region CheckWeapons
                        Inventory invv = GetComponentInChildren<Inventory>();
                        ItemGame currentItem = invv.currentUse;

                        if (inv != null)
                        {
                            if (currentItem != null && SearchRifleHand(currentItem.ID) != null)
                            {
                                if (currentItem._Type == TypeItem.Rifle)
                                {
                                    CharacterAnm.SetBool("isRifle", true);
                                    CharacterAnm.SetBool("isPistol", false);
                                    LoopRifle = true;
                                    SearchRifleHand(currentItem.ID).ChangeStateRifle(true);
                                    rifleID = currentItem.ID;

                                    RifleHand[] allRifle = GetComponentsInChildren<RifleHand>();
                                    for (int i = 0; i < allRifle.Length; i++)
                                    {
                                        if (allRifle[i].id != rifleID && allRifle[i].activeStatus == true)
                                        {
                                            allRifle[i].ChangeStateRifle(false);
                                        }
                                    }
                                }
                                if (currentItem._Type == TypeItem.Pistol)
                                {
                                    CharacterAnm.SetBool("isPistol", true);
                                    CharacterAnm.SetBool("isRifle", false);
                                    LoopRifle = true;
                                    SearchRifleHand(currentItem.ID).ChangeStateRifle(true);
                                    pistolID = currentItem.ID;

                                    RifleHand[] allRifle = GetComponentsInChildren<RifleHand>();
                                    for (int i = 0; i < allRifle.Length; i++)
                                    {
                                        if (allRifle[i].id != pistolID && allRifle[i].activeStatus == true)
                                        {
                                            allRifle[i].ChangeStateRifle(false);
                                        }
                                    }
                                }
                                if (currentItem._Type == TypeItem.Item)
                                {
                                    CharacterAnm.SetBool("isRifle", false);
                                    CharacterAnm.SetBool("isPistol", false);
                                    LoopRifle = false;

                                    RifleHand[] allRifle = GetComponentsInChildren<RifleHand>();
                                    for (int i = 0; i < allRifle.Length; i++)
                                    {
                                        if (allRifle[i].activeStatus == true)
                                        {
                                            allRifle[i].ChangeStateRifle(false);
                                        }
                                    }
                                    rifleID = 0;
                                    pistolID = 0;
                                }
                            }
                            else
                            {
                                CharacterAnm.SetBool("isRifle", false);
                                CharacterAnm.SetBool("isPistol", false);
                                LoopRifle = false;

                                RifleHand[] allRifle = GetComponentsInChildren<RifleHand>();
                                for (int i = 0; i < allRifle.Length; i++)
                                {
                                    if(allRifle[i].activeStatus == true)
                                    {
                                        allRifle[i].ChangeStateRifle(false);
                                    }
                                }
                                rifleID = 0;
                                pistolID = 0;
                            }
                        }
                        #endregion
                        #region Windows
                        if (SomeWindowIsOpen() == false)
                        {
                            if (GameInput.Key.GetKeyDown("Inv"))
                            {
                                invIsOpen = true;
                                SetCursor(true);
                            }
                            else if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                menuIsOpen = true;
                                SetCursor(true);
                            }
                            if (GameInput.Key.GetKeyDown("Map"))
                            {
                                mapIsOpen = true;
                                SetCursor(true);
                            }
                        }
                        else
                        {
                            if (GameInput.Key.GetKeyDown("Inv"))
                            {
                                invIsOpen = false;
                                SetCursor(false);
                            }
                            else if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                CloseMenu();
                            }
                            if (GameInput.Key.GetKeyDown("Map"))
                            {
                                mapIsOpen = false;
                                SetCursor(false);
                            }
                        }

                        menu.SetActive(menuIsOpen);
                        if(mapIsOpen)
                        {
                            map.GetComponent<CanvasGroup>().alpha = 1;
                            map.GetComponent<CanvasGroup>().blocksRaycasts = true;
                        }
                        else
                        {
                            map.GetComponent<CanvasGroup>().alpha = 0;
                            map.GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                        if (invIsOpen)
                        {
                            inv.GetComponent<CanvasGroup>().alpha = 1;
                            inv.GetComponent<CanvasGroup>().blocksRaycasts = true;
                        }
                        else
                        {
                            inv.GetComponent<CanvasGroup>().alpha = 0;
                            inv.GetComponent<CanvasGroup>().blocksRaycasts = false;
                            if(inv_ != null)
                                inv_.OffNote();
                        }
                        #endregion
                        #region H&S
                        healthSlider.value = _playerDetails.health;
                        staminaSlider.value = _playerDetails.stamina;
                        #endregion

                        KnockdownWindow.SetActive(false);
                        CharacterAnm.SetBool("isKnockedown", false);
                    }
                    else
                    {
                        PlayerKnockedDown();

                        for (int i = 0; i < hideOnKnockedown.Length; i++)
                        {
                            hideOnKnockedown[i].SetActive(false);
                        }

                        KnockdownWindow.SetActive(true);
                        CharacterAnm.SetBool("isKnockedown", true);
                    }

                    PlayerGravity();

                    #region GameManager
                    if (inHouse)
                    {
                        FindObjectOfType<GameManager>()._playerPlace = PlayerPlace.House;
                    }else
                    {
                        FindObjectOfType<GameManager>()._playerPlace = PlayerPlace.Forest;
                    }

                    TextConvertLanguage _timeT = new TextConvertLanguage("Time:", "Czas:");
                    TextConvertLanguage _dayT = new TextConvertLanguage("Day:", "Dzien:");
                    timeTMP.text = TextConvertLanguage.GetText(_timeT) + FindObjectOfType<GameManager>().TimeDay;
                    dayTMP.text = TextConvertLanguage.GetText(_dayT) + FindObjectOfType<GameManager>().Day;
                    #endregion

                    /////DEMO
                    if (Input.GetKeyDown(KeyCode.H))
                        _playerDetails.health = 100;
                    if (Input.GetKeyDown(KeyCode.K))
                    {
                        TakeDamagePlayer _td = new TakeDamagePlayer();
                        _td.Damage = 20;
                        TakeDamage(_td);
                    }

                    //if(Input.GetKeyDown(KeyCode.K))
                    //KnockDownPlayer();
                    //if (Input.GetKeyDown(KeyCode.O))
                    //End();
                }
            }
            if (FindObjectOfType<Camera>() != null)
            {
                nicknameTMP.transform.LookAt(FindObjectOfType<Camera>().transform);
            }
        }
        #endregion

        #region Player
        private void PlayerMove()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            //isCrouch = Input.GetKey(KeyCode.LeftControl);
            isRun = GameInput.Key.GetKey("Run");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * playerSpeed * Time.deltaTime);

            if (GameInput.Key.GetKeyDown("Jump") && isGrounded && _playerDetails.stamina > 10)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
                _playerDetails.stamina -= 10;
                _onJump.Invoke();
            }

            PlayerAnimations(x, z);
        }
        private void PlayerAnimations(float x, float z)
        {
            if (isCrouch)
            {
                controller.height = Mathf.SmoothDamp(controller.height, heightAwake / 2, ref yVel, 19 * Time.deltaTime);
                CharacterAnm.SetBool("Crouch", true);
            }
            else
            {
                if(_CrunchTrigger.isSomethingInTrigger == false)
                {
                    CharacterAnm.SetBool("Crouch", false);
                    controller.height = Mathf.SmoothDamp(controller.height, heightAwake, ref yVel, 15 * Time.deltaTime);
                }
            }
            if (isRun && _playerDetails.stamina > 2 && isRunPossible)
            {
                CharacterAnm.SetBool("Run", true);
                playerSpeed = speedAwake * 2;
                cameraAnm.speed = 2;
                if(x != 0 || z != 0)
                {
                    if (_playerDetails.stamina > 1f)
                    {
                        _playerDetails.stamina -= 10f * Time.deltaTime;
                    }
                    if (_playerDetails.stamina <= 2)
                    {
                        isRunPossible = false;
                        PV.RPC(nameof(onEndStaminaOutRun), RpcTarget.All);
                    }
                }           
            }
            else
            {
                CharacterAnm.SetBool("Run", false);
                playerSpeed = speedAwake;
                cameraAnm.speed = 1;
                if(_playerDetails.stamina > 35 && !isRunPossible)
                {
                    isRunPossible = true;
                }
            }
            if (isGrounded)
            {
                CharacterAnm.SetBool("InJump", false);
            }
            else
                CharacterAnm.SetBool("InJump", true);

            if (x != 0 || z != 0 && isGrounded)
            {
                CharacterAnm.SetBool("Walk", true);
                cameraAnm.SetBool("Walk", true);

                if (CharacterAnm.GetBool("Run") == false && _playerDetails.stamina < 99)
                {
                    _playerDetails.stamina += 4f * Time.deltaTime;
                }

                isMoving = true;
            }
            if(x == 0 && z == 0)
            {
                CharacterAnm.SetBool("Walk", false);
                cameraAnm.SetBool("Walk", false);

                if(_playerDetails.stamina < 99)
                {
                    _playerDetails.stamina += 4.5f * Time.deltaTime;
                }
                isMoving = false;
            }

            CharacterAnm.SetFloat("Horizontal", x);
            CharacterAnm.SetFloat("Vertical", z);
        }
        private void PlayerGravity()
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (isGrounded && playerVelocity.y < 0)
                playerVelocity.y = -2f;
        }
        private void PlayerRotation()
        {
            if (Cursor.visible == true)
                return;

            float MX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            float MY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

            xRot -= MY;
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
            playerBody.Rotate(Vector3.up * MX);
        }
        private void PlayerKnockedDown()
        {
            PlayerAnimations(0,0);
        }
        private void PlayerCoord(int xC, int yC)
        {
            #region Text
            TextConvertLanguage _CordT = new TextConvertLanguage("Cordination:", "Koordynacja:");
            #endregion
            _cordText.text = TextConvertLanguage.GetText(_CordT);
            _cordTextx.text = "x: " + xC.ToString();
            _cordTexty.text = "y: " + yC.ToString();
        }
        #endregion

        #region AI

        public void TakeDamage(TakeDamagePlayer TDP)
        {
            PV.RPC(nameof(TakeDamage_RPC), RpcTarget.All, TDP.Damage);
        }
        [PunRPC]
        public void TakeDamage_RPC(float TDP)
        {
            _Hurt.Play();
            AddHealth(-TDP);

            if (PV.IsMine && isKnocked == isKnockedDown.No)
            {
                _onGetDamage.Invoke();
            }
        }

        #endregion

        #region Health & Stamina & Respawn

        private void AddHealth(float h)
        {
            _playerDetails.health += h;

            if (_playerDetails.health <= 0)
                KnockDownPlayer();
        }
        public void Revive()
        {
            PV.RPC(nameof(KnockDownPlayer_RPC), RpcTarget.AllBuffered, false);
        }
        [PunRPC]
        private void onEndStaminaOutRun() => _onRunEndOutStamina.Invoke();

        #endregion

        #region Others
        public void OpenInv()
        {
            if (SomeWindowIsOpen() == false)
            {
                invIsOpen = true;
                SetCursor(true);
            }
        }
        public void CloseMenu()
        {
            menuIsOpen = false;
            SetCursor(false);
        }
        public void CloseInv()
        {
            invIsOpen = false;
            SetCursor(false);
        }
        private IEnumerator EndCutScene(float time, VideoPlayer vp)
        {
            yield return new WaitForSeconds(time);
            vp.gameObject.SetActive(false);
        }
        public void SetCursor(bool index)
        {
            Cursor.visible = index;
            if(!index)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }else
                Cursor.lockState = CursorLockMode.None;
        }
        [PunRPC]
        private void ShowCharacter_RPC(int character)
        {
            GetCharacter = (Character)character;

            if (PV.IsMine == false)
            {
                if (character == 0)
                {
                    for (int i = 0; i < Justins.Length; i++)
                    {
                        Justins[i].SetActive(true);
                    }
                    Justin.SetActive(true);
                }
                if (character == 1)
                {
                    for (int i = 0; i < Sunils.Length; i++)
                    {
                        Sunils[i].SetActive(true);
                    }
                    Sunil.SetActive(true);
                }
                if (character == 2)
                {
                    for (int i = 0; i < Mayanks.Length; i++)
                    {
                        Mayanks[i].SetActive(true);
                    }
                    Mayank.SetActive(true);
                }
                if (character == 3)
                {
                    for (int i = 0; i < Praveens.Length; i++)
                    {
                        Praveens[i].SetActive(true);
                    }
                    Praveen.SetActive(true);
                }
            }
        }
        private void SetupCharacter(int character)
        {
            if(character == 0)
            {
                Justin.SetActive(true);
                CharacterAnm = Justin.GetComponent<Animator>();
            }
            if(character == 1)
            {
                Sunil.SetActive(true);
                CharacterAnm = Sunil.GetComponent<Animator>();
            }
            if(character == 2)
            {
                Mayank.SetActive(true);
                CharacterAnm = Mayank.GetComponent<Animator>();
            }
            if(character == 3)
            {
                Praveen.SetActive(true);
                CharacterAnm = Praveen.GetComponent<Animator>();
            }
        }

        #region End Demo
        public void End()
        {
            PV.RPC(nameof(End_RPC), RpcTarget.All);
        }
        [PunRPC]
        private void End_RPC()
        {
            if (!PV.IsMine)
                return;
            endAnm.SetTrigger("End");
            SetCursor(true);
        }
        public void Stop()
        {
            PV.RPC(nameof(End_RPC), RpcTarget.All);
        }
        [PunRPC]
        private void Stop_RPC()
        {
            if (!PV.IsMine)
                return;
            AudioListener.pause = true;
            enabled = false;
        }
        #endregion
        public void FootstepPlay()
        {
            PV.RPC(nameof(FootstepPlay_RPC), RpcTarget.Others);
        }
        [PunRPC]
        private void FootstepPlay_RPC()
        {
            fsS.Play();
        }
        public bool SomeWindowIsOpen()
        {
            if (menuIsOpen || invIsOpen || mapIsOpen || dontMoveAndRotate)
            {
                PlayerAnimations(0, 0);
                return true;                
            }
            else
                return false;
        }
        public RifleHand SearchRifleHand(int id)
        {
            RifleHand[] allRifle = GetComponentsInChildren<RifleHand>();

            foreach(RifleHand one in allRifle)
            {
                if(one.itemRifle.ID == id)
                {
                    return one;
                }
            }
            return null;
        }

        #region Menus Settings
        public void DisconnectUser()
        {
            RoomManager.Instance.DisconnectPlayer();
        }
        public void DisconnectToDesktop()
        {
            RoomManager.Instance.DisconnectPlayer();
            Application.Quit();
        }
        #endregion
        #endregion

        #region Knocdown
        private void KnockDownPlayer()
        {
            PV.RPC(nameof(KnockDownPlayer_RPC), RpcTarget.AllBuffered, true);
            _onKnockdown.Invoke();
        }
        [PunRPC]
        private void KnockDownPlayer_RPC(bool index)
        {
            if(index)
            {
                isKnocked = isKnockedDown.Yes;
            }else
            {
                isKnocked = isKnockedDown.No;
            }

            if(PV.IsMine)
            {
                SetCursor(true);
            }
        }
        #endregion

        #region Items System
        [PunRPC]
        private void Bullet_RPC(float xF, float yF, float zF, Vector3 lookQ)
        {
            Vector3 positionBullet = new Vector3(xF, yF, zF);

            GameObject bullet = Instantiate(projectile, positionBullet, Quaternion.LookRotation(lookQ));
            if (bullet != null)
                Destroy(bullet, 4f);
        }
        public bool DropItemByPath(string _path, Quaternion rotation)
        {
            if(PointDrop.GetComponent<CheckTrigger>().isSomethingInTrigger)
            {
                return false;
            }else
            {
                PhotonNetwork.Instantiate(Path.Combine(_path), PointDrop.position, rotation);
                return true;
            }
        }
        public bool DropItemByInfo(ItemGame info, int howMany)
        {
            if (PointDrop.GetComponent<CheckTrigger>().isSomethingInTrigger)
            {
                return false;
            }
            else
            {
                string _path = "ItemsReal/BoxWithItem";
                GameObject one = PhotonNetwork.Instantiate(Path.Combine(_path), PointDrop.position, Quaternion.identity);
                one.GetComponent<PickupSystem>().SetupObject(info.ID, howMany);

                return true;
            }
        }  
        public void DropItemsByDisconnect(Dictionary<ItemGame, int> index)
        {
            string _path = "ItemsReal/BoxWithItem";
            GameObject one = PhotonNetwork.Instantiate(Path.Combine(_path), transform.position, Quaternion.identity);
        }

        #endregion

        #region Override
        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            DisconnectUser();

            //Feedback ID (2) == Disconnect By Master
            int id_Feedback = 2;
            FeedbackManager.instance.AddFeedback(id_Feedback);
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            DisconnectUser();

            int id_Feedback = 1;
            FeedbackManager.instance.AddFeedback(id_Feedback);

            Debug.Log(cause);
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            //Draw Sphere Check Ground ;-)
            Gizmos.DrawSphere(GC.position, GD);
        }
        #endregion
    }

    #region Enums
    public enum isKnockedDown
    {
        No,
        Yes
    }
    #endregion
    #region Class
    [System.Serializable]
    public class PlayerStatus
    {
        public float health;
        public float stamina;
    }
    #endregion
    #region Struct
    public struct TakeDamagePlayer
    {
        public float Damage;

        public GameObject _Particle;
        public Vector3 _TransformParticle;
    }
    #endregion
}
