using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class AdultsController : GenericAdultController
{
    //--------- Instance
    public static AdultsController instance;

    private bool isRespawningDeer = false;
    private bool isRespawningWolf = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (GetAdultAsCountOfType(TypeAdults.Deer) < (int)(maxAdults / 2) && !isRespawningDeer)
        {
            RespawnAdult(TypeAdults.Deer, maxAdults);
            isRespawningDeer = true;
        }
        if (GetAdultAsCountOfType(TypeAdults.Deer) >= maxAdults)
        {
            isRespawningDeer = false;
        }
        if (GetAdultAsCountOfType(TypeAdults.Wolf) < (int)(maxAdults / 2) && !isRespawningWolf)
        {
            RespawnAdult(TypeAdults.Wolf, maxAdults);
            isRespawningWolf = true;
        }
        if (GetAdultAsCountOfType(TypeAdults.Wolf) >= maxAdults)
        {
            isRespawningWolf = false;
        }
    }

    private void CheckAdultInWorld(TypeAdults type)
    {
        int currentAd = GetAdultAsCountOfType(type);
        int maxAd = maxAdults;
        int howManyRespawn = maxAd - currentAd;

        RespawnAdult(type, howManyRespawn);
    }
}
public abstract class GenericAdultController: MonoBehaviour
{
    #region Adults
    private List<Adult> AllAdults = new List<Adult>();
    private List<Adult> allAdults
    {
        set
        {
            AllAdults = value;
        }
        get
        {
            return AllAdults;
        }
    }
    public ref readonly List<Adult> GetAllAdults => ref AllAdults;
    public int AllAdultsCount => AllAdults.Count;
    #endregion

    #region AdultsProporties
    public readonly int maxAdults = 7;
    private int currentAdults;
    #endregion

    //------- Const Strings
    private const string locationDeer = "Adults/Deer AI";
    private const string locationWolf = "Adults/Wolf AI";

    #region Methods
    private void Update()
    {
        if (currentAdults != AllAdultsCount)
        {
            ChangeCountOfAdults();
        }
    }

    #region Virtual & Abstract
    public virtual void ChangeCountOfAdults()
    {
        currentAdults = AllAdultsCount;
    }
    public bool isMaxAdultsOfType(TypeAdults type)
    {
        List<Adult> adultList = new List<Adult>();

        foreach(Adult one in GetAllAdults)
        {
            if(one.GetTypeAdults == type)
            {
                adultList.Add(one);
                break;
            }
        }

        if (adultList.Count >= 10)
            return true;

        return false;
    }
    public int GetAdultAsCountOfType(TypeAdults type)
    {
        List<Adult> adultList = new List<Adult>();

        foreach (Adult one in GetAllAdults)
        {
            if (one.GetTypeAdults == type)
            {
                adultList.Add(one);
                break;
            }
        }

        return adultList.Count;
    }
    public void RespawnAdult(TypeAdults typeA, int howMany = 1)
    {
        if (typeA == TypeAdults.Deer)
            StartCoroutine(RespawnWithWaitingTime(2.5f, locationDeer, howMany));
        if(typeA == TypeAdults.Wolf)
            StartCoroutine(RespawnWithWaitingTime(3f, locationWolf, howMany));
    }

    IEnumerator RespawnWithWaitingTime(float time,string path, int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            yield return new WaitForSeconds(time);

            // Find our vars
            AdultPoints currentPoint = GetRandomPoint().GetComponent<AdultPoints>();
            Adult currentAdult = PhotonNetwork.Instantiate(Path.Combine(path), currentPoint.getRandomTransform().position, Quaternion.identity).GetComponent<Adult>();

            // Setup Vars
            currentAdult.transform.SetParent(transform.Find("Adult Parent"));
            currentAdult.SetupWaypoints(currentPoint.gameObject);
            allAdults.Add(currentAdult);
        }
    }

    private GameObject GetRandomPoint()
    {
        AdultPoints[] allPoints = FindObjectsOfType<AdultPoints>();
        int random = 0;

        return allPoints[random].gameObject;
    }
    #endregion
    #endregion
}