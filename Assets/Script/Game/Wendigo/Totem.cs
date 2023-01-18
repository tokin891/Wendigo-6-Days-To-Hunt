using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Game.Player;

namespace Game.AI
{
    public class Totem : MonoBehaviour
    {
        [SerializeField] float distancePlayerMin;
        [SerializeField] GameObject Alert;

        private PhotonView PV;

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
        }
        public void AlertWendigo(bool index)
        {
            PV.RPC(nameof(ShowHideAlert), RpcTarget.All, index);
        }

        [PunRPC]
        private void ShowHideAlert(bool show)
        {
            Alert.SetActive(show);
        }

        CharacterMove GetClosestPlayer(CharacterMove[] enemies)
        {
            CharacterMove bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (CharacterMove potentialTarget in enemies)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }
    }
}
