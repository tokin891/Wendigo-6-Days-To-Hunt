using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FlareObject : MonoBehaviour
{
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] GameObject _partcileDamage;
    [SerializeField] AudioSource ad;
    [SerializeField] float damage;

    private PhotonView PV;
    private bool dontLoop = false;
    private Game.Player.CharacterMove target;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        StartCoroutine(waitForExp());
    }
    private void Update()
    {
        Game.AI.WendigoAI ai = FindObjectOfType<Game.AI.WendigoAI>();
        float distanceAI = Vector3.Distance(transform.position, ai.transform.position);
        if(distanceAI < 14)
        {
            //blesh
        }
    }
    public void setupTarget(Game.Player.CharacterMove targ)
    {
        target = targ;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (dontLoop)
            return;
        if(collision.transform.GetComponent<Game.AI.WendigoAI>() != null || collision.transform.tag == "Wendigo")
        {
            if(PV.IsMine)
            {
                Game.AI.TakeDamage td = new Game.AI.TakeDamage();
                td.Damage = (int)damage;
                td._Particle = _partcileDamage;
                td._TransformParticle = collision.transform.position;
                td._Type = Game.AI.TakeDamage.DamageType.Nothing;

                collision.transform.GetComponent<Game.AI.WendigoAI>()._TakeDamage(td, target.transform);
            }
            Explosion();
        }
        Debug.Log(collision.gameObject.name);
    }
    IEnumerator waitForExp()
    {
        yield return new WaitForSeconds(12f);

        Dezactive();
    }

    private void Dezactive()
    {
        ParticleSystem[] PS = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < PS.Length; i++)
        {
            PS[i].Stop();
        }

        Destroy(gameObject, 5f);
    }
    private void Explosion()
    {
        _explosion.Play();
        ad.Play();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(gameObject, 2f);
        dontLoop = true;
    }
}
