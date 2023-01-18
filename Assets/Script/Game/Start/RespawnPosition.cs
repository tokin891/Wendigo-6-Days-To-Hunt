using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPosition : MonoBehaviour
{
    public Transform[] Points;

    public Transform GetPosition()
    {
        int r = Random.Range(0, Points.Length);

        return Points[r];
    }
}
