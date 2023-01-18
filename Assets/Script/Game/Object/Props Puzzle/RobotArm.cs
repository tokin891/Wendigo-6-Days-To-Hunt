using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RobotArm : MonoBehaviour
{
    [SerializeField] private int curretlyInteger;
    public ItemGame indexFlara;
    public bool isPossibleToRotate = true;
    public bool isCorrect;
    public bool curretlyFlara;
    public bool flareInArm;

    private Animator anm;
    private PhotonView pv;
   [SerializeField] private GameObject flaraObj;

    private void Awake()
    {
        anm = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
        if(flaraObj != null)
        {
            flaraObj.SetActive(false);
            flareInArm = true;
        }
    }

    private void Update()
    {
        if(flareInArm)
        {
            if (curretlyFlara == false)
            {
                flaraObj.SetActive(false);
            }
            else
            {
                flaraObj.SetActive(true);
            }
        }
      
        if (anm.GetInteger("arm") != curretlyInteger)
            return;

        isCorrect = true;
    }

    public void ChangeRotation()
    {
        if (!isPossibleToRotate)
            return;

        if (anm.GetInteger("arm") > 3)
        {
            pv.RPC(nameof(SetAnimation),RpcTarget.All, 1);
        }
        else
            pv.RPC(nameof(SetAnimation), RpcTarget.All, anm.GetInteger("arm") + 1);

    }

    [PunRPC]
    private void SetAnimation(int integer)
    {
        anm.SetInteger("arm", integer);
    }
}
