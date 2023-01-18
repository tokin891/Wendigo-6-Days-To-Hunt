using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Adult : AdultGenericBehaviour
{
    //----------Setup
    [Header("Setup")]
    [SerializeField] private TypeAdults _selectType = new TypeAdults();
    [SerializeField] private float _selectSpeed = 1.35f;
    [SerializeField] private float _selectLowestDistanceToPlayer = 14f;
    [SerializeField] private float _setHealth = 100;

    //----------Details
    public ref readonly float Speed => ref _selectSpeed;
    public ref readonly float LowestDistanceToPlayer => ref _selectLowestDistanceToPlayer;
    readonly float SpeedMultiplyInRun = 3.55f;
    readonly float DistanceLowerTham = 1f;
    readonly float ResetTime = 6f;

    private AdultState _StateManager = new AdultState();
    private AdultPatrol _StatePatrolManager = new AdultPatrol();
    public AdultPoints currentPoints
    {
        set
        {
            if(value != null)
            {
                ap = value;
            }
        }
        get
        {
            if (ap != null)
            {
                return ap;
            }
            else
                return null;
        }
    }

    //----------Private
    private float currentHealth;
    private PhotonView PV;
    private Animator Anm;
    private NavMeshAgent Agent;
    private Transform currentPointEscape;
    private float timeToGo;
    private float speedStart;
    private AdultPoints ap;
    private PickupSystem _pickupDead;
    private bool dontLoopChangeP = false;
    private int currentTargetAsCount = 0;
    private int destPoint = 0;

    #region Update & Awake
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Anm = GetComponentInChildren<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        _pickupDead = GetComponent<PickupSystem>();

        SetAdultType(_selectType); 
        ChangeHealth(_setHealth);
        currentHealth = _setHealth;
        _pickupDead.isEnabled = false;

        if (!PhotonNetwork.IsMasterClient)
            return;

        speedStart = Speed;
        SetAndGetSpeed = Speed;

        TryChangeStateToWalk();
        _StatePatrolManager = AdultPatrol.Normal;

        Invoke(nameof(GotoNextPoint), 0.5f);
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        if (currentPoints == null)
             return;
        if (Agent.enabled == false)
            return;

        if(_StatePatrolManager == AdultPatrol.Normal)
        {
            switch ((int)_StateManager)
            {
                //Idle
                case 0:
                    // Do Idle
                    isIdle();

                    break;

                //Walk
                case 1:
                    // Do Walk
                    isWalk();
                    if (!Agent.pathPending && Agent.remainingDistance < DistanceLowerTham)
                        GotoNextPoint();

                    break;

                //Run
                case 2:

                    // Do Run
                    isRun();
                    if (!Agent.pathPending && Agent.remainingDistance < DistanceLowerTham)
                        GotoNextPoint();

                    break;

                //Error
                case 3:

                    // Something Went Wrong!!!
                    Debug.LogError(this.name + " state Manager is null reference");

                    return;
            }

            _CheckPlayers();
        }

        #region Static Set
        Agent.speed = SetAndGetSpeed;
        #endregion
    }
    #endregion

    #region All Interact State
    public void TryChangeStateToIdle()
    {
        if (_isPossibleToIdle() == false)
            return;

        _StateManager = AdultState.Idle;
    }
    private bool _isPossibleToIdle()
    {
        if (_StateManager == AdultState.Idle)
            return false;

        return true;
    }
    public void TryChangeStateToWalk()
    {
        if (_isPossibleToWalk() == false)
            return;

        _StateManager = AdultState.Walk;
    }
    private bool _isPossibleToWalk()
    {
        if (_StateManager == AdultState.Walk)
            return false;

        return true;
    }
    public void TryChangeStateToRun()
    {
        if (_isPossibleToRun() == false)
            return;

        _StateManager = AdultState.Run;
    }
    private bool _isPossibleToRun()
    {
        if (_StateManager == AdultState.Run)
            return false;

        return true;
    }
    public void TryToRest(float time)
    {
        if (_isPossibleToRest() == false)
            return;

        timeToGo = time;
    }
    private bool _isPossibleToRest()
    {
        if (timeToGo > 0)
            return false;

        return true;
    }
    #endregion

    #region State Do
    private void isIdle()
    {
        SetAndGetSpeed = 0f;
        Agent.isStopped = true;
        Agent.SetDestination(transform.position);

        #region Animation
        Anm.SetInteger("State", 0);
        #endregion
    }
    private void isWalk()
    {
        SetAndGetSpeed = speedStart;

        #region Animation
        Anm.SetInteger("State", 1);
        #endregion
    }
    private void isRun()
    {
        SetAndGetSpeed = speedStart * SpeedMultiplyInRun;

        #region Animation
        Anm.SetInteger("State", 2);
        #endregion
    }

    void GotoNextPoint()
    {
        if (currentPoints == null && currentPoints._points.Count == 0)
            return;

        Agent.destination = currentPoints._points[destPoint].position;
        destPoint = (destPoint + 1) % currentPoints._points.Count;

        TryChangeStateToWalk();
    }

    private void _CheckPlayers()
    {
        Game.Player.CharacterMove[] allPlayers = FindObjectsOfType<Game.Player.CharacterMove>();
        for (int i = 0; i < allPlayers.Length; i++)
        {
            float distancePlayer = Vector3.Distance(transform.position, allPlayers[i].transform.position);
            if (distancePlayer < LowestDistanceToPlayer)
                TryChangeStateToRun();
        }
    }
    #endregion

    #region Damage
    public void EventTakeDamage(TakeDamageAdult index)
    {
        PV.RPC(nameof(EventTakeDamage_RPC), RpcTarget.All, index.DamageFloat);

        if (index._Particle != null)
            PhotonNetwork.Instantiate("Particle/" + index._Particle.name, index._TransformParticle, Quaternion.identity);
    }

    [PunRPC]
    private void EventTakeDamage_RPC(float damage_)
    {
        TakeHealth(damage_);
        currentHealth -= damage_;
        Debug.Log(GetHealth);

        if (currentHealth <= 0)
        {
            _pickupDead.isEnabled = true;
        }

        if (!PhotonNetwork.IsMasterClient)
            return;
        // Starfe only for Master

        TryChangeStateToRun();
        if (currentHealth <= 0)
        {
            Dead();
            Debug.Log(gameObject.name + " is dead");
        }
    }

    private void Dead()
    {
        Agent.destination = transform.position;
        Agent.isStopped = true;


        Agent.enabled = false;
        Anm.SetInteger("State", 3);
    }
    #endregion

    #region Override
    public override void TakeHealth(float _health)
    {
        Debug.Log("Take Damage Of Adult: " + this.GetTypeAdults.ToString() + ",  Damage Info: " + _health);
    }
    #endregion

    #region Some Voids

    public void SetupWaypoints(GameObject waypoint)
    {
        Debug.Log(waypoint);
        currentPoints = waypoint.GetComponent<AdultPoints>();
    }
    #endregion
}
public enum AdultState
{
    Idle,
    Walk,
    Run
}
public enum AdultPatrol
{
    Normal,
    Escape,
    Reset
}
public struct TakeDamageAdult
{
    public float DamageFloat;

    public GameObject _Particle;
    public Vector3 _TransformParticle;
}