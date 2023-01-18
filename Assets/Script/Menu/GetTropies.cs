using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTropies : MonoBehaviour
{
    public static void UnlockTropiesStatic(int id)
    {
        GameJolt.API.Trophies.Unlock(id, (bool succes) =>
        {
            Debug.Log("Trophies "+ id + " " + succes);
        });
    }
    public void UnlockTropiesVoid(int id)
    {
        GameJolt.API.Trophies.Unlock(id, (bool succes) =>
        {
            Debug.Log("Trophies " + id + " " + succes);
        });
    }
}
