using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrapSystem : TrapBehaviour
{
    [Header("Details")]
    [SerializeField] private float delayTrapCatch = 8f;
    [SerializeField] private string[] _catchTags;
    [SerializeField] private float closetDistance = 4.5f;

    private Animator _anmTrap;
    private float nttf = 0;
    private bool isClose = false;

    private void Awake()
    {
        _anmTrap = GetComponent<Animator>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        CheckCatchTarget(CheckYourTarget(), closetDistance);
    }


    private List<Transform> CheckYourTarget()
    {
        Transform[] allObjects = FindObjectsOfType<Transform>();
        List<Transform> allCatchTransform = new List<Transform>();
        foreach (Transform one in allObjects)
        {
            for (int i = 0; i < _catchTags.Length; i++)
            {
                if (one.transform.tag == _catchTags[i])
                {
                    allCatchTransform.Add(one);

                    break;
                }
            }
        }

        return allCatchTransform; 
    }

    public override void OnCatchTarget()
    {
        isClose = true;
    }

    [PunRPC]
    public void SetupTrap_RPC()
    {

    }
}

public abstract class TrapBehaviour: MonoBehaviour
{
    private PhotonView _pv;
    public PhotonView GetPV
    {
        set
        {
            _pv = value;
        }

        get
        {
            if (_pv != null)
                return _pv;

            return null;
        }
    }

    private void Awake()
    {
        GetPV = GetComponent<PhotonView>();
    }

    public void CheckCatchTarget(List<Transform> index, float distanceClosets)
    {
        for (int i = 0; i < index.Count; i++)
        {
            float distanceIndex = Vector3.Distance(transform.position, index[i].position);

            if(distanceIndex < distanceClosets)
            {
                OnCatchTarget();
            }
        }
    }

    public abstract void OnCatchTarget();
}
