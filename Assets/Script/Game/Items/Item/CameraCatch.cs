using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCatch : MonoBehaviour
{
    private Game.AI.WendigoAI wendi = null;
    private bool wendiInDistance = false;
    [SerializeField] float distanceWendi= 15;

    private void Update()
    {
        Game.AI.WendigoAI ai = FindObjectOfType<Game.AI.WendigoAI>();
        float distance = Vector3.Distance(transform.position, ai.transform.position);

        if(distance < distanceWendi && wendiInDistance == false)
        {
            CatchWendgio(ai);
            wendiInDistance = true;
        }else
        {
            wendiInDistance = false;
            UnCatchWendigo(ai);
        }
    }

    private void UnCatchWendigo(Game.AI.WendigoAI ai)
    {
        wendi = null;


    }

    private void CatchWendgio(Game.AI.WendigoAI ai)
    {
        wendi = ai;

        //Signalize
        Debug.Log("Wendi In Radius");
        Map.instance.SignalizeWendigo();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 255, 255, 55);
        if (wendi != null)
            Gizmos.DrawLine(transform.position, wendi.transform.position);
        Gizmos.DrawSphere(transform.position, distanceWendi);
    }
}
