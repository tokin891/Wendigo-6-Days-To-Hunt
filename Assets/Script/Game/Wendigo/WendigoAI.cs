using System;
using System.Collections;
using Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    #region Enums
    public enum WendigoState
    {
        Idle,
        Patrol,
        Target,
        Totem
    }
    public enum WendigoStyle
    {
        Stop,
        Walk,
        Run,
        Damage,
        Scream
    }
    public enum WendigoAgressive
    {
        No,
        Yes
    }
    public enum isDead
    {
        No,
        Yes
    }
    #endregion

    #region Class & Struct
    [Serializable]
    public class WendigoDetails
    {
        public int _Health;
        public WendigoState _WendigoState = new WendigoState();
        public WendigoStyle _WendigoStyle = new WendigoStyle();
        public WendigoAgressive _WendigoAgressive = new WendigoAgressive();
        public isDead _WendigoDead = new isDead();
    }
    public struct TakeDamage
    {
        public int Damage;
        public DamageType _Type;
        public GameObject _Particle;
        public Vector3 _TransformParticle;

        public enum DamageType
        {
            Nothing,
            RunToPlayer
        }
    }
    #endregion

    public class WendigoAI : MonoBehaviour
    {
        [Header("State")]
        public WendigoDetails _WendigoSettings = new WendigoDetails();
        [Header("Health")]
        [SerializeField] private int healthStart;
        [SerializeField] private int healthMax;

        [Header("Details")]
        [SerializeField] float SpeedWalk;
        [SerializeField] float SpeedRun;
        [SerializeField] float catchTarget;
        [SerializeField] float acceleration = 2f;
        [SerializeField] float deceleration = 60f;
        [SerializeField] float closeEnoughMeters = 4f;
        [SerializeField] GameObject hitPlayer;
        [SerializeField] LayerMask ignoreBody;

        [Header("Animations Frame")]
        [SerializeField] float SecEndDamage;
        [SerializeField] float SecEndScream;

        [Header("Object")]
        [SerializeField] GameObject tagOnMap;

        private NavMeshAgent agent;
        private Animator anm;
        private Transform targetPlayer;
        private Transform targetPoint;
        private Totem _targetTotem;
        private PhotonView pv;

        PatrolPoints Points;
        GameObject hitObject;
        Vector3 distanceTargetPoint = Vector3.zero;
        int pointsCount = 0;
        float catchTargetAwake;
        bool checkDoubleTargetPlayer = false;
        bool isPatrol = false;
        bool isEnd = false;
        bool goToPlayer = false;
        bool goToTotem = false;
        bool runToTotem = false;
        public int maxAttack = 2;
        public int usedAttack = 0;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            agent = GetComponent<NavMeshAgent>();
            anm = GetComponentInChildren<Animator>();
            Points = FindObjectOfType<PatrolPoints>();

            if (PhotonNetwork.IsMasterClient)
            {
                _SetTargetPoints();
                agent.autoBraking = false;
                catchTargetAwake = catchTarget;

                if(healthStart <= healthMax)
                {
                    _WendigoSettings._Health = healthStart;
                }else
                {
                    int minusHealth = healthMax - healthStart;
                    healthStart -= minusHealth;

                    _WendigoSettings._Health = healthStart;
                }
                Patrol();
                InvokeRepeating(nameof(WendiGoToUs), 200, 285);
            }
        }
        private void Update()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            if(_WendigoSettings._Health <= 0 && !isEnd)  /// DEMO !!!!
            {
                StartCoroutine(EndDemo(5f));
                isEnd = true;
            }
            if(Input.GetKeyDown(KeyCode.U))
            {
                TakeDamage tdD = new TakeDamage();
                tdD.Damage = 50;
                _TakeDamage(tdD);
            }

            if (_WendigoSettings._WendigoDead == isDead.Yes)
                return;
            if (_WendigoSettings._WendigoState == WendigoState.Patrol && _WendigoSettings._WendigoState != WendigoState.Totem)
            {
                if(!isPatrol)
                    Patrol();
                _CheckChangePoint();
                goToTotem = false;
                runToTotem = false;
            }
            if (_WendigoSettings._WendigoState == WendigoState.Target && targetPlayer != null)
            {
                float disntance = Vector3.Distance(transform.position, targetPlayer.position);

                if(disntance > 50)
                {
                    if (goToPlayer == false)
                    {
                        Target();
                        goToPlayer = true;
                    }
                }else
                {
                    _WendigoSettings._WendigoStyle = WendigoStyle.Run;

                    agent.destination = targetPlayer.position;
                    goToPlayer = false;
                }
                if (Vector3.Distance(transform.position, distanceTargetPoint) <= 5)
                    //Vector3.Distance(transform.position, distanceTargetPoint) == disntance + catchTarget ||
                    //Vector3.Distance(transform.position, distanceTargetPoint) == disntance - catchTarget
                {
                    Patrol();
                    distanceTargetPoint = Vector3.zero;
                }

                if (catchTarget > catchTargetAwake)
                {
                    float distanceAB = Vector3.Distance(transform.position, targetPlayer.position);
                    if (distanceAB < catchTargetAwake - 8f)
                        catchTarget = catchTargetAwake;
                }
            }else
            {
                goToPlayer = false;
            }
            if (_WendigoSettings._WendigoState == WendigoState.Idle)
            {
                Idle();
            }
            if(_WendigoSettings._WendigoState == WendigoState.Totem)
            {
                float DistanceTotem = Vector3.Distance(transform.position, _targetTotem.transform.position);
                if(DistanceTotem < 5)
                {
                    IdleSeconds(10f);
                    usedAttack = 0;
                    catchTarget = catchTargetAwake;
                    goToTotem = false;
                    runToTotem = false;

                    _targetTotem.AlertWendigo(false);
                    _targetTotem = null;
                }
                else
                {
                    catchTarget = 0;
                }
            }
            if (_targetTotem != null)
            {
                float DistanceTotem = Vector3.Distance(transform.position, _targetTotem.transform.position);
                if (DistanceTotem < 5)
                {
                    IdleSeconds(10f);
                    usedAttack = 0;
                    catchTarget = catchTargetAwake;
                    goToTotem = false;
                    runToTotem = false;

                    _targetTotem.AlertWendigo(false);
                    _targetTotem = null;
                }
            }

            if (_WendigoSettings._WendigoStyle == WendigoStyle.Walk)
            {
                agent.speed = SpeedWalk;
                anm.SetBool("Walk", true);
                anm.SetBool("Run", false);
            }
            if (_WendigoSettings._WendigoStyle == WendigoStyle.Run)
            {
                agent.speed = SpeedRun;
                anm.SetBool("Walk", false);
                anm.SetBool("Run", true);
            }
            if (_WendigoSettings._WendigoStyle == WendigoStyle.Stop)
            {
                agent.speed = 0f;
                agent.destination = transform.position;
                anm.SetBool("Walk", false);
                anm.SetBool("Run", false);
            }
            if(_WendigoSettings._WendigoStyle == WendigoStyle.Damage)
            {
                agent.speed = 0f;
                agent.destination = transform.position;
                anm.SetBool("Walk", false);
                anm.SetBool("Run", false);
                pv.RPC(nameof(damage_rpc), RpcTarget.All);
            }
            if (_WendigoSettings._WendigoStyle == WendigoStyle.Scream)
            {
                agent.speed = 0f;
                agent.destination = transform.position;
                anm.SetBool("Walk", false);
                anm.SetBool("Run", false);
                pv.RPC(nameof(scream_rpc), RpcTarget.All);
            }

            if (FindObjectOfType<CharacterMove>() != null)
                _CheckTarget();

            if (_WendigoSettings._WendigoState != WendigoState.Patrol)
                isPatrol = false;            
        }
        private void Patrol()
        {
            if (_WendigoSettings._WendigoState == WendigoState.Totem)
                return;

            isPatrol = true;
            _WendigoSettings._WendigoStyle = WendigoStyle.Walk;

            agent.SetDestination(targetPoint.position);
        }
        private void Target()
        {
            _WendigoSettings._WendigoStyle = WendigoStyle.Run;

            agent.SetDestination(targetPlayer.position);
            distanceTargetPoint = targetPlayer.position;
        }
        private void Idle()
        {
            _WendigoSettings._WendigoStyle = WendigoStyle.Stop;
        }
        public void TargetTotem(Totem totem)
        {
            _targetTotem = totem;
            _WendigoSettings._WendigoState = WendigoState.Totem;
            _WendigoSettings._WendigoStyle = WendigoStyle.Run;

            if(goToTotem == false)
            {
                agent.SetDestination(totem.transform.position);
                goToTotem = true;
            }
        }
        public void IdleSeconds(float seconds, WendigoStyle style = WendigoStyle.Stop)
        {
            _WendigoSettings._WendigoStyle = WendigoStyle.Stop;
            StartCoroutine(_idleIE(seconds, style));
        }
        public void GetDamage()
        {
            _WendigoSettings._WendigoStyle = WendigoStyle.Stop;
            IdleSeconds(SecEndDamage, WendigoStyle.Damage);
        }
        public void RunToTotem()
        {
            if(runToTotem == false)
            {
                TargetTotem(GetDistanceTotem(150));
                runToTotem = true;
            }
        }
        public void Attack()
        {
            usedAttack++;
            CharacterMove cm = _GetClosestEnemy(FindObjectsOfType<CharacterMove>());
            if(cm != null)
            {
                float distn = Vector3.Distance(transform.position, cm.transform.position);
                if (distn < closeEnoughMeters + 1.5f)
                {
                    TakeDamagePlayer TDP = new TakeDamagePlayer();
                    TDP.Damage = 35;
                    cm.TakeDamage(TDP);
                }

                if (usedAttack >= maxAttack)
                {
                    RunToTotem();
                    Debug.Log("Run To Totem");
                }
            }        
        }

        private void WendiGoToUs()
        {
            if (_GetClosestEnemy(FindObjectsOfType<CharacterMove>()) == null && _WendigoSettings._WendigoState == WendigoState.Totem)
                return;
            catchTarget = 10000;
        }
        private void Scream()
        {
            _WendigoSettings._WendigoStyle = WendigoStyle.Stop;
            IdleSeconds(SecEndScream, WendigoStyle.Scream);
        }
        private void _CheckTarget()
        {
            CharacterMove[] allPlayers = FindObjectsOfType<CharacterMove>();
            CharacterMove nearestPlayer = _GetClosestEnemy(allPlayers);
            if(nearestPlayer == null)
            {
                targetPlayer = null;
                if(_WendigoSettings._WendigoState != WendigoState.Totem)
                {
                    _WendigoSettings._WendigoState = WendigoState.Patrol;
                }
                checkDoubleTargetPlayer = false;
                return;
            }
            float Distance = Vector3.Distance(transform.position, nearestPlayer.transform.position);

            if (Distance < catchTarget && _WendigoSettings._WendigoState != WendigoState.Totem)
            {
                if(catchTarget < 70)
                {
                    if (_WendigoSettings._WendigoState != WendigoState.Totem)
                    {
                        if (_WendigoSettings._WendigoStyle != WendigoStyle.Damage || _WendigoSettings._WendigoStyle != WendigoStyle.Scream)
                        {
                            targetPlayer = nearestPlayer.transform;
                            _WendigoSettings._WendigoState = WendigoState.Target;
                            checkDoubleTargetPlayer = true;
                            StopAllCoroutines();
                        }
                        else
                        {
                            agent.speed = 0f;
                            agent.destination = transform.position;
                        }
                    }
                    if (Distance < closeEnoughMeters)
                    {
                        anm.SetTrigger("Attack");
                    }
                    if (Distance < catchTargetAwake)
                        catchTarget = catchTargetAwake;
                }else
                {
                    if(Distance <= 50)
                    {
                        catchTarget = 65;
                    }

                    targetPlayer = nearestPlayer.transform;
                    checkDoubleTargetPlayer = true;
                    _WendigoSettings._WendigoState = WendigoState.Target;

                    if (Distance < closeEnoughMeters)
                    {
                        pv.RPC(nameof(Attack_RPC), RpcTarget.All);
                    }
                    if (Distance < catchTargetAwake)
                        catchTarget = catchTargetAwake;
                }
            }
            else if(_WendigoSettings._WendigoState != WendigoState.Totem)
            {
                if (checkDoubleTargetPlayer)
                {
                    _WendigoSettings._WendigoState = WendigoState.Patrol;
                    catchTarget = catchTargetAwake;
                    checkDoubleTargetPlayer = false;
                    Debug.Log("Patrol State 01");
                    return;
                }
                targetPlayer = null;
            }
        }
        CharacterMove _GetClosestEnemy(CharacterMove[] enemies)
        {
            CharacterMove bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (CharacterMove potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr && potentialTarget.isKnocked == isKnockedDown.No && !potentialTarget.inHouse)
                {
                    if (usedAttack < maxAttack)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget;
                    }
                    else
                    {
                        if(_WendigoSettings._WendigoState != WendigoState.Totem)
                        {
                            RunToTotem();
                        }
                        return null;
                    }
                }
            }

            return bestTarget;
        }
        Totem GetDistanceTotem(float minDistance)
        {
            Totem[] allTotem = FindObjectsOfType<Totem>();
            float closetDistance = Mathf.Infinity;
            foreach (Totem item in allTotem)
            {
                float directionToTarget = Vector3.Distance(transform.position, item.transform.position);
                if(directionToTarget < closetDistance & Vector3.Distance(transform.position,item.transform.position) > minDistance)
                {
                    Debug.Log("Find totem " + item.name);
                    return item;
                }
            }
            return null;
        }
        private void _CheckChangePoint()
        {
            float DistancePoint = Vector3.Distance(transform.position, targetPoint.position);
            if (DistancePoint <= 12f)
            {
                IdleSeconds(5f);
                _SetTargetPoints();
            }
        }
        private void _SetTargetPoints()
        {
            if (pointsCount >= Points.Points.Length)
            {
                pointsCount = 0;
            }
            else
                pointsCount++;

            targetPoint = Points.Points[pointsCount];
        }
        IEnumerator _idleIE(float sec, WendigoStyle style)
        {
            _WendigoSettings._WendigoState = WendigoState.Idle;
            _WendigoSettings._WendigoStyle = style;
            yield return new WaitForSeconds(sec);

            if(_WendigoSettings._WendigoState == WendigoState.Idle)
            {
                _WendigoSettings._WendigoState = WendigoState.Patrol;
                isPatrol = false;
            }
           // _WendigoSettings._WendigoStyle = WendigoStyle.Stop;
        }

        public void ChangeAgressive(bool index)
        {
            pv.RPC(nameof(ChangeAgressive_RPC), RpcTarget.AllBuffered, index);
        }
        [PunRPC]
        public void ChangeAgressive_RPC(bool index)
        {
            if(index)
            {
                _WendigoSettings._WendigoAgressive = WendigoAgressive.Yes;
            }else
            {
                _WendigoSettings._WendigoAgressive = WendigoAgressive.No;
            }
        }
        [PunRPC]
        private void Attack_RPC()
        {
            anm.SetTrigger("Attack");
        }
        public void SignalizeOnMap(float time)
        {
            StartCoroutine(OffSignalize(time));
            tagOnMap.SetActive(true);
        }
        private IEnumerator OffSignalize(float time)
        {
            yield return new WaitForSeconds(time);

            tagOnMap.SetActive(false);
        }

        [PunRPC]
        private void damage_rpc()
        {
            anm.SetTrigger("Damage");
        }
        [PunRPC]
        private void scream_rpc()
        {
            anm.SetTrigger("Scream");
        }
        private IEnumerator EndDemo(float time)
        {
            CharacterMove[] allP = FindObjectsOfType<CharacterMove>();
            for (int i = 0; i < allP.Length; i++)
            {
                allP[i].Stop();
            }

            yield return new WaitForSeconds(time);

            CharacterMove[] allP_ = FindObjectsOfType<CharacterMove>();
            for (int i = 0; i < allP_.Length; i++)
            {
                allP_[i].End();
            }
        }

        #region Health
        public void _TakeDamage(TakeDamage damage, Transform player = null)
        {
            if(damage._Particle != null)
                PhotonNetwork.Instantiate("Particle/" + damage._Particle.name, damage._TransformParticle, Quaternion.identity);

            pv.RPC(nameof(_SendMessageHealth_RPC), RpcTarget.MasterClient, damage.Damage);

            #region GoPlayer
            if (damage._Type == TakeDamage.DamageType.RunToPlayer && _WendigoSettings._WendigoState != WendigoState.Totem)
            {
                //Run To Player

                float distanceAB = Vector3.Distance(transform.position, player.position);
                catchTarget = distanceAB + 18f;
            }
            #endregion
        }
        [PunRPC]
        private void _SendMessageHealth_RPC(int hp)
        {
            if (_WendigoSettings._Health - hp <= 0)
            {
                _SetHealth(hp, true);
                Debug.Log(_WendigoSettings._Health);
            }
            else
                _SetHealth(hp, false);
        }
        private void _SetHealth(int hp, bool isDead)
        {
            _WendigoSettings._Health -= hp;

            if(isDead)
            {
                anm.SetTrigger("Dead");
                _WendigoSettings._WendigoDead = AI.isDead.Yes;
                agent.enabled = false;
            }
        }
        #endregion

        void OnDrawGizmosSelected()
        {
            // Display the explosion radius when selected
            Gizmos.color = new Color(1, 1, 0, 0.75F);
            Gizmos.DrawWireSphere(transform.position, catchTarget);
        }
    }
}