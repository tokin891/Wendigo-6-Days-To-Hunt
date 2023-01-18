using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    public bool isSomethingInTrigger;

    private void OnTriggerEnter(Collider other)
    {
        isSomethingInTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isSomethingInTrigger = false;
    }
}
