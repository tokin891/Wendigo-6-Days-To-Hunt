using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class WendigoInvectorSetup : MonoBehaviour
{
    [SerializeField] Invector.vMelee.vMeleeManager Vm;
    [SerializeField] Invector.vCharacterController.AI.vControlAIMelee Vc;
    [SerializeField] Invector.vCharacterController.AI.FSMBehaviour.vFSMBehaviourController Vv;
    [SerializeField] NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        if(PhotonNetwork.IsMasterClient == false)
        {
            Vm.enabled = false;
            Vc.enabled = false;
            Vv.enabled = false;
        }
    }
}
