using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Weapons
{
    public class FireWepon : MonoBehaviour
    {
        #region Varibals

        [Header("Details")]
        [SerializeField, Space] float range;
        [SerializeField] int maxAmmo;
        [SerializeField] float fireRate = 0.5F;
        [SerializeField] float speedObj = 100;
        [SerializeField] ItemGame itemIndex;
        [SerializeField] ItemGame ammoIndex;
        [SerializeField] public RelodeType _RelodeType = new RelodeType();
        [SerializeField] GunType _GunType = new GunType();
        [SerializeField] ShotType _ShotType = new ShotType();
        [SerializeField] LayerMask _ignore;

        [Header("Animations")]
        [SerializeField] Animator[] anmArmWeapon;

        [Header("Damage")]
        [SerializeField] int damage;
        [SerializeField] GameObject _ParticleObject;

        [Header("Effects")]
        [SerializeField] ParticleSystem Muzzle;
        [SerializeField] AudioSource ShotAudio;

        [Header("Object")]
        [SerializeField] Camera _camera;
        [SerializeField] Animator _cameraAnm;
        [SerializeField] TMP_Text ammoText;
        [SerializeField] GameObject _inHouseText;

        [Header("Obj Fire")]
        [SerializeField] Transform placeObj;
        [SerializeField] GameObject respawnObj;

        [Header("Events")]
        [SerializeField] UnityEvent _OnRelode;

        //--------------- Script Info
        public int CurrentAmount;
        public int InventoryAmount;
        private float lastFireTime = 0.0F;
        private bool isReloding = false;
        private bool dontLoopRelode = false;
        private Inventory inv_;
        bool isShotingSeries = false;

        #endregion

        #region Update & Awake & Start
        private void Awake()
        {
            if(_RelodeType == RelodeType.All)
            {
                isReloding = false;
            }
            inv_ = GetComponentInParent<Player.CharacterMove>().inv_;

            if(dontLoopRelode)
            {
                dontLoopRelode = false;
            }
        }
        private void Start()
        {
            CurrentAmount = maxAmmo;
        }
        private void Update()
        {
            #region Shot
            if(!GetComponentInParent<Player.CharacterMove>().inHouse)
            {
                if (GameInput.Key.GetKeyDown("Shot") && _ShotType == ShotType.Regularly)
                {
                    if (CurrentAmount > 0 && Time.time > lastFireTime && GetComponentInParent<Player.CharacterMove>().SomeWindowIsOpen() == false)
                    {
                        lastFireTime = Time.time + fireRate;
                        isReloding = false;
                        ShotAllTypeDo();

                        switch (_GunType)
                        {
                            case GunType.hit:
                                FireHit();
                                break;

                            case GunType.obj:
                                FireObj();
                                break;

                            default:
                                return;
                        }
                    }
                }
                if (GameInput.Key.GetKey("Shot") && _ShotType == ShotType.Series)
                {
                    if (CurrentAmount > 0 && Time.time > lastFireTime && GetComponentInParent<Player.CharacterMove>().SomeWindowIsOpen() == false)
                    {
                        lastFireTime = Time.time + fireRate;
                        isReloding = false;
                        ShotAllTypeDo();

                        switch (_GunType)
                        {
                            case GunType.hit:
                                FireHit();
                                break;

                            case GunType.obj:
                                FireObj();
                                break;

                            default:
                                return;
                        }
                    }
                    if (CurrentAmount > 0 && GetComponentInParent<Player.CharacterMove>().SomeWindowIsOpen() == false)
                    {
                        isShotingSeries = true;
                        _cameraAnm.SetBool("isShoting", true);
                    }
                    else
                    {
                        isShotingSeries = false;
                        _cameraAnm.SetBool("isShoting", false);
                    }
                }
                else
                {
                    isShotingSeries = false;
                    _cameraAnm.SetBool("isShoting", false);
                }

                _inHouseText.SetActive(false);
            }    
            else if(_inHouseText != null)
            {
                _inHouseText.SetActive(true);
            }
            #endregion

            #region Relode           
            if (GameInput.Key.GetKeyDown("Relode") && InventoryAmount > 0 && CurrentAmount < maxAmmo)
            {
                if(_RelodeType == RelodeType.All)
                {
                    if(dontLoopRelode == false)
                    {
                        Relode();
                        dontLoopRelode = true;
                    }
                }
                else
                {
                    Relode();
                }
            }
            if (isReloding)
            {
                for (int i = 0; i < anmArmWeapon.Length; i++)
                {
                    anmArmWeapon[i].SetBool("RelodeB", true);
                }
            }
            else
            {
                for (int i = 0; i < anmArmWeapon.Length; i++)
                {
                    anmArmWeapon[i].SetBool("RelodeB", false);
                }
            }
            #endregion

            if (inv_.CheckAmountItem(ammoIndex) != null)
            {
                InventoryAmount = inv_.CheckAmountItem(ammoIndex).stackableRealtime;
            }
            else
                InventoryAmount = 0;

            if (ammoText != null)
            {
                ammoText.text = CurrentAmount.ToString() + "/" + InventoryAmount.ToString();
            }
        }
        #endregion

        #region Shot
        private void FireObj()
        {
            Debug.Log("Fire Obj");

            GameObject flareGO = PhotonNetwork.Instantiate("Objects/" + respawnObj.name, placeObj.position, placeObj.rotation);
            Rigidbody rb = flareGO.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * speedObj, ForceMode.Impulse);

            #region setup Objects
            if(flareGO.GetComponent<FlareObject>() != null)
            {
                flareGO.GetComponent<FlareObject>().setupTarget(GetComponentInParent<Player.CharacterMove>());
            }
            #endregion
        }
        private void FireHit()
        {
            RaycastHit hit;

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, range - _ignore))
            {
                AI.WendigoAI _Wendi = hit.transform.GetComponent<AI.WendigoAI>();
                Adult _adult = hit.transform.GetComponent<Adult>();
                DamageTriggerPlayer _player = hit.transform.GetComponent<DamageTriggerPlayer>();

                if (_Wendi != null)
                {
                    AI.TakeDamage _damg = new AI.TakeDamage();
                    _damg.Damage = damage;
                    _damg._Type = AI.TakeDamage.DamageType.RunToPlayer;
                    _damg._TransformParticle = hit.point;
                    _damg._Particle = _ParticleObject;

                    _Wendi._TakeDamage(_damg, GetComponentInParent<Player.CharacterMove>().transform);
                }

                if(_adult != null)
                {
                    TakeDamageAdult _damg = new TakeDamageAdult();
                    _damg.DamageFloat = damage;
                    _damg._Particle = _ParticleObject;
                    _damg._TransformParticle = hit.point;

                    _adult.EventTakeDamage(_damg);
                }

                if(_player != null)
                {
                    Player.TakeDamagePlayer TDP = new Player.TakeDamagePlayer();
                    TDP.Damage = damage;
                    TDP._Particle = _ParticleObject;
                    TDP._TransformParticle = hit.point;

                    _player.TakeDamage(TDP, GetComponentInParent<Player.CharacterMove>());
                }

                if(_Wendi == null && _adult == null && _player == null)
                    GetComponentInParent<Player.CharacterMove>().PV.RPC("Bullet_RPC", RpcTarget.All,
                              hit.point.x, hit.point.y, hit.point.z, hit.normal);

                if (hit.rigidbody != null)
                    hit.rigidbody.AddForce(-hit.normal * 30f);
            }

            GetComponentInParent<Player.CharacterMove>().SearchRifleHand(itemIndex.ID).ShotRifle();
        }
        private void ShotAllTypeDo()
        {
            if(_ShotType == ShotType.Regularly)
            {
                _cameraAnm.SetTrigger("Shot");
            }
            CurrentAmount--;
            for (int i = 0; i < anmArmWeapon.Length; i++)
            {
                anmArmWeapon[i].SetTrigger("Shot");
            }
            if (Muzzle != null)
                Muzzle.Play();
            ShotAudio.Play();
            dontLoopRelode = false;

            GetComponentInParent<Player.CharacterMove>().SearchRifleHand(itemIndex.ID).ShotRifle();
        }
        #endregion

        #region Relode
        private void Relode()
        {
            if (_RelodeType == RelodeType.Single)
            {
                if (!isReloding)
                {
                    SingleRelode();
                }
                else
                    StopRelode();
            }
            if (_RelodeType == RelodeType.All)
            {
                AllRelode();
            }
        }
        private void SingleRelode()
        {
            isReloding = true;
        }
        private void AllRelode()
        {
            for (int i = 0; i < anmArmWeapon.Length; i++)
            {
                anmArmWeapon[i].SetTrigger("Relode");
            }
        }
        public void AddAmmo()
        {
            if (_RelodeType == RelodeType.All)
            {
                if (CurrentAmount + InventoryAmount <= maxAmmo)
                {
                    ChangaAmmo(InventoryAmount, InventoryAmount);
                }
                if (CurrentAmount + InventoryAmount > maxAmmo)
                {
                    int add = maxAmmo - CurrentAmount;

                    ChangaAmmo(add, add);
                }

                dontLoopRelode = false;
            }
            else if (_RelodeType == RelodeType.Single)
            {
                if (InventoryAmount > 0)
                {
                    ChangaAmmo(1, 1);
                }
                if(InventoryAmount <= 1)
                {
                    ChangaAmmo(1, 1);
                    StopRelode();
                }
            }
            _OnRelode.Invoke();
        }
        private void ChangaAmmo(int ammoAdd, int ammoRemoveInv)
        {
            CurrentAmount += ammoAdd;
            if(inv_.CheckAmountItem(ammoIndex) != null)
            {
                if (ammoRemoveInv != inv_.CheckAmountItem(ammoIndex).stackableRealtime)
                {
                    inv_.CheckAmountItem(ammoIndex).stackableRealtime -= ammoRemoveInv;
                }
                else
                    inv_.CheckAmountItem(ammoIndex).ClearAll();
            }

            if (_RelodeType == RelodeType.Single && CurrentAmount == maxAmmo || InventoryAmount <= 0)
            {
                StopRelode();
            }
        }
        public void StopRelode()
        {
            isReloding = false;
            dontLoopRelode = false;
        }
        #endregion
    }

    #region Enums
    public enum RelodeType
    {
        Single,
        All
    }
    public enum GunType
    {
        hit,
        obj
    }
    public enum ShotType
    {
        Regularly,
        Series
    }
    #endregion
}
