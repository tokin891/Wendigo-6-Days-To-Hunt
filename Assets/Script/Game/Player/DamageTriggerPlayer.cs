using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamageTriggerPlayer : MonoBehaviour
{
    public float _takeDamage;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    public void TakeDamage(Game.Player.TakeDamagePlayer structurDamage, Game.Player.CharacterMove fromPlayer)
    {
        if (GetComponentInParent<Game.Player.CharacterMove>() == fromPlayer)
            return;

        pv.RPC(nameof(SendToMasterCheck), RpcTarget.MasterClient, structurDamage.Damage);

        if (structurDamage._Particle != null)
            PhotonNetwork.Instantiate("Particle/" + structurDamage._Particle.name, structurDamage._TransformParticle, Quaternion.identity);
    }
    [PunRPC]
    private void SendToMasterCheck(float damage)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        if (GameInput.IsFriendlyFire == false)
            return;

        pv.RPC(nameof(SendDamage_RPC), RpcTarget.All, damage);
    }
    [PunRPC]
    private void SendDamage_RPC(float damage)
    {
        if (!pv.IsMine)
            return;

        Game.Player.TakeDamagePlayer structurDamage = new Game.Player.TakeDamagePlayer();
        structurDamage.Damage = damage;
        GetComponentInParent<Game.Player.CharacterMove>().TakeDamage(structurDamage);
    }
}
